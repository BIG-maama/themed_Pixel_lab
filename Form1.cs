////using System;
////using System.Collections.Generic;
////using System.ComponentModel;
////using System.Data;
////using System.Drawing;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;
////using System.Windows.Forms;
////using Emgu.CV;
////using Emgu.CV.CvEnum;
////using Emgu.CV.Structure;
////using NAudio.Wave;
////using MathNet.Numerics;

////using PixelLab.ColorSystems;       // ← أضف هذا السطر
////using PixelLab.ColorSystems.Models; // ← وهذا
////namespace Homwore
////{
////    public partial class Form1 : Form
////    {
////        public Form1()
////        {
////            InitializeComponent();
////            var manager = new ColorSystemManager();
////            var color = Color.FromArgb(120, 80, 200);
////            var results = manager.ConvertAll(color);
////            foreach (var r in results)
////                Console.WriteLine(r);
////        }
////    }
////}
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using Emgu.CV;
//using Emgu.CV.CvEnum;
//using Emgu.CV.Structure;
//using NAudio.Wave;
//using MathNet.Numerics;

//using PixelLab.ColorSystems;
//using PixelLab.ColorSystems.Models;

//namespace Homwore
//{
//    public partial class Form1 : Form
//    {
//        // instance واحد من نافذة الـ 3D Viewer 
//        private ColorSpaceViewer _viewer;

//        public Form1()
//        {
//            InitializeComponent();

//            // ── اختبار سريع: طباعة تحويل لون في الـ Console ────────
//            var manager = new ColorSystemManager();
//            var color   = Color.FromArgb(120, 80, 200);
//            var results = manager.ConvertAll(color);
//            foreach (var r in results)
////                Console.WriteLine(r);

//            // ── زر فتح نافذة الـ 3D Viewer ─────────────────────────
//            var btn = new Button
//            {
//                Text     = "🎨 Open Color Space Viewer (3D)",
//                Size     = new Size(220, 40),
//                Location = new Point(20, 20),
//                Font     = new Font("Segoe UI", 10f)
//            };
//            btn.Click += (s, e) =>
//            {
//                // إذا مفتوحة بالفعل، اعرضها — وإلا افتح واحدة جديدة
//                if (_viewer == null || _viewer.IsDisposed)
//                    _viewer = new ColorSpaceViewer();
//                _viewer.Show();
//                _viewer.BringToFront();
//            };
//            this.Controls.Add(btn);
//        }

//        /// <summary>
//        /// استدعِ هذه الدالة عند الضغط على بكسل في الصورة
//        /// لتمرير اللون للـ 3D Viewer وتحديثه تلقائياً
//        /// </summary>
//        public void NotifyColorPicked(Color pickedColor)
//        {
//            if (_viewer != null && !_viewer.IsDisposed)
//                _viewer.SetColor(pickedColor);
//        }

//        private void Form1_Load(object sender, EventArgs e)
//        {

//        }

//    }
//}
//using PixelLab.ColorSystems;
//using PixelLab.ColorSystems.Models;
//using PixelLab.Forms;
//using System;
//using System.Drawing;
//using System.IO;
//using System.Windows.Forms;

//namespace Homwore
//{
//    public partial class Form1 : Form
//    {
//        // ── المتغيرات الأساسية ──
//        private Bitmap originalImage;      // الصورة الأصلية (لا تُعدّل)
//        private Bitmap currentImage;       // الصورة المعروضة حالياً

//        // ── مكونات الفريق ──
//        private ColorSystemManager colorManager;   // ← PixelLab
//        private ColorSpaceViewer viewer3D;         // ← 3D Viewer
//        private Form3 reducerForm;                 // ← Form3
//        private FormChannels channelsForm;         // ← FormChannels

//        // ── عناصر الواجهة (سنضيفها لاحقاً) ──
//        // TODO: PictureBox, Buttons, Labels, DataGridView

//        public Form1()
//        {
//            InitializeComponent();
//            this.WindowState = FormWindowState.Maximized;
//            this.Text = "PixelLab - مختبر الصور المتعدد";

//            colorManager = new ColorSystemManager();
//            SetupUI();  
//        }

//        private void SetupUI()
//        {
//            this.BackColor = Color.FromArgb(245, 245, 250);

//            // ── عنوان ──
//            var lblTitle = new Label
//            {
//                Text = "🎨 PixelLab - مختبر الصور والأنظمة اللونية",
//                Location = new Point(20, 15),
//                Size = new Size(600, 35),
//                Font = new Font("Segoe UI", 16, FontStyle.Bold),
//                ForeColor = Color.FromArgb(0, 100, 200)
//            };
//            this.Controls.Add(lblTitle);

//            // ── منطقة السحب والإفلات ──
//            var dropPanel = new Panel
//            {
//                Location = new Point(20, 60),
//                Size = new Size(400, 55),
//                BackColor = Color.White,
//                BorderStyle = BorderStyle.FixedSingle,
//                AllowDrop = true  // ← مهم!
//            };
//            var lblDrop = new Label
//            {
//                Text = "🖱️ اسحب صورة هنا أو اضغط Load Image",
//                AutoSize = false,
//                Size = new Size(400, 55),
//                TextAlign = ContentAlignment.MiddleCenter,
//                Font = new Font("Segoe UI", 11)
//            };
//            dropPanel.Controls.Add(lblDrop);
//            this.Controls.Add(dropPanel);

//            // ── أحداث السحب والإفلات ──
//            dropPanel.DragEnter += (s, e) => {
//                if (e.Data.GetDataPresent(DataFormats.FileDrop))
//                    e.Effect = DragDropEffects.Copy;
//            };
//            dropPanel.DragDrop += (s, e) => {
//                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
//                if (files.Length > 0) LoadImage(files[0]);
//            };
//            // ─ـ الأزرار ──
//            var btnLoad = CreateButton("📂 Load Image", 20, 125, BtnLoad_Click);
//            var btnReset = CreateButton("🔄 Reset", 160, 125, BtnReset_Click);
//            var btnSave = CreateButton("💾 Save", 300, 125, BtnSave_Click);
//            var btn3D = CreateButton("🌐 3D Viewer", 440, 125, Btn3D_Click);
//            var btnReducer = CreateButton("🎨 Reducer", 580, 125, BtnReducer_Click);
//            var btnChannels = CreateButton("📊 Channels", 720, 125, BtnChannels_Click);
//        }
//        private Button CreateButton(string text, int x, int y, EventHandler click)
//        {
//            var btn = new Button
//            {
//                Text = text,
//                Location = new Point(x, y),
//                Size = new Size(130, 40),
//                Font = new Font("Segoe UI", 9, FontStyle.Bold),
//                BackColor = Color.FromArgb(0, 120, 215),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat,
//                Cursor = Cursors.Hand
//            };
//            btn.FlatAppearance.BorderSize = 0;
//            btn.Click += click;
//            this.Controls.Add(btn);
//            return btn;
//        }

//        /// <summary>
//        /// Load an image from disk into the form's image variables and show it.
//        /// </summary>
//        private void LoadImage(string filePath)
//        {
//            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
//                return;

//            try
//            {
//                using (var img = Image.FromFile(filePath))
//                {
//                    var bmp = new Bitmap(img);

//                    // dispose previous images if any
//                    originalImage?.Dispose();
//                    currentImage?.Dispose();

//                    originalImage = new Bitmap(bmp);
//                    currentImage = new Bitmap(bmp);

//                    // show the image as the form background until a PictureBox is added
//                    this.BackgroundImage = currentImage;
//                    this.BackgroundImageLayout = ImageLayout.Zoom;
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Failed to load image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void BtnLoad_Click(object sender, EventArgs e)
//        {
//            using (var dlg = new OpenFileDialog())
//            {
//                dlg.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff|All Files|*.*";
//                if (dlg.ShowDialog() == DialogResult.OK)
//                    LoadImage(dlg.FileName);
//            }
//        }

//        // Added missing event handlers referenced when creating buttons
//        private void BtnSave_Click(object sender, EventArgs e)
//        {
//            if (currentImage == null)
//            {
//                MessageBox.Show("No image to save.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            using (var dlg = new SaveFileDialog())
//            {
//                dlg.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap|*.bmp|GIF|*.gif";
//                dlg.DefaultExt = "png";
//                if (dlg.ShowDialog() == DialogResult.OK)
//                {
//                    var ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
//                    var fmt = System.Drawing.Imaging.ImageFormat.Png;
//                    if (ext == ".jpg" || ext == ".jpeg") fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
//                    else if (ext == ".bmp") fmt = System.Drawing.Imaging.ImageFormat.Bmp;
//                    else if (ext == ".gif") fmt = System.Drawing.Imaging.ImageFormat.Gif;

//                    currentImage.Save(dlg.FileName, fmt);
//                }
//            }
//        }

//        private void BtnReset_Click(object sender, EventArgs e)
//        {
//            if (originalImage == null)
//            {
//                MessageBox.Show("No original image to reset to.", "Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            currentImage?.Dispose();
//            currentImage = new Bitmap(originalImage);
//            this.BackgroundImage = currentImage;
//            this.BackgroundImageLayout = ImageLayout.Zoom;
//        }

//        private void Btn3D_Click(object sender, EventArgs e)
//        {
//            if (viewer3D == null || viewer3D.IsDisposed)
//                viewer3D = new ColorSpaceViewer();

//            viewer3D.Show();
//            viewer3D.BringToFront();
//        }

//        private void BtnReducer_Click(object sender, EventArgs e)
//        {
//            if (reducerForm == null || reducerForm.IsDisposed)
//                reducerForm = new Form3();

//            reducerForm.Show();
//            reducerForm.BringToFront();
//        }

//        private void BtnChannels_Click(object sender, EventArgs e)
//        {
//            if (channelsForm == null || channelsForm.IsDisposed)
//                channelsForm = new FormChannels();

//            channelsForm.Show();
//            channelsForm.BringToFront();
//        }

//        private void Form1_Load(object sender, EventArgs e) { }

//    }
////}
//using PixelLab.ColorSystems;
//using PixelLab.ColorSystems.Models;
//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Runtime.InteropServices;
//using System.Windows.Forms;

//namespace Homwore
//{
//    public partial class Form1 : Form
//    {
//        // ── المتغيرات الأساسية ──
//        private Bitmap originalImage;
//        private Bitmap currentImage;
//        private string currentFilePath;
//        private FormColorSliders slidersForm;

//        // ── مكونات الفريق ──
//        private ColorSystemManager colorManager;
//        private ColorSpaceViewer viewer3D;
//        private Form3 reducerForm;
//        private FormChannels channelsForm;

//        // ── عناصر الواجهة ──
//        private PictureBox picMain;
//        private Label lblInfoRef;
//        private DataGridView dgvColors;
//        private Panel pnlLeft;      // ← لوحة اليسار
//        private Panel pnlRight;     // ← لوحة اليمين

//        public Form1()
//        {
//            InitializeComponent();
//            this.WindowState = FormWindowState.Maximized;
//            this.Text = "PixelLab - مختبر الصور المتعدد";

//            colorManager = new ColorSystemManager();
//            SetupUI();
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  بناء الواجهة الرسومية — بأحجام نسبية
//        // ═══════════════════════════════════════════════════════════
//        private void SetupUI()
//        {
//            this.BackColor = Color.FromArgb(245, 245, 250);

//            // ── لوحة اليسار (70% من العرض) ──
//            pnlLeft = new Panel
//            {
//                Dock = DockStyle.Left,
//                Width = (int)(this.ClientSize.Width * 0.68),
//                BackColor = Color.FromArgb(245, 245, 250)
//            };

//            // ── لوحة اليمين (30% من العرض) ──
//            pnlRight = new Panel
//            {
//                Dock = DockStyle.Right,
//                Width = (int)(this.ClientSize.Width * 0.30),
//                BackColor = Color.White,
//                BorderStyle = BorderStyle.FixedSingle
//            };

//            // ── عنوان ──
//            var lblTitle = new Label
//            {
//                Text = "🎨 PixelLab - مختبر الصور والأنظمة اللونية",
//                Location = new Point(10, 10),
//                Size = new Size(600, 35),
//                Font = new Font("Segoe UI", 14, FontStyle.Bold),
//                ForeColor = Color.FromArgb(0, 100, 200)
//            };
//            pnlLeft.Controls.Add(lblTitle);

//            // ── منطقة السحب والإفلات ──
//            var dropPanel = new Panel
//            {
//                Location = new Point(10, 50),
//                Size = new Size(380, 50),
//                BackColor = Color.White,
//                BorderStyle = BorderStyle.FixedSingle,
//                AllowDrop = true
//            };
//            var lblDrop = new Label
//            {
//                Text = "🖱️ اسحب صورة هنا أو اضغط Load",
//                AutoSize = false,
//                Size = new Size(380, 50),
//                TextAlign = ContentAlignment.MiddleCenter,
//                Font = new Font("Segoe UI", 10)
//            };
//            dropPanel.Controls.Add(lblDrop);
//            pnlLeft.Controls.Add(dropPanel);

//            dropPanel.DragEnter += (s, e) =>
//            {
//                if (e.Data.GetDataPresent(DataFormats.FileDrop))
//                    e.Effect = DragDropEffects.Copy;
//            };
//            dropPanel.DragDrop += (s, e) =>
//            {
//                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
//                if (files.Length > 0) LoadImage(files[0]);
//            };

//            // ── الأزرار ──
//            CreateButton("📂 Load", 10, 110, BtnLoad_Click, pnlLeft);
//            CreateButton("🔄 Reset", 120, 110, BtnReset_Click, pnlLeft);
//            CreateButton("💾 Save", 230, 110, BtnSave_Click, pnlLeft);
//            CreateButton("🌐 3D", 340, 110, Btn3D_Click, pnlLeft);
//            CreateButton("🎨 Reducer", 450, 110, BtnReducer_Click, pnlLeft);
//            CreateButton("📊 Channels", 560, 110, BtnChannels_Click, pnlLeft);
//            CreateButton("🎚️ Sliders", 670, 110, BtnSliders_Click, pnlLeft);

//            // ── معلومات الصورة ──
//            lblInfoRef = new Label
//            {
//                Location = new Point(10, 165),
//                Size = new Size(380, 80),
//                Font = new Font("Consolas", 9),
//                BackColor = Color.White,
//                BorderStyle = BorderStyle.FixedSingle,
//                Padding = new Padding(6),
//                Text = "📋 لم يتم تحميل صورة بعد"
//            };
//            pnlLeft.Controls.Add(lblInfoRef);
//            var lblConvert = new Label
//            {
//                Text = "🔄 تحويل الصورة كاملة:",
//                Location = new Point(10, 480),
//                Size = new Size(200, 25),
//                Font = new Font("Segoe UI", 10, FontStyle.Bold)
//            };
//            pnlRight.Controls.Add(lblConvert);

//            var cmbConvert = new ComboBox
//            {
//                Location = new Point(10, 510),
//                Size = new Size(200, 25),
//                DropDownStyle = ComboBoxStyle.DropDownList
//            };
//            cmbConvert.Items.AddRange(new string[] { "RGB", "CMY", "HSV", "YUV", "YCbCr", "L*a*b*" });
//            cmbConvert.SelectedIndex = 0;
//            cmbConvert.SelectedIndexChanged += (s, e) => ConvertImageToSystem(cmbConvert.SelectedItem.ToString());
//            pnlRight.Controls.Add(cmbConvert);

//            // PictureBox صغير للنتيجة
//            var picConverted = new PictureBox
//            {
//                Name = "picConverted",
//                Location = new Point(10, 545),
//                Size = new Size(pnlRight.Width - 20, 150),
//                SizeMode = PictureBoxSizeMode.Zoom,
//                BackColor = Color.FromArgb(220, 220, 220),
//                BorderStyle = BorderStyle.Fixed3D,
//                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
//            };
//            pnlRight.Controls.Add(picConverted);

//            // ── PictureBox للصورة الرئيسية ──
//            picMain = new PictureBox
//            {
//                Location = new Point(10, 255),
//                Size = new Size(pnlLeft.Width - 20, pnlLeft.Height - 265),
//                SizeMode = PictureBoxSizeMode.Zoom,
//                BackColor = Color.FromArgb(220, 220, 220),
//                BorderStyle = BorderStyle.Fixed3D,
//                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
//            };
//            picMain.MouseClick += PicMain_MouseClick;
//            pnlLeft.Controls.Add(picMain);

//            // ═══════════════════════════════════════════════════════
//            //  لوحة اليمين — الأنظمة اللونية
//            // ═══════════════════════════════════════════════════════

//            // ── عنوان ──
//            var lblRightTitle = new Label
//            {
//                Text = "🎨 الأنظمة اللونية",
//                Location = new Point(10, 10),
//                Size = new Size(250, 30),
//                Font = new Font("Segoe UI", 12, FontStyle.Bold),
//                ForeColor = Color.FromArgb(0, 100, 200)
//            };
//            pnlRight.Controls.Add(lblRightTitle);

//            // ── تعليمات ──
//            var lblHint = new Label
//            {
//                Text = "🖱️ انقر على بكسل في الصورة\nلعرض قيمه في جميع الأنظمة",
//                Location = new Point(10, 45),
//                Size = new Size(250, 40),
//                Font = new Font("Segoe UI", 8, FontStyle.Italic),
//                ForeColor = Color.DarkBlue
//            };
//            pnlRight.Controls.Add(lblHint);

//            // ── جدول الأنظمة اللونية ──
//            dgvColors = new DataGridView
//            {
//                Location = new Point(5, 90),
//                Size = new Size(pnlRight.Width - 10, 280),
//                ReadOnly = true,
//                AllowUserToAddRows = false,
//                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
//                BackgroundColor = Color.White,
//                BorderStyle = BorderStyle.Fixed3D,
//                Font = new Font("Consolas", 9),
//                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
//                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
//            };
//            dgvColors.Columns.Add("System", "System");
//            dgvColors.Columns.Add("Ch1", "Ch1");
//            dgvColors.Columns.Add("Ch2", "Ch2");
//            dgvColors.Columns.Add("Ch3", "Ch3");
//            pnlRight.Controls.Add(dgvColors);

//            // ── معاينة اللون المختار ──
//            var lblPreview = new Label
//            {
//                Text = "🎯 اللون المختار:",
//                Location = new Point(10, 380),
//                Size = new Size(150, 25),
//                Font = new Font("Segoe UI", 10, FontStyle.Bold)
//            };
//            pnlRight.Controls.Add(lblPreview);

//            var pnlColorPreview = new Panel
//            {
//                Location = new Point(10, 410),
//                Size = new Size(pnlRight.Width - 20, 60),
//                BackColor = Color.Black,
//                BorderStyle = BorderStyle.Fixed3D,
//                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
//            };
//            pnlColorPreview.Name = "colorPreview";
//            pnlRight.Controls.Add(pnlColorPreview);

//            // ── إضافة اللوحات للـ Form ──
//            this.Controls.Add(pnlLeft);
//            this.Controls.Add(pnlRight);

//            // ── تعديل الأحجام عند تغيير حجم النافذة ──
//            this.Resize += (s, e) =>
//            {
//                pnlLeft.Width = (int)(this.ClientSize.Width * 0.68);
//                pnlRight.Width = (int)(this.ClientSize.Width * 0.30);
//            };
//        }

//        private Button CreateButton(string text, int x, int y, EventHandler click, Panel parent)
//        {
//            var btn = new Button
//            {
//                Text = text,
//                Location = new Point(x, y),
//                Size = new Size(100, 35),
//                Font = new Font("Segoe UI", 8, FontStyle.Bold),
//                BackColor = Color.FromArgb(0, 120, 215),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat,
//                Cursor = Cursors.Hand
//            };
//            btn.FlatAppearance.BorderSize = 0;
//            btn.Click += click;
//            parent.Controls.Add(btn);
//            return btn;
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  تحميل الصورة
//        // ═══════════════════════════════════════════════════════════
//        private void LoadImage(string filePath)
//        {
//            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
//                return;

//            try
//            {
//                using (var img = Image.FromFile(filePath))
//                {
//                    var bmp = new Bitmap(img);

//                    originalImage?.Dispose();
//                    currentImage?.Dispose();

//                    originalImage = new Bitmap(bmp);
//                    currentImage = new Bitmap(bmp);
//                    currentFilePath = filePath;

//                    picMain.Image = currentImage;
//                    UpdateInfo(filePath);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("فشل تحميل الصورة: " + ex.Message, "خطأ",
//                              MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void UpdateInfo(string filePath)
//        {
//            if (lblInfoRef == null) return;

//            var fi = new FileInfo(filePath);
//            string sizeKB = (fi.Length / 1024.0).ToString("F2");

//            lblInfoRef.Text = $"📁 Name: {Path.GetFileName(filePath)}\n" +
//                             $"📐 Size: {currentImage.Width} × {currentImage.Height} px\n" +
//                             $"💾 File: {sizeKB} KB\n" +
//                             $"🎨 Format: {Path.GetExtension(filePath).ToUpper()}";
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  أحداث الأزرار
//        // ═══════════════════════════════════════════════════════════
//        private void BtnLoad_Click(object sender, EventArgs e)
//        {
//            using (var dlg = new OpenFileDialog())
//            {
//                dlg.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff|All Files|*.*";
//                if (dlg.ShowDialog() == DialogResult.OK)
//                    LoadImage(dlg.FileName);
//            }
//        }

//        private void BtnSave_Click(object sender, EventArgs e)
//        {
//            if (currentImage == null)
//            {
//                MessageBox.Show("لا توجد صورة للحفظ.", "حفظ",
//                              MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            using (var dlg = new SaveFileDialog())
//            {
//                dlg.Filter = "PNG|*.png|JPEG|*.jpg;*.jpeg|Bitmap|*.bmp|GIF|*.gif";
//                dlg.DefaultExt = "png";
//                dlg.FileName = "PixelLab_" + Path.GetFileNameWithoutExtension(currentFilePath ?? "image") + ".png";

//                if (dlg.ShowDialog() == DialogResult.OK)
//                {
//                    var ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
//                    var fmt = System.Drawing.Imaging.ImageFormat.Png;
//                    if (ext == ".jpg" || ext == ".jpeg") fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
//                    else if (ext == ".bmp") fmt = System.Drawing.Imaging.ImageFormat.Bmp;
//                    else if (ext == ".gif") fmt = System.Drawing.Imaging.ImageFormat.Gif;

//                    currentImage.Save(dlg.FileName, fmt);
//                    MessageBox.Show("✅ تم الحفظ بنجاح!", "حفظ",
//                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//            }
//        }

//        private void BtnReset_Click(object sender, EventArgs e)
//        {
//            if (originalImage == null)
//            {
//                MessageBox.Show("لا توجد صورة أصلية.", "إعادة ضبط",
//                              MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            currentImage?.Dispose();
//            currentImage = new Bitmap(originalImage);
//            picMain.Image = currentImage;

//            MessageBox.Show("✅ تمت إعادة الضبط", "إعادة ضبط",
//                          MessageBoxButtons.OK, MessageBoxIcon.Information);
//        }

//        private void Btn3D_Click(object sender, EventArgs e)
//        {
//            if (viewer3D == null || viewer3D.IsDisposed)
//                viewer3D = new ColorSpaceViewer();

//            viewer3D.Show();
//            viewer3D.BringToFront();
//        }

//        private void BtnReducer_Click(object sender, EventArgs e)
//        {
//            if (reducerForm == null || reducerForm.IsDisposed)
//            {
//                reducerForm = new Form3();

                
//                reducerForm.ImageModified += (modifiedImage) =>
//                {
//                    currentImage?.Dispose();
//                    currentImage = new Bitmap(modifiedImage);
//                    picMain.Image = currentImage;
//                };
//            }

//            if (currentImage != null)
//                reducerForm.SetImage(new Bitmap(currentImage));

//            reducerForm.Show();
//            reducerForm.BringToFront();
//        }

        //private void BtnChannels_Click(object sender, EventArgs e)
        //{
        //    if (channelsForm == null || channelsForm.IsDisposed)
        //    {
        //        channelsForm = new FormChannels();
        //        channelsForm.ImageModified += (modifiedImage) =>
        //        {
        //            currentImage?.Dispose();
        //            currentImage = new Bitmap(modifiedImage);
        //            picMain.Image = currentImage;
        //        };
        //    }
        //    if (currentImage != null)
        //        channelsForm.SetImage(new Bitmap(currentImage));

        //    channelsForm.Show();
        //    channelsForm.BringToFront();
        //}
        //private void BtnChannels_Click(object sender, EventArgs e)
        //{
        //    if (channelsForm == null || channelsForm.IsDisposed)
        //    {
        //        channelsForm = new FormChannels();

        //        // ✅ استمع للتعديلات
        //        channelsForm.ImageModified += (modifiedImage) =>
        //        {
        //            currentImage?.Dispose();
        //            currentImage = new Bitmap(modifiedImage);
        //            picMain.Image = currentImage;
        //        };
        //    }

        //    if (currentImage != null)
        //        channelsForm.SetImage(new Bitmap(currentImage));

        //    channelsForm.Show();
        //    channelsForm.BringToFront();
        //}

        //// ═══════════════════════════════════════════════════════════
        ////  النقر على بكسل — عرض الأنظمة اللونية (متطلب 5)
        //// ═══════════════════════════════════════════════════════════
        //private void PicMain_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (currentImage == null || picMain.Image == null) return;

        //    Point pixelPos = GetImageCoordinates(e.Location);
        //    if (pixelPos.X < 0) return;

        //    Color pickedColor = currentImage.GetPixel(pixelPos.X, pixelPos.Y);

        //    // تحديث 3D Viewer
        //    if (viewer3D != null && !viewer3D.IsDisposed)
        //        viewer3D.SetColor(pickedColor);

        //    // تحديث معاينة اللون
        //    var pnlPreview = pnlRight.Controls["colorPreview"] as Panel;
        //    if (pnlPreview != null)
        //        pnlPreview.BackColor = pickedColor;

        //    // عرض في الجدول
        //    UpdateColorGrid(pickedColor);
        //}

        //private Point GetImageCoordinates(Point mousePos)
        //{
        //    Rectangle imgRect = GetImageRectangle();
        //    if (!imgRect.Contains(mousePos)) return new Point(-1, -1);

        //    float scaleX = (float)currentImage.Width / imgRect.Width;
        //    float scaleY = (float)currentImage.Height / imgRect.Height;

        //    int x = (int)((mousePos.X - imgRect.X) * scaleX);
        //    int y = (int)((mousePos.Y - imgRect.Y) * scaleY);

        //    return new Point(
        //        Math.Max(0, Math.Min(currentImage.Width - 1, x)),
        //        Math.Max(0, Math.Min(currentImage.Height - 1, y))
        //    );
        //}

        //private Rectangle GetImageRectangle()
        //{
        //    if (picMain.Image == null || currentImage == null) return Rectangle.Empty;

        //    float picAspect = (float)picMain.Width / picMain.Height;
        //    float imgAspect = (float)currentImage.Width / currentImage.Height;

        //    int w, h, x, y;
        //    if (picAspect > imgAspect)
        //    {
        //        h = picMain.Height;
        //        w = (int)(h * imgAspect);
        //        x = (picMain.Width - w) / 2;
        //        y = 0;
        //    }
        //    else
        //    {
        //        w = picMain.Width;
        //        h = (int)(w / imgAspect);
        //        x = 0;
        //        y = (picMain.Height - h) / 2;
        //    }
        //    return new Rectangle(x, y, w, h);
        //}

        //private void UpdateColorGrid(Color color)
        //{
        //    dgvColors.Rows.Clear();

        //    // RGB
        //    dgvColors.Rows.Add("RGB", $"R: {color.R}", $"G: {color.G}", $"B: {color.B}");

        //    // باقي الأنظمة
        //    var results = colorManager.ConvertAll(color);
        //    foreach (var r in results)
        //    {
        //        dgvColors.Rows.Add(
        //            r.SystemName,
        //            $"{r.Channel1Name}: {r.Channel1:F2}",
        //            $"{r.Channel2Name}: {r.Channel2:F2}",
        //            $"{r.Channel3Name}: {r.Channel3:F2}"
        //        );
        //    }

        //    // تمييز RGB
        //    if (dgvColors.Rows.Count > 0)
        //        dgvColors.Rows[0].DefaultCellStyle.BackColor = Color.LightCyan;
        //}

        //private void Form1_Load(object sender, EventArgs e) { }
        //private void ConvertImageToSystem(string systemName)
        //{
        //    if (currentImage == null) return;

        //    // إذا كان RGB، اعرض الأصلية
        //    if (systemName == "RGB")
        //    {
        //        var pic = pnlRight.Controls["picConverted"] as PictureBox;
        //        if (pic != null) pic.Image = new Bitmap(currentImage);
        //        return;
        //    }

        //    // تحويل الصورة كاملة (بطيء — استخدم صورة صغيرة!)
        //    Bitmap small = ResizeImage(currentImage, 300, 300);
        //    Bitmap result = new Bitmap(small.Width, small.Height);

        //    var converter = colorManager; // استخدم المحول المناسب

        //    for (int y = 0; y < small.Height; y++)
        //    {
        //        for (int x = 0; x < small.Width; x++)
        //        {
        //            Color pixel = small.GetPixel(x, y);
        //            var converted = colorManager.ConvertTo(systemName, pixel);

        //            // تحويل العكسي للون RGB
        //            Color rgb = colorManager.UpdateChannel(systemName,
        //                converted.Channel1, converted.Channel2, converted.Channel3);

        //            result.SetPixel(x, y, rgb);
        //        }
        //    }

        //    var picBox = pnlRight.Controls["picConverted"] as PictureBox;
        //    if (picBox != null) picBox.Image = result;
        //}
//        private void ConvertImageToSystem(string systemName)
//        {
//            if (currentImage == null) return;

//            var picBox = pnlRight.Controls["picConverted"] as PictureBox;
//            if (picBox == null) return;

//            // إذا كان RGB، اعرض الأصلية
//            if (systemName == "RGB")
//            {
//                picBox.Image = new Bitmap(currentImage);
//                return;
//            }

//            // تصغير الصورة أولاً للسرعة
//            Bitmap small = ResizeImage(currentImage, 200, 200);
//            Bitmap result = new Bitmap(small.Width, small.Height);

//            // استخدام LockBits للسرعة
//            BitmapData srcData = small.LockBits(
//                new Rectangle(0, 0, small.Width, small.Height),
//                ImageLockMode.ReadOnly,
//                PixelFormat.Format24bppRgb);

//            BitmapData dstData = result.LockBits(
//                new Rectangle(0, 0, result.Width, result.Height),
//                ImageLockMode.WriteOnly,
//                PixelFormat.Format24bppRgb);

//            try
//            {
//                int stride = srcData.Stride;
//                byte[] srcBuffer = new byte[Math.Abs(stride) * small.Height];
//                byte[] dstBuffer = new byte[Math.Abs(stride) * result.Height];

//                Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBuffer.Length);

//                for (int y = 0; y < small.Height; y++)
//                {
//                    for (int x = 0; x < small.Width; x++)
//                    {
//                        int idx = y * stride + x * 3;

//                        byte b = srcBuffer[idx];
//                        byte g = srcBuffer[idx + 1];
//                        byte r = srcBuffer[idx + 2];

//                        Color pixel = Color.FromArgb(r, g, b);
//                        var converted = colorManager.ConvertTo(systemName, pixel);
//                        Color rgb = colorManager.UpdateChannel(systemName,
//                            converted.Channel1, converted.Channel2, converted.Channel3);

//                        dstBuffer[idx] = rgb.B;
//                        dstBuffer[idx + 1] = rgb.G;
//                        dstBuffer[idx + 2] = rgb.R;
//                    }
//                }

//                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
//            }
//            finally
//            {
//                small.UnlockBits(srcData);
//                result.UnlockBits(dstData);
//            }

//            picBox.Image = result;
//        }

//        private Bitmap ResizeImage(Bitmap img, int maxW, int maxH)
//        {
//            double r = Math.Min((double)maxW / img.Width, (double)maxH / img.Height);
//            int w = (int)(img.Width * r);
//            int h = (int)(img.Height * r);
//            Bitmap b = new Bitmap(w, h);
//            using (Graphics g = Graphics.FromImage(b))
//            {
//                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
//                g.DrawImage(img, 0, 0, w, h);
//            }
//            return b;
//        }
//        private void BtnSliders_Click(object sender, EventArgs e)
//        {
//            if (slidersForm == null || slidersForm.IsDisposed)
//            {
//                slidersForm = new FormColorSliders();

//                // استمع للتعديلات
//                slidersForm.ImageModified += (modifiedImage) =>
//                {
//                    currentImage?.Dispose();
//                    currentImage = new Bitmap(modifiedImage);
//                    picMain.Image = currentImage;
//                };
//            }

//            if (currentImage != null)
//                slidersForm.SetImage(new Bitmap(currentImage));

//            slidersForm.Show();
//            slidersForm.BringToFront();
//        }
//    }
//}

