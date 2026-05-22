using System;
using System.Drawing;

namespace Homwore
{
    /// <summary>
    /// كلاس مساعد: يحوّل نقطة ثلاثية الأبعاد (x,y,z) إلى نقطة على الشاشة (2D)
    /// باستخدام Isometric Projection — بسيطة وتعطي مظهر 3D واضح
    /// </summary>
    public class Projection3D
    {
        // زوايا الدوران الحالية (بالدرجات)
        public double AngleX { get; set; } = 30.0;
        public double AngleY { get; set; } = -45.0;

        // مركز الشاشة — النقطة اللي يُرسم حولها المجسم
        public int CenterX { get; set; } = 300;
        public int CenterY { get; set; } = 300;

        // مقياس الرسم — كبّره تكبّر المجسم
        public double Scale { get; set; } = 200.0;

        /// <summary>
        /// يحوّل نقطة 3D (x,y,z كلها بين 0 و 1) إلى نقطة 2D على الشاشة
        /// </summary>
        public PointF Project(double x, double y, double z)
        {
            // تحويل الزوايا لـ Radians
            double radX = AngleX * Math.PI / 180.0;
            double radY = AngleY * Math.PI / 180.0;

            // دوران حول محور Y
            double x1 = x * Math.Cos(radY) - z * Math.Sin(radY);
            double z1 = x * Math.Sin(radY) + z * Math.Cos(radY);

            // دوران حول محور X
            double y2 = y * Math.Cos(radX) - z1 * Math.Sin(radX);
            double z2 = y * Math.Sin(radX) + z1 * Math.Cos(radX);

            // إسقاط على الشاشة
            float screenX = (float)(CenterX + x1 * Scale);
            float screenY = (float)(CenterY - y2 * Scale);

            return new PointF(screenX, screenY);
        }

        /// <summary>
        /// يحوّل نقطة شاشة إلى عمق Z التقريبي (لترتيب الرسم — الأبعد أولاً)
        /// </summary>
        public double GetDepth(double x, double y, double z)
        {
            double radX = AngleX * Math.PI / 180.0;
            double radY = AngleY * Math.PI / 180.0;
            double z1 = x * Math.Sin(radY) + z * Math.Cos(radY);
            double z2 = y * Math.Sin(radX) + z1 * Math.Cos(radX);
            return z2;
        }
    }
}