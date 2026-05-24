//GetSelectedConverter
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Homwore
//{
//    public partial class Form3 : Form
//    {
//        public event Action<Bitmap> ImageModified;
//        Bitmap originalImage;
//        public Form3()
//        {
//            InitializeComponent();
//            this.WindowState = FormWindowState.Maximized;
//        }

//        private void btnUpload_Click(object sender, EventArgs e)
//        {
//            OpenFileDialog open = new OpenFileDialog();

//            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";

//            if (open.ShowDialog() == DialogResult.OK)
//            {
//                //  تحميل الصورة الأصلية
//                originalImage = new Bitmap(open.FileName);
//                pictureBoxOriginal.Image = originalImage; // عرض الصورة في مربع الأصل

//                trackBarColors.Value = 256;

//                lblColorCount.Text = "عدد المستويات: 256";

//                // عرض الصورة الأصلية في مربع النتيجة أيضاً عند البداية
//                pictureBoxResult.Image = (Bitmap)originalImage.Clone();
//            }
//        }

//        private void trackBarColors_Scroll(object sender, EventArgs e)
//        {
//            if (originalImage == null) return;

//            int levels = trackBarColors.Value;
//            lblColorCount.Text = "عدد المستويات: " + levels.ToString();

//            // ✅ FIX: تحرير الصورة القديمة قبل إنشاء الجديدة (منع تسريب الذاكرة)
//            pictureBoxResult.Image?.Dispose();

//            // ✅ FIX: استدعاء ReduceColors مرة واحدة فقط (كان يُستدعى مرتين!)
//            Bitmap result = ImageProcessor.ReduceColors(originalImage, levels);
//            pictureBoxResult.Image = result;
//            ImageModified?.Invoke(result);
//        }

//        private void Form3_Load(object sender, EventArgs e)
//        {

//        }
//        public void SetImage(Bitmap img)
//        {
//            // ✅ FIX: تحرير الصور القديمة
//            originalImage?.Dispose();
//            pictureBoxResult.Image?.Dispose();

//            originalImage = new Bitmap(img);
//            pictureBoxOriginal.Image = originalImage;
//            pictureBoxResult.Image = (Bitmap)originalImage.Clone();
//            trackBarColors.Value = 256;
//            lblColorCount.Text = "عدد المستويات: 256";
//        }
//    }
//}
//using System;
//using System.Drawing;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using PixelLab.ColorSystems.Base;
//using PixelLab.ColorSystems.Converters;

//namespace Homwore
//{
//    public partial class Form3 : Form
//    {
//        public event Action<Bitmap> ImageModified;
//        Bitmap originalImage;

//        public Form3()
//        {
//            InitializeComponent();
//            this.WindowState = FormWindowState.Maximized;
//            cmbColorSystem.SelectedIndexChanged += new EventHandler(cmbColorSystem_Changed);
//        }


//        public void SetImage(Bitmap img)
//        {

//            originalImage?.Dispose();
//            pictureBoxResult.Image?.Dispose();

//            originalImage = new Bitmap(img);
//            pictureBoxOriginal.Image = originalImage;
//            pictureBoxResult.Image = (Bitmap)originalImage.Clone();

//            trackBarColors.Value = 256;
//            lblColorCount.Text = "عدد المستويات: 256";


//            ApplyReduceColors();
//        }

//        private void btnUpload_Click(object sender, EventArgs e)
//        {
//            OpenFileDialog open = new OpenFileDialog();
//            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";

//            if (open.ShowDialog() == DialogResult.OK)
//            {
//                originalImage?.Dispose();
//                pictureBoxResult.Image?.Dispose();

//                originalImage = new Bitmap(open.FileName);
//                pictureBoxOriginal.Image = originalImage;
//                trackBarColors.Value = 256;
//                lblColorCount.Text = "عدد المستويات: 256";
//                pictureBoxResult.Image = (Bitmap)originalImage.Clone();
//            }
//        }

//        private void trackBarColors_Scroll(object sender, EventArgs e)
//        {
//            if (originalImage == null) return;
//            lblColorCount.Text = "عدد المستويات: " + trackBarColors.Value;
//            ApplyReduceColors();
//        }

//        private void cmbColorSystem_Changed(object sender, EventArgs e)
//        {
//            ApplyReduceColors();
//        } 
//        private void Form3_Load(object sender, EventArgs e)
//        {
//            cmbColorSystem.SelectedIndex = 0;
//        }

//        private void ApplyReduceColors()
//        {
//            if (originalImage == null) return;

//            int levels = trackBarColors.Value;
//            ColorSpaceConverter converter = GetSelectedConverter();
//            Bitmap snapshot = (Bitmap)originalImage.Clone();

//            Task.Run(() =>
//            {
//                Bitmap result = ImageProcessor.ReduceColors(snapshot, levels, converter);
//                pictureBoxResult.Invoke((Action)(() =>
//                {
//                    pictureBoxResult.Image?.Dispose();
//                    pictureBoxResult.Image = result;
//                    ImageModified?.Invoke(result);
//                }));
//            });
//        }

//        private ColorSpaceConverter GetSelectedConverter()
//        {
//            string selected = cmbColorSystem.SelectedItem?.ToString();
//            if (selected == "CMY" || selected == "CMYK") return new CmykConverter();
//            if (selected == "HSV") return new HsvConverter();
//            if (selected == "YUV") return new YuvConverter();
//            if (selected == "LAB") return new LabConverter();
//            if (selected == "YCbCr") return new YCbCrConverter();
//            return null;
//        }
//    }
//}
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelLab.ColorSystems.Base;
using PixelLab.ColorSystems.Converters;

namespace Homwore
{
    public partial class Form3 : Form
    {
        public event Action<Bitmap> ImageModified;
        Bitmap originalImage;

        // ── ألوان الثيم ─────────────────────────────────────────────
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

        public Form3()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = C_BgBase;
            this.ForeColor = C_TxtPri;
            this.Text = "PixelLab — Color Reducer";

            ApplyTheme();

            cmbColorSystem.SelectedIndexChanged += new EventHandler(cmbColorSystem_Changed);
        }

        // ════════════════════════════════════════════════════════════
        //  تطبيق الثيم على كل عناصر الفورم (المُنشأة من Designer)
        // ════════════════════════════════════════════════════════════
        private void ApplyTheme()
        {
            // ── Title bar ────────────────────────────────────────────
            var titleBar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = C_BgTitle };
            titleBar.Paint += (s, e) =>
            {
                using var br = new SolidBrush(C_Accent);
                e.Graphics.FillRectangle(br, new Rectangle(16, 11, 3, 26));
                using var pen = new Pen(C_Border, 1);
                e.Graphics.DrawLine(pen, 0, titleBar.Height - 1, titleBar.Width, titleBar.Height - 1);
            };
            var lblTitleBar = new Label
            {
                Text = "PixelLab",
                Location = new Point(28, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = C_TxtPri,
                BackColor = Color.Transparent
            };
            var lblSubBar = new Label
            {
                Text = "Color Reducer — تقليل الألوان",
                Location = new Point(28, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 8f),
                ForeColor = C_TxtDim,
                BackColor = Color.Transparent
            };
            titleBar.Controls.AddRange(new Control[] { lblTitleBar, lblSubBar });
            this.Controls.Add(titleBar);

            // ── PictureBoxes ──────────────────────────────────────────
            pictureBoxOriginal.BackColor = C_BgBase;
            pictureBoxOriginal.BorderStyle = BorderStyle.None;
            pictureBoxOriginal.HandleCreated += (s, e) => SetRound(pictureBoxOriginal, 6);
            pictureBoxOriginal.Resize += (s, e) => SetRound(pictureBoxOriginal, 6);

            pictureBoxResult.BackColor = C_BgBase;
            pictureBoxResult.BorderStyle = BorderStyle.None;
            pictureBoxResult.HandleCreated += (s, e) => SetRound(pictureBoxResult, 6);
            pictureBoxResult.Resize += (s, e) => SetRound(pictureBoxResult, 6);

            // ── Upload button ─────────────────────────────────────────
            StyleButton(btnUpload);

            // ── Track bar ─────────────────────────────────────────────
            trackBarColors.BackColor = C_BgPanel;
            trackBarColors.ForeColor = C_Accent;

            // ── Labels ───────────────────────────────────────────────
            lblColorCount.ForeColor = C_TxtSec;
            lblColorCount.BackColor = Color.Transparent;
            lblColorCount.Font = new Font("Cascadia Code", 9f);

            // ── ComboBox ─────────────────────────────────────────────
            StyleCombo(cmbColorSystem);

            // ── Panel backgrounds ─────────────────────────────────────
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel pnl)
                {
                    pnl.BackColor = C_BgPanel;
                    PaintPanelBorder(pnl);
                }
                if (ctrl is Label lbl && lbl != lblColorCount)
                {
                    lbl.ForeColor = C_TxtMuted;
                    lbl.BackColor = Color.Transparent;
                }
            }
        }
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int ew, int eh);
        // ════════════════════════════════════════════════════════════
        //  Helpers
        // ════════════════════════════════════════════════════════════
        private static void SetRound(Control c, int r = 8) =>
     c.Region = Region.FromHrgn(
         CreateRoundRectRgn(0, 0, c.Width + 1, c.Height + 1, r, r));

        private void StyleButton(Button btn)
        {
            btn.BackColor = C_AccentDim;
            btn.ForeColor = Color.FromArgb(156, 191, 238);
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
            btn.FlatAppearance.BorderColor = C_BorderAct;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(34, 72, 128);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(18, 46, 90);
            btn.HandleCreated += (s, e) => SetRound(btn, 8);
            btn.Resize += (s, e) => SetRound(btn, 8);
        }

        private void StyleCombo(ComboBox cmb)
        {
            cmb.BackColor = C_BgHeader;
            cmb.ForeColor = C_TxtSec;
            cmb.Font = new Font("Segoe UI", 9f);
            cmb.FlatStyle = FlatStyle.Flat;
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
                e.Graphics.DrawString(cmb.Items[e.Index].ToString(), e.Font, fb, e.Bounds.X + 4, e.Bounds.Y + 2);
            };
        }

        private void PaintPanelBorder(Panel pnl)
        {
            pnl.Paint += (s, e) =>
            {
                using var pen = new Pen(C_Border, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, pnl.Width - 1, pnl.Height - 1);
            };
        }

        // ════════════════════════════════════════════════════════════
        //  المنطق — لم يتغير
        // ════════════════════════════════════════════════════════════
        public void SetImage(Bitmap img)
        {
            originalImage?.Dispose();
            pictureBoxResult.Image?.Dispose();

            originalImage = new Bitmap(img);
            pictureBoxOriginal.Image = originalImage;
            pictureBoxResult.Image = (Bitmap)originalImage.Clone();

            trackBarColors.Value = 256;
            lblColorCount.Text = "عدد المستويات: 256";

            ApplyReduceColors();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";

            if (open.ShowDialog() == DialogResult.OK)
            {
                originalImage?.Dispose();
                pictureBoxResult.Image?.Dispose();

                originalImage = new Bitmap(open.FileName);
                pictureBoxOriginal.Image = originalImage;
                trackBarColors.Value = 256;
                lblColorCount.Text = "عدد المستويات: 256";
                pictureBoxResult.Image = (Bitmap)originalImage.Clone();
            }
        }

        private void trackBarColors_Scroll(object sender, EventArgs e)
        {
            if (originalImage == null) return;
            lblColorCount.Text = "عدد المستويات: " + trackBarColors.Value;
            ApplyReduceColors();
        }

        private void cmbColorSystem_Changed(object sender, EventArgs e)
        {
            ApplyReduceColors();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            cmbColorSystem.SelectedIndex = 0;
        }

        private void ApplyReduceColors()
        {
            if (originalImage == null) return;

            int levels = trackBarColors.Value;
            ColorSpaceConverter converter = GetSelectedConverter();
            Bitmap snapshot = (Bitmap)originalImage.Clone();

            Task.Run(() =>
            {
                Bitmap result = ImageProcessor.ReduceColors(snapshot, levels, converter);
                pictureBoxResult.Invoke((Action)(() =>
                {
                    pictureBoxResult.Image?.Dispose();
                    pictureBoxResult.Image = result;
                    ImageModified?.Invoke(result);
                }));
            });
        }

        private ColorSpaceConverter GetSelectedConverter()
        {
            string selected = cmbColorSystem.SelectedItem?.ToString();
            if (selected == "CMY" || selected == "CMYK") return new CmykConverter();
            if (selected == "HSV") return new HsvConverter();
            if (selected == "YUV") return new YuvConverter();
            if (selected == "LAB") return new LabConverter();
            if (selected == "YCbCr") return new YCbCrConverter();
            return null;
        }
    }
}