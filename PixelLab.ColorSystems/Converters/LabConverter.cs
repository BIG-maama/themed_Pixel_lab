using System;
using System.Drawing;
using PixelLab.ColorSystems.Base;
using PixelLab.ColorSystems.Models;

namespace PixelLab.ColorSystems.Converters
{
    public sealed class LabConverter : ColorSpaceConverter
    {
        public override string SystemName { get { return "L*a*b*"; } }

        public override string[] ChannelNames
        {
            get { return new string[] { "L* (Lightness)", "a* (Green-Red)", "b* (Blue-Yellow)" }; }
        }

        private const double Xn = 0.95047;
        private const double Yn = 1.00000;
        private const double Zn = 1.08883;

        public override ColorResult FromRgb(Color rgb)
        {
            double x, y, z;
            RgbToXyz(rgb, out x, out y, out z);

            double l = 116 * F(y / Yn) - 16;
            double a = 500 * (F(x / Xn) - F(y / Yn));
            double b = 200 * (F(y / Yn) - F(z / Zn));

            return new ColorResult(
                SystemName,
                "L*", Math.Round(l, 2),
                "a*", Math.Round(a, 2),
                "b*", Math.Round(b, 2));
        }

        public override Color ToRgb(double l, double a, double b)
        {
            double fy = (l + 16) / 116;
            double fx = a / 500 + fy;
            double fz = fy - b / 200;

            double x = Xn * FInv(fx);
            double y = Yn * FInv(fy);
            double z = Zn * FInv(fz);

            return XyzToRgb(x, y, z);
        }

        private static void RgbToXyz(Color rgb, out double x, out double y, out double z)
        {
            double r = GammaExpand(rgb.R / 255.0);
            double g = GammaExpand(rgb.G / 255.0);
            double b = GammaExpand(rgb.B / 255.0);

            x = r * 0.4124564 + g * 0.3575761 + b * 0.1804375;
            y = r * 0.2126729 + g * 0.7151522 + b * 0.0721750;
            z = r * 0.0193339 + g * 0.1191920 + b * 0.9503041;
        }

        private static Color XyzToRgb(double x, double y, double z)
        {
            double r = GammaCompress(x * 3.2404542 - y * 1.5371385 - z * 0.4985314);
            double g = GammaCompress(-x * 0.9692660 + y * 1.8760108 + z * 0.0415560);
            double b = GammaCompress(x * 0.0556434 - y * 0.2040259 + z * 1.0572252);

            return Color.FromArgb(
                Clamp((int)(r * 255)),
                Clamp((int)(g * 255)),
                Clamp((int)(b * 255)));
        }

        private static double F(double t)
        {
            if (t > 0.008856)
                return Math.Pow(t, 1.0 / 3.0);
            return 7.787 * t + 16.0 / 116.0;
        }

        private static double FInv(double t)
        {
            double t3 = t * t * t;
            if (t3 > 0.008856) return t3;
            return (t - 16.0 / 116.0) / 7.787;
        }
    }
}