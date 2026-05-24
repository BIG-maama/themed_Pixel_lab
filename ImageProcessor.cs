//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Homwore
//{
//    public static class ImageProcessor
//    {

//        public static Bitmap ReduceColors(Bitmap original, int colorLevels)
//        {
//            int width = original.Width;
//            int height = original.Height;
//            Bitmap newBitmap = new Bitmap(width, height);

//            //عامل التقسيم بناءً على عدد المستويات
//            int factor = 256 / (colorLevels - 1);

//            // 1. فتح القفل عن بيانات الصورة (للوصول السريع للذاكرة)
//            BitmapData data = original.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
//            BitmapData resData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

//            // 2. استخدام المؤشرات 
//            unsafe
//            {
//                byte* pRaw = (byte*)data.Scan0;      // مؤشر للصورة الأصلية
//                byte* pRes = (byte*)resData.Scan0;    // مؤشر للصورة الناتجة

//                for (int y = 0; y < height; y++)
//                {
//                    for (int x = 0; x < width; x++)
//                    {
//                        // حساب موقع البكسل الحالي في الذاكرة (كل بكسل 4 بايت: B, G, R, A)
//                        int index = (y * data.Stride) + (x * 4);

//                        // نعالج القنوات الثلاث (B, G, R)
//                        for (int i = 0; i < 3; i++)
//                        {
//                            int oldValue = pRaw[index + i];
//                            int newValue = (int)(Math.Round((double)oldValue / factor) * factor);

//                            // التأكد من الحدود [0, 255]
//                            pRes[index + i] = (byte)Math.Max(0, Math.Min(255, newValue));
//                        }

//                        // الحفاظ على قناة الشفافية (Alpha) كما هي
//                        pRes[index + 3] = pRaw[index + 3];
//                    }
//                }
//            }

//            // 3. فك القفل عن الصور
//            original.UnlockBits(data);
//            newBitmap.UnlockBits(resData);

//            return newBitmap;
//        }
//    }
//}
using System;
using System.Drawing;
using System.Drawing.Imaging;
using PixelLab.ColorSystems.Base;
using PixelLab.ColorSystems.Models;

namespace Homwore
{
    public static class ImageProcessor
    {
        public static Bitmap ReduceColors(Bitmap original, int colorLevels, ColorSpaceConverter converter = null)
        {
            if (colorLevels < 2 || colorLevels > 256)
                throw new ArgumentException("colorLevels يجب أن يكون بين 2 و 256");

            int width = original.Width;
            int height = original.Height;
            Bitmap newBitmap = new Bitmap(width, height);

            BitmapData data = original.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            BitmapData resData = newBitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            int step = 255 / (colorLevels - 1);

            unsafe
            {
                byte* pRaw = (byte*)data.Scan0;
                byte* pRes = (byte*)resData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = (y * data.Stride) + (x * 4);

                        byte b = pRaw[index];
                        byte g = pRaw[index + 1];
                        byte r = pRaw[index + 2];
                        byte a = pRaw[index + 3];

                        Color resultColor;

                        if (converter == null)
                        {
                            // ======= RGB =======
                            resultColor = Color.FromArgb(
                                ReduceChannel(r, step),
                                ReduceChannel(g, step),
                                ReduceChannel(b, step));
                        }
                        else
                        {
                            // ======= باقي الأنظمة =======
                            Color original_pixel = Color.FromArgb(r, g, b);

                            // RGB → النظام المطلوب
                            ColorResult converted = converter.FromRgb(original_pixel);

                            // تقليل القنوات بنطاق كل قناة الصحيح ← التعديل هون
                            double newCh1 = ReduceAnyChannel(converted.Channel1, converter.SystemName, 1, colorLevels);
                            double newCh2 = ReduceAnyChannel(converted.Channel2, converter.SystemName, 2, colorLevels);
                            double newCh3 = ReduceAnyChannel(converted.Channel3, converter.SystemName, 3, colorLevels);

                            // النظام المطلوب → RGB
                            resultColor = converter.ToRgb(newCh1, newCh2, newCh3);
                        }

                        pRes[index] = resultColor.B;
                        pRes[index + 1] = resultColor.G;
                        pRes[index + 2] = resultColor.R;
                        pRes[index + 3] = a;
                    }
                }
            }

            original.UnlockBits(data);
            newBitmap.UnlockBits(resData);

            return newBitmap;
        }

        // تقليل قناة RGB (0-255)
        private static byte ReduceChannel(byte value, int step)
        {
            int newVal = (int)(Math.Round((double)value / step) * step);
            return (byte)Math.Max(0, Math.Min(255, newVal));
        }

        

        private static double ReduceAnyChannel(double value, string systemName, int channelNumber, int colorLevels)
        {
            double min, max;

           
            switch (systemName)
            {
                case "HSV":
                    if (channelNumber == 1) { min = 0; max = 360; } // H
                    else { min = 0; max = 1; } // S, V
                    break;

                case "YUV":
                    if (channelNumber == 1) { min = 0; max = 255; } // Y
                    else if (channelNumber == 2) { min = -111; max = 111; } // U
                    else { min = -157; max = 157; } // V
                    break;

                case "L*a*b*":
                    if (channelNumber == 1) { min = 0; max = 100; } // L
                    else { min = -128; max = 127; } // a, b
                    break;

                case "CMY":
                case "YCbCr":
                default:
                    min = 0; max = 255;
                    break;
            }

            double range = max - min;
            double step = range / (colorLevels - 1);
            double shifted = value - min;
            double reduced = Math.Round(shifted / step) * step;
            return Math.Max(min, Math.Min(max, min + reduced));
        }
    }
}