//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using PixelLab.ColorSystems.Base;
//using PixelLab.ColorSystems.Converters;
//using PixelLab.ColorSystems.Models;

//namespace PixelLab.ColorSystems
//{
//    /// <summary>
//    /// الكلاس الرئيسي — هذا اللي يستخدمه باقي المجموعة.
//    ///
//    /// مثال:
//    ///   var manager = new ColorSystemManager();
//    ///   var results = manager.ConvertAll(color);
//    ///   Color newColor = manager.UpdateChannel("HSV", 260, 0.6, 0.78);
//    /// </summary>
//    public class ColorSystemManager
//    {
//        private readonly Dictionary<string, ColorSpaceConverter> _converters;

//        public ColorSystemManager()
//        {
//            _converters = new Dictionary<string, ColorSpaceConverter>();
//            _converters.Add("CMY", new CmyConverter());
//            _converters.Add("HSV", new HsvConverter());
//            _converters.Add("YUV", new YuvConverter());
//            _converters.Add("YCbCr", new YCbCrConverter());
//            _converters.Add("L*a*b*", new LabConverter());
//        }

//        // أسماء الأنظمة — لبناء الـ ComboBox في الواجهة
//        public List<string> AvailableSystems
//        {
//            get { return new List<string>(_converters.Keys); }
//        }

//        // تحويل لون لجميع الأنظمة مرة واحدة
//        public List<ColorResult> ConvertAll(Color rgb)
//        {
//            List<ColorResult> results = new List<ColorResult>();
//            foreach (ColorSpaceConverter converter in _converters.Values)
//                results.Add(converter.FromRgb(rgb));
//            return results;
//        }

//        // تحويل لنظام واحد محدد
//        public ColorResult ConvertTo(string systemName, Color rgb)
//        {
//            if (!_converters.ContainsKey(systemName))
//                throw new ArgumentException("النظام غير موجود: " + systemName);
//            return _converters[systemName].FromRgb(rgb);
//        }

//        // تحديث قيم المركبات وإرجاع اللون الجديد (للـ Sliders)
//        public Color UpdateChannel(string systemName, double ch1, double ch2, double ch3)
//        {
//            if (!_converters.ContainsKey(systemName))
//                throw new ArgumentException("النظام غير موجود: " + systemName);
//            return _converters[systemName].ToRgb(ch1, ch2, ch3);
//        }

//        // أسماء مركبات نظام معين (لتسمية الـ Sliders)
//        public string[] GetChannelNames(string systemName)
//        {
//            if (!_converters.ContainsKey(systemName))
//                throw new ArgumentException("النظام غير موجود: " + systemName);
//            return _converters[systemName].ChannelNames;
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PixelLab.ColorSystems
{
    namespace Models
    {
        public class ColorConversionResult
        {
            public string SystemName { get; set; }
            public string Channel1Name { get; set; }
            public string Channel2Name { get; set; }
            public string Channel3Name { get; set; }
            public double Channel1 { get; set; }
            public double Channel2 { get; set; }
            public double Channel3 { get; set; }

            public override string ToString()
            {
                return $"{SystemName}: {Channel1Name}={Channel1:F2}, {Channel2Name}={Channel2:F2}, {Channel3Name}={Channel3:F2}";
            }
        }
    }

    /// <summary>
    /// مدير الأنظمة اللونية - تحويلات دقيقة بين جميع الأنظمة
    /// </summary>
    public class ColorSystemManager
    {
        private List<string> _systems = new List<string> { "RGB", "CMY", "HSV", "HSL", "YUV", "YCbCr", "L*a*b*", "XYZ" };

        public List<string> AvailableSystems => _systems;

        public string[] GetChannelNames(string systemName)
        {
            switch (systemName)
            {
                case "RGB": return new[] { "R", "G", "B" };
                case "CMY": return new[] { "C", "M", "Y" };
                case "HSV": return new[] { "H", "S", "V" };
                case "HSL": return new[] { "H", "S", "L" };
                case "YUV": return new[] { "Y", "U", "V" };
                case "YCbCr": return new[] { "Y", "Cb", "Cr" };
                case "L*a*b*": return new[] { "L*", "a*", "b*" };
                case "XYZ": return new[] { "X", "Y", "Z" };
                default: return new[] { "Ch1", "Ch2", "Ch3" };
            }
        }

        public List<Models.ColorConversionResult> ConvertAll(Color color)
        {
            var results = new List<Models.ColorConversionResult>();

            results.Add(ConvertTo("HSV", color));
            results.Add(ConvertTo("HSL", color));
            results.Add(ConvertTo("YUV", color));
            results.Add(ConvertTo("YCbCr", color));
            results.Add(ConvertTo("L*a*b*", color));
            results.Add(ConvertTo("XYZ", color));
            results.Add(ConvertTo("CMY", color));

            return results;
        }

        public Models.ColorConversionResult ConvertTo(string systemName, Color color)
        {
            var result = new Models.ColorConversionResult { SystemName = systemName };
            var names = GetChannelNames(systemName);
            result.Channel1Name = names[0];
            result.Channel2Name = names[1];
            result.Channel3Name = names[2];

            switch (systemName)
            {
                case "RGB":
                    result.Channel1 = color.R;
                    result.Channel2 = color.G;
                    result.Channel3 = color.B;
                    break;

                case "CMY":
                    result.Channel1 = (1 - color.R / 255.0) * 100;
                    result.Channel2 = (1 - color.G / 255.0) * 100;
                    result.Channel3 = (1 - color.B / 255.0) * 100;
                    break;

                case "HSV":
                    RgbToHsv(color.R, color.G, color.B, out double h, out double s, out double v);
                    result.Channel1 = h;
                    result.Channel2 = s * 100;
                    result.Channel3 = v * 100;
                    break;

                case "HSL":
                    RgbToHsl(color.R, color.G, color.B, out double h2, out double s2, out double l);
                    result.Channel1 = h2;
                    result.Channel2 = s2 * 100;
                    result.Channel3 = l * 100;
                    break;

                case "YUV":
                    result.Channel1 = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
                    result.Channel2 = -0.14713 * color.R - 0.28886 * color.G + 0.436 * color.B;
                    result.Channel3 = 0.615 * color.R - 0.51499 * color.G - 0.10001 * color.B;
                    break;

                case "YCbCr":
                    result.Channel1 = 16 + (65.481 * color.R / 255.0 + 128.553 * color.G / 255.0 + 24.966 * color.B / 255.0);
                    result.Channel2 = 128 + (-37.797 * color.R / 255.0 - 74.203 * color.G / 255.0 + 112.0 * color.B / 255.0);
                    result.Channel3 = 128 + (112.0 * color.R / 255.0 - 93.786 * color.G / 255.0 - 18.214 * color.B / 255.0);
                    break;

                case "L*a*b*":
                    var lab = RgbToLab(color.R, color.G, color.B);
                    result.Channel1 = lab.L;
                    result.Channel2 = lab.a;
                    result.Channel3 = lab.b;
                    break;

                case "XYZ":
                    var xyz = RgbToXyz(color.R, color.G, color.B);
                    result.Channel1 = xyz.X;
                    result.Channel2 = xyz.Y;
                    result.Channel3 = xyz.Z;
                    break;
            }

            return result;
        }

        public Color UpdateChannel(string systemName, double v1, double v2, double v3)
        {
            switch (systemName)
            {
                case "RGB":
                    return Color.FromArgb(
                        Clamp((int)v1),
                        Clamp((int)v2),
                        Clamp((int)v3));

                case "CMY":
                    int r = Clamp((int)((1 - v1 / 100.0) * 255));
                    int g = Clamp((int)((1 - v2 / 100.0) * 255));
                    int b = Clamp((int)((1 - v3 / 100.0) * 255));
                    return Color.FromArgb(r, g, b);

                case "HSV":
                    return HsvToRgb(v1, v2 / 100.0, v3 / 100.0);

                case "HSL":
                    return HslToRgb(v1, v2 / 100.0, v3 / 100.0);

                case "YUV":
                    return YuvToRgb(v1, v2, v3);

                case "YCbCr":
                    return YCbCrToRgb(v1, v2, v3);

                case "L*a*b*":
                    return LabToRgb(v1, v2, v3);

                case "XYZ":
                    return XyzToRgb(v1, v2, v3);

                default:
                    return Color.FromArgb(128, 128, 128);
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  RGB → HSV
        // ═══════════════════════════════════════════════════════════
        private void RgbToHsv(int r, int g, int b, out double h, out double s, out double v)
        {
            double rd = r / 255.0, gd = g / 255.0, bd = b / 255.0;
            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            double diff = max - min;

            v = max;
            s = max == 0 ? 0 : diff / max;

            if (diff == 0)
                h = 0;
            else if (max == rd)
                h = (60 * ((gd - bd) / diff) + 360) % 360;
            else if (max == gd)
                h = (60 * ((bd - rd) / diff) + 120) % 360;
            else
                h = (60 * ((rd - gd) / diff) + 240) % 360;
        }

        private Color HsvToRgb(double h, double s, double v)
        {
            if (s == 0) { int val = Clamp((int)(v * 255)); return Color.FromArgb(val, val, val); }
            int sector = (int)(h / 60) % 6;
            double frac = (h / 60) - Math.Floor(h / 60);
            double p = v * (1 - s), q = v * (1 - frac * s), t = v * (1 - (1 - frac) * s);
            double r, g, b;
            if (sector == 0) { r = v; g = t; b = p; }
            else if (sector == 1) { r = q; g = v; b = p; }
            else if (sector == 2) { r = p; g = v; b = t; }
            else if (sector == 3) { r = p; g = q; b = v; }
            else if (sector == 4) { r = t; g = p; b = v; }
            else { r = v; g = p; b = q; }
            return Color.FromArgb(Clamp(r), Clamp(g), Clamp(b));
        }

        // ═══════════════════════════════════════════════════════════
        //  RGB → HSL
        // ═══════════════════════════════════════════════════════════
        private void RgbToHsl(int r, int g, int b, out double h, out double s, out double l)
        {
            double rd = r / 255.0, gd = g / 255.0, bd = b / 255.0;
            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            double diff = max - min;

            l = (max + min) / 2.0;

            if (diff == 0)
            {
                h = 0; s = 0;
            }
            else
            {
                s = l > 0.5 ? diff / (2.0 - max - min) : diff / (max + min);
                if (max == rd) h = ((gd - bd) / diff + (gd < bd ? 6 : 0)) / 6.0 * 360;
                else if (max == gd) h = ((bd - rd) / diff + 2) / 6.0 * 360;
                else h = ((rd - gd) / diff + 4) / 6.0 * 360;
            }
        }

        private Color HslToRgb(double h, double s, double l)
        {
            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;
            double r, g, b;

            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return Color.FromArgb(
                Clamp(r + m),
                Clamp(g + m),
                Clamp(b + m));
        }

        // ═══════════════════════════════════════════════════════════
        //  YUV
        // ═══════════════════════════════════════════════════════════
        private Color YuvToRgb(double y, double u, double v)
        {
            int r = Clamp((int)(y + 1.13983 * v));
            int g = Clamp((int)(y - 0.39465 * u - 0.58060 * v));
            int b = Clamp((int)(y + 2.03211 * u));
            return Color.FromArgb(r, g, b);
        }

        // ═══════════════════════════════════════════════════════════
        //  YCbCr
        // ═══════════════════════════════════════════════════════════
        private Color YCbCrToRgb(double y, double cb, double cr)
        {
            double yNorm = (y - 16) / 219.0;
            double cbNorm = (cb - 128) / 224.0;
            double crNorm = (cr - 128) / 224.0;

            int r = Clamp((int)((yNorm + 1.402 * crNorm) * 255));
            int g = Clamp((int)((yNorm - 0.344136 * cbNorm - 0.714136 * crNorm) * 255));
            int b = Clamp((int)((yNorm + 1.772 * cbNorm) * 255));
            return Color.FromArgb(r, g, b);
        }

        // ═══════════════════════════════════════════════════════════
        //  LAB
        // ═══════════════════════════════════════════════════════════
        private (double L, double a, double b) RgbToLab(int r, int g, int b)
        {
            var xyz = RgbToXyz(r, g, b);
            double xRef = 95.047, yRef = 100.0, zRef = 108.883;

            double fx = FLab(xyz.X / xRef);
            double fy = FLab(xyz.Y / yRef);
            double fz = FLab(xyz.Z / zRef);

            double L = 116 * fy - 16;
            double a = 500 * (fx - fy);
            double b2 = 200 * (fy - fz);

            return (L, a, b2);
        }

        private Color LabToRgb(double l, double a, double b)
        {
            double yRef = 100.0, xRef = 95.047, zRef = 108.883;

            double fy = (l + 16) / 116;
            double fx = a / 500 + fy;
            double fz = fy - b / 200;

            double x = xRef * FInvLab(fx);
            double y = yRef * FInvLab(fy);
            double z = zRef * FInvLab(fz);

            return XyzToRgb(x, y, z);
        }

        private double FLab(double t) => t > 0.008856 ? Math.Pow(t, 1.0 / 3.0) : (7.787 * t + 16.0 / 116.0);
        private double FInvLab(double t)
        {
            double t3 = t * t * t;
            return t3 > 0.008856 ? t3 : (t - 16.0 / 116.0) / 7.787;
        }

        // ═══════════════════════════════════════════════════════════
        //  XYZ
        // ═══════════════════════════════════════════════════════════
        private (double X, double Y, double Z) RgbToXyz(int r, int g, int b)
        {
            double rd = r / 255.0, gd = g / 255.0, bd = b / 255.0;

            rd = rd > 0.04045 ? Math.Pow((rd + 0.055) / 1.055, 2.4) : rd / 12.92;
            gd = gd > 0.04045 ? Math.Pow((gd + 0.055) / 1.055, 2.4) : gd / 12.92;
            bd = bd > 0.04045 ? Math.Pow((bd + 0.055) / 1.055, 2.4) : bd / 12.92;

            double X = rd * 0.4124564 + gd * 0.3575761 + bd * 0.1804375;
            double Y = rd * 0.2126729 + gd * 0.7151522 + bd * 0.0721750;
            double Z = rd * 0.0193339 + gd * 0.1191920 + bd * 0.9503041;

            return (X * 100, Y * 100, Z * 100);
        }

        private Color XyzToRgb(double x, double y, double z)
        {
            x /= 100; y /= 100; z /= 100;

            double r = x * 3.2404542 + y * -1.5371385 + z * -0.4985314;
            double g = x * -0.9692660 + y * 1.8760108 + z * 0.0415560;
            double b = x * 0.0556434 + y * -0.2040259 + z * 1.0572252;

            r = r > 0.0031308 ? 1.055 * Math.Pow(r, 1.0 / 2.4) - 0.055 : 12.92 * r;
            g = g > 0.0031308 ? 1.055 * Math.Pow(g, 1.0 / 2.4) - 0.055 : 12.92 * g;
            b = b > 0.0031308 ? 1.055 * Math.Pow(b, 1.0 / 2.4) - 0.055 : 12.92 * b;

            return Color.FromArgb(Clamp(r), Clamp(g), Clamp(b));
        }

        private int Clamp(double v) => v < 0 ? 0 : v > 255 ? 255 : (int)(v + 0.5);
        private int Clamp(int v) => v < 0 ? 0 : v > 255 ? 255 : v;
    }
}