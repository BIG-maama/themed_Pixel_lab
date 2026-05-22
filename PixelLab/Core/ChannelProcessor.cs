//using System;
//using System.Drawing;

//namespace PixelLab.Core
//{
//    public class ChannelProcessor
//    {
//        public bool EnableR = true;
//        public bool EnableG = true;
//        public bool EnableB = true;

//        public float RFactor = 1f;
//        public float GFactor = 1f;
//        public float BFactor = 1f;

//        public Bitmap Process(Bitmap source)
//        {
//            Bitmap workingImage = source;
//            if (source.Width > 800 || source.Height > 800)
//            {
//                workingImage = ResizeImage(source, 800, 800);
//            }

//            Bitmap result = new Bitmap(workingImage.Width, workingImage.Height);
//            //Bitmap result = new Bitmap(source.Width, source.Height);

//            for (int x = 0; x < source.Width; x++)
//            {
//                for (int y = 0; y < source.Height; y++)
//                {
//                    Color p = source.GetPixel(x, y);

//                    int r = EnableR ? (int)(p.R * RFactor) : 0;
//                    int g = EnableG ? (int)(p.G * GFactor) : 0;
//                    int b = EnableB ? (int)(p.B * BFactor) : 0;

//                    result.SetPixel(x, y, Color.FromArgb(
//                        Clamp(r), Clamp(g), Clamp(b)
//                    ));
//                }
//            }

//            return result;
//        }

//        public Bitmap ExtractRed(Bitmap source)
//        {
//            Bitmap result = new Bitmap(source.Width, source.Height);

//            for (int x = 0; x < source.Width; x++)
//                for (int y = 0; y < source.Height; y++)
//                {
//                    Color p = source.GetPixel(x, y);
//                    result.SetPixel(x, y, Color.FromArgb(p.R, 0, 0));
//                }

//            return result;
//        }

//        public Bitmap ExtractGreen(Bitmap source)
//        {
//            Bitmap result = new Bitmap(source.Width, source.Height);

//            for (int x = 0; x < source.Width; x++)
//                for (int y = 0; y < source.Height; y++)
//                {
//                    Color p = source.GetPixel(x, y);
//                    result.SetPixel(x, y, Color.FromArgb(0, p.G, 0));
//                }

//            return result;
//        }
//        private Bitmap ResizeImage(Bitmap img, int maxWidth, int maxHeight)
//        {
//            double ratio = Math.Min((double)maxWidth / img.Width, (double)maxHeight / img.Height);
//            int newWidth = (int)(img.Width * ratio);
//            int newHeight = (int)(img.Height * ratio);

//            Bitmap result = new Bitmap(newWidth, newHeight);
//            using (Graphics g = Graphics.FromImage(result))
//            {
//                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
//                g.DrawImage(img, 0, 0, newWidth, newHeight);
//            }
//            return result;
//        }

//        public Bitmap ExtractBlue(Bitmap source)
//        {
//            Bitmap result = new Bitmap(source.Width, source.Height);

//            for (int x = 0; x < source.Width; x++)
//                for (int y = 0; y < source.Height; y++)
//                {
//                    Color p = source.GetPixel(x, y);
//                    result.SetPixel(x, y, Color.FromArgb(0, 0, p.B));
//                }

//            return result;
//        }

//        private int Clamp(int v)
//        {
//            if (v < 0) return 0;
//            if (v > 255) return 255;
//            return v;
//        }
//    }
//}
//using System;
//using System.Drawing;
//using System.Drawing.Drawing2D;

//namespace PixelLab.Core
//{
//    // ✅ delegate بدلاً من Func
//    public delegate Color ColorConverter(Color c);

//    public class ChannelProcessor
//    {
//        public bool EnableR = true;
//        public bool EnableG = true;
//        public bool EnableB = true;

//        public float RFactor = 1f;
//        public float GFactor = 1f;
//        public float BFactor = 1f;

//        private const int MAX_SIZE = 500;

//        public Bitmap Process(Bitmap source)
//        {
//            if (source == null)
//                throw new ArgumentNullException(nameof(source));

//            Bitmap workingImage = source;
//            bool needDispose = false;

//            if (source.Width > MAX_SIZE || source.Height > MAX_SIZE)
//            {
//                workingImage = ResizeImageSafe(source, MAX_SIZE, MAX_SIZE);
//                needDispose = true;
//            }

//            try
//            {
//                Bitmap result = new Bitmap(workingImage.Width, workingImage.Height);

//                for (int y = 0; y < workingImage.Height; y++)
//                {
//                    for (int x = 0; x < workingImage.Width; x++)
//                    {
//                        Color p = workingImage.GetPixel(x, y);

//                        int r = EnableR ? (int)(p.R * RFactor) : 0;
//                        int g = EnableG ? (int)(p.G * GFactor) : 0;
//                        int b = EnableB ? (int)(p.B * BFactor) : 0;

//                        result.SetPixel(x, y, Color.FromArgb(
//                            Clamp(r), Clamp(g), Clamp(b)
//                        ));
//                    }
//                }

//                return result;
//            }
//            finally
//            {
//                if (needDispose && workingImage != source)
//                {
//                    workingImage.Dispose();
//                }
//            }
//        }

//        private Bitmap ResizeImageSafe(Bitmap img, int maxWidth, int maxHeight)
//        {
//            try
//            {
//                double ratioX = (double)maxWidth / img.Width;
//                double ratioY = (double)maxHeight / img.Height;
//                double ratio = Math.Min(ratioX, ratioY);

//                if (ratio >= 1.0)
//                    return new Bitmap(img);

//                int newWidth = (int)(img.Width * ratio);
//                int newHeight = (int)(img.Height * ratio);

//                newWidth = Math.Max(1, newWidth);
//                newHeight = Math.Max(1, newHeight);

//                Bitmap result = new Bitmap(newWidth, newHeight);

//                using (Graphics g = Graphics.FromImage(result))
//                {
//                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
//                    g.SmoothingMode = SmoothingMode.HighQuality;
//                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
//                    g.CompositingQuality = CompositingQuality.HighQuality;

//                    g.DrawImage(img, 0, 0, newWidth, newHeight);
//                }

//                return result;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Resize failed: " + ex.Message);
//                return new Bitmap(img);
//            }
//        }

//        // ✅ استخدام delegate بدلاً من Func
//        public Bitmap ExtractRed(Bitmap source)
//        {
//            return ExtractChannel(source, delegate (Color c) {
//                return Color.FromArgb(c.R, 0, 0);
//            });
//        }

//        public Bitmap ExtractGreen(Bitmap source)
//        {
//            return ExtractChannel(source, delegate (Color c) {
//                return Color.FromArgb(0, c.G, 0);
//            });
//        }

//        public Bitmap ExtractBlue(Bitmap source)
//        {
//            return ExtractChannel(source, delegate (Color c) {
//                return Color.FromArgb(0, 0, c.B);
//            });
//        }

//        // ✅ الدالة المساعدة تستخدم delegate
//        private Bitmap ExtractChannel(Bitmap source, ColorConverter converter)
//        {
//            if (source == null) return null;

//            Bitmap result = new Bitmap(source.Width, source.Height);

//            for (int y = 0; y < source.Height; y++)
//            {
//                for (int x = 0; x < source.Width; x++)
//                {
//                    Color p = source.GetPixel(x, y);
//                    result.SetPixel(x, y, converter(p));
//                }
//            }

//            return result;
//        }

//        private int Clamp(int v)
//        {
//            if (v < 0) return 0;
//            if (v > 255) return 255;
//            return v;
//        }

//    }
//}
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PixelLab.Core
{
    /// <summary>
    /// معالج قنوات الألوان - يتيح تفعيل/تعطيل القنوات وتعديل شدة كل قناة
    /// </summary>
    public class ChannelProcessor
    {
        public bool EnableR { get; set; } = true;
        public bool EnableG { get; set; } = true;
        public bool EnableB { get; set; } = true;

        public float RFactor { get; set; } = 1.0f;
        public float GFactor { get; set; } = 1.0f;
        public float BFactor { get; set; } = 1.0f;

        public Bitmap Process(Bitmap original)
        {
            if (original == null) return null;

            Bitmap result = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            BitmapData srcData = original.LockBits(
                new Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = result.LockBits(
                new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                byte[] srcBuffer = new byte[Math.Abs(srcData.Stride) * original.Height];
                byte[] dstBuffer = new byte[Math.Abs(dstData.Stride) * result.Height];
                Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBuffer.Length);

                for (int y = 0; y < original.Height; y++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        int idx = y * srcData.Stride + x * 4;

                        byte b = EnableB ? (byte)Math.Min(255, srcBuffer[idx] * BFactor) : (byte)0;
                        byte g = EnableG ? (byte)Math.Min(255, srcBuffer[idx + 1] * GFactor) : (byte)0;
                        byte r = EnableR ? (byte)Math.Min(255, srcBuffer[idx + 2] * RFactor) : (byte)0;
                        byte a = srcBuffer[idx + 3];

                        dstBuffer[idx] = b;
                        dstBuffer[idx + 1] = g;
                        dstBuffer[idx + 2] = r;
                        dstBuffer[idx + 3] = a;
                    }
                }

                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
            }
            finally
            {
                original.UnlockBits(srcData);
                result.UnlockBits(dstData);
            }

            return result;
        }

        /// <summary>
        /// استخراج قناة الأحمر - تظهر البكسل باللون الأحمر (R فقط)
        /// </summary>
        public Bitmap ExtractRed(Bitmap original)
        {
            if (original == null) return null;

            Bitmap result = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            BitmapData srcData = original.LockBits(
                new Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = result.LockBits(
                new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                byte[] srcBuffer = new byte[Math.Abs(srcData.Stride) * original.Height];
                byte[] dstBuffer = new byte[Math.Abs(dstData.Stride) * result.Height];
                Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBuffer.Length);

                for (int y = 0; y < original.Height; y++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        int idx = y * srcData.Stride + x * 4;
                        byte r = srcBuffer[idx + 2]; // قناة الأحمر

                        dstBuffer[idx] = 0;       // B = 0
                        dstBuffer[idx + 1] = 0;     // G = 0
                        dstBuffer[idx + 2] = r;     // R = قيمة الأحمر
                        dstBuffer[idx + 3] = 255;   // A = 255
                    }
                }

                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
            }
            finally
            {
                original.UnlockBits(srcData);
                result.UnlockBits(dstData);
            }

            return result;
        }

        /// <summary>
        /// استخراج قناة الأخضر - تظهر البكسل باللون الأخضر (G فقط)
        /// </summary>
        public Bitmap ExtractGreen(Bitmap original)
        {
            if (original == null) return null;

            Bitmap result = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            BitmapData srcData = original.LockBits(
                new Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = result.LockBits(
                new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                byte[] srcBuffer = new byte[Math.Abs(srcData.Stride) * original.Height];
                byte[] dstBuffer = new byte[Math.Abs(dstData.Stride) * result.Height];
                Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBuffer.Length);

                for (int y = 0; y < original.Height; y++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        int idx = y * srcData.Stride + x * 4;
                        byte g = srcBuffer[idx + 1]; // قناة الأخضر

                        dstBuffer[idx] = 0;       // B = 0
                        dstBuffer[idx + 1] = g;     // G = قيمة الأخضر
                        dstBuffer[idx + 2] = 0;     // R = 0
                        dstBuffer[idx + 3] = 255;   // A = 255
                    }
                }

                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
            }
            finally
            {
                original.UnlockBits(srcData);
                result.UnlockBits(dstData);
            }

            return result;
        }

        /// <summary>
        /// استخراج قناة الأزرق - تظهر البكسل باللون الأزرق (B فقط)
        /// </summary>
        public Bitmap ExtractBlue(Bitmap original)
        {
            if (original == null) return null;

            Bitmap result = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            BitmapData srcData = original.LockBits(
                new Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = result.LockBits(
                new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                byte[] srcBuffer = new byte[Math.Abs(srcData.Stride) * original.Height];
                byte[] dstBuffer = new byte[Math.Abs(dstData.Stride) * result.Height];
                Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBuffer.Length);

                for (int y = 0; y < original.Height; y++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        int idx = y * srcData.Stride + x * 4;
                        byte b = srcBuffer[idx]; // قناة الأزرق

                        dstBuffer[idx] = b;       // B = قيمة الأزرق
                        dstBuffer[idx + 1] = 0;     // G = 0
                        dstBuffer[idx + 2] = 0;     // R = 0
                        dstBuffer[idx + 3] = 255;   // A = 255
                    }
                }

                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
            }
            finally
            {
                original.UnlockBits(srcData);
                result.UnlockBits(dstData);
            }

            return result;
        }
    }
}