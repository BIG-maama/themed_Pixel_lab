////using System;
////using System.Drawing;
////using System.Drawing.Imaging;
////using System.Runtime.InteropServices;
////using System.Windows.Forms;
////using PixelLab.ColorSystems;

////namespace Homwore
////{
////    public class FormColorSliders : Form
////    {
////        public event Action<Bitmap> ImageModified;
////        private ColorSystemManager manager;
////        private TrackBar[] sliders = new TrackBar[3];
////        private Label[] lblNames = new Label[3];
////        private Label[] lblValues = new Label[3];
////        private Panel colorPreview;
////        private ComboBox cmbSystem;
////        private string currentSystem = "RGB";
////        private Bitmap originalImage;
////        private PictureBox picResult;
////        private Label lblStatus;
////        private CheckBox chkShowEffect;


////        private double offset1 = 0, offset2 = 0, offset3 = 0;


////        private Timer _sliderTimer;

////        public FormColorSliders()
////        {
////            this.Text = "Color Sliders - تعديل النظام اللوني";
////            this.Size = new Size(900, 700);
////            this.BackColor = Color.FromArgb(245, 245, 250);
////            manager = new ColorSystemManager();


////            _sliderTimer = new Timer();
////            _sliderTimer.Interval = 200; 
////            _sliderTimer.Tick += (s, e) => { _sliderTimer.Stop(); ApplyToImage(null, null); };

////            SetupUI();
////        }

////        private void SetupUI()
////        {
////            var lblTitle = new Label
////            {
////                Text = "⚔  Color System Sliders",
////                Location = new Point(20, 15),
////                Size = new Size(400, 30),
////                Font = new Font("Segoe UI", 14, FontStyle.Bold),
////                ForeColor = Color.FromArgb(0, 100, 200)
////            };
////            this.Controls.Add(lblTitle);

////            cmbSystem = new ComboBox
////            {
////                Location = new Point(20, 55),
////                Size = new Size(200, 25),
////                DropDownStyle = ComboBoxStyle.DropDownList,
////                BackColor = Color.White,
////                Font = new Font("Segoe UI", 10)
////            };
////            cmbSystem.Items.AddRange(manager.AvailableSystems.ToArray());
////            if (cmbSystem.Items.Count > 0)
////            {
////                cmbSystem.SelectedIndex = 0;
////                currentSystem = cmbSystem.Items[0].ToString();
////            }
////            cmbSystem.SelectedIndexChanged += ComboChanged;
////            this.Controls.Add(cmbSystem);

////            var lblPreview = new Label
////            {
////                Text = "Preview:",
////                Location = new Point(250, 55),
////                AutoSize = true,
////                Font = new Font("Segoe UI", 10, FontStyle.Bold)
////            };
////            this.Controls.Add(lblPreview);

////            colorPreview = new Panel
////            {
////                Location = new Point(320, 50),
////                Size = new Size(80, 40),
////                BackColor = Color.Black,
////                BorderStyle = BorderStyle.Fixed3D
////            };
////            this.Controls.Add(colorPreview);

////            chkShowEffect = new CheckBox
////            {
////                Text = "إظهار تأثير واضح (عكس/تعديل)",
////                Location = new Point(420, 55),
////                AutoSize = true,
////                Checked = true,
////                Font = new Font("Segoe UI", 9)
////            };
////            this.Controls.Add(chkShowEffect);

////            for (int i = 0; i < 3; i++)
////            {
////                lblNames[i] = new Label
////                {
////                    Location = new Point(20, 100 + i * 70),
////                    Size = new Size(120, 25),
////                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
////                    ForeColor = Color.FromArgb(60, 60, 80)
////                };

////                sliders[i] = new TrackBar
////                {
////                    Location = new Point(150, 95 + i * 70),
////                    Size = new Size(350, 50),
////                    Minimum = -100,
////                    Maximum = 100,
////                    Value = 0,
////                    TickFrequency = 10,
////                    BackColor = Color.FromArgb(245, 245, 250)
////                };
////                sliders[i].Scroll += Slider_Scroll;

////                lblValues[i] = new Label
////                {
////                    Location = new Point(510, 100 + i * 70),
////                    Size = new Size(80, 25),
////                    Font = new Font("Consolas", 10),
////                    ForeColor = Color.FromArgb(100, 100, 120)
////                };

////                this.Controls.Add(lblNames[i]);
////                this.Controls.Add(sliders[i]);
////                this.Controls.Add(lblValues[i]);
////            }

////            var btnReset = new Button
////            {
////                Text = "↺ Reset",
////                Location = new Point(20, 320),
////                Size = new Size(100, 35),
////                BackColor = Color.FromArgb(100, 100, 120),
////                ForeColor = Color.White,
////                FlatStyle = FlatStyle.Flat,
////                Font = new Font("Segoe UI", 9, FontStyle.Bold),
////                Cursor = Cursors.Hand
////            };
////            btnReset.FlatAppearance.BorderSize = 0;
////            btnReset.Click += (s, e) =>
////            {
////                for (int i = 0; i < 3; i++) sliders[i].Value = 0;
////                offset1 = offset2 = offset3 = 0;
////                UpdateSliderDisplay();
////                ApplyToImage(null, null);
////            };
////            this.Controls.Add(btnReset);

////            var btnApply = new Button
////            {
////                Text = "🔄 تطبيق على الصورة",
////                Location = new Point(140, 320),
////                Size = new Size(200, 35),
////                BackColor = Color.FromArgb(0, 150, 80),
////                ForeColor = Color.White,
////                FlatStyle = FlatStyle.Flat,
////                Font = new Font("Segoe UI", 10, FontStyle.Bold),
////                Cursor = Cursors.Hand
////            };
////            btnApply.FlatAppearance.BorderSize = 0;
////            btnApply.Click += ApplyToImage;
////            this.Controls.Add(btnApply);

////            lblStatus = new Label
////            {
////                Location = new Point(20, 365),
////                Size = new Size(600, 22),
////                Font = new Font("Segoe UI", 9),
////                ForeColor = Color.Gray
////            };
////            this.Controls.Add(lblStatus);

////            picResult = new PictureBox
////            {
////                Location = new Point(20, 395),
////                Size = new Size(520, 240),
////                SizeMode = PictureBoxSizeMode.Zoom,
////                BackColor = Color.FromArgb(220, 220, 220),
////                BorderStyle = BorderStyle.Fixed3D
////            };
////            this.Controls.Add(picResult);

////            UpdateSliderLabels();
////            UpdateSliderDisplay();
////        }

////        private void ComboChanged(object sender, EventArgs e)
////        {
////            if (cmbSystem.SelectedItem == null) return;
////            currentSystem = cmbSystem.SelectedItem.ToString();
////            UpdateSliderLabels();
////            for (int i = 0; i < 3; i++) sliders[i].Value = 0;
////            offset1 = offset2 = offset3 = 0;
////            UpdateSliderDisplay();
////        }

////        private void UpdateSliderLabels()
////        {
////            if (string.IsNullOrEmpty(currentSystem)) return;
////            string[] names = manager.GetChannelNames(currentSystem);
////            for (int i = 0; i < 3 && i < names.Length; i++)
////                lblNames[i].Text = names[i];
////        }

////        private void UpdateSliderDisplay()
////        {
////            lblValues[0].Text = $"{offset1:F1}";
////            lblValues[1].Text = $"{offset2:F1}";
////            lblValues[2].Text = $"{offset3:F1}";

////            var midColor = manager.UpdateChannel(currentSystem,
////                128 + offset1,
////                currentSystem == "HSV" || currentSystem == "HSL" ? offset2 : offset2 / 100.0,
////                currentSystem == "HSV" || currentSystem == "HSL" ? offset3 : offset3 / 100.0);
////            colorPreview.BackColor = midColor;
////        }

////        private void Slider_Scroll(object sender, EventArgs e)
////        {
////            offset1 = sliders[0].Value;
////            offset2 = sliders[1].Value;
////            offset3 = sliders[2].Value;
////            UpdateSliderDisplay();


////            _sliderTimer.Stop();
////            _sliderTimer.Start();
////        }

////        private void ApplyToImage(object sender, EventArgs e)
////        {
////            if (originalImage == null)
////            {
////                lblStatus.Text = "⚠️ لا توجد صورة محملة.";
////                lblStatus.ForeColor = Color.OrangeRed;
////                return;
////            }

////            lblStatus.Text = "⏳ جاري المعالجة...";
////            lblStatus.ForeColor = Color.Blue;
////            Application.DoEvents();

////            try
////            {
////                Bitmap result = new Bitmap(originalImage.Width, originalImage.Height);

////                BitmapData srcData = originalImage.LockBits(
////                    new Rectangle(0, 0, originalImage.Width, originalImage.Height),
////                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
////                BitmapData dstData = result.LockBits(
////                    new Rectangle(0, 0, result.Width, result.Height),
////                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

////                try
////                {
////                    int stride = srcData.Stride;
////                    byte[] srcBuffer = new byte[Math.Abs(stride) * originalImage.Height];
////                    byte[] dstBuffer = new byte[Math.Abs(stride) * result.Height];
////                    Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBuffer.Length);

////                    for (int y = 0; y < originalImage.Height; y++)
////                    {
////                        for (int x = 0; x < originalImage.Width; x++)
////                        {
////                            int idx = y * stride + x * 3;
////                            byte b = srcBuffer[idx];
////                            byte g = srcBuffer[idx + 1];
////                            byte r = srcBuffer[idx + 2];
////                            Color pixel = Color.FromArgb(r, g, b);

////                            var converted = manager.ConvertTo(currentSystem, pixel);

////                            double newC1 = converted.Channel1 + offset1;
////                            double newC2 = converted.Channel2 + offset2;
////                            double newC3 = converted.Channel3 + offset3;


////                            if (chkShowEffect.Checked)
////                            {
////                                (newC1, newC2, newC3) = ApplyVisualEffect(currentSystem, newC1, newC2, newC3);
////                            }


////                            Color newColor = manager.UpdateChannel(currentSystem, newC1, newC2, newC3);

////                            dstBuffer[idx] = newColor.B;
////                            dstBuffer[idx + 1] = newColor.G;
////                            dstBuffer[idx + 2] = newColor.R;
////                        }
////                    }
////                    Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
////                }
////                finally
////                {
////                    originalImage.UnlockBits(srcData);
////                    result.UnlockBits(dstData);
////                }


////                picResult.Image?.Dispose();
////                picResult.Image = result;
////                ImageModified?.Invoke(result);
////                lblStatus.Text = $"✅ تم التطبيق - {currentSystem} | الإزاحات: ({offset1:F1}, {offset2:F1}, {offset3:F1})";
////                lblStatus.ForeColor = Color.Green;
////            }
////            catch (Exception ex)
////            {
////                lblStatus.Text = $"✗ خطأ: {ex.Message}";
////                lblStatus.ForeColor = Color.Red;
////            }
////        }

////        private (double, double, double) ApplyVisualEffect(string system, double c1, double c2, double c3)
////        {
////            switch (system)
////            {
////                case "HSV":
////                    return ((c1 + 180) % 360, Math.Min(100, c2 * 1.3), c3);
////                case "HSL":
////                    return ((c1 + 180) % 360, c2, Math.Min(100, c3 * 0.8));
////                case "YUV":
////                case "YCbCr":
////                    return (c1 * 0.9, c2, c3);
////                case "L*a*b*":
////                    return (Math.Min(100, c1 * 1.1), c2 * 1.2, c3 * 1.2);
////                case "CMY":
////                    return (100 - c1, 100 - c2, 100 - c3);
////                default:
////                    return (c1, c2, c3);
////            }
////        }

////        public void SetImage(Bitmap img)
////        {

////            originalImage?.Dispose();
////            picResult.Image?.Dispose();

////            originalImage = new Bitmap(img);
////            picResult.Image = new Bitmap(img);
////            lblStatus.Text = "📷 تم تحميل الصورة. حرك الأشرطة ثم اضغط تطبيق.";
////            lblStatus.ForeColor = Color.Gray;
////        }
////    }
////}
//// ===================================================================
////  FormColorSliders_FIXED.cs
////  الإصلاح: UpdateSliderDisplay كانت تمرر قيم غلط لـ UpdateChannel
////  الآن: كل نظام يحسب midpoint صحيح بناءً على نطاقه الحقيقي
//// ===================================================================

//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Runtime.InteropServices;
//using System.Windows.Forms;
//using PixelLab.ColorSystems;

//namespace Homwore
//{
//    public class FormColorSliders : Form
//    {
//        public event Action<Bitmap> ImageModified;

//        private ColorSystemManager manager;
//        private TrackBar[] sliders = new TrackBar[3];
//        private Label[] lblNames = new Label[3];
//        private Label[] lblValues = new Label[3];
//        private Panel colorPreview;
//        private ComboBox cmbSystem;
//        private string currentSystem = "HSV";
//        private Bitmap originalImage;
//        private PictureBox picResult;
//        private Label lblStatus;
//        private CheckBox chkShowEffect;

//      //  private double offset1 = 0, offset2 = 0, offset3 = 0;
//        private Timer _sliderTimer;

//        public FormColorSliders()
//        {
//            this.Text = "Color Sliders - تعديل النظام اللوني";
//            this.Size = new Size(900, 700);
//            this.BackColor = Color.FromArgb(245, 245, 250);
//            manager = new ColorSystemManager();

//            _sliderTimer = new Timer();
//            _sliderTimer.Interval = 200;
//            _sliderTimer.Tick += (s, e) => { _sliderTimer.Stop(); ApplyToImage(null, null); };

//            SetupUI();
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  النطاق الحقيقي لكل قناة في كل نظام
//        //  هذا هو جوهر الإصلاح — بدونه القيم المحسوبة خطأ
//        // ═══════════════════════════════════════════════════════════
//        private (double min, double max) GetChannelRange(string system, int channelIndex)
//        {
//            switch (system)
//            {
//                case "HSV":
//                    if (channelIndex == 0) return (0, 360);   // H: 0-360 درجة
//                    return (0, 100);                           // S,V: 0-100%

//                case "HSL":
//                    if (channelIndex == 0) return (0, 360);   // H: 0-360
//                    return (0, 100);                           // S,L: 0-100%

//                case "YUV":
//                    if (channelIndex == 0) return (0, 255);   // Y: 0-255
//                    if (channelIndex == 1) return (-111, 111); // U: -111 to +111
//                    return (-157, 157);                        // V: -157 to +157

//                case "YCbCr":
//                    if (channelIndex == 0) return (16, 235);  // Y: 16-235
//                    return (16, 240);                          // Cb, Cr: 16-240

//                case "L*a*b*":
//                    if (channelIndex == 0) return (0, 100);   // L*: 0-100
//                    return (-128, 127);                        // a*, b*: -128 to 127

//                case "CMY":
//                case "CMYK":
//                    return (0, 100);                           // كل القنوات: 0-100%

//                case "RGB":
//                default:
//                    return (0, 255);
//            }
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  تحويل قيمة الـ Slider (-100 to +100) إلى offset حقيقي
//        //  بناءً على نطاق القناة
//        // ═══════════════════════════════════════════════════════════
//        private double SliderToOffset(int sliderValue, string system, int channelIndex)
//        {
//            var (min, max) = GetChannelRange(system, channelIndex);
//            double range = max - min;
//            // slider 100 = 50% من النطاق الكامل
//            return (sliderValue / 100.0) * (range * 0.5);
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  الـ midpoint الصحيح لكل قناة (للـ preview)
//        // ═══════════════════════════════════════════════════════════
//        private double GetMidpoint(string system, int channelIndex)
//        {
//            var (min, max) = GetChannelRange(system, channelIndex);
//            return (min + max) / 2.0;
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  FIXED: UpdateSliderDisplay — كانت المشكلة هنا
//        // ═══════════════════════════════════════════════════════════
//        private void UpdateSliderDisplay()
//        {
//            // عرض قيمة الـ offset الحقيقية (بوحدة النظام)
//            double realOffset1 = SliderToOffset(sliders[0].Value, currentSystem, 0);
//            double realOffset2 = SliderToOffset(sliders[1].Value, currentSystem, 1);
//            double realOffset3 = SliderToOffset(sliders[2].Value, currentSystem, 2);

//            lblValues[0].Text = $"{realOffset1:+0.0;-0.0;0}";
//            lblValues[1].Text = $"{realOffset2:+0.0;-0.0;0}";
//            lblValues[2].Text = $"{realOffset3:+0.0;-0.0;0}";

//            // ─── الإصلاح الأساسي ───────────────────────────────
//            // نحسب لون المعاينة من midpoint + offset لكل قناة
//            // بدل ما كانت تحسب 128 + offset بشكل ثابت للكل
//            double previewCh1 = GetMidpoint(currentSystem, 0) + realOffset1;
//            double previewCh2 = GetMidpoint(currentSystem, 1) + realOffset2;
//            double previewCh3 = GetMidpoint(currentSystem, 2) + realOffset3;

//            // Clamp داخل النطاق
//            var (min1, max1) = GetChannelRange(currentSystem, 0);
//            var (min2, max2) = GetChannelRange(currentSystem, 1);
//            var (min3, max3) = GetChannelRange(currentSystem, 2);
//            previewCh1 = Math.Max(min1, Math.Min(max1, previewCh1));
//            previewCh2 = Math.Max(min2, Math.Min(max2, previewCh2));
//            previewCh3 = Math.Max(min3, Math.Min(max3, previewCh3));

//            try
//            {
//                Color previewColor = manager.UpdateChannel(currentSystem, previewCh1, previewCh2, previewCh3);
//                colorPreview.BackColor = previewColor;
//            }
//            catch
//            {
//                colorPreview.BackColor = Color.Gray;
//            }
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  FIXED: ApplyToImage — يستخدم GetChannelRange بشكل صحيح
//        // ═══════════════════════════════════════════════════════════
//        private void ApplyToImage(object sender, EventArgs e)
//        {
//            if (originalImage == null)
//            {
//                lblStatus.Text = "⚠️ لا توجد صورة محملة.";
//                lblStatus.ForeColor = Color.OrangeRed;
//                return;
//            }

//            lblStatus.Text = "⏳ جاري المعالجة...";
//            lblStatus.ForeColor = Color.Blue;
//            Application.DoEvents();

//            // احسب الـ offsets الحقيقية مرة واحدة قبل الـ loop
//            double realOffset1 = SliderToOffset(sliders[0].Value, currentSystem, 0);
//            double realOffset2 = SliderToOffset(sliders[1].Value, currentSystem, 1);
//            double realOffset3 = SliderToOffset(sliders[2].Value, currentSystem, 2);

//            var (min1, max1) = GetChannelRange(currentSystem, 0);
//            var (min2, max2) = GetChannelRange(currentSystem, 1);
//            var (min3, max3) = GetChannelRange(currentSystem, 2);

//            try
//            {
//                Bitmap result = new Bitmap(originalImage.Width, originalImage.Height);

//                BitmapData srcData = originalImage.LockBits(
//                    new Rectangle(0, 0, originalImage.Width, originalImage.Height),
//                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
//                BitmapData dstData = result.LockBits(
//                    new Rectangle(0, 0, result.Width, result.Height),
//                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

//                try
//                {
//                    int stride = srcData.Stride;
//                    byte[] srcBuffer = new byte[Math.Abs(stride) * originalImage.Height];
//                    byte[] dstBuffer = new byte[Math.Abs(stride) * result.Height];
//                    Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBuffer.Length);

//                    for (int y = 0; y < originalImage.Height; y++)
//                    {
//                        for (int x = 0; x < originalImage.Width; x++)
//                        {
//                            int idx = y * stride + x * 3;
//                            byte bVal = srcBuffer[idx];
//                            byte gVal = srcBuffer[idx + 1];
//                            byte rVal = srcBuffer[idx + 2];

//                            Color pixel = Color.FromArgb(rVal, gVal, bVal);

//                            // تحويل للنظام المختار
//                            var converted = manager.ConvertTo(currentSystem, pixel);

//                            // تطبيق الـ offset + Clamp داخل النطاق
//                            double newC1 = Math.Max(min1, Math.Min(max1, converted.Channel1 + realOffset1));
//                            double newC2 = Math.Max(min2, Math.Min(max2, converted.Channel2 + realOffset2));
//                            double newC3 = Math.Max(min3, Math.Min(max3, converted.Channel3 + realOffset3));

//                            // تحويل رجوع لـ RGB
//                            Color newColor = manager.UpdateChannel(currentSystem, newC1, newC2, newC3);

//                            dstBuffer[idx] = newColor.B;
//                            dstBuffer[idx + 1] = newColor.G;
//                            dstBuffer[idx + 2] = newColor.R;
//                        }
//                    }
//                    Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
//                }
//                finally
//                {
//                    originalImage.UnlockBits(srcData);
//                    result.UnlockBits(dstData);
//                }

//                picResult.Image?.Dispose();
//                picResult.Image = result;
//                ImageModified?.Invoke(result);

//                lblStatus.Text = $"✅ تم — {currentSystem} | " +
//                                 $"Δ({realOffset1:+0.0;-0.0}, {realOffset2:+0.0;-0.0}, {realOffset3:+0.0;-0.0})";
//                lblStatus.ForeColor = Color.Green;
//            }
//            catch (Exception ex)
//            {
//                lblStatus.Text = $"✗ خطأ: {ex.Message}";
//                lblStatus.ForeColor = Color.Red;
//            }
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  باقي الكود — لم يتغير
//        // ═══════════════════════════════════════════════════════════
//        private void SetupUI()
//        {
//            var lblTitle = new Label
//            {
//                Text = "⚔  Color System Sliders",
//                Location = new Point(20, 15),
//                Size = new Size(400, 30),
//                Font = new Font("Segoe UI", 14, FontStyle.Bold),
//                ForeColor = Color.FromArgb(0, 100, 200)
//            };
//            this.Controls.Add(lblTitle);

//            cmbSystem = new ComboBox
//            {
//                Location = new Point(20, 55),
//                Size = new Size(200, 25),
//                DropDownStyle = ComboBoxStyle.DropDownList,
//                Font = new Font("Segoe UI", 10)
//            };
//            cmbSystem.Items.AddRange(manager.AvailableSystems.ToArray());
//            if (cmbSystem.Items.Count > 0)
//            {
//                cmbSystem.SelectedIndex = 0;
//                currentSystem = cmbSystem.Items[0].ToString();
//            }
//            cmbSystem.SelectedIndexChanged += ComboChanged;
//            this.Controls.Add(cmbSystem);

//            var lblPreview = new Label
//            {
//                Text = "Preview:",
//                Location = new Point(250, 58),
//                AutoSize = true,
//                Font = new Font("Segoe UI", 10, FontStyle.Bold)
//            };
//            this.Controls.Add(lblPreview);

//            colorPreview = new Panel
//            {
//                Location = new Point(320, 50),
//                Size = new Size(80, 40),
//                BackColor = Color.Black,
//                BorderStyle = BorderStyle.Fixed3D
//            };
//            this.Controls.Add(colorPreview);

//            chkShowEffect = new CheckBox
//            {
//                Text = "تطبيق الـ offset على الصورة",
//                Location = new Point(420, 58),
//                AutoSize = true,
//                Checked = true,
//                Font = new Font("Segoe UI", 9)
//            };
//            this.Controls.Add(chkShowEffect);

//            for (int i = 0; i < 3; i++)
//            {
//                lblNames[i] = new Label
//                {
//                    Location = new Point(20, 105 + i * 70),
//                    Size = new Size(130, 25),
//                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
//                    ForeColor = Color.FromArgb(60, 60, 80)
//                };

//                sliders[i] = new TrackBar
//                {
//                    Location = new Point(160, 100 + i * 70),
//                    Size = new Size(350, 50),
//                    Minimum = -100,
//                    Maximum = 100,
//                    Value = 0,
//                    TickFrequency = 10
//                };
//                sliders[i].Scroll += Slider_Scroll;

//                lblValues[i] = new Label
//                {
//                    Location = new Point(520, 105 + i * 70),
//                    Size = new Size(90, 25),
//                    Font = new Font("Consolas", 10),
//                    ForeColor = Color.FromArgb(100, 100, 120)
//                };

//                this.Controls.Add(lblNames[i]);
//                this.Controls.Add(sliders[i]);
//                this.Controls.Add(lblValues[i]);
//            }

//            var btnReset = new Button
//            {
//                Text = "↺ Reset",
//                Location = new Point(20, 325),
//                Size = new Size(100, 35),
//                BackColor = Color.FromArgb(100, 100, 120),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat,
//                Font = new Font("Segoe UI", 9, FontStyle.Bold),
//                Cursor = Cursors.Hand
//            };
//            btnReset.FlatAppearance.BorderSize = 0;
//            btnReset.Click += (s, e) =>
//            {
//                for (int i = 0; i < 3; i++) sliders[i].Value = 0;
//                UpdateSliderDisplay();
//                ApplyToImage(null, null);
//            };
//            this.Controls.Add(btnReset);

//            var btnApply = new Button
//            {
//                Text = "🔄 تطبيق على الصورة",
//                Location = new Point(140, 325),
//                Size = new Size(200, 35),
//                BackColor = Color.FromArgb(0, 150, 80),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat,
//                Font = new Font("Segoe UI", 10, FontStyle.Bold),
//                Cursor = Cursors.Hand
//            };
//            btnApply.FlatAppearance.BorderSize = 0;
//            btnApply.Click += ApplyToImage;
//            this.Controls.Add(btnApply);

//            lblStatus = new Label
//            {
//                Location = new Point(20, 370),
//                Size = new Size(700, 22),
//                Font = new Font("Segoe UI", 9),
//                ForeColor = Color.Gray
//            };
//            this.Controls.Add(lblStatus);

//            picResult = new PictureBox
//            {
//                Location = new Point(20, 400),
//                Size = new Size(520, 240),
//                SizeMode = PictureBoxSizeMode.Zoom,
//                BackColor = Color.FromArgb(220, 220, 220),
//                BorderStyle = BorderStyle.Fixed3D
//            };
//            this.Controls.Add(picResult);

//            UpdateSliderLabels();
//            UpdateSliderDisplay();
//        }

//        private void ComboChanged(object sender, EventArgs e)
//        {
//            if (cmbSystem.SelectedItem == null) return;
//            currentSystem = cmbSystem.SelectedItem.ToString();
//            UpdateSliderLabels();
//            for (int i = 0; i < 3; i++) sliders[i].Value = 0;
//            UpdateSliderDisplay();
//        }

//        private void UpdateSliderLabels()
//        {
//            if (string.IsNullOrEmpty(currentSystem)) return;
//            string[] names = manager.GetChannelNames(currentSystem);
//            for (int i = 0; i < 3 && i < names.Length; i++)
//            {
//                var (min, max) = GetChannelRange(currentSystem, i);
//                lblNames[i].Text = $"{names[i]} [{min:0}~{max:0}]";
//            }
//        }

//        private void Slider_Scroll(object sender, EventArgs e)
//        {
//            UpdateSliderDisplay();
//            _sliderTimer.Stop();
//            _sliderTimer.Start();
//        }

//        public void SetImage(Bitmap img)
//        {
//            originalImage?.Dispose();
//            picResult.Image?.Dispose();
//            originalImage = new Bitmap(img);
//            picResult.Image = new Bitmap(img);
//            lblStatus.Text = "📷 تم تحميل الصورة. حرك الأشرطة ثم اضغط تطبيق.";
//            lblStatus.ForeColor = Color.Gray;
//        }
//    }
//}
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private string currentSystem = "HSV";
        private Bitmap originalImage;
        private PictureBox picResult;
        private Label lblStatus;
        private CheckBox chkShowEffect;

        private Timer _sliderTimer;

        // ── ألوان الثيم ─────────────────────────────────────────────
        static readonly Color C_BgBase = Color.FromArgb(13, 15, 20);
        static readonly Color C_BgPanel = Color.FromArgb(18, 21, 28);
        static readonly Color C_BgCard = Color.FromArgb(24, 28, 38);
        static readonly Color C_BgHeader = Color.FromArgb(20, 24, 32);
        static readonly Color C_BgTitle = Color.FromArgb(11, 13, 18);
        static readonly Color C_Border = Color.FromArgb(30, 36, 56);
        static readonly Color C_BorderMid = Color.FromArgb(37, 45, 64);
        static readonly Color C_BorderAct = Color.FromArgb(58, 112, 176);
        static readonly Color C_Accent = Color.FromArgb(64, 156, 255);
        static readonly Color C_AccentDim = Color.FromArgb(26, 58, 106);
        static readonly Color C_TxtPri = Color.FromArgb(230, 235, 245);
        static readonly Color C_TxtSec = Color.FromArgb(200, 216, 238);
        static readonly Color C_TxtMuted = Color.FromArgb(90, 112, 144);
        static readonly Color C_TxtDim = Color.FromArgb(58, 74, 99);

        // ── ألوان القنوات الثلاث ─────────────────────────────────────
        static readonly Color C_Ch1 = Color.FromArgb(255, 200, 80);
        static readonly Color C_Ch1Dim = Color.FromArgb(60, 46, 12);
        static readonly Color C_Ch2 = Color.FromArgb(52, 211, 153);
        static readonly Color C_Ch2Dim = Color.FromArgb(14, 50, 35);
        static readonly Color C_Ch3 = Color.FromArgb(64, 156, 255);
        static readonly Color C_Ch3Dim = Color.FromArgb(14, 35, 70);

        // ── P/Invoke ──────────────────────────────────────────────────
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int l, int t, int r, int b, int ew, int eh);

        private static void SetRound(Control c, int rad = 8) =>
            c.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, c.Width + 1, c.Height + 1, rad, rad));

        // ════════════════════════════════════════════════════════════
        public FormColorSliders()
        {
            this.Text = "PixelLab — Color Sliders";
            this.Size = new Size(900, 720);
            this.BackColor = C_BgBase;
            this.ForeColor = C_TxtPri;

            manager = new ColorSystemManager();

            _sliderTimer = new Timer();
            _sliderTimer.Interval = 200;
            _sliderTimer.Tick += (s, e) => { _sliderTimer.Stop(); ApplyToImage(null, null); };

            BuildTitleBar();
            SetupUI();
        }

        // ════════════════════════════════════════════════════════════
        //  Title bar
        // ════════════════════════════════════════════════════════════
        private void BuildTitleBar()
        {
            var titleBar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = C_BgTitle };
            titleBar.Paint += (s, e) =>
            {
                using var br = new SolidBrush(C_Accent);
                e.Graphics.FillRectangle(br, new Rectangle(16, 11, 3, 26));
                using var pen = new Pen(C_Border, 1);
                e.Graphics.DrawLine(pen, 0, titleBar.Height - 1, titleBar.Width, titleBar.Height - 1);
            };
            var lblTitle = new Label
            {
                Text = "PixelLab",
                Location = new Point(28, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = C_TxtPri,
                BackColor = Color.Transparent
            };
            var lblSub = new Label
            {
                Text = "Color Sliders — تعديل النظام اللوني",
                Location = new Point(28, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 8f),
                ForeColor = C_TxtDim,
                BackColor = Color.Transparent
            };
            titleBar.Controls.AddRange(new Control[] { lblTitle, lblSub });
            this.Controls.Add(titleBar);
        }

        // ════════════════════════════════════════════════════════════
        //  SetupUI — نفس المنطق، ألوان مُحدَّثة فقط
        // ════════════════════════════════════════════════════════════
        private void SetupUI()
        {
            // المحتوى يبدأ تحت الـ title bar (y + 48)
            const int TOP = 58; // 48 titleBar + 10 margin

            // ── ComboBox ──────────────────────────────────────────────
            cmbSystem = new ComboBox
            {
                Location = new Point(20, TOP),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = C_BgHeader,
                ForeColor = C_TxtSec,
                Font = new Font("Segoe UI", 9f),
                FlatStyle = FlatStyle.Flat
            };
            StyleCombo(cmbSystem);
            cmbSystem.Items.AddRange(manager.AvailableSystems.ToArray());
            if (cmbSystem.Items.Count > 0)
            {
                cmbSystem.SelectedIndex = 0;
                currentSystem = cmbSystem.Items[0].ToString();
            }
            cmbSystem.SelectedIndexChanged += ComboChanged;
            this.Controls.Add(cmbSystem);

            // ── Preview label + panel ─────────────────────────────────
            var lblPreviewLbl = new Label
            {
                Text = "PREVIEW",
                Location = new Point(240, TOP + 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = C_Accent,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblPreviewLbl);

            colorPreview = new Panel
            {
                Location = new Point(310, TOP),
                Size = new Size(80, 30),
                BackColor = Color.Black,
                BorderStyle = BorderStyle.None
            };
            colorPreview.HandleCreated += (s, e) => SetRound(colorPreview, 6);
            colorPreview.Resize += (s, e) => SetRound(colorPreview, 6);
            colorPreview.Paint += (s, e) =>
            {
                using var pen = new Pen(C_BorderMid, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, colorPreview.Width - 1, colorPreview.Height - 1);
            };
            this.Controls.Add(colorPreview);

            // ── CheckBox ──────────────────────────────────────────────
            chkShowEffect = new CheckBox
            {
                Text = "تطبيق الـ offset على الصورة",
                Location = new Point(410, TOP + 4),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = C_TxtMuted,
                BackColor = Color.Transparent
            };
            this.Controls.Add(chkShowEffect);

            // ── Channel rows ──────────────────────────────────────────
            Color[] chAccent = { C_Ch1, C_Ch2, C_Ch3 };
            Color[] chDim = { C_Ch1Dim, C_Ch2Dim, C_Ch3Dim };

            for (int i = 0; i < 3; i++)
            {
                int yBase = TOP + 50 + i * 80;

                // header strip
                var strip = new Panel
                {
                    Location = new Point(20, yBase),
                    Size = new Size(840, 26),
                    BackColor = chDim[i]
                };
                int ci = i; // capture for lambda
                strip.Paint += (s, e) =>
                {
                    using var pen = new Pen(chAccent[ci], 1);
                    e.Graphics.DrawLine(pen, 0, 0, 0, ((Panel)s).Height);
                };

                lblNames[i] = new Label
                {
                    Location = new Point(8, 5),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                    ForeColor = chAccent[i],
                    BackColor = Color.Transparent
                };
                strip.Controls.Add(lblNames[i]);
                this.Controls.Add(strip);

                // slider
                sliders[i] = new TrackBar
                {
                    Location = new Point(20, yBase + 32),
                    Size = new Size(700, 40),
                    Minimum = -100,
                    Maximum = 100,
                    Value = 0,
                    TickFrequency = 10,
                    BackColor = C_BgPanel
                };
                sliders[i].Scroll += Slider_Scroll;
                this.Controls.Add(sliders[i]);

                // value label
                lblValues[i] = new Label
                {
                    Location = new Point(730, yBase + 38),
                    Size = new Size(100, 22),
                    Font = new Font("Cascadia Code", 9f),
                    ForeColor = chAccent[i],
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                this.Controls.Add(lblValues[i]);
            }

            // ── Buttons ───────────────────────────────────────────────
            int btnY = TOP + 50 + 3 * 80 + 10;

            var btnReset = MakeButton("↺  Reset", 20, btnY, 110, 34);
            btnReset.Click += (s, e) =>
            {
                for (int i = 0; i < 3; i++) sliders[i].Value = 0;
                UpdateSliderDisplay();
                ApplyToImage(null, null);
            };
            this.Controls.Add(btnReset);

            var btnApply = MakeButton("🔄  تطبيق على الصورة", 145, btnY, 200, 34);
            btnApply.Click += ApplyToImage;
            this.Controls.Add(btnApply);

            // ── Status label ──────────────────────────────────────────
            lblStatus = new Label
            {
                Location = new Point(20, btnY + 44),
                Size = new Size(840, 22),
                Font = new Font("Segoe UI", 9f),
                ForeColor = C_TxtMuted,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblStatus);

            // ── Result PictureBox ─────────────────────────────────────
            picResult = new PictureBox
            {
                Location = new Point(20, btnY + 74),
                Size = new Size(840, 220),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = C_BgBase,
                BorderStyle = BorderStyle.None
            };
            picResult.HandleCreated += (s, e) => SetRound(picResult, 6);
            picResult.Resize += (s, e) => SetRound(picResult, 6);
            this.Controls.Add(picResult);

            UpdateSliderLabels();
            UpdateSliderDisplay();
        }

        // ════════════════════════════════════════════════════════════
        //  Helpers
        // ════════════════════════════════════════════════════════════
        private Button MakeButton(string text, int x, int y, int w = 130, int h = 34)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                BackColor = C_AccentDim,
                ForeColor = Color.FromArgb(156, 191, 238),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            btn.FlatAppearance.BorderColor = C_BorderAct;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(34, 72, 128);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(18, 46, 90);
            btn.HandleCreated += (s, e) => SetRound(btn, 8);
            btn.Resize += (s, e) => SetRound(btn, 8);
            return btn;
        }

        private void StyleCombo(ComboBox cmb)
        {
            cmb.DrawMode = DrawMode.OwnerDrawFixed;
            cmb.DrawItem += (s, e) =>
            {
                e.DrawBackground();
                if (e.Index < 0) return;
                using var br = new SolidBrush(
                    (e.State & DrawItemState.Selected) != 0 ? Color.FromArgb(14, 32, 64) : C_BgHeader);
                e.Graphics.FillRectangle(br, e.Bounds);
                using var fb = new SolidBrush(
                    (e.State & DrawItemState.Selected) != 0 ? C_Accent : C_TxtSec);
                e.Graphics.DrawString(cmb.Items[e.Index].ToString(), e.Font, fb,
                    e.Bounds.X + 4, e.Bounds.Y + 2);
            };
        }

        // ════════════════════════════════════════════════════════════
        //  المنطق — لم يتغير أي سطر
        // ════════════════════════════════════════════════════════════
        private (double min, double max) GetChannelRange(string system, int channelIndex)
        {
            switch (system)
            {
                case "HSV":
                    if (channelIndex == 0) return (0, 360);
                    return (0, 100);
                case "HSL":
                    if (channelIndex == 0) return (0, 360);
                    return (0, 100);
                case "YUV":
                    if (channelIndex == 0) return (0, 255);
                    if (channelIndex == 1) return (-111, 111);
                    return (-157, 157);
                case "YCbCr":
                    if (channelIndex == 0) return (16, 235);
                    return (16, 240);
                case "L*a*b*":
                    if (channelIndex == 0) return (0, 100);
                    return (-128, 127);
                case "CMY":
                case "CMYK":
                    return (0, 100);
                case "RGB":
                default:
                    return (0, 255);
            }
        }

        private double SliderToOffset(int sliderValue, string system, int channelIndex)
        {
            var (min, max) = GetChannelRange(system, channelIndex);
            double range = max - min;
            return (sliderValue / 100.0) * (range * 0.5);
        }

        private double GetMidpoint(string system, int channelIndex)
        {
            var (min, max) = GetChannelRange(system, channelIndex);
            return (min + max) / 2.0;
        }

        private void UpdateSliderDisplay()
        {
            double realOffset1 = SliderToOffset(sliders[0].Value, currentSystem, 0);
            double realOffset2 = SliderToOffset(sliders[1].Value, currentSystem, 1);
            double realOffset3 = SliderToOffset(sliders[2].Value, currentSystem, 2);

            lblValues[0].Text = $"{realOffset1:+0.0;-0.0;0}";
            lblValues[1].Text = $"{realOffset2:+0.0;-0.0;0}";
            lblValues[2].Text = $"{realOffset3:+0.0;-0.0;0}";

            double previewCh1 = GetMidpoint(currentSystem, 0) + realOffset1;
            double previewCh2 = GetMidpoint(currentSystem, 1) + realOffset2;
            double previewCh3 = GetMidpoint(currentSystem, 2) + realOffset3;

            var (min1, max1) = GetChannelRange(currentSystem, 0);
            var (min2, max2) = GetChannelRange(currentSystem, 1);
            var (min3, max3) = GetChannelRange(currentSystem, 2);
            previewCh1 = Math.Max(min1, Math.Min(max1, previewCh1));
            previewCh2 = Math.Max(min2, Math.Min(max2, previewCh2));
            previewCh3 = Math.Max(min3, Math.Min(max3, previewCh3));

            try
            {
                Color previewColor = manager.UpdateChannel(currentSystem, previewCh1, previewCh2, previewCh3);
                colorPreview.BackColor = previewColor;
            }
            catch
            {
                colorPreview.BackColor = Color.FromArgb(40, 40, 50);
            }
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
            lblStatus.ForeColor = C_Accent;
            Application.DoEvents();

            double realOffset1 = SliderToOffset(sliders[0].Value, currentSystem, 0);
            double realOffset2 = SliderToOffset(sliders[1].Value, currentSystem, 1);
            double realOffset3 = SliderToOffset(sliders[2].Value, currentSystem, 2);

            var (min1, max1) = GetChannelRange(currentSystem, 0);
            var (min2, max2) = GetChannelRange(currentSystem, 1);
            var (min3, max3) = GetChannelRange(currentSystem, 2);

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
                            byte bVal = srcBuffer[idx];
                            byte gVal = srcBuffer[idx + 1];
                            byte rVal = srcBuffer[idx + 2];

                            Color pixel = Color.FromArgb(rVal, gVal, bVal);
                            var converted = manager.ConvertTo(currentSystem, pixel);

                            double newC1 = Math.Max(min1, Math.Min(max1, converted.Channel1 + realOffset1));
                            double newC2 = Math.Max(min2, Math.Min(max2, converted.Channel2 + realOffset2));
                            double newC3 = Math.Max(min3, Math.Min(max3, converted.Channel3 + realOffset3));

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

                picResult.Image?.Dispose();
                picResult.Image = result;
                ImageModified?.Invoke(result);

                lblStatus.Text = $"✅ تم — {currentSystem} | " +
                                      $"Δ({realOffset1:+0.0;-0.0}, {realOffset2:+0.0;-0.0}, {realOffset3:+0.0;-0.0})";
                lblStatus.ForeColor = C_Ch2;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ خطأ: {ex.Message}";
                lblStatus.ForeColor = Color.OrangeRed;
            }
        }

        private void ComboChanged(object sender, EventArgs e)
        {
            if (cmbSystem.SelectedItem == null) return;
            currentSystem = cmbSystem.SelectedItem.ToString();
            UpdateSliderLabels();
            for (int i = 0; i < 3; i++) sliders[i].Value = 0;
            UpdateSliderDisplay();
        }

        private void UpdateSliderLabels()
        {
            if (string.IsNullOrEmpty(currentSystem)) return;
            string[] names = manager.GetChannelNames(currentSystem);
            for (int i = 0; i < 3 && i < names.Length; i++)
            {
                var (min, max) = GetChannelRange(currentSystem, i);
                lblNames[i].Text = $"{names[i]}  [{min:0} ~ {max:0}]";
            }
        }

        private void Slider_Scroll(object sender, EventArgs e)
        {
            UpdateSliderDisplay();
            _sliderTimer.Stop();
            _sliderTimer.Start();
        }

        public void SetImage(Bitmap img)
        {
            originalImage?.Dispose();
            picResult.Image?.Dispose();
            originalImage = new Bitmap(img);
            picResult.Image = new Bitmap(img);
            lblStatus.Text = "📷 تم تحميل الصورة. حرك الأشرطة ثم اضغط تطبيق.";
            lblStatus.ForeColor = C_TxtMuted;
        }
    }
}