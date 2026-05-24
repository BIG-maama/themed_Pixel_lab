
using System;
using System.Drawing;
using PixelLab.ColorSystems.Base;
using PixelLab.ColorSystems.Models;

namespace PixelLab.ColorSystems.Converters
{
    public class CmykResult
    {
        public double C { get; }   
        public double M { get; }   
        public double Y { get; }   
        public double K { get; }   

        public CmykResult(double c, double m, double y, double k)
        {
            C = Math.Round(c, 2);
            M = Math.Round(m, 2);
            Y = Math.Round(y, 2);
            K = Math.Round(k, 2);
        }

        public override string ToString()
            => $"CMYK → C: {C:F1}%, M: {M:F1}%, Y: {Y:F1}%, K: {K:F1}%";
    }

    public sealed class CmykConverter : ColorSpaceConverter
    {
      
        public override string SystemName => "CMYK";

       
        public override string[] ChannelNames
            => new[] { "C (Cyan) %", "M (Magenta) %", "Y (Yellow) %" };

        public CmykResult FromRgbFull(Color rgb)
        {
            double r = rgb.R / 255.0;
            double g = rgb.G / 255.0;
            double b = rgb.B / 255.0;

            double k = 1.0 - Math.Max(r, Math.Max(g, b));

            if (k >= 1.0)
                return new CmykResult(0, 0, 0, 100);

            double c = (1.0 - r - k) / (1.0 - k) * 100.0;
            double m = (1.0 - g - k) / (1.0 - k) * 100.0;
            double y = (1.0 - b - k) / (1.0 - k) * 100.0;

            return new CmykResult(c, m, y, k * 100.0);
        }

     
        public override ColorResult FromRgb(Color rgb)
        {
            var cmyk = FromRgbFull(rgb);
            return new ColorResult(
                SystemName,
                "C", cmyk.C,
                "M", cmyk.M,
                "Y", cmyk.Y);
        }

     
        public Color ToRgbFull(double c, double m, double y, double k)
        {
          
            c = Math.Max(0, Math.Min(100, c));
            m = Math.Max(0, Math.Min(100, m));
            y = Math.Max(0, Math.Min(100, y));
            k = Math.Max(0, Math.Min(100, k));

            double cN = c / 100.0, mN = m / 100.0,
                   yN = y / 100.0, kN = k / 100.0;

            int r = Clamp((int)(255 * (1 - cN) * (1 - kN)));
            int g = Clamp((int)(255 * (1 - mN) * (1 - kN)));
            int b = Clamp((int)(255 * (1 - yN) * (1 - kN)));

            return Color.FromArgb(r, g, b);
        }

   
        public override Color ToRgb(double c, double m, double y)
        {
 
            double r1 = 1 - c / 100.0;
            double g1 = 1 - m / 100.0;
            double b1 = 1 - y / 100.0;
            double kAuto = 1 - Math.Max(r1, Math.Max(g1, b1));

            return ToRgbFull(c, m, y, kAuto * 100.0);
        }
    }
}