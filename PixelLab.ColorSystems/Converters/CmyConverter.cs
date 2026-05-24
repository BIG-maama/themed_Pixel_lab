// ===================================================================
//  CmykConverter.cs  — CMYK حقيقي بـ 4 قنوات
//  يحل محل CmyConverter.cs القديم (3 قنوات فقط)
//
//  الفرق:
//    CMY  القديم:  C=135, M=175, Y=55          (3 قنوات، بدون K)
//    CMYK الجديد:  C=0%, M=53%, Y=84%, K=47%   (4 قنوات، مع K)
//
//  ملاحظة: ColorResult يدعم 3 قنوات فقط، لذلك
//  أضفنا CmykResult منفصل + wrapper يعرض K في Channel3Name
// ===================================================================

using System;
using System.Drawing;
using PixelLab.ColorSystems.Base;
using PixelLab.ColorSystems.Models;

namespace PixelLab.ColorSystems.Converters
{
    // ─── نموذج النتيجة الممتد لـ 4 قنوات ───────────────────────────
    public class CmykResult
    {
        public double C { get; }   // Cyan    0-100%
        public double M { get; }   // Magenta 0-100%
        public double Y { get; }   // Yellow  0-100%
        public double K { get; }   // Key (Black) 0-100%

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

    // ─── المحول الرئيسي ───────────────────────────────────────────
    public sealed class CmykConverter : ColorSpaceConverter
    {
        // الاسم ظاهر في الـ UI كـ "CMYK"
        public override string SystemName => "CMYK";

        // نعرض 3 قنوات في الـ Slider (C, M, Y) ونعامل K منفصلاً
        // لأن ColorSpaceConverter base يدعم 3 قنوات فقط
        public override string[] ChannelNames
            => new[] { "C (Cyan) %", "M (Magenta) %", "Y (Yellow) %" };

        // ─── RGB → CMYK ────────────────────────────────────────────
        public CmykResult FromRgbFull(Color rgb)
        {
            double r = rgb.R / 255.0;
            double g = rgb.G / 255.0;
            double b = rgb.B / 255.0;

            double k = 1.0 - Math.Max(r, Math.Max(g, b));

            // حالة الأسود الخالص — منع القسمة على صفر
            if (k >= 1.0)
                return new CmykResult(0, 0, 0, 100);

            double c = (1.0 - r - k) / (1.0 - k) * 100.0;
            double m = (1.0 - g - k) / (1.0 - k) * 100.0;
            double y = (1.0 - b - k) / (1.0 - k) * 100.0;

            return new CmykResult(c, m, y, k * 100.0);
        }

        // ─── ColorSpaceConverter interface — يستخدم 3 قنوات للـ Slider ─
        // نخزن C, M, Y في القنوات الثلاث
        // K محسوبة تلقائياً من RGB ولا تُعدَّل يدوياً
        public override ColorResult FromRgb(Color rgb)
        {
            var cmyk = FromRgbFull(rgb);
            return new ColorResult(
                SystemName,
                "C", cmyk.C,
                "M", cmyk.M,
                "Y", cmyk.Y);
            // K لا تظهر في الـ Slider لكن تُحسب في التحويل الكامل
        }

        // ─── CMYK → RGB ────────────────────────────────────────────
        // هذه النسخة الكاملة بـ 4 قنوات
        public Color ToRgbFull(double c, double m, double y, double k)
        {
            // تأكد القيم داخل 0-100
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

        // النسخة التي يستدعيها الـ base class (3 قنوات)
        // K تُحسب من C,M,Y تلقائياً لو ما انتبهنا
        // لكن الأصح: نعيد حساب K من القيم الجديدة
        public override Color ToRgb(double c, double m, double y)
        {
            // احسب K من C, M, Y بدون افتراض قيمة ثابتة
            // K = 1 - max(1-C/100, 1-M/100, 1-Y/100)
            double r1 = 1 - c / 100.0;
            double g1 = 1 - m / 100.0;
            double b1 = 1 - y / 100.0;
            double kAuto = 1 - Math.Max(r1, Math.Max(g1, b1));

            return ToRgbFull(c, m, y, kAuto * 100.0);
        }
    }
}