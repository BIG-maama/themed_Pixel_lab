using System;
using System.Drawing;
using PixelLab.ColorSystems.Base;
using PixelLab.ColorSystems.Models;

namespace PixelLab.ColorSystems.Converters
{
    public sealed class YuvConverter : ColorSpaceConverter
    {
        public override string SystemName { get { return "YUV"; } }

        public override string[] ChannelNames
        {
            get { return new string[] { "Y (Luma)", "U (Blue diff)", "V (Red diff)" }; }
        }

        public override ColorResult FromRgb(Color rgb)
        {
            double r = rgb.R;
            double g = rgb.G;
            double b = rgb.B;

            double y = 0.299 * r + 0.587 * g + 0.114 * b;
            double u = -0.14713 * r - 0.28886 * g + 0.436 * b;
            double v = 0.615 * r - 0.51499 * g - 0.10001 * b;

            return new ColorResult(
                SystemName,
                "Y", Math.Round(y, 2),
                "U", Math.Round(u, 2),
                "V", Math.Round(v, 2));
        }

        public override Color ToRgb(double y, double u, double v)
        {
            int r = Clamp((int)(y + 1.13983 * v));
            int g = Clamp((int)(y - 0.39465 * u - 0.58060 * v));
            int b = Clamp((int)(y + 2.03211 * u));
            return Color.FromArgb(r, g, b);
        }
    }
}