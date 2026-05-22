using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homwore
{
    public static class ImageProcessor
    {
      
        public static Bitmap ReduceColors(Bitmap original, int colorLevels)
        {
            int width = original.Width;
            int height = original.Height;
            Bitmap newBitmap = new Bitmap(width, height);

            //عامل التقسيم بناءً على عدد المستويات
            int factor = 256 / (colorLevels - 1);

            // 1. فتح القفل عن بيانات الصورة (للوصول السريع للذاكرة)
            BitmapData data = original.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resData = newBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // 2. استخدام المؤشرات 
            unsafe
            {
                byte* pRaw = (byte*)data.Scan0;      // مؤشر للصورة الأصلية
                byte* pRes = (byte*)resData.Scan0;    // مؤشر للصورة الناتجة

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // حساب موقع البكسل الحالي في الذاكرة (كل بكسل 4 بايت: B, G, R, A)
                        int index = (y * data.Stride) + (x * 4);
                        
                        // نعالج القنوات الثلاث (B, G, R)
                        for (int i = 0; i < 3; i++)
                        {
                            int oldValue = pRaw[index + i];
                            int newValue = (int)(Math.Round((double)oldValue / factor) * factor);

                            // التأكد من الحدود [0, 255]
                            pRes[index + i] = (byte)Math.Max(0, Math.Min(255, newValue));
                        }

                        // الحفاظ على قناة الشفافية (Alpha) كما هي
                        pRes[index + 3] = pRaw[index + 3];
                    }
                }
            }

            // 3. فك القفل عن الصور
            original.UnlockBits(data);
            newBitmap.UnlockBits(resData);

            return newBitmap;
        }
    }
}
