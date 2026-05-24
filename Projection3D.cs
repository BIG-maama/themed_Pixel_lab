using System;
using System.Drawing;

namespace Homwore
{

    public class Projection3D
    {
        public double AngleX { get; set; } = 30.0;
        public double AngleY { get; set; } = -45.0;

        public int CenterX { get; set; } = 300;
        public int CenterY { get; set; } = 300;

        public double Scale { get; set; } = 200.0;

        
        public PointF Project(double x, double y, double z)
        {
            double radX = AngleX * Math.PI / 180.0;
            double radY = AngleY * Math.PI / 180.0;

            double x1 = x * Math.Cos(radY) - z * Math.Sin(radY);
            double z1 = x * Math.Sin(radY) + z * Math.Cos(radY);

            double y2 = y * Math.Cos(radX) - z1 * Math.Sin(radX);
            double z2 = y * Math.Sin(radX) + z1 * Math.Cos(radX);

            float screenX = (float)(CenterX + x1 * Scale);
            float screenY = (float)(CenterY - y2 * Scale);

            return new PointF(screenX, screenY);
        }

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