using System;
using System.Drawing;
using PixelLab.ColorSystems.Base;
using PixelLab.ColorSystems.Models;

namespace PixelLab.ColorSystems.Converters
{
    public sealed class HsvConverter : ColorSpaceConverter
    {
        public override string SystemName { get { return "HSV"; } }

        public override string[] ChannelNames
        {
            get { return new string[] { "H (Hue)", "S (Saturation)", "V (Value)" }; }
        }

        public override ColorResult FromRgb(Color rgb)
        {
            double r = rgb.R / 255.0;
            double g = rgb.G / 255.0;
            double b = rgb.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double diff = max - min;

            double s = (max == 0) ? 0 : diff / max;
            double v = max;
            double h = ComputeHue(r, g, b, max, diff);

            return new ColorResult(
                SystemName,
                "H", Math.Round(h, 2),
                "S", Math.Round(s, 4),
                "V", Math.Round(v, 4));
        }

        public override Color ToRgb(double h, double s, double v)
        {
            if (s == 0)
            {
                int gray = Clamp((int)(v * 255));
                return Color.FromArgb(gray, gray, gray);
            }

            int sector = (int)(h / 60) % 6;
            double frac = (h / 60) - Math.Floor(h / 60);
            double p = v * (1 - s);
            double q = v * (1 - frac * s);
            double t = v * (1 - (1 - frac) * s);

            double r, g, b;

            if (sector == 0) { r = v; g = t; b = p; }
            else if (sector == 1) { r = q; g = v; b = p; }
            else if (sector == 2) { r = p; g = v; b = t; }
            else if (sector == 3) { r = p; g = q; b = v; }
            else if (sector == 4) { r = t; g = p; b = v; }
            else { r = v; g = p; b = q; }

            return Color.FromArgb(
                Clamp((int)(r * 255)),
                Clamp((int)(g * 255)),
                Clamp((int)(b * 255)));
        }

        private static double ComputeHue(double r, double g, double b, double max, double diff)
        {
            if (diff == 0) return 0;

            double h;
            if (max == r) h = 60 * (((g - b) / diff) % 6);
            else if (max == g) h = 60 * (((b - r) / diff) + 2);
            else h = 60 * (((r - g) / diff) + 4);

            if (h < 0) h += 360;
            return h;
        }
    }
}