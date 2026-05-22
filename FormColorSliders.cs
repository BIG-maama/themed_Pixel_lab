using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PixelLab.ColorSystems;

namespace Homwore
{
    public class FormColorSliders : Form
    {
        public event Action<Bitmap> ImageModified;
        private ColorSystemManager manager;
        private TrackBar[] sliders = new TrackBar[3];
        private Label[] lblNames = new Label[3];
        private Label[] lblValues = new Label[3];
        private Panel colorPreview;
        private ComboBox cmbSystem;
        private string currentSystem = "RGB";
        private Bitmap originalImage;
        private PictureBox picResult;
        private Label lblStatus;
        private CheckBox chkShowEffect;

        // إزاحات القنوات
        private double offset1 = 0, offset2 = 0, offset3 = 0;

        // ✅ FIX: Debouncing Timer — تأخير 200ms قبل التطبيق
        private Timer _sliderTimer;

        public FormColorSliders()
        {
            this.Text = "Color Sliders - تعديل النظام اللوني";
            this.Size = new Size(900, 700);
            this.BackColor = Color.FromArgb(245, 245, 250);
            manager = new ColorSystemManager();

            // ✅ FIX: إعداد Debouncing Timer
            _sliderTimer = new Timer();
            _sliderTimer.Interval = 200; // 200ms انتظر حتى يتوقف المستخدم
            _sliderTimer.Tick += (s, e) => { _sliderTimer.Stop(); ApplyToImage(null, null); };

            SetupUI();
        }

        private void SetupUI()
        {
            var lblTitle = new Label
            {
                Text = "🎚️ Color System Sliders",
                Location = new Point(20, 15),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 200)
            };
            this.Controls.Add(lblTitle);

            cmbSystem = new ComboBox
            {
                Location = new Point(20, 55),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            cmbSystem.Items.AddRange(manager.AvailableSystems.ToArray());
            if (cmbSystem.Items.Count > 0)
            {
                cmbSystem.SelectedIndex = 0;
                currentSystem = cmbSystem.Items[0].ToString();
            }
            cmbSystem.SelectedIndexChanged += ComboChanged;
            this.Controls.Add(cmbSystem);

            var lblPreview = new Label
            {
                Text = "Preview:",
                Location = new Point(250, 55),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblPreview);

            colorPreview = new Panel
            {
                Location = new Point(320, 50),
                Size = new Size(80, 40),
                BackColor = Color.Black,
                BorderStyle = BorderStyle.Fixed3D
            };
            this.Controls.Add(colorPreview);

            chkShowEffect = new CheckBox
            {
                Text = "إظهار تأثير واضح (عكس/تعديل)",
                Location = new Point(420, 55),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9)
            };
            this.Controls.Add(chkShowEffect);

            for (int i = 0; i < 3; i++)
            {
                lblNames[i] = new Label
                {
                    Location = new Point(20, 100 + i * 70),
                    Size = new Size(120, 25),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(60, 60, 80)
                };

                sliders[i] = new TrackBar
                {
                    Location = new Point(150, 95 + i * 70),
                    Size = new Size(350, 50),
                    Minimum = -100,
                    Maximum = 100,
                    Value = 0,
                    TickFrequency = 10,
                    BackColor = Color.FromArgb(245, 245, 250)
                };
                sliders[i].Scroll += Slider_Scroll;

                lblValues[i] = new Label
                {
                    Location = new Point(510, 100 + i * 70),
                    Size = new Size(80, 25),
                    Font = new Font("Consolas", 10),
                    ForeColor = Color.FromArgb(100, 100, 120)
                };

                this.Controls.Add(lblNames[i]);
                this.Controls.Add(sliders[i]);
                this.Controls.Add(lblValues[i]);
            }

            var btnReset = new Button
            {
                Text = "↺ Reset",
                Location = new Point(20, 320),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(100, 100, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += (s, e) =>
            {
                for (int i = 0; i < 3; i++) sliders[i].Value = 0;
                offset1 = offset2 = offset3 = 0;
                UpdateSliderDisplay();
                ApplyToImage(null, null);
            };
            this.Controls.Add(btnReset);

            var btnApply = new Button
            {
                Text = "🔄 تطبيق على الصورة",
                Location = new Point(140, 320),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(0, 150, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += ApplyToImage;
            this.Controls.Add(btnApply);

            lblStatus = new Label
            {
                Location = new Point(20, 365),
                Size = new Size(600, 22),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };
            this.Controls.Add(lblStatus);

            picResult = new PictureBox
            {
                Location = new Point(20, 395),
                Size = new Size(520, 240),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(220, 220, 220),
                BorderStyle = BorderStyle.Fixed3D
            };
            this.Controls.Add(picResult);

            UpdateSliderLabels();
            UpdateSliderDisplay();
        }

        private void ComboChanged(object sender, EventArgs e)
        {
            if (cmbSystem.SelectedItem == null) return;
            currentSystem = cmbSystem.SelectedItem.ToString();
            UpdateSliderLabels();
            for (int i = 0; i < 3; i++) sliders[i].Value = 0;
            offset1 = offset2 = offset3 = 0;
            UpdateSliderDisplay();
        }

        private void UpdateSliderLabels()
        {
            if (string.IsNullOrEmpty(currentSystem)) return;
            string[] names = manager.GetChannelNames(currentSystem);
            for (int i = 0; i < 3 && i < names.Length; i++)
                lblNames[i].Text = names[i];
        }

        private void UpdateSliderDisplay()
        {
            lblValues[0].Text = $"{offset1:F1}";
            lblValues[1].Text = $"{offset2:F1}";
            lblValues[2].Text = $"{offset3:F1}";

            var midColor = manager.UpdateChannel(currentSystem,
                128 + offset1,
                currentSystem == "HSV" || currentSystem == "HSL" ? offset2 : offset2 / 100.0,
                currentSystem == "HSV" || currentSystem == "HSL" ? offset3 : offset3 / 100.0);
            colorPreview.BackColor = midColor;
        }

        private void Slider_Scroll(object sender, EventArgs e)
        {
            offset1 = sliders[0].Value;
            offset2 = sliders[1].Value;
            offset3 = sliders[2].Value;
            UpdateSliderDisplay();

            // ✅ FIX: Debouncing — أعد المؤقت بدل التطبيق الفوري
            _sliderTimer.Stop();
            _sliderTimer.Start();
        }

        private void ApplyToImage(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                lblStatus.Text = "⚠️ لا توجد صورة محملة.";
                lblStatus.ForeColor = Color.OrangeRed;
                return;
            }

            lblStatus.Text = "⏳ جاري المعالجة...";
            lblStatus.ForeColor = Color.Blue;
            Application.DoEvents();

            try
            {
                Bitmap result = new Bitmap(originalImage.Width, originalImage.Height);

                BitmapData srcData = originalImage.LockBits(
                    new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData dstData = result.LockBits(
                    new Rectangle(0, 0, result.Width, result.Height),
                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                try
                {
                    int stride = srcData.Stride;
                    byte[] srcBuffer = new byte[Math.Abs(stride) * originalImage.Height];
                    byte[] dstBuffer = new byte[Math.Abs(stride) * result.Height];
                    Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBuffer.Length);

                    for (int y = 0; y < originalImage.Height; y++)
                    {
                        for (int x = 0; x < originalImage.Width; x++)
                        {
                            int idx = y * stride + x * 3;
                            byte b = srcBuffer[idx];
                            byte g = srcBuffer[idx + 1];
                            byte r = srcBuffer[idx + 2];
                            Color pixel = Color.FromArgb(r, g, b);

                            // 1. تحويل البكسل للنظام اللوني المختار
                            var converted = manager.ConvertTo(currentSystem, pixel);

                            // 2. تطبيق الإزاحات
                            double newC1 = converted.Channel1 + offset1;
                            double newC2 = converted.Channel2 + offset2;
                            double newC3 = converted.Channel3 + offset3;

                            // 3. إذا كان مفعل "إظهار تأثير واضح"
                            if (chkShowEffect.Checked)
                            {
                                (newC1, newC2, newC3) = ApplyVisualEffect(currentSystem, newC1, newC2, newC3);
                            }

                            // 4. تحويل العكسي
                            Color newColor = manager.UpdateChannel(currentSystem, newC1, newC2, newC3);

                            dstBuffer[idx] = newColor.B;
                            dstBuffer[idx + 1] = newColor.G;
                            dstBuffer[idx + 2] = newColor.R;
                        }
                    }
                    Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
                }
                finally
                {
                    originalImage.UnlockBits(srcData);
                    result.UnlockBits(dstData);
                }

                // ✅ FIX: تحرير الصورة القديمة قبل تعيين الجديدة
                picResult.Image?.Dispose();
                picResult.Image = result;
                ImageModified?.Invoke(result);
                lblStatus.Text = $"✅ تم التطبيق - {currentSystem} | الإزاحات: ({offset1:F1}, {offset2:F1}, {offset3:F1})";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ خطأ: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private (double, double, double) ApplyVisualEffect(string system, double c1, double c2, double c3)
        {
            switch (system)
            {
                case "HSV":
                    return ((c1 + 180) % 360, Math.Min(100, c2 * 1.3), c3);
                case "HSL":
                    return ((c1 + 180) % 360, c2, Math.Min(100, c3 * 0.8));
                case "YUV":
                case "YCbCr":
                    return (c1 * 0.9, c2, c3);
                case "L*a*b*":
                    return (Math.Min(100, c1 * 1.1), c2 * 1.2, c3 * 1.2);
                case "CMY":
                    return (100 - c1, 100 - c2, 100 - c3);
                default:
                    return (c1, c2, c3);
            }
        }

        public void SetImage(Bitmap img)
        {
            // ✅ FIX: تحرير الصور القديمة
            originalImage?.Dispose();
            picResult.Image?.Dispose();

            originalImage = new Bitmap(img);
            picResult.Image = new Bitmap(img);
            lblStatus.Text = "📷 تم تحميل الصورة. حرك الأشرطة ثم اضغط تطبيق.";
            lblStatus.ForeColor = Color.Gray;
        }
    }
}