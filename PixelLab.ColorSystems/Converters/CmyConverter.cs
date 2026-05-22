using System.Drawing;
using PixelLab.ColorSystems.Base;
using PixelLab.ColorSystems.Models;

namespace PixelLab.ColorSystems.Converters
{
    public sealed class CmyConverter : ColorSpaceConverter
    {
        public override string SystemName { get { return "CMY"; } }

        public override string[] ChannelNames
        {
            get { return new string[] { "C (Cyan)", "M (Magenta)", "Y (Yellow)" }; }
        }

        public override ColorResult FromRgb(Color rgb)
        {
            double c = 255 - rgb.R;
            double m = 255 - rgb.G;
            double y = 255 - rgb.B;

            return new ColorResult(
                SystemName,
                "C", c,
                "M", m,
                "Y", y);
        }

        public override Color ToRgb(double c, double m, double y)
        {
            int r = Clamp(255 - (int)c);
            int g = Clamp(255 - (int)m);
            int b = Clamp(255 - (int)y);
            return Color.FromArgb(r, g, b);
        }
    }
}