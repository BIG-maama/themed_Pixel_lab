//using PixelLab.ColorSystems;
//using PixelLab.ColorSystems.Models;
//using System;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Runtime.InteropServices;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Homwore
//{
//    public partial class Form1 : Form
//    {
//        // ── Win32: زوايا مدورة ─────────────────────────────────────
//        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
//        private static extern IntPtr CreateRoundRectRgn(
//            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
//            int nWidthEllipse, int nHeightEllipse);

//        private static void SetRound(Control c, int r = 8) =>
//            c.Region = Region.FromHrgn(
//                CreateRoundRectRgn(0, 0, c.Width + 1, c.Height + 1, r, r));

//        // ── ألوان الثيم ────────────────────────────────────────────
//        static readonly Color C_BgBase = Color.FromArgb(13, 15, 20);
//        static readonly Color C_BgPanel = Color.FromArgb(18, 21, 28);
//        static readonly Color C_BgCard = Color.FromArgb(24, 28, 38);
//        static readonly Color C_BgElev = Color.FromArgb(26, 31, 46);
//        static readonly Color C_BgHeader = Color.FromArgb(20, 24, 32);
//        static readonly Color C_BgTitle = Color.FromArgb(11, 13, 18);
//        static readonly Color C_Border = Color.FromArgb(30, 36, 56);
//        static readonly Color C_BorderMid = Color.FromArgb(37, 45, 64);
//        static readonly Color C_BorderAct = Color.FromArgb(58, 112, 176);
//        static readonly Color C_Accent = Color.FromArgb(64, 156, 255);
//        static readonly Color C_AccentDim = Color.FromArgb(26, 58, 106);
//        static readonly Color C_Green = Color.FromArgb(52, 211, 153);
//        static readonly Color C_TxtPri = Color.FromArgb(230, 235, 245);
//        static readonly Color C_TxtSec = Color.FromArgb(200, 216, 238);
//        static readonly Color C_TxtMuted = Color.FromArgb(90, 112, 144);
//        static readonly Color C_TxtDim = Color.FromArgb(58, 74, 99);

//        private Bitmap originalImage;
//        private Bitmap currentImage;
//        private string currentFilePath;
//        private FormColorSliders slidersForm;

//        private ColorSystemManager colorManager;
//        private ColorSpaceViewer viewer3D;
//        private Form3 reducerForm;
//        private FormChannels channelsForm;

//        private PictureBox picMain;
//        private Label lblInfoRef;
//        private DataGridView dgvColors;
//        private Panel pnlLeft;
//        private Panel pnlRight;
//        private PictureBox picConverted;
//        private ComboBox cmbConvert;
//        private Label lblConvertStatus;
//        private Button btnApplyConvert;

//        // ✅ FIX: للإلغاء عند تغيير النظام اللوني قبل انتهاء التحويل
//        private CancellationTokenSource _convertCts;

//        // ✅ FIX: إيفينت جديد: عند تعديل الصورة من خلال التحويل اللوني
//        public event Action<Bitmap> ImageModified;

//        public Form1()
//        {
//            InitializeComponent();
//            this.WindowState = FormWindowState.Maximized;
//            this.Text = "PixelLab — مختبر الصور والأنظمة اللونية";
//            this.BackColor = C_BgBase;
//            this.ForeColor = C_TxtPri;

//            colorManager = new ColorSystemManager();
//            SetupUI();
//        }

//        private void SetupUI()
//        {
//            // ── شريط عنوان مخصص ─────────────────────────────────
//            var titleBar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = C_BgTitle };
//            titleBar.Paint += (s, e) =>
//            {
//                using var br = new SolidBrush(C_Accent);
//                e.Graphics.FillRectangle(br, new Rectangle(16, 11, 3, 26));
//                using var pen = new Pen(C_Border, 1);
//                e.Graphics.DrawLine(pen, 0, titleBar.Height - 1, titleBar.Width, titleBar.Height - 1);
//            };
//            var _lblTitleBar = new Label
//            {
//                Text = "PixelLab",
//                Location = new Point(28, 8),
//                AutoSize = true,
//                Font = new Font("Segoe UI", 13, FontStyle.Bold),
//                ForeColor = C_TxtPri,
//                BackColor = Color.Transparent
//            };
//            var _lblSubBar = new Label
//            {
//                Text = "مختبر الصور والأنظمة اللونية",
//                Location = new Point(28, 30),
//                AutoSize = true,
//                Font = new Font("Segoe UI", 8f),
//                ForeColor = C_TxtDim,
//                BackColor = Color.Transparent
//            };
//            titleBar.Controls.AddRange(new Control[] { _lblTitleBar, _lblSubBar });
//            this.Controls.Add(titleBar);

//            pnlLeft = new Panel
//            {
//                Dock = DockStyle.Left,
//                Width = (int)(this.ClientSize.Width * 0.68),
//                BackColor = C_BgPanel
//            };

//            pnlRight = new Panel
//            {
//                Dock = DockStyle.Right,
//                Width = (int)(this.ClientSize.Width * 0.30),
//                BackColor = C_BgCard,
//                BorderStyle = BorderStyle.None
//            };
//            pnlRight.Paint += (s, e) =>
//            {
//                using var pen = new Pen(C_Border, 1);
//                e.Graphics.DrawLine(pen, 0, 0, 0, pnlRight.Height);
//            };

//            var lblTitle = new Label
//            {
//                Text = "PixelLab",
//                Location = new Point(10, 10),
//                Size = new Size(400, 30),
//                Font = new Font("Segoe UI", 12, FontStyle.Bold),
//                ForeColor = C_Accent,
//                BackColor = Color.Transparent
//            };
//            pnlLeft.Controls.Add(lblTitle);

//            var dropPanel = new Panel
//            {
//                Location = new Point(10, 50),
//                Size = new Size(380, 46),
//                BackColor = C_BgElev,
//                BorderStyle = BorderStyle.None,
//                AllowDrop = true
//            };
//            dropPanel.Paint += (s, e) =>
//            {
//                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
//                using var pen = new Pen(Color.FromArgb(42, 64, 112), 1f);
//                pen.DashStyle = DashStyle.Dash;
//                pen.DashPattern = new float[] { 4, 3 };
//                e.Graphics.DrawPath(pen, RoundedPath(new Rectangle(1, 1, dropPanel.Width - 3, dropPanel.Height - 3), 5));
//            };
//            var lblDrop = new Label
//            {
//                Text = "🖱️ اسحب صورة هنا ",
//                AutoSize = false,
//                Size = new Size(380, 46),
//                TextAlign = ContentAlignment.MiddleCenter,
//                Font = new Font("Segoe UI", 9),
//                ForeColor = C_TxtMuted,
//                BackColor = Color.Transparent
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

//            CreateButton("📂 Load", 10, 110, BtnLoad_Click, pnlLeft);
//            CreateButton("🔄 Reset", 120, 110, BtnReset_Click, pnlLeft);
//            CreateButton("💾 Save", 230, 110, BtnSave_Click, pnlLeft);
//            CreateButton("🌐 3D", 340, 110, Btn3D_Click, pnlLeft);
//            CreateButton("🎨 Reducer", 450, 110, BtnReducer_Click, pnlLeft);
//            CreateButton("📊 Channels", 560, 110, BtnChannels_Click, pnlLeft);
//            CreateButton("🎚️ Sliders", 670, 110, BtnSliders_Click, pnlLeft);

//            lblInfoRef = new Label
//            {
//                Location = new Point(10, 165),
//                Size = new Size(380, 80),
//                Font = new Font("Cascadia Code", 8.5f),
//                BackColor = C_BgElev,
//                ForeColor = C_TxtMuted,
//                BorderStyle = BorderStyle.None,
//                Padding = new Padding(8, 6, 0, 0),
//                Text = "  لم يتم تحميل صورة بعد"
//            };
//            pnlLeft.Controls.Add(lblInfoRef);

//            picMain = new PictureBox
//            {
//                Location = new Point(10, 255),
//                Size = new Size(pnlLeft.Width - 20, pnlLeft.Height - 265),
//                SizeMode = PictureBoxSizeMode.Zoom,
//                BackColor = Color.FromArgb(10, 12, 17),
//                BorderStyle = BorderStyle.None,
//                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
//            };
//            picMain.HandleCreated += (s, e) => SetRound(picMain, 6);
//            picMain.Resize += (s, e) => SetRound(picMain, 6);
//            picMain.MouseClick += PicMain_MouseClick;
//            pnlLeft.Controls.Add(picMain);

//            // ═══════════════════════════════════════════════════════
//            //  لوحة اليمين
//            // ═══════════════════════════════════════════════════════
//            var lblRightTitle = new Label
//            {
//                Text = "الأنظمة اللونية",
//                Location = new Point(10, 10),
//                Size = new Size(250, 22),
//                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
//                ForeColor = C_Accent,
//                BackColor = Color.Transparent
//            };
//            pnlRight.Controls.Add(lblRightTitle);

//            var lblHint = new Label
//            {
//                Text = "انقر على بكسل في الصورة لعرض قيمه في جميع الأنظمة",
//                Location = new Point(10, 34),
//                Size = new Size(pnlRight.Width - 20, 30),
//                Font = new Font("Segoe UI", 7.5f, FontStyle.Italic),
//                ForeColor = C_TxtDim,
//                BackColor = Color.Transparent
//            };
//            pnlRight.Controls.Add(lblHint);

//            dgvColors = new DataGridView
//            {
//                Location = new Point(5, 70),
//                Size = new Size(pnlRight.Width - 10, 250),
//                ReadOnly = true,
//                AllowUserToAddRows = false,
//                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
//                BackgroundColor = Color.FromArgb(10, 12, 17),
//                GridColor = Color.FromArgb(30, 36, 56),
//                BorderStyle = BorderStyle.None,
//                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
//                Font = new Font("Cascadia Code", 8f),
//                RowHeadersVisible = false,
//                EnableHeadersVisualStyles = false,
//                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
//                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
//                ColumnHeadersHeight = 26,
//                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
//            };
//            dgvColors.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 24, 32);
//            dgvColors.ColumnHeadersDefaultCellStyle.ForeColor = C_Accent;
//            dgvColors.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
//            dgvColors.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
//            dgvColors.DefaultCellStyle.BackColor = Color.FromArgb(18, 21, 28);
//            dgvColors.DefaultCellStyle.ForeColor = C_TxtMuted;
//            dgvColors.DefaultCellStyle.SelectionBackColor = Color.FromArgb(14, 32, 64);
//            dgvColors.DefaultCellStyle.SelectionForeColor = C_TxtSec;
//            dgvColors.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(20, 24, 32);
//            dgvColors.CellFormatting += (s, e) =>
//            {
//                if (e.ColumnIndex == 0 && e.Value != null)
//                {
//                    e.CellStyle.ForeColor = C_TxtSec;
//                    e.CellStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
//                }
//            };
//            dgvColors.Columns.Add("System", "النظام");
//            dgvColors.Columns.Add("Ch1", "Q1");
//            dgvColors.Columns.Add("Ch2", "Q2");
//            dgvColors.Columns.Add("Ch3", "Q3");
//            pnlRight.Controls.Add(dgvColors);

//            var lblPreview = new Label
//            {
//                Text = "اللون المختار",
//                Location = new Point(10, 330),
//                Size = new Size(150, 20),
//                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
//                ForeColor = C_Accent,
//                BackColor = Color.Transparent
//            };
//            pnlRight.Controls.Add(lblPreview);

//            var pnlColorPreview = new Panel
//            {
//                Location = new Point(10, 353),
//                Size = new Size(pnlRight.Width - 20, 36),
//                BackColor = Color.FromArgb(30, 30, 45),
//                BorderStyle = BorderStyle.None,
//                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
//            };
//            pnlColorPreview.Name = "colorPreview";
//            pnlRight.Controls.Add(pnlColorPreview);

//            var lblConvert = new Label
//            {
//                Text = "تحويل الصورة كاملة",
//                Location = new Point(10, 400),
//                Size = new Size(200, 20),
//                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
//                ForeColor = C_Accent,
//                BackColor = Color.Transparent
//            };
//            pnlRight.Controls.Add(lblConvert);

//            cmbConvert = new ComboBox
//            {
//                Location = new Point(10, 422),
//                Size = new Size(150, 26),
//                DropDownStyle = ComboBoxStyle.DropDownList,
//                BackColor = Color.FromArgb(20, 24, 32),
//                ForeColor = C_TxtSec,
//                FlatStyle = FlatStyle.Flat,
//                Font = new Font("Segoe UI", 9f)
//            };
//            cmbConvert.Items.AddRange(colorManager.AvailableSystems.ToArray());
//            cmbConvert.SelectedIndex = 0;
//            cmbConvert.SelectedIndexChanged += (s, e) => ConvertImageToSystem(cmbConvert.SelectedItem.ToString());
//            pnlRight.Controls.Add(cmbConvert);

//            // ✅ زر تطبيق التحويل على الصورة الرئيسية
//            btnApplyConvert = new Button
//            {
//                Text = "✓ تطبيق",
//                Location = new Point(170, 421),
//                Size = new Size(80, 28),
//                BackColor = C_AccentDim,
//                ForeColor = Color.FromArgb(156, 191, 238),
//                FlatStyle = FlatStyle.Flat,
//                Font = new Font("Segoe UI", 8, FontStyle.Bold),
//                Cursor = Cursors.Hand,
//                Enabled = false
//            };
//            btnApplyConvert.FlatAppearance.BorderColor = C_BorderAct;
//            btnApplyConvert.FlatAppearance.BorderSize = 1;
//            btnApplyConvert.FlatAppearance.MouseOverBackColor = Color.FromArgb(34, 72, 128);
//            btnApplyConvert.HandleCreated += (s, e) => SetRound(btnApplyConvert, 6);
//            btnApplyConvert.Resize += (s, e) => SetRound(btnApplyConvert, 6);
//            btnApplyConvert.Click += BtnApplyConvert_Click;
//            pnlRight.Controls.Add(btnApplyConvert);

//            lblConvertStatus = new Label
//            {
//                Location = new Point(10, 454),
//                Size = new Size(pnlRight.Width - 20, 20),
//                Font = new Font("Segoe UI", 8),
//                ForeColor = C_TxtDim,
//                BackColor = Color.Transparent,
//                Text = "اختر نظاماً لرؤية التأثير"
//            };
//            pnlRight.Controls.Add(lblConvertStatus);

//            picConverted = new PictureBox
//            {
//                Name = "picConverted",
//                Location = new Point(10, 478),
//                Size = new Size(pnlRight.Width - 20, 200),
//                SizeMode = PictureBoxSizeMode.Zoom,
//                BackColor = Color.FromArgb(10, 12, 17),
//                BorderStyle = BorderStyle.None,
//                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
//            };
//            picConverted.HandleCreated += (s, e) => SetRound(picConverted, 5);
//            picConverted.Resize += (s, e) => SetRound(picConverted, 5);
//            pnlRight.Controls.Add(picConverted);

//            this.Controls.Add(pnlLeft);
//            this.Controls.Add(pnlRight);

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
//                Size = new Size(100, 34),
//                Font = new Font("Segoe UI", 8, FontStyle.Bold),
//                BackColor = C_AccentDim,
//                ForeColor = Color.FromArgb(156, 191, 238),
//                FlatStyle = FlatStyle.Flat,
//                Cursor = Cursors.Hand,
//                UseVisualStyleBackColor = false
//            };
//            btn.FlatAppearance.BorderColor = C_BorderAct;
//            btn.FlatAppearance.BorderSize = 1;
//            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(34, 72, 128);
//            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(18, 46, 90);
//            // زوايا مدورة
//            btn.HandleCreated += (s, e) => SetRound(btn, 8);
//            btn.Resize += (s, e) => SetRound(btn, 8);
//            btn.Click += click;
//            parent.Controls.Add(btn);
//            return btn;
//        }

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
//                    ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
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
//            ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
//            ImageModified?.Invoke(currentImage);
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
//                    ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
//                };
//            }
//            if (currentImage != null)
//                reducerForm.SetImage(new Bitmap(currentImage));
//            reducerForm.Show();
//            reducerForm.BringToFront();
//        }

//        private void BtnChannels_Click(object sender, EventArgs e)
//        {
//            if (channelsForm == null || channelsForm.IsDisposed)
//            {
//                channelsForm = new FormChannels();
//                channelsForm.ImageModified += (modifiedImage) =>
//                {
//                    currentImage?.Dispose();
//                    currentImage = new Bitmap(modifiedImage);
//                    picMain.Image = currentImage;
//                    ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
//                };
//            }
//            if (currentImage != null)
//                channelsForm.SetImage(new Bitmap(currentImage));
//            channelsForm.Show();
//            channelsForm.BringToFront();
//        }

//        private void BtnSliders_Click(object sender, EventArgs e)
//        {
//            if (slidersForm == null || slidersForm.IsDisposed)
//            {
//                slidersForm = new FormColorSliders();
//                slidersForm.ImageModified += (modifiedImage) =>
//                {
//                    currentImage?.Dispose();
//                    currentImage = new Bitmap(modifiedImage);
//                    picMain.Image = currentImage;
//                    ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
//                };
//            }
//            if (currentImage != null)
//                slidersForm.SetImage(new Bitmap(currentImage));
//            slidersForm.Show();
//            slidersForm.BringToFront();
//        }

//        // ✅ زر تطبيق التحويل اللوني على الصورة الرئيسية
//        private void BtnApplyConvert_Click(object sender, EventArgs e)
//        {
//            if (picConverted.Image == null || currentImage == null) return;

//            // نسخ الصورة المحولة إلى currentImage
//            currentImage?.Dispose();
//            currentImage = new Bitmap(picConverted.Image);
//            picMain.Image = currentImage;

//            // ✅ إطلاق الإيفينت لإعلام باقي النماذج
//            ImageModified?.Invoke(currentImage);

//            lblConvertStatus.Text = "✅ تم تطبيق التحويل على الصورة الرئيسية";
//            lblConvertStatus.ForeColor = C_Green;
//        }

//        private void PicMain_MouseClick(object sender, MouseEventArgs e)
//        {
//            if (currentImage == null || picMain.Image == null) return;
//            Point pixelPos = GetImageCoordinates(e.Location);
//            if (pixelPos.X < 0) return;
//            Color pickedColor = currentImage.GetPixel(pixelPos.X, pixelPos.Y);
//            if (viewer3D != null && !viewer3D.IsDisposed)
//                viewer3D.SetColor(pickedColor);
//            var pnlPreview = pnlRight.Controls["colorPreview"] as Panel;
//            if (pnlPreview != null)
//                pnlPreview.BackColor = pickedColor;
//            UpdateColorGrid(pickedColor);
//        }

//        private Point GetImageCoordinates(Point mousePos)
//        {
//            Rectangle imgRect = GetImageRectangle();
//            if (!imgRect.Contains(mousePos)) return new Point(-1, -1);
//            float scaleX = (float)currentImage.Width / imgRect.Width;
//            float scaleY = (float)currentImage.Height / imgRect.Height;
//            int x = (int)((mousePos.X - imgRect.X) * scaleX);
//            int y = (int)((mousePos.Y - imgRect.Y) * scaleY);
//            return new Point(
//                Math.Max(0, Math.Min(currentImage.Width - 1, x)),
//                Math.Max(0, Math.Min(currentImage.Height - 1, y))
//            );
//        }

//        private Rectangle GetImageRectangle()
//        {
//            if (picMain.Image == null || currentImage == null) return Rectangle.Empty;
//            float picAspect = (float)picMain.Width / picMain.Height;
//            float imgAspect = (float)currentImage.Width / currentImage.Height;
//            int w, h, x, y;
//            if (picAspect > imgAspect)
//            {
//                h = picMain.Height;
//                w = (int)(h * imgAspect);
//                x = (picMain.Width - w) / 2;
//                y = 0;
//            }
//            else
//            {
//                w = picMain.Width;
//                h = (int)(w / imgAspect);
//                x = 0;
//                y = (picMain.Height - h) / 2;
//            }
//            return new Rectangle(x, y, w, h);
//        }

//        private void UpdateColorGrid(Color color)
//        {
//            dgvColors.Rows.Clear();
//            dgvColors.Rows.Add("RGB", $"R: {color.R}", $"G: {color.G}", $"B: {color.B}");

//            double cmykC = 1 - color.R / 255.0;
//            double cmykM = 1 - color.G / 255.0;
//            double cmykY = 1 - color.B / 255.0;
//            double cmykK = Math.Min(cmykC, Math.Min(cmykM, cmykY));
//            if (cmykK < 1) { cmykC = (cmykC - cmykK) / (1 - cmykK); cmykM = (cmykM - cmykK) / (1 - cmykK); cmykY = (cmykY - cmykK) / (1 - cmykK); }
//            else { cmykC = cmykM = cmykY = 0; }
//            dgvColors.Rows.Add("CMYK", $"C:{cmykC * 100:F0}%", $"M:{cmykM * 100:F0}%", $"Y:{cmykY * 100:F0}% K:{cmykK * 100:F0}%");

//            var results = colorManager.ConvertAll(color);
//            foreach (var r in results)
//            {
//                dgvColors.Rows.Add(
//                    r.SystemName,
//                    $"{r.Channel1Name}: {r.Channel1:F2}",
//                    $"{r.Channel2Name}: {r.Channel2:F2}",
//                    $"{r.Channel3Name}: {r.Channel3:F2}"
//                );
//            }
//            if (dgvColors.Rows.Count > 0)
//            {
//                dgvColors.Rows[0].DefaultCellStyle.BackColor = Color.FromArgb(14, 32, 64);
//                dgvColors.Rows[0].DefaultCellStyle.ForeColor = C_Accent;
//                dgvColors.Rows[0].DefaultCellStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
//            }
//            if (dgvColors.Rows.Count > 1)
//            {
//                dgvColors.Rows[1].DefaultCellStyle.BackColor = Color.FromArgb(20, 28, 20);
//                dgvColors.Rows[1].DefaultCellStyle.ForeColor = Color.FromArgb(100, 200, 140);
//            }
//        }

//        private void Form1_Load(object sender, EventArgs e) { }

//        // ── مساعد: مسار مدوّر الزوايا لرسم الحد المنقط ───────────
//        private static System.Drawing.Drawing2D.GraphicsPath RoundedPath(Rectangle r, int rad)
//        {
//            var p = new System.Drawing.Drawing2D.GraphicsPath();
//            int d = rad * 2;
//            p.AddArc(r.X, r.Y, d, d, 180, 90);
//            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
//            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
//            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
//            p.CloseFigure();
//            return p;
//        }

//        // ═══════════════════════════════════════════════════════════
//        //  تحويل الصورة كاملة — ✅ FIX: async/await + CancellationToken
//        // ═══════════════════════════════════════════════════════════
//        private async void ConvertImageToSystem(string systemName)
//        {
//            if (currentImage == null || picConverted == null) return;

//            // ✅ FIX: إلغاء أي تحويل سابق لم ينته بعد
//            _convertCts?.Cancel();
//            _convertCts?.Dispose();
//            _convertCts = new CancellationTokenSource();
//            var token = _convertCts.Token;

//            if (systemName == "RGB")
//            {
//                // ✅ FIX: Dispose الصورة القديمة
//                picConverted.Image?.Dispose();
//                picConverted.Image = new Bitmap(currentImage);
//                lblConvertStatus.Text = "✓ RGB (الأصلية)";
//                lblConvertStatus.ForeColor = C_Green;
//                btnApplyConvert.Enabled = false;
//                return;
//            }

//            // ── تجميد واجهة بشكل مرئي فقط ─────────────────────────
//            lblConvertStatus.Text = "⏳ جاري التحويل...";
//            lblConvertStatus.ForeColor = C_Accent;
//            btnApplyConvert.Enabled = false;
//            cmbConvert.Enabled = false;   // منع تغيير النظام أثناء التحويل

//            try
//            {
//                // ✅ FIX: نسخ البيانات الضرورية قبل الدخول للـ Task
//                int targetW = Math.Min(currentImage.Width, 400);
//                int targetH = Math.Min(currentImage.Height, 400);
//                bool needResize = currentImage.Width > 400 || currentImage.Height > 400;

//                // نسخ بيانات الصورة المصدر كـ byte[]
//                Bitmap sourceSnap = needResize
//                    ? ResizeImage(currentImage, targetW, targetH)
//                    : new Bitmap(currentImage);

//                int sw = sourceSnap.Width, sh = sourceSnap.Height;
//                BitmapData snapData = sourceSnap.LockBits(
//                    new Rectangle(0, 0, sw, sh),
//                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
//                int stride = snapData.Stride;
//                byte[] srcBuffer = new byte[Math.Abs(stride) * sh];
//                Marshal.Copy(snapData.Scan0, srcBuffer, 0, srcBuffer.Length);
//                sourceSnap.UnlockBits(snapData);
//                sourceSnap.Dispose();

//                // ✅ FIX: المعالجة تجري في Background Thread
//                byte[] dstBuffer = await Task.Run(() =>
//                {
//                    byte[] dst = new byte[srcBuffer.Length];
//                    for (int y = 0; y < sh; y++)
//                    {
//                        // ✅ FIX: فحص الإلغاء كل سطر
//                        token.ThrowIfCancellationRequested();
//                        for (int x = 0; x < sw; x++)
//                        {
//                            int idx = y * stride + x * 3;
//                            Color pixel = Color.FromArgb(srcBuffer[idx + 2], srcBuffer[idx + 1], srcBuffer[idx]);
//                            var cv = colorManager.ConvertTo(systemName, pixel);
//                            double m1 = ModifyForVisualEffect(systemName, cv.Channel1, cv.Channel2, cv.Channel3, 1);
//                            double m2 = ModifyForVisualEffect(systemName, cv.Channel1, cv.Channel2, cv.Channel3, 2);
//                            double m3 = ModifyForVisualEffect(systemName, cv.Channel1, cv.Channel2, cv.Channel3, 3);
//                            Color nc = colorManager.UpdateChannel(systemName, m1, m2, m3);
//                            dst[idx] = nc.B;
//                            dst[idx + 1] = nc.G;
//                            dst[idx + 2] = nc.R;
//                        }
//                    }
//                    return dst;
//                }, token);

//                // ✅ FIX: العودة للـ UI Thread لتحديث الواجهة
//                if (token.IsCancellationRequested) return;

//                Bitmap result = new Bitmap(sw, sh, PixelFormat.Format24bppRgb);
//                BitmapData dstData = result.LockBits(
//                    new Rectangle(0, 0, sw, sh),
//                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
//                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
//                result.UnlockBits(dstData);

//                // ✅ FIX: Dispose الصورة القديمة
//                picConverted.Image?.Dispose();
//                picConverted.Image = result;
//                lblConvertStatus.Text = $"✓ تم التحويل إلى {systemName} — اضغط 'تطبيق'";
//                lblConvertStatus.ForeColor = C_Green;
//                btnApplyConvert.Enabled = true;
//            }
//            catch (OperationCanceledException)
//            {
//                // تم الإلغاء — طبيعي عند تغيير النظام بسرعة
//                lblConvertStatus.Text = "—";
//                lblConvertStatus.ForeColor = C_TxtDim;
//            }
//            catch (AggregateException ae) when (ae.InnerExceptions.Count == 1 &&
//                                                ae.InnerException is OperationCanceledException)
//            {
//                // .NET Framework 4.x يلف OperationCanceledException داخل AggregateException
//                lblConvertStatus.Text = "—";
//                lblConvertStatus.ForeColor = C_TxtDim;
//            }
//            catch (Exception ex)
//            {
//                lblConvertStatus.Text = $"✗ خطأ: {ex.Message}";
//                lblConvertStatus.ForeColor = Color.FromArgb(255, 80, 80);
//                btnApplyConvert.Enabled = false;
//            }
//            finally
//            {
//                // ✅ FIX: إعادة تفعيل الـ ComboBox دائماً
//                cmbConvert.Enabled = true;
//            }
//        }

//        private double ModifyForVisualEffect(string system, double c1, double c2, double c3, int channel)
//        {
//            switch (system)
//            {
//                case "HSV":
//                    if (channel == 1) return (c1 + 180) % 360;
//                    if (channel == 2) return Math.Min(100, c2 * 1.5);
//                    return c3;
//                case "HSL":
//                    if (channel == 1) return (c1 + 180) % 360;
//                    if (channel == 3) return Math.Min(100, c3 * 0.7);
//                    return c2;
//                case "YUV":
//                case "YCbCr":
//                    if (channel == 1) return c1 * 0.8;
//                    return channel == 2 ? c2 : c3;
//                case "L*a*b*":
//                    if (channel == 1) return Math.Min(100, c1 * 1.2);
//                    return channel == 2 ? c2 : c3;
//                case "CMY":
//                    return channel == 1 ? 100 - c1 : (channel == 2 ? 100 - c2 : 100 - c3);
//                case "XYZ":
//                    return c1 * 0.9;
//                default:
//                    return channel == 1 ? c1 : (channel == 2 ? c2 : c3);
//            }
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
//    }
//}
using PixelLab.ColorSystems;
using PixelLab.ColorSystems.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;
using System.Windows.Forms;

namespace Homwore
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        private static void SetRound(Control c, int r = 8) =>
            c.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, c.Width + 1, c.Height + 1, r, r));

     
        static readonly Color C_BgBase = Color.FromArgb(13, 15, 20);
        static readonly Color C_BgPanel = Color.FromArgb(18, 21, 28);
        static readonly Color C_BgCard = Color.FromArgb(24, 28, 38);
        static readonly Color C_BgElev = Color.FromArgb(26, 31, 46);
        static readonly Color C_BgHeader = Color.FromArgb(20, 24, 32);
        static readonly Color C_BgTitle = Color.FromArgb(11, 13, 18);
        static readonly Color C_Border = Color.FromArgb(30, 36, 56);
        static readonly Color C_BorderMid = Color.FromArgb(37, 45, 64);
        static readonly Color C_BorderAct = Color.FromArgb(58, 112, 176);
        static readonly Color C_Accent = Color.FromArgb(64, 156, 255);
        static readonly Color C_AccentDim = Color.FromArgb(26, 58, 106);
        static readonly Color C_Green = Color.FromArgb(52, 211, 153);
        static readonly Color C_TxtPri = Color.FromArgb(230, 235, 245);
        static readonly Color C_TxtSec = Color.FromArgb(200, 216, 238);
        static readonly Color C_TxtMuted = Color.FromArgb(90, 112, 144);
        static readonly Color C_TxtDim = Color.FromArgb(58, 74, 99);

        private Bitmap originalImage;
        private Bitmap currentImage;
        private string currentFilePath;
        private FormColorSliders slidersForm;

        private ColorSystemManager colorManager;
        private ColorSpaceViewer viewer3D;
        private Form3 reducerForm;
        private FormChannels channelsForm;

        private PictureBox picMain;
        private Label lblInfoRef;
        private DataGridView dgvColors;
        private Panel pnlLeft;
        private Panel pnlRight;
        private PictureBox picConverted;
        private ComboBox cmbConvert;
        private Label lblConvertStatus;
        private Button btnApplyConvert;

        
        private CancellationTokenSource _convertCts;

  
        public event Action<Bitmap> ImageModified;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Text = "PixelLab — مختبر الصور والأنظمة اللونية";
            this.BackColor = C_BgBase;
            this.ForeColor = C_TxtPri;

            colorManager = new ColorSystemManager();
            SetupUI();
        }

        private void SetupUI()
        {
            var titleBar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = C_BgTitle };
            titleBar.Paint += (s, e) =>
            {
                using var br = new SolidBrush(C_Accent);
                e.Graphics.FillRectangle(br, new Rectangle(16, 11, 3, 26));
                using var pen = new Pen(C_Border, 1);
                e.Graphics.DrawLine(pen, 0, titleBar.Height - 1, titleBar.Width, titleBar.Height - 1);
            };
            var _lblTitleBar = new Label
            {
                Text = "PixelLab",
                Location = new Point(28, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = C_TxtPri,
                BackColor = Color.Transparent
            };
            var _lblSubBar = new Label
            {
                Text = "مختبر الصور والأنظمة اللونية",
                Location = new Point(28, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 8f),
                ForeColor = C_TxtDim,
                BackColor = Color.Transparent
            };
            titleBar.Controls.AddRange(new Control[] { _lblTitleBar, _lblSubBar });
            this.Controls.Add(titleBar);

            pnlLeft = new Panel
            {
                Dock = DockStyle.Left,
                Width = (int)(this.ClientSize.Width * 0.68),
                BackColor = C_BgPanel
            };

            pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = (int)(this.ClientSize.Width * 0.30),
                BackColor = C_BgCard,
                BorderStyle = BorderStyle.None
            };
            pnlRight.Paint += (s, e) =>
            {
                using var pen = new Pen(C_Border, 1);
                e.Graphics.DrawLine(pen, 0, 0, 0, pnlRight.Height);
            };

            var lblTitle = new Label
            {
                Text = "PixelLab",
                Location = new Point(10, 10),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = C_Accent,
                BackColor = Color.Transparent
            };
            pnlLeft.Controls.Add(lblTitle);

            var dropPanel = new Panel
            {
                Location = new Point(10, 50),
                Size = new Size(380, 46),
                BackColor = C_BgElev,
                BorderStyle = BorderStyle.None,
                AllowDrop = true
            };
            dropPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(Color.FromArgb(42, 64, 112), 1f);
                pen.DashStyle = DashStyle.Dash;
                pen.DashPattern = new float[] { 4, 3 };
                e.Graphics.DrawPath(pen, RoundedPath(new Rectangle(1, 1, dropPanel.Width - 3, dropPanel.Height - 3), 5));
            };
            var lblDrop = new Label
            {
                Text = "🖱️ اسحب صورة هنا أو اضغط Load",
                AutoSize = false,
                Size = new Size(380, 46),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9),
                ForeColor = C_TxtMuted,
                BackColor = Color.Transparent
            };
            dropPanel.Controls.Add(lblDrop);
            pnlLeft.Controls.Add(dropPanel);

            dropPanel.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
            };
            dropPanel.DragDrop += (s, e) =>
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0) LoadImage(files[0]);
            };

            CreateButton("📂 Load", 10, 110, BtnLoad_Click, pnlLeft);
            CreateButton("🔄 Reset", 120, 110, BtnReset_Click, pnlLeft);
            CreateButton("💾 Save", 230, 110, BtnSave_Click, pnlLeft);
            CreateButton("🌐 3D", 340, 110, Btn3D_Click, pnlLeft);
            CreateButton("🎨 Reducer", 450, 110, BtnReducer_Click, pnlLeft);
            CreateButton("📊 Channels", 560, 110, BtnChannels_Click, pnlLeft);
            CreateButton("⚔ Sliders", 670, 110, BtnSliders_Click, pnlLeft);

            lblInfoRef = new Label
            {
                Location = new Point(10, 165),
                Size = new Size(380, 80),
                Font = new Font("Cascadia Code", 8.5f),
                BackColor = C_BgElev,
                ForeColor = C_TxtMuted,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(8, 6, 0, 0),
                Text = "  لم يتم تحميل صورة بعد"
            };
            pnlLeft.Controls.Add(lblInfoRef);

            picMain = new PictureBox
            {
                Location = new Point(10, 255),
                Size = new Size(pnlLeft.Width - 20, pnlLeft.Height - 265),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(10, 12, 17),
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            picMain.HandleCreated += (s, e) => SetRound(picMain, 6);
            picMain.Resize += (s, e) => SetRound(picMain, 6);
            picMain.MouseClick += PicMain_MouseClick;
            picMain.MouseMove += PicMain_MouseMove;
            pnlLeft.Controls.Add(picMain);

         
            var lblRightTitle = new Label
            {
                Text = "الأنظمة اللونية",
                Location = new Point(10, 10),
                Size = new Size(250, 22),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = C_Accent,
                BackColor = Color.Transparent
            };
            pnlRight.Controls.Add(lblRightTitle);

            var lblHint = new Label
            {
                Text = "انقر على بكسل في الصورة لعرض قيمه في جميع الأنظمة",
                Location = new Point(10, 34),
                Size = new Size(pnlRight.Width - 20, 30),
                Font = new Font("Segoe UI", 7.5f, FontStyle.Italic),
                ForeColor = C_TxtDim,
                BackColor = Color.Transparent
            };
            pnlRight.Controls.Add(lblHint);

            dgvColors = new DataGridView
            {
                Location = new Point(5, 70),
                Size = new Size(pnlRight.Width - 10, 250),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(10, 12, 17),
                GridColor = Color.FromArgb(30, 36, 56),
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                Font = new Font("Cascadia Code", 8f),
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 26,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            dgvColors.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 24, 32);
            dgvColors.ColumnHeadersDefaultCellStyle.ForeColor = C_Accent;
            dgvColors.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            dgvColors.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvColors.DefaultCellStyle.BackColor = Color.FromArgb(18, 21, 28);
            dgvColors.DefaultCellStyle.ForeColor = C_TxtMuted;
            dgvColors.DefaultCellStyle.SelectionBackColor = Color.FromArgb(14, 32, 64);
            dgvColors.DefaultCellStyle.SelectionForeColor = C_TxtSec;
            dgvColors.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(20, 24, 32);
            dgvColors.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == 0 && e.Value != null)
                {
                    e.CellStyle.ForeColor = C_TxtSec;
                    e.CellStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
                }
            };
            dgvColors.Columns.Add("System", "النظام");
            dgvColors.Columns.Add("Ch1", "Q1");
            dgvColors.Columns.Add("Ch2", "Q2");
            dgvColors.Columns.Add("Ch3", "Q3");
            pnlRight.Controls.Add(dgvColors);

            var lblPreview = new Label
            {
                Text = "اللون المختار",
                Location = new Point(10, 330),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = C_Accent,
                BackColor = Color.Transparent
            };
            pnlRight.Controls.Add(lblPreview);

            var pnlColorPreview = new Panel
            {
                Location = new Point(10, 353),
                Size = new Size(pnlRight.Width - 20, 36),
                BackColor = Color.FromArgb(30, 30, 45),
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlColorPreview.Name = "colorPreview";
            pnlRight.Controls.Add(pnlColorPreview);

            var lblConvert = new Label
            {
                Text = "تحويل الصورة كاملة",
                Location = new Point(10, 400),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = C_Accent,
                BackColor = Color.Transparent
            };
            pnlRight.Controls.Add(lblConvert);

            cmbConvert = new ComboBox
            {
                Location = new Point(10, 422),
                Size = new Size(150, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(20, 24, 32),
                ForeColor = C_TxtSec,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f)
            };
            cmbConvert.Items.AddRange(colorManager.AvailableSystems.ToArray());
            cmbConvert.SelectedIndex = 0;
            cmbConvert.SelectedIndexChanged += (s, e) => ConvertImageToSystem(cmbConvert.SelectedItem.ToString());
            pnlRight.Controls.Add(cmbConvert);

       
            btnApplyConvert = new Button
            {
                Text = "✓ تطبيق",
                Location = new Point(170, 421),
                Size = new Size(80, 28),
                BackColor = C_AccentDim,
                ForeColor = Color.FromArgb(156, 191, 238),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnApplyConvert.FlatAppearance.BorderColor = C_BorderAct;
            btnApplyConvert.FlatAppearance.BorderSize = 1;
            btnApplyConvert.FlatAppearance.MouseOverBackColor = Color.FromArgb(34, 72, 128);
            btnApplyConvert.HandleCreated += (s, e) => SetRound(btnApplyConvert, 6);
            btnApplyConvert.Resize += (s, e) => SetRound(btnApplyConvert, 6);
            btnApplyConvert.Click += BtnApplyConvert_Click;
            pnlRight.Controls.Add(btnApplyConvert);

            lblConvertStatus = new Label
            {
                Location = new Point(10, 454),
                Size = new Size(pnlRight.Width - 20, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = C_TxtDim,
                BackColor = Color.Transparent,
                Text = "اختر نظاماً لرؤية التأثير"
            };
            pnlRight.Controls.Add(lblConvertStatus);

            picConverted = new PictureBox
            {
                Name = "picConverted",
                Location = new Point(10, 478),
                Size = new Size(pnlRight.Width - 20, 200),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(10, 12, 17),
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            picConverted.HandleCreated += (s, e) => SetRound(picConverted, 5);
            picConverted.Resize += (s, e) => SetRound(picConverted, 5);
            pnlRight.Controls.Add(picConverted);

            this.Controls.Add(pnlLeft);
            this.Controls.Add(pnlRight);

            this.Resize += (s, e) =>
            {
                pnlLeft.Width = (int)(this.ClientSize.Width * 0.68);
                pnlRight.Width = (int)(this.ClientSize.Width * 0.30);
            };
        }

        private Button CreateButton(string text, int x, int y, EventHandler click, Panel parent)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 34),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
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
            btn.Click += click;
            parent.Controls.Add(btn);
            return btn;
        }

        private void LoadImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            try
            {
                using (var img = Image.FromFile(filePath))
                {
                    var bmp = new Bitmap(img);
                    originalImage?.Dispose();
                    currentImage?.Dispose();
                    originalImage = new Bitmap(bmp);
                    currentImage = new Bitmap(bmp);
                    currentFilePath = filePath;
                    picMain.Image = currentImage;
                    UpdateInfo(filePath);
                    ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("فشل تحميل الصورة: " + ex.Message, "خطأ",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateInfo(string filePath)
        {
            if (lblInfoRef == null) return;
            var fi = new FileInfo(filePath);
            string sizeKB = (fi.Length / 1024.0).ToString("F2");
            lblInfoRef.Text = $"📁 Name: {Path.GetFileName(filePath)}\n" +
                             $"📐 Size: {currentImage.Width} × {currentImage.Height} px\n" +
                             $"💾 File: {sizeKB} KB\n" +
                             $"🎨 Format: {Path.GetExtension(filePath).ToUpper()}";
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff|All Files|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                    LoadImage(dlg.FileName);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (currentImage == null)
            {
                MessageBox.Show("لا توجد صورة للحفظ.", "حفظ",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "PNG|*.png|JPEG|*.jpg;*.jpeg|Bitmap|*.bmp|GIF|*.gif";
                dlg.DefaultExt = "png";
                dlg.FileName = "PixelLab_" + Path.GetFileNameWithoutExtension(currentFilePath ?? "image") + ".png";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
                    var fmt = System.Drawing.Imaging.ImageFormat.Png;
                    if (ext == ".jpg" || ext == ".jpeg") fmt = System.Drawing.Imaging.ImageFormat.Jpeg;
                    else if (ext == ".bmp") fmt = System.Drawing.Imaging.ImageFormat.Bmp;
                    else if (ext == ".gif") fmt = System.Drawing.Imaging.ImageFormat.Gif;
                    currentImage.Save(dlg.FileName, fmt);
                    MessageBox.Show("✅ تم الحفظ بنجاح!", "حفظ",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("لا توجد صورة أصلية.", "إعادة ضبط",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            currentImage?.Dispose();
            currentImage = new Bitmap(originalImage);
            picMain.Image = currentImage;
            ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
            ImageModified?.Invoke(currentImage);
            MessageBox.Show("✅ تمت إعادة الضبط", "إعادة ضبط",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Btn3D_Click(object sender, EventArgs e)
        {
            if (viewer3D == null || viewer3D.IsDisposed)
                viewer3D = new ColorSpaceViewer();
            viewer3D.Show();
            viewer3D.BringToFront();
        }

        private void BtnReducer_Click(object sender, EventArgs e)
        {
            if (reducerForm == null || reducerForm.IsDisposed)
            {
                reducerForm = new Form3();
                reducerForm.ImageModified += (modifiedImage) =>
                {
                    currentImage?.Dispose();
                    currentImage = new Bitmap(modifiedImage);
                    picMain.Image = currentImage;
                    ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
                };
            }
            if (currentImage != null)
                reducerForm.SetImage(new Bitmap(currentImage));
            reducerForm.Show();
            reducerForm.BringToFront();
        }

        private void BtnChannels_Click(object sender, EventArgs e)
        {
            if (channelsForm == null || channelsForm.IsDisposed)
            {
                channelsForm = new FormChannels();
                channelsForm.ImageModified += (modifiedImage) =>
                {
                    currentImage?.Dispose();
                    currentImage = new Bitmap(modifiedImage);
                    picMain.Image = currentImage;
                    ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
                };
            }
            if (currentImage != null)
                channelsForm.SetImage(new Bitmap(currentImage));
            channelsForm.Show();
            channelsForm.BringToFront();
        }

        private void BtnSliders_Click(object sender, EventArgs e)
        {
            if (slidersForm == null || slidersForm.IsDisposed)
            {
                slidersForm = new FormColorSliders();
                slidersForm.ImageModified += (modifiedImage) =>
                {
                    currentImage?.Dispose();
                    currentImage = new Bitmap(modifiedImage);
                    picMain.Image = currentImage;
                    ConvertImageToSystem(cmbConvert.SelectedItem?.ToString() ?? "RGB");
                };
            }
            if (currentImage != null)
                slidersForm.SetImage(new Bitmap(currentImage));
            slidersForm.Show();
            slidersForm.BringToFront();
        }

 
        private void BtnApplyConvert_Click(object sender, EventArgs e)
        {
            if (picConverted.Image == null || currentImage == null) return;

           
            currentImage?.Dispose();
            currentImage = new Bitmap(picConverted.Image);
            picMain.Image = currentImage;

          
            ImageModified?.Invoke(currentImage);

            lblConvertStatus.Text = "✅ تم تطبيق التحويل على الصورة الرئيسية";
            lblConvertStatus.ForeColor = C_Green;
        }

        private void PicMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentImage == null || picMain.Image == null) return;
            Point pixelPos = GetImageCoordinates(e.Location);
            if (pixelPos.X < 0) return;
            Color pickedColor = currentImage.GetPixel(pixelPos.X, pixelPos.Y);
            if (viewer3D != null && !viewer3D.IsDisposed)
                viewer3D.SetColor(pickedColor);
            var pnlPreview = pnlRight.Controls["colorPreview"] as Panel;
            if (pnlPreview != null)
                pnlPreview.BackColor = pickedColor;
            UpdateColorGrid(pickedColor);
        }

        private void PicMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentImage == null || picMain.Image == null) return;

            Point pixelPos = GetImageCoordinates(e.Location);
            if (pixelPos.X < 0) return;

            Color hoveredColor = currentImage.GetPixel(pixelPos.X, pixelPos.Y);

           
            var pnlPreview = pnlRight.Controls["colorPreview"] as Panel;
            if (pnlPreview != null)
                pnlPreview.BackColor = hoveredColor;

           
            UpdateColorGrid(hoveredColor);

          
            if (viewer3D != null && !viewer3D.IsDisposed)
                viewer3D.SetColor(hoveredColor);
        }

  
        private void HoverTimer_Tick(object sender, EventArgs e) { }

        private Point GetImageCoordinates(Point mousePos)
        {
            Rectangle imgRect = GetImageRectangle();
            if (!imgRect.Contains(mousePos)) return new Point(-1, -1);
            float scaleX = (float)currentImage.Width / imgRect.Width;
            float scaleY = (float)currentImage.Height / imgRect.Height;
            int x = (int)((mousePos.X - imgRect.X) * scaleX);
            int y = (int)((mousePos.Y - imgRect.Y) * scaleY);
            return new Point(
                Math.Max(0, Math.Min(currentImage.Width - 1, x)),
                Math.Max(0, Math.Min(currentImage.Height - 1, y))
            );
        }

        private Rectangle GetImageRectangle()
        {
            if (picMain.Image == null || currentImage == null) return Rectangle.Empty;
            float picAspect = (float)picMain.Width / picMain.Height;
            float imgAspect = (float)currentImage.Width / currentImage.Height;
            int w, h, x, y;
            if (picAspect > imgAspect)
            {
                h = picMain.Height;
                w = (int)(h * imgAspect);
                x = (picMain.Width - w) / 2;
                y = 0;
            }
            else
            {
                w = picMain.Width;
                h = (int)(w / imgAspect);
                x = 0;
                y = (picMain.Height - h) / 2;
            }
            return new Rectangle(x, y, w, h);
        }

        private void UpdateColorGrid(Color color)
        {
            dgvColors.Rows.Clear();
            dgvColors.Rows.Add("RGB", $"R: {color.R}", $"G: {color.G}", $"B: {color.B}");

            double cmykC = 1 - color.R / 255.0;
            double cmykM = 1 - color.G / 255.0;
            double cmykY = 1 - color.B / 255.0;
            double cmykK = Math.Min(cmykC, Math.Min(cmykM, cmykY));
            if (cmykK < 1) { cmykC = (cmykC - cmykK) / (1 - cmykK); cmykM = (cmykM - cmykK) / (1 - cmykK); cmykY = (cmykY - cmykK) / (1 - cmykK); }
            else { cmykC = cmykM = cmykY = 0; }
            dgvColors.Rows.Add("CMYK", $"C:{cmykC * 100:F0}%", $"M:{cmykM * 100:F0}%", $"Y:{cmykY * 100:F0}% K:{cmykK * 100:F0}%");

            var results = colorManager.ConvertAll(color);
            foreach (var r in results)
            {
                dgvColors.Rows.Add(
                    r.SystemName,
                    $"{r.Channel1Name}: {r.Channel1:F2}",
                    $"{r.Channel2Name}: {r.Channel2:F2}",
                    $"{r.Channel3Name}: {r.Channel3:F2}"
                );
            }
            if (dgvColors.Rows.Count > 0)
            {
                dgvColors.Rows[0].DefaultCellStyle.BackColor = Color.FromArgb(14, 32, 64);
                dgvColors.Rows[0].DefaultCellStyle.ForeColor = C_Accent;
                dgvColors.Rows[0].DefaultCellStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            }
            if (dgvColors.Rows.Count > 1)
            {
                dgvColors.Rows[1].DefaultCellStyle.BackColor = Color.FromArgb(20, 28, 20);
                dgvColors.Rows[1].DefaultCellStyle.ForeColor = Color.FromArgb(100, 200, 140);
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedPath(Rectangle r, int rad)
        {
            var p = new System.Drawing.Drawing2D.GraphicsPath();
            int d = rad * 2;
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private async void ConvertImageToSystem(string systemName)
        {
            if (currentImage == null || picConverted == null) return;

         
            _convertCts?.Cancel();
            _convertCts?.Dispose();
            _convertCts = new CancellationTokenSource();
            var token = _convertCts.Token;

            if (systemName == "RGB")
            {
               
                picConverted.Image?.Dispose();
                picConverted.Image = new Bitmap(currentImage);
                lblConvertStatus.Text = "✓ RGB (الأصلية)";
                lblConvertStatus.ForeColor = C_Green;
                btnApplyConvert.Enabled = false;
                return;
            }

            lblConvertStatus.Text = "⏳ جاري التحويل...";
            lblConvertStatus.ForeColor = C_Accent;
            btnApplyConvert.Enabled = false;
            cmbConvert.Enabled = false;  

            try
            {
          
                int targetW = Math.Min(currentImage.Width, 400);
                int targetH = Math.Min(currentImage.Height, 400);
                bool needResize = currentImage.Width > 400 || currentImage.Height > 400;

                Bitmap sourceSnap = needResize
                    ? ResizeImage(currentImage, targetW, targetH)
                    : new Bitmap(currentImage);

                int sw = sourceSnap.Width, sh = sourceSnap.Height;
                BitmapData snapData = sourceSnap.LockBits(
                    new Rectangle(0, 0, sw, sh),
                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                int stride = snapData.Stride;
                byte[] srcBuffer = new byte[Math.Abs(stride) * sh];
                Marshal.Copy(snapData.Scan0, srcBuffer, 0, srcBuffer.Length);
                sourceSnap.UnlockBits(snapData);
                sourceSnap.Dispose();
                byte[] dstBuffer = await Task.Run(() =>
                {
                    byte[] dst = new byte[srcBuffer.Length];
                    for (int y = 0; y < sh; y++)
                    {
                      
                        token.ThrowIfCancellationRequested();
                        for (int x = 0; x < sw; x++)
                        {
                            int idx = y * stride + x * 3;
                            Color pixel = Color.FromArgb(srcBuffer[idx + 2], srcBuffer[idx + 1], srcBuffer[idx]);
                            var cv = colorManager.ConvertTo(systemName, pixel);
                            double m1 = ModifyForVisualEffect(systemName, cv.Channel1, cv.Channel2, cv.Channel3, 1);
                            double m2 = ModifyForVisualEffect(systemName, cv.Channel1, cv.Channel2, cv.Channel3, 2);
                            double m3 = ModifyForVisualEffect(systemName, cv.Channel1, cv.Channel2, cv.Channel3, 3);
                            Color nc = colorManager.UpdateChannel(systemName, m1, m2, m3);
                            dst[idx] = nc.B;
                            dst[idx + 1] = nc.G;
                            dst[idx + 2] = nc.R;
                        }
                    }
                    return dst;
                }, token);

             
                if (token.IsCancellationRequested) return;

                Bitmap result = new Bitmap(sw, sh, PixelFormat.Format24bppRgb);
                BitmapData dstData = result.LockBits(
                    new Rectangle(0, 0, sw, sh),
                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBuffer.Length);
                result.UnlockBits(dstData);

              
                picConverted.Image?.Dispose();
                picConverted.Image = result;
                lblConvertStatus.Text = $"✓ تم التحويل إلى {systemName} — اضغط 'تطبيق'";
                lblConvertStatus.ForeColor = C_Green;
                btnApplyConvert.Enabled = true;
            }
            catch (OperationCanceledException)
            {
               
                lblConvertStatus.Text = "—";
                lblConvertStatus.ForeColor = C_TxtDim;
            }
            catch (AggregateException ae) when (ae.InnerExceptions.Count == 1 &&
                                                ae.InnerException is OperationCanceledException)
            {
              
                lblConvertStatus.Text = "—";
                lblConvertStatus.ForeColor = C_TxtDim;
            }
            catch (Exception ex)
            {
                lblConvertStatus.Text = $"✗ خطأ: {ex.Message}";
                lblConvertStatus.ForeColor = Color.FromArgb(255, 80, 80);
                btnApplyConvert.Enabled = false;
            }
            finally
            {
           
                cmbConvert.Enabled = true;
            }
        }

        private double ModifyForVisualEffect(string system, double c1, double c2, double c3, int channel)
        {
            switch (system)
            {
                case "HSV":
                    if (channel == 1) return (c1 + 180) % 360;
                    if (channel == 2) return Math.Min(100, c2 * 1.5);
                    return c3;
                case "HSL":
                    if (channel == 1) return (c1 + 180) % 360;
                    if (channel == 3) return Math.Min(100, c3 * 0.7);
                    return c2;
                case "YUV":
                case "YCbCr":
                    if (channel == 1) return c1 * 0.8;
                    return channel == 2 ? c2 : c3;
                case "L*a*b*":
                    if (channel == 1) return Math.Min(100, c1 * 1.2);
                    return channel == 2 ? c2 : c3;
                case "CMY":
                    return channel == 1 ? 100 - c1 : (channel == 2 ? 100 - c2 : 100 - c3);
                case "XYZ":
                    return c1 * 0.9;
                default:
                    return channel == 1 ? c1 : (channel == 2 ? c2 : c3);
            }
        }

        private Bitmap ResizeImage(Bitmap img, int maxW, int maxH)
        {
            double r = Math.Min((double)maxW / img.Width, (double)maxH / img.Height);
            int w = (int)(img.Width * r);
            int h = (int)(img.Height * r);
            Bitmap b = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, w, h);
            }
            return b;
        }
    }
}