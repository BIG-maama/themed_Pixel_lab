using System;
using System.Drawing;
using PixelLab.ColorSystems.Base;
using PixelLab.ColorSystems.Models;

namespace PixelLab.ColorSystems.Converters
{
    public sealed class YCbCrConverter : ColorSpaceConverter
    {
        public override string SystemName { get { return "YCbCr"; } }

        public override string[] ChannelNames
        {
            get { return new string[] { "Y (Luma)", "Cb (Blue)", "Cr (Red)" }; }
        }

        public override ColorResult FromRgb(Color rgb)
        {
            double r = rgb.R;
            double g = rgb.G;
            double b = rgb.B;

            double y = 0.299 * r + 0.587 * g + 0.114 * b;
            double cb = -0.168736 * r - 0.331264 * g + 0.5 * b + 128;
            double cr = 0.5 * r - 0.418688 * g - 0.081312 * b + 128;

            return new ColorResult(
                SystemName,
                "Y", Math.Round(y, 2),
                "Cb", Math.Round(cb, 2),
                "Cr", Math.Round(cr, 2));
        }

        public override Color ToRgb(double y, double cb, double cr)
        {
            cb -= 128;
            cr -= 128;

            int r = Clamp((int)(y + 1.402 * cr));
            int g = Clamp((int)(y - 0.344136 * cb - 0.714136 * cr));
            int b = Clamp((int)(y + 1.772 * cb));
            return Color.FromArgb(r, g, b);
        }
    }
}