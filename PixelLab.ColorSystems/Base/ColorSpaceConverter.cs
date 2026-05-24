using System;
using System.Drawing;
using PixelLab.ColorSystems.Models;

namespace PixelLab.ColorSystems.Base
{
    public abstract class ColorSpaceConverter
    {
        public abstract string SystemName { get; }
        public abstract string[] ChannelNames { get; }

        public abstract ColorResult FromRgb(Color rgb);
        public abstract Color ToRgb(double ch1, double ch2, double ch3);

        protected static int Clamp(int v)
        {
            if (v < 0) return 0;
            if (v > 255) return 255;
            return v;
        }

        protected static double GammaExpand(double c)
        {
            if (c <= 0.04045)
                return c / 12.92;
            return Math.Pow((c + 0.055) / 1.055, 2.4);
        }

        protected static double GammaCompress(double c)
        {
            if (c <= 0.0031308)
                return 12.92 * c;
            return 1.055 * Math.Pow(c, 1.0 / 2.4) - 0.055;
        }
    }
}