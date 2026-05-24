using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PixelLab.ColorSystems;
using PixelLab.ColorSystems.Models;
using Label = System.Windows.Forms.Label;

namespace Homwore
{
    public class ColorSpaceViewer : Form
    {
        private Panel pnlCanvas;
        private Panel pnlInfo;
        private ComboBox cmbSystem;
        private Label lblSystem;
        private Label lblColorSwatch;
        private Label lblRGB;
        private Label lblHSV;
        private Label lblYUV;
        private Label lblYCbCr;
        private Label lblLab;
        private Label lblCMYK;
        private Label lblInstruction;
        private Label lblTitle;
        private Button btnReset;

        private Projection3D projection = new Projection3D();
        private bool isDragging = false;
        private bool hasDragged = false;
        private Point lastMouse;
        private double zoom = 1.0;

        private Color selectedColor = Color.FromArgb(0, 0, 255);
        private ColorSystemManager colorManager = new ColorSystemManager();
        private string currentSystem = "RGB";

        private struct ColorPoint3D
        {
            public double X, Y, Z;
            public Color C;
        }

        // ✅ ذاكرة مؤقتة للنقاط (للنقر السريع)
        private List<ColorPoint3D> hsvPoints = new List<ColorPoint3D>();
        private List<(PointF screen, Color color, double depth)> rgbPointsCache = new List<(PointF, Color, double)>();

        public ColorSpaceViewer()
        {
            InitializeUI();
            UpdateColorInfo(selectedColor);
        }

        private void InitializeUI()
        {
            this.Text = "PixelLab — Color Space Viewer 3D";
            this.Size = new Size(1024, 720);
            this.MinimumSize = new Size(900, 600);
            this.BackColor = Color.FromArgb(24, 24, 32);
            this.ForeColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);

            lblTitle = new Label
            {
                Text = "🌈 Color Space Viewer",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 200, 255),
                BackColor = Color.Transparent,
                Location = new Point(16, 12),
                AutoSize = true
            };

            lblSystem = new Label
            {
                Text = "Color System:",
                ForeColor = Color.FromArgb(180, 180, 200),
                Location = new Point(16, 52),
                AutoSize = true
            };

            cmbSystem = new ComboBox
            {
                Location = new Point(16, 72),
                Size = new Size(180, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(40, 40, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f)
            };
            cmbSystem.Items.AddRange(new string[] { "RGB", "HSV", "Lab (2D)", "YUV (2D)", "YCbCr (2D)", "CMYK (Info)" });
            cmbSystem.SelectedIndex = 0;
            cmbSystem.SelectedIndexChanged += (s, e) =>
            {
                currentSystem = cmbSystem.SelectedItem.ToString();
                if (currentSystem == "HSV") BuildHSVCache();
                if (currentSystem == "RGB") BuildRGBCache();
                pnlCanvas.Invalidate();
            };

            btnReset = new Button
            {
                Text = "⟳ Reset View",
                Location = new Point(210, 68),
                Size = new Size(110, 28),
                BackColor = Color.FromArgb(60, 60, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9f)
            };
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 120);
            btnReset.Click += (s, e) =>
            {
                projection.AngleX = 30;
                projection.AngleY = -45;
                zoom = 1.0;
                projection.Scale = 200 * zoom;
                if (currentSystem == "HSV") BuildHSVCache();
                if (currentSystem == "RGB") BuildRGBCache();
                pnlCanvas.Invalidate();
            };

            lblInstruction = new Label
            {
                Text = "🖱 Drag to rotate   •   Scroll to zoom   •   Click a point to pick color",
                ForeColor = Color.FromArgb(120, 120, 150),
                Location = new Point(16, 108),
                AutoSize = true,
                Font = new Font("Segoe UI", 8f)
            };

            pnlCanvas = new Panel
            {
                Location = new Point(8, 130),
                BackColor = Color.FromArgb(18, 18, 26),
                BorderStyle = BorderStyle.None,
                Cursor = Cursors.Cross
            };
            pnlCanvas.Paint += PnlCanvas_Paint;
            pnlCanvas.MouseDown += PnlCanvas_MouseDown;
            pnlCanvas.MouseMove += PnlCanvas_MouseMove;
            pnlCanvas.MouseUp += (s, e) => { isDragging = false; };
            pnlCanvas.MouseWheel += PnlCanvas_MouseWheel;
            pnlCanvas.MouseClick += PnlCanvas_MouseClick;

            pnlInfo = new Panel
            {
                BackColor = Color.FromArgb(30, 30, 42),
                BorderStyle = BorderStyle.None
            };

            lblColorSwatch = new Label
            {
                Location = new Point(12, 12),
                Size = new Size(200, 50),
                BackColor = selectedColor,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "",
                Cursor = Cursors.Hand
            };

            lblRGB = MakeInfoLabel(70);
            lblHSV = MakeInfoLabel(100);
            lblYUV = MakeInfoLabel(130);
            lblYCbCr = MakeInfoLabel(160);
            lblLab = MakeInfoLabel(190);
            lblCMYK = MakeInfoLabel(220);

            var btnPick = new Button
            {
                Text = "🎨 Pick Color",
                Location = new Point(12, 255),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(60, 60, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPick.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 120);
            btnPick.Click += (s, e) =>
            {
                using (var dlg = new ColorDialog())
                {
                    dlg.Color = selectedColor;
                    dlg.FullOpen = true;
                    if (dlg.ShowDialog() == DialogResult.OK)
                        UpdateColorInfo(dlg.Color);
                }
            };

            pnlInfo.Controls.AddRange(new Control[] {
                lblColorSwatch, btnPick, lblRGB, lblHSV, lblYUV, lblYCbCr, lblLab, lblCMYK
            });

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSystem, cmbSystem, btnReset,
                lblInstruction, pnlCanvas, pnlInfo
            });

            this.Resize += (s, e) => LayoutControls();
            this.Shown += (s, e) => { LayoutControls(); BuildRGBCache(); BuildHSVCache(); };

            projection.AngleX = 30;
            projection.AngleY = -45;
            projection.Scale = 200;
        }

        private Label MakeInfoLabel(int y)
        {
            return new Label
            {
                Location = new Point(12, y),
                Size = new Size(210, 22),
                ForeColor = Color.FromArgb(200, 200, 220),
                BackColor = Color.Transparent,
                Font = new Font("Consolas", 8.5f),
                AutoSize = false
            };
        }

        private void LayoutControls()
        {
            int infoW = 250;
            int margin = 8;
            int canvasX = margin;
            int canvasY = 130;
            int canvasW = this.ClientSize.Width - infoW - margin * 3;
            int canvasH = this.ClientSize.Height - canvasY - margin;

            pnlCanvas.SetBounds(canvasX, canvasY, canvasW, canvasH);
            pnlInfo.SetBounds(canvasX + canvasW + margin, margin, infoW, this.ClientSize.Height - margin * 2);

            projection.CenterX = canvasW / 2;
            projection.CenterY = canvasH / 2;
            pnlCanvas.Invalidate();
        }

        private void PnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(18, 18, 26));

            if (currentSystem == "RGB")
                DrawRGBCube(g);
            else if (currentSystem == "HSV")
                DrawHSVCone(g);
            else if (currentSystem == "Lab (2D)")
                DrawLabPlane(g);
            else if (currentSystem == "YUV (2D)")
                DrawYUVPlane(g);
            else if (currentSystem == "YCbCr (2D)")
                DrawYCbCrPlane(g);
            else if (currentSystem == "CMYK (Info)")
                DrawCMYKInfo(g);

            DrawSelectedColorMarker(g);
            DrawLegend(g);
        }

        
        private void BuildRGBCache()
        {
            rgbPointsCache.Clear();
            int step = 12;
            for (int ri = 0; ri <= step; ri++)
            {
                double r = ri / (double)step;
                for (int gi = 0; gi <= step; gi++)
                {
                    double gr = gi / (double)step;
                    for (int bi = 0; bi <= step; bi++)
                    {
                        double b = bi / (double)step;
                        bool onSurface = ri == 0 || ri == step || gi == 0 || gi == step || bi == 0 || bi == step;
                        if (!onSurface) continue;

                        PointF pt = projection.Project(r - 0.5, gr - 0.5, b - 0.5);
                        double dep = projection.GetDepth(r - 0.5, gr - 0.5, b - 0.5);
                        rgbPointsCache.Add((pt, Color.FromArgb((int)(r * 255), (int)(gr * 255), (int)(b * 255)), dep));
                    }
                }
            }
            rgbPointsCache.Sort((a, b2) => a.depth.CompareTo(b2.depth));
        }

        private void BuildHSVCache()
        {
            hsvPoints.Clear();
            int hSteps = 36, sSteps = 8, vSteps = 8;
            for (int vi = 0; vi <= vSteps; vi++)
            {
                double v = vi / (double)vSteps;
                double radius = v;
                for (int si = 0; si <= sSteps; si++)
                {
                    double s = si / (double)sSteps;
                    for (int hi = 0; hi < hSteps; hi++)
                    {
                        double h = hi * 360.0 / hSteps;
                        Color col = HsvToRgb(h, s, v);
                        double angle = h * Math.PI / 180.0;
                        double x = radius * s * Math.Cos(angle);
                        double z = radius * s * Math.Sin(angle);
                        double y = v - 0.5;
                        hsvPoints.Add(new ColorPoint3D { X = x * 0.5, Y = y, Z = z * 0.5, C = col });
                    }
                }
            }
        }

        private void DrawRGBCube(Graphics g)
        {
            if (rgbPointsCache.Count == 0) BuildRGBCache();

            foreach (var p in rgbPointsCache)
            {
                using (var brush = new SolidBrush(p.color))
                    g.FillEllipse(brush, p.screen.X - 3, p.screen.Y - 3, 6, 6);
            }

            DrawAxis(g, -0.5, -0.5, -0.5, 0.5, -0.5, -0.5, Color.Red, "R");
            DrawAxis(g, -0.5, -0.5, -0.5, -0.5, 0.5, -0.5, Color.Lime, "G");
            DrawAxis(g, -0.5, -0.5, -0.5, -0.5, -0.5, 0.5, Color.DeepSkyBlue, "B");
        }

        private void DrawAxis(Graphics g, double x1, double y1, double z1,
                                           double x2, double y2, double z2,
                                           Color col, string label)
        {
            PointF p1 = projection.Project(x1, y1, z1);
            PointF p2 = projection.Project(x2, y2, z2);
            using (var pen = new Pen(col, 1.5f))
                g.DrawLine(pen, p1, p2);
            using (var brush = new SolidBrush(col))
            using (var font = new Font("Segoe UI", 9f, FontStyle.Bold))
                g.DrawString(label, font, brush, p2.X + 4, p2.Y - 8);
        }

        private void DrawHSVCone(Graphics g)
        {
            if (hsvPoints.Count == 0) BuildHSVCache();

            var points = new List<(PointF screen, Color color, double depth)>();
            foreach (var cp in hsvPoints)
            {
                PointF pt = projection.Project(cp.X, cp.Y, cp.Z);
                double dep = projection.GetDepth(cp.X, cp.Y, cp.Z);
                points.Add((pt, cp.C, dep));
            }

            points.Sort((a, b2) => a.depth.CompareTo(b2.depth));

            foreach (var p in points)
            {
                using (var brush = new SolidBrush(p.color))
                    g.FillEllipse(brush, p.screen.X - 3, p.screen.Y - 3, 6, 6);
            }

            DrawAxis(g, 0, -0.5, 0, 0, 0.5, 0, Color.White, "V");
        }

        private Color HsvToRgb(double h, double s, double v)
        {
            if (s == 0) { int g2 = (int)(v * 255); return Color.FromArgb(g2, g2, g2); }
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

        private void DrawLabPlane(Graphics g)
        {
            int w = pnlCanvas.Width, h = pnlCanvas.Height;
            int step = 4;

            for (int ax = -128; ax <= 127; ax += step)
            {
                for (int bx = -128; bx <= 127; bx += step)
                {
                    Color col = LabToRgb(60, ax, bx);
                    int sx = (int)(w / 2 + ax * (w / 280.0));
                    int sy = (int)(h / 2 - bx * (h / 280.0));
                    using (var brush = new SolidBrush(col))
                        g.FillRectangle(brush, sx, sy, step - 1, step - 1);
                }
            }

            using (var pen = new Pen(Color.FromArgb(100, 100, 100), 1))
            {
                g.DrawLine(pen, 0, h / 2, w, h / 2);
                g.DrawLine(pen, w / 2, 0, w / 2, h);
            }

            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(Color.FromArgb(180, 180, 200)))
            {
                g.DrawString("a* (Green → Red)", font, brush, w - 140, h / 2 + 6);
                g.DrawString("b*", font, brush, w / 2 + 6, 6);
                g.DrawString("(Blue ↓  Yellow ↑)", font, brush, w / 2 + 6, 22);
                g.DrawString("L* = 60 (fixed)", font, brush, 8, 8);
            }
        }

        private Color LabToRgb(double l, double a, double b)
        {
            double fy = (l + 16) / 116;
            double fx = a / 500 + fy;
            double fz = fy - b / 200;
            double x = 0.95047 * FInvLab(fx);
            double y = 1.00000 * FInvLab(fy);
            double z = 1.08883 * FInvLab(fz);
            double r = GammaCompress(3.2404542 * x - 1.5371385 * y - 0.4985314 * z);
            double g2 = GammaCompress(-0.9692660 * x + 1.8760108 * y + 0.0415560 * z);
            double bv = GammaCompress(0.0556434 * x - 0.2040259 * y + 1.0572252 * z);
            return Color.FromArgb(Clamp(r), Clamp(g2), Clamp(bv));
        }

        private double FInvLab(double t)
        {
            double t3 = t * t * t;
            return t3 > 0.008856 ? t3 : (t - 16.0 / 116.0) / 7.787;
        }

        private double GammaCompress(double c)
        {
            if (c < 0) c = 0;
            return c <= 0.0031308 ? 12.92 * c : 1.055 * Math.Pow(c, 1.0 / 2.4) - 0.055;
        }

        private void DrawYUVPlane(Graphics g)
        {
            int w = pnlCanvas.Width, h = pnlCanvas.Height;
            int step = 4;

            for (int u = -112; u <= 112; u += step)
            {
                for (int v2 = -112; v2 <= 112; v2 += step)
                {
                    double y2 = 128;
                    int r2 = Clamp((y2 + 1.13983 * v2) / 255.0);
                    int gv = Clamp((y2 - 0.39465 * u - 0.58060 * v2) / 255.0);
                    int bv = Clamp((y2 + 2.03211 * u) / 255.0);
                    Color col = Color.FromArgb(r2, gv, bv);
                    int sx = (int)(w / 2 + u * (w / 240.0));
                    int sy = (int)(h / 2 - v2 * (h / 240.0));
                    using (var brush = new SolidBrush(col))
                        g.FillRectangle(brush, sx, sy, step - 1, step - 1);
                }
            }

            using (var pen = new Pen(Color.FromArgb(100, 100, 100), 1))
            {
                g.DrawLine(pen, 0, h / 2, w, h / 2);
                g.DrawLine(pen, w / 2, 0, w / 2, h);
            }

            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(Color.FromArgb(180, 180, 200)))
            {
                g.DrawString("U (Blue difference)", font, brush, w - 160, h / 2 + 6);
                g.DrawString("V (Red difference)", font, brush, w / 2 + 6, 6);
                g.DrawString("Y = 128 (fixed)", font, brush, 8, 8);
            }
        }

        private void DrawYCbCrPlane(Graphics g)
        {
            int w = pnlCanvas.Width, h = pnlCanvas.Height;
            int step = 4;

            for (int cb = -112; cb <= 112; cb += step)
            {
                for (int cr = -112; cr <= 112; cr += step)
                {
                    double y2 = 128;
                    int r2 = Clamp((y2 + 1.402 * cr) / 255.0);
                    int gv = Clamp((y2 - 0.344136 * cb - 0.714136 * cr) / 255.0);
                    int bv = Clamp((y2 + 1.772 * cb) / 255.0);
                    Color col = Color.FromArgb(r2, gv, bv);
                    int sx = (int)(w / 2 + cb * (w / 240.0));
                    int sy = (int)(h / 2 - cr * (h / 240.0));
                    using (var brush = new SolidBrush(col))
                        g.FillRectangle(brush, sx, sy, step - 1, step - 1);
                }
            }

            using (var pen = new Pen(Color.FromArgb(100, 100, 100), 1))
            {
                g.DrawLine(pen, 0, h / 2, w, h / 2);
                g.DrawLine(pen, w / 2, 0, w / 2, h);
            }

            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(Color.FromArgb(180, 180, 200)))
            {
                g.DrawString("Cb (Blue chrominance)", font, brush, w - 180, h / 2 + 6);
                g.DrawString("Cr (Red chrominance)", font, brush, w / 2 + 6, 6);
                g.DrawString("Y = 128 (fixed)", font, brush, 8, 8);
            }
        }

        private void DrawCMYKInfo(Graphics g)
        {
            int w = pnlCanvas.Width, h = pnlCanvas.Height;
            using (var font = new Font("Segoe UI", 14f, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(0, 200, 255)))
                g.DrawString("CMYK Color Space", font, brush, w / 2 - 120, 30);

            using (var font = new Font("Segoe UI", 10f))
            using (var brush = new SolidBrush(Color.FromArgb(200, 200, 220)))
            {
                string info = "CMYK is a subtractive color model used in printing.\n\n" +
                    "It represents colors using four ink components:\n" +
                    "  • C = Cyan\n" +
                    "  • M = Magenta\n" +
                    "  • Y = Yellow\n" +
                    "  • K = Key (Black)\n\n" +
                    "Unlike RGB (additive, for screens), CMYK\n" +
                    "is used for physical color reproduction.\n\n" +
                    "Use the info panel on the right to see CMYK values\n" +
                    "for the selected color.";

                var lines = info.Split(new[] { "\n" }, StringSplitOptions.None);
                int y = 80;
                foreach (var line in lines)
                {
                    g.DrawString(line, font, brush, 40, y);
                    y += 22;
                }
            }

            using (var brush = new SolidBrush(selectedColor))
                g.FillRectangle(brush, w / 2 - 60, h - 160, 120, 80);
            using (var pen = new Pen(Color.White, 2))
                g.DrawRectangle(pen, w / 2 - 60, h - 160, 120, 80);
        }

        private void DrawSelectedColorMarker(Graphics g)
        {
            PointF pt = GetColorPosition(selectedColor);
            if (pt.X < 0 || pt.Y < 0) return;

            using (var pen = new Pen(Color.White, 2f))
                g.DrawEllipse(pen, pt.X - 8, pt.Y - 8, 16, 16);
            using (var pen = new Pen(Color.Black, 1f))
                g.DrawEllipse(pen, pt.X - 9, pt.Y - 9, 18, 18);
        }

        private PointF GetColorPosition(Color col)
        {
            if (currentSystem == "RGB")
            {
                double r = col.R / 255.0 - 0.5;
                double grn = col.G / 255.0 - 0.5;
                double b = col.B / 255.0 - 0.5;
                return projection.Project(r, grn, b);
            }
            if (currentSystem == "HSV")
            {
                double h, s, v;
                RgbToHsv(col.R, col.G, col.B, out h, out s, out v);
                double angle = h * Math.PI / 180.0;
                double radius = v;
                double x = radius * s * Math.Cos(angle) * 0.5;
                double z = radius * s * Math.Sin(angle) * 0.5;
                double y = v - 0.5;
                return projection.Project(x, y, z);
            }
            if (currentSystem == "Lab (2D)")
            {
                int w = pnlCanvas.Width, h = pnlCanvas.Height;
                var res = colorManager.ConvertTo("L*a*b*", col);
                float sx = (float)(w / 2 + res.Channel2 * (w / 280.0));
                float sy = (float)(h / 2 - res.Channel3 * (h / 280.0));
                return new PointF(sx, sy);
            }
            if (currentSystem == "YUV (2D)")
            {
                int w = pnlCanvas.Width, h = pnlCanvas.Height;
                var res = colorManager.ConvertTo("YUV", col);
                float sx = (float)(w / 2 + res.Channel2 * (w / 240.0));
                float sy = (float)(h / 2 - res.Channel3 * (h / 240.0));
                return new PointF(sx, sy);
            }
            if (currentSystem == "YCbCr (2D)")
            {
                int w = pnlCanvas.Width, h = pnlCanvas.Height;
                var res = colorManager.ConvertTo("YCbCr", col);
                float sx = (float)(w / 2 + (res.Channel2 - 128) * (w / 240.0));
                float sy = (float)(h / 2 - (res.Channel3 - 128) * (h / 240.0));
                return new PointF(sx, sy);
            }
            return new PointF(-1, -1);
        }

        private void RgbToHsv(int r, int g, int b, out double h, out double s, out double v)
        {
            double rd = r / 255.0, gd = g / 255.0, bd = b / 255.0;
            double max = Math.Max(rd, Math.Max(gd, bd));
            double min = Math.Min(rd, Math.Min(gd, bd));
            double diff = max - min;

            v = max;
            s = max == 0 ? 0 : diff / max;

            if (diff == 0) h = 0;
            else if (max == rd) h = (60 * ((gd - bd) / diff) + 360) % 360;
            else if (max == gd) h = (60 * ((bd - rd) / diff) + 120) % 360;
            else h = (60 * ((rd - gd) / diff) + 240) % 360;
        }

        private void DrawLegend(Graphics g)
        {
            string legend = currentSystem == "RGB" ? "RGB Color Cube — surface points" :
                            currentSystem == "HSV" ? "HSV Cone — Hue·Saturation·Value" :
                            currentSystem == "Lab (2D)" ? "CIE L*a*b* — chromaticity plane (L*=60)" :
                            currentSystem == "YCbCr (2D)" ? "YCbCr — Cb/Cr chrominance plane (Y=128)" :
                            currentSystem == "CMYK (Info)" ? "CMYK — Subtractive color model (Info)" :
                                                           "YUV — chromaticity plane (Y=128)";
            using (var font = new Font("Segoe UI", 8f))
            using (var brush = new SolidBrush(Color.FromArgb(130, 130, 160)))
                g.DrawString(legend, font, brush, 8, pnlCanvas.Height - 20);
        }

        private void UpdateColorInfo(Color col)
        {
            selectedColor = col;
            lblColorSwatch.BackColor = col;

            var results = colorManager.ConvertAll(col);

            lblRGB.Text = $"RGB    ({col.R}, {col.G}, {col.B})";

            // CMYK manual calculation
            double cmykC = 1 - col.R / 255.0;
            double cmykM = 1 - col.G / 255.0;
            double cmykY = 1 - col.B / 255.0;
            double cmykK = Math.Min(cmykC, Math.Min(cmykM, cmykY));
            if (cmykK < 1) { cmykC = (cmykC - cmykK) / (1 - cmykK); cmykM = (cmykM - cmykK) / (1 - cmykK); cmykY = (cmykY - cmykK) / (1 - cmykK); }
            else { cmykC = cmykM = cmykY = 0; }
            lblCMYK.Text = $"CMYK   C:{cmykC * 100:F0}% M:{cmykM * 100:F0}% Y:{cmykY * 100:F0}% K:{cmykK * 100:F0}%";

            foreach (var r in results)
            {
                if (r.SystemName == "HSV")
                    lblHSV.Text = $"HSV    H:{r.Channel1:F0}°  S:{r.Channel2:F2}  V:{r.Channel3:F2}";
                else if (r.SystemName == "YUV")
                    lblYUV.Text = $"YUV    Y:{r.Channel1:F1}  U:{r.Channel2:F1}  V:{r.Channel3:F1}";
                else if (r.SystemName == "YCbCr")
                    lblYCbCr.Text = $"YCbCr  Y:{r.Channel1:F1}  Cb:{r.Channel2:F1}  Cr:{r.Channel3:F1}";
                else if (r.SystemName == "L*a*b*")
                    lblLab.Text = $"L*a*b* L:{r.Channel1:F1}  a:{r.Channel2:F1}  b:{r.Channel3:F1}";
            }

            pnlCanvas.Invalidate();
        }

        private void PnlCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                hasDragged = false;
                lastMouse = e.Location;
            }
        }

        private void PnlCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;
            int dx = e.X - lastMouse.X;
            int dy = e.Y - lastMouse.Y;

            if (Math.Abs(dx) > 3 || Math.Abs(dy) > 3)
                hasDragged = true;

            projection.AngleY += dx * 0.5;
            projection.AngleX += dy * 0.5;

            lastMouse = e.Location;

            if (currentSystem == "RGB") BuildRGBCache();
            if (currentSystem == "HSV") BuildHSVCache();

            pnlCanvas.Invalidate();
        }

        private void PnlCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            zoom += e.Delta > 0 ? 0.1 : -0.1;
            zoom = Math.Max(0.3, Math.Min(zoom, 3.0));
            projection.Scale = 200 * zoom;
            if (currentSystem == "RGB") BuildRGBCache();
            if (currentSystem == "HSV") BuildHSVCache();
            pnlCanvas.Invalidate();
        }


        private void PnlCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (hasDragged) return;

            if (currentSystem == "Lab (2D)")
            {
                int w = pnlCanvas.Width, h = pnlCanvas.Height;
                double a = (e.X - w / 2) / (w / 280.0);
                double b = -(e.Y - h / 2) / (h / 280.0);
                Color col = LabToRgb(60, a, b);
                UpdateColorInfo(col);
                return;
            }

            if (currentSystem == "YUV (2D)")
            {
                int w = pnlCanvas.Width, h = pnlCanvas.Height;
                double u = (e.X - w / 2) / (w / 240.0);
                double v = -(e.Y - h / 2) / (h / 240.0);
                int r2 = Clamp((128 + 1.13983 * v) / 255.0);
                int g2 = Clamp((128 - 0.39465 * u - 0.58060 * v) / 255.0);
                int b2 = Clamp((128 + 2.03211 * u) / 255.0);
                UpdateColorInfo(Color.FromArgb(r2, g2, b2));
                return;
            }

            if (currentSystem == "YCbCr (2D)")
            {
                int w = pnlCanvas.Width, h = pnlCanvas.Height;
                double cb = (e.X - w / 2) / (w / 240.0);
                double cr = -(e.Y - h / 2) / (h / 240.0);
                int r2 = Clamp((128 + 1.402 * cr) / 255.0);
                int g2 = Clamp((128 - 0.344136 * cb - 0.714136 * cr) / 255.0);
                int b2 = Clamp((128 + 1.772 * cb) / 255.0);
                UpdateColorInfo(Color.FromArgb(r2, g2, b2));
                return;
            }

           
            if (currentSystem == "HSV")
            {
                Color closest = FindClosestHSV(e.Location);
                if (closest != Color.Empty)
                    UpdateColorInfo(closest);
                return;
            }

            
            Color closestRGB = FindClosestColor(e.Location);
            if (closestRGB != Color.Empty)
                UpdateColorInfo(closestRGB);
        }

        private Color FindClosestColor(Point mouse)
        {
            double bestDist = 25;
            Color bestCol = Color.Empty;

            foreach (var p in rgbPointsCache)
            {
                double dist = Math.Sqrt(Math.Pow(p.screen.X - mouse.X, 2) + Math.Pow(p.screen.Y - mouse.Y, 2));
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestCol = p.color;
                }
            }
            return bestCol;
        }


        private Color FindClosestHSV(Point mouse)
        {
            double bestDist = 25;
            Color bestCol = Color.Empty;

            foreach (var cp in hsvPoints)
            {
                PointF pt = projection.Project(cp.X, cp.Y, cp.Z);
                double dist = Math.Sqrt(Math.Pow(pt.X - mouse.X, 2) + Math.Pow(pt.Y - mouse.Y, 2));
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestCol = cp.C;
                }
            }
            return bestCol;
        }

        private int Clamp(double v) => v < 0 ? 0 : v > 1 ? 255 : (int)(v * 255);
        private int Clamp(int v) => v < 0 ? 0 : v > 255 ? 255 : v;

        public void SetColor(Color col) => UpdateColorInfo(col);
    }
}