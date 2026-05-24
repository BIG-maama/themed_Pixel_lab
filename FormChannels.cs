//using System;
//using System.Drawing;
//using System.Windows.Forms;
//using PixelLab.Core;

//namespace Homwore
//{
//    public class FormChannels : Form
//    {
//        public event Action<Bitmap> ImageModified;
//        PictureBox originalBox;
//        PictureBox resultBox;
//        PictureBox redBox;
//        PictureBox greenBox;
//        PictureBox blueBox;

//        Button btnLoad;

//        CheckBox chkR;
//        CheckBox chkG;
//        CheckBox chkB;

//        TrackBar trackR;
//        TrackBar trackG;
//        TrackBar trackB;

//        Bitmap original;
//        ChannelProcessor processor = new ChannelProcessor();

//        // ✅ FIX: Debouncing Timer — منع المعالجة المتكررة عند تحريك الـ Slider
//        private Timer _debounceTimer;


//        public FormChannels()
//        {
//            // ✅ FIX: إعداد Timer للـ Debouncing (300ms)
//            _debounceTimer = new Timer();
//            _debounceTimer.Interval = 300;
//            _debounceTimer.Tick += (s, e) => { _debounceTimer.Stop(); ApplyChanges(); };

//            BuildUI();
//        }

//        private void BuildUI()
//        {
//            this.Text = "PixelLab - Channels Module";
//            this.Size = new Size(1200, 700);

//            originalBox = new PictureBox() { Location = new Point(20, 20), Size = new Size(300, 250), SizeMode = PictureBoxSizeMode.Zoom };
//            resultBox = new PictureBox() { Location = new Point(340, 20), Size = new Size(300, 250), SizeMode = PictureBoxSizeMode.Zoom };

//            redBox = new PictureBox() { Location = new Point(20, 300), Size = new Size(200, 200), SizeMode = PictureBoxSizeMode.Zoom };
//            greenBox = new PictureBox() { Location = new Point(240, 300), Size = new Size(200, 200), SizeMode = PictureBoxSizeMode.Zoom };
//            blueBox = new PictureBox() { Location = new Point(460, 300), Size = new Size(200, 200), SizeMode = PictureBoxSizeMode.Zoom };

//            btnLoad = new Button() { Text = "Load", Location = new Point(700, 20) };
//            btnLoad.Click += LoadImage;

//            chkR = new CheckBox() { Text = "R", Location = new Point(700, 80), Checked = true };
//            chkG = new CheckBox() { Text = "G", Location = new Point(700, 110), Checked = true };
//            chkB = new CheckBox() { Text = "B", Location = new Point(700, 140), Checked = true };

//            trackR = new TrackBar() { Minimum = 0, Maximum = 200, Value = 100, Location = new Point(700, 200) };
//            trackG = new TrackBar() { Minimum = 0, Maximum = 200, Value = 100, Location = new Point(700, 260) };
//            trackB = new TrackBar() { Minimum = 0, Maximum = 200, Value = 100, Location = new Point(700, 320) };

//            chkR.CheckedChanged += Apply;
//            chkG.CheckedChanged += Apply;
//            chkB.CheckedChanged += Apply;

//            // ✅ FIX: الـ TrackBars تستخدم Debouncing بدل Apply مباشرة
//            trackR.Scroll += (s, e) => { _debounceTimer.Stop(); _debounceTimer.Start(); };
//            trackG.Scroll += (s, e) => { _debounceTimer.Stop(); _debounceTimer.Start(); };
//            trackB.Scroll += (s, e) => { _debounceTimer.Stop(); _debounceTimer.Start(); };

//            this.Controls.Add(originalBox);
//            this.Controls.Add(resultBox);
//            this.Controls.Add(redBox);
//            this.Controls.Add(greenBox);
//            this.Controls.Add(blueBox);
//            this.Controls.Add(btnLoad);

//            this.Controls.Add(chkR);
//            this.Controls.Add(chkG);
//            this.Controls.Add(chkB);

//            this.Controls.Add(trackR);
//            this.Controls.Add(trackG);
//            this.Controls.Add(trackB);
//        }

//        private void LoadImage(object sender, EventArgs e)
//        {
//            OpenFileDialog dlg = new OpenFileDialog();
//            dlg.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp";

//            if (dlg.ShowDialog() == DialogResult.OK)
//            {
//                original = new Bitmap(dlg.FileName);
//                originalBox.Image = original;
//                Apply(null, null);
//            }
//        }

//        // ✅ FIX: دالة Apply الفورية (للـ CheckBoxes فقط)
//        private void Apply(object sender, EventArgs e)
//        {
//            _debounceTimer.Stop();
//            ApplyChanges();
//        }

//        // ✅ FIX: المعالجة الفعلية مع Dispose للصور القديمة
//        private void ApplyChanges()
//        {
//            if (original == null) return;

//            processor.EnableR = chkR.Checked;
//            processor.EnableG = chkG.Checked;
//            processor.EnableB = chkB.Checked;

//            processor.RFactor = trackR.Value / 100f;
//            processor.GFactor = trackG.Value / 100f;
//            processor.BFactor = trackB.Value / 100f;

//            // ✅ FIX: تحرير الصور القديمة قبل إنشاء الجديدة
//            resultBox.Image?.Dispose();
//            redBox.Image?.Dispose();
//            greenBox.Image?.Dispose();
//            blueBox.Image?.Dispose();

//            // ✅ FIX: Process يُستدعى مرة واحدة فقط (كان يُستدعى مرتين!)
//            Bitmap result = processor.Process(original);
//            resultBox.Image = result;
//            redBox.Image = processor.ExtractRed(original);
//            greenBox.Image = processor.ExtractGreen(original);
//            blueBox.Image = processor.ExtractBlue(original);

//            ImageModified?.Invoke(result);
//        }

//        public void SetImage(Bitmap img)
//        {
//            // ✅ FIX: تحرير الصورة القديمة
//            original?.Dispose();
//            original = new Bitmap(img);
//            originalBox.Image = original;
//            ApplyChanges();
//        }

//    }
//}
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PixelLab.Core;

namespace Homwore
{
    public class FormChannels : Form
    {
        public event Action<Bitmap> ImageModified;

        PictureBox originalBox;
        PictureBox resultBox;
        PictureBox redBox;
        PictureBox greenBox;
        PictureBox blueBox;

        Button btnLoad;
        CheckBox chkR, chkG, chkB;
        TrackBar trackR, trackG, trackB;

        Label lblRTitle, lblGTitle, lblBTitle;
        Label lblRVal, lblGVal, lblBVal;

        Bitmap original;
        ChannelProcessor processor = new ChannelProcessor();

        private Timer _debounceTimer;

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

        // ── ألوان القنوات ────────────────────────────────────────────
        static readonly Color C_Red = Color.FromArgb(255, 80, 80);
        static readonly Color C_RedDim = Color.FromArgb(80, 20, 20);
        static readonly Color C_GreenCh = Color.FromArgb(52, 211, 153);
        static readonly Color C_GreenDim = Color.FromArgb(14, 50, 35);
        static readonly Color C_Blue = Color.FromArgb(64, 156, 255);
        static readonly Color C_BlueDim = Color.FromArgb(14, 35, 70);

        // ────────────────────────────────────────────────────────────
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int l, int t, int r, int b, int ew, int eh);

        private static void SetRound(Control c, int rad = 8) =>
            c.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, c.Width + 1, c.Height + 1, rad, rad));

        // ════════════════════════════════════════════════════════════
        public FormChannels()
        {
            _debounceTimer = new Timer();
            _debounceTimer.Interval = 300;
            _debounceTimer.Tick += (s, e) => { _debounceTimer.Stop(); ApplyChanges(); };

            BuildUI();
        }

        // ════════════════════════════════════════════════════════════
        //  بناء الواجهة بالثيم الداكن
        // ════════════════════════════════════════════════════════════
        private void BuildUI()
        {
            this.Text = "PixelLab — Channels";
            this.Size = new Size(1200, 750);
            this.BackColor = C_BgBase;
            this.ForeColor = C_TxtPri;

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
                Text = "Channels — تحليل القنوات اللونية",
                Location = new Point(28, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 8f),
                ForeColor = C_TxtDim,
                BackColor = Color.Transparent
            };
            titleBar.Controls.AddRange(new Control[] { lblTitleBar, lblSubBar });
            this.Controls.Add(titleBar);

            // ── Main content panel ────────────────────────────────────
            var pnlMain = new Panel { Location = new Point(0, 48), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            pnlMain.BackColor = C_BgPanel;
            this.Controls.Add(pnlMain);
            this.Resize += (s, e) =>
            {
                pnlMain.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 48);
            };
            pnlMain.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 48);

            // ── Load button ───────────────────────────────────────────
            btnLoad = MakeButton("📂 Load Image", 20, 18);
            btnLoad.Click += LoadImage;
            pnlMain.Controls.Add(btnLoad);

            // ── Section label: Preview ────────────────────────────────
            pnlMain.Controls.Add(MakeSectionLabel("ORIGINAL", 20, 70));
            pnlMain.Controls.Add(MakeSectionLabel("RESULT", 340, 70));

            // ── PictureBoxes — original & result ─────────────────────
            originalBox = MakePicBox(20, 90, 300, 260);
            resultBox = MakePicBox(340, 90, 300, 260);
            pnlMain.Controls.Add(originalBox);
            pnlMain.Controls.Add(resultBox);

            // ── Divider ───────────────────────────────────────────────
            var divider = new Panel
            {
                Location = new Point(660, 70),
                Size = new Size(1, 290),
                BackColor = C_BorderMid
            };
            pnlMain.Controls.Add(divider);

            // ── Controls panel (right) ────────────────────────────────
            var pnlControls = new Panel
            {
                Location = new Point(670, 70),
                Size = new Size(480, 290),
                BackColor = C_BgCard
            };
            pnlControls.Paint += PaintRoundedBorder;
            pnlMain.Controls.Add(pnlControls);

            BuildChannelRow(pnlControls, "R", "RED CHANNEL", C_Red, C_RedDim, 0, 10,
                out chkR, out trackR, out lblRTitle, out lblRVal);
            BuildChannelRow(pnlControls, "G", "GREEN CHANNEL", C_GreenCh, C_GreenDim, 1, 105,
                out chkG, out trackG, out lblGTitle, out lblGVal);
            BuildChannelRow(pnlControls, "B", "BLUE CHANNEL", C_Blue, C_BlueDim, 2, 200,
                out chkB, out trackB, out lblBTitle, out lblBVal);

            // ── Channel image boxes ───────────────────────────────────
            pnlMain.Controls.Add(MakeSectionLabel("RED", 20, 370));
            pnlMain.Controls.Add(MakeSectionLabel("GREEN", 240, 370));
            pnlMain.Controls.Add(MakeSectionLabel("BLUE", 460, 370));

            redBox = MakePicBox(20, 390, 200, 180);
            greenBox = MakePicBox(240, 390, 200, 180);
            blueBox = MakePicBox(460, 390, 200, 180);
            pnlMain.Controls.Add(redBox);
            pnlMain.Controls.Add(greenBox);
            pnlMain.Controls.Add(blueBox);
        }

        // ════════════════════════════════════════════════════════════
        //  بناء صف قناة واحد (checkbox + slider + labels)
        // ════════════════════════════════════════════════════════════
        private void BuildChannelRow(
            Panel parent, string key, string title,
            Color accentColor, Color dimColor, int index, int yOffset,
            out CheckBox chk, out TrackBar track, out Label lblTitle, out Label lblVal)
        {
            // header strip
            var strip = new Panel
            {
                Location = new Point(8, yOffset),
                Size = new Size(460, 28),
                BackColor = dimColor
            };
            strip.Paint += (s, e) =>
            {
                using var pen = new Pen(accentColor, 1);
                e.Graphics.DrawLine(pen, 0, 0, 0, strip.Height);
            };

            lblTitle = new Label
            {
                Text = title,
                Location = new Point(8, 6),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = accentColor,
                BackColor = Color.Transparent
            };

            chk = new CheckBox
            {
                Text = "",
                Location = new Point(430, 5),
                Size = new Size(24, 20),
                Checked = true,
                ForeColor = accentColor,
                BackColor = Color.Transparent
            };
            chk.CheckedChanged += Apply;

            strip.Controls.AddRange(new Control[] { lblTitle, chk });
            parent.Controls.Add(strip);

            // slider row
            track = new TrackBar
            {
                Location = new Point(8, yOffset + 34),
                Size = new Size(340, 45),
                Minimum = 0,
                Maximum = 200,
                Value = 100,
                TickFrequency = 20,
                BackColor = C_BgCard
            };
            track.Scroll += (s, e) => { _debounceTimer.Stop(); _debounceTimer.Start(); };

            lblVal = new Label
            {
                Text = "1.00×",
                Location = new Point(358, yOffset + 46),
                Size = new Size(60, 22),
                Font = new Font("Cascadia Code", 9f),
                ForeColor = accentColor,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            TrackBar localTrack = track;
            Label localLbl = lblVal;
            localTrack.Scroll += (s, e) => localLbl.Text = $"{localTrack.Value / 100f:0.00}×";

            parent.Controls.Add(track);
            parent.Controls.Add(lblVal);
        }

        // ════════════════════════════════════════════════════════════
        //  Factory helpers
        // ════════════════════════════════════════════════════════════
        private PictureBox MakePicBox(int x, int y, int w, int h)
        {
            var pb = new PictureBox
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = C_BgBase,
                BorderStyle = BorderStyle.None
            };
            pb.HandleCreated += (s, e) => SetRound(pb, 6);
            pb.Resize += (s, e) => SetRound(pb, 6);
            return pb;
        }

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

        private Label MakeSectionLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = C_Accent,
                BackColor = Color.Transparent
            };
        }

        private void PaintRoundedBorder(object sender, PaintEventArgs e)
        {
            var ctrl = (Control)sender;
            using var pen = new Pen(C_BorderMid, 1f);
            pen.DashStyle = DashStyle.Solid;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rc = new Rectangle(1, 1, ctrl.Width - 3, ctrl.Height - 3);
            DrawRoundedRect(e.Graphics, pen, rc, 6);
        }

        private static void DrawRoundedRect(Graphics g, Pen pen, Rectangle r, int rad)
        {
            using var path = new GraphicsPath();
            int d = rad * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            g.DrawPath(pen, path);
        }

        // ════════════════════════════════════════════════════════════
        //  المنطق — لم يتغير
        // ════════════════════════════════════════════════════════════
        private void LoadImage(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                original = new Bitmap(dlg.FileName);
                originalBox.Image = original;
                Apply(null, null);
            }
        }

        private void Apply(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            ApplyChanges();
        }

        private void ApplyChanges()
        {
            if (original == null) return;

            processor.EnableR = chkR.Checked;
            processor.EnableG = chkG.Checked;
            processor.EnableB = chkB.Checked;

            processor.RFactor = trackR.Value / 100f;
            processor.GFactor = trackG.Value / 100f;
            processor.BFactor = trackB.Value / 100f;

            resultBox.Image?.Dispose();
            redBox.Image?.Dispose();
            greenBox.Image?.Dispose();
            blueBox.Image?.Dispose();

            Bitmap result = processor.Process(original);
            resultBox.Image = result;
            redBox.Image = processor.ExtractRed(original);
            greenBox.Image = processor.ExtractGreen(original);
            blueBox.Image = processor.ExtractBlue(original);

            ImageModified?.Invoke(result);
        }

        public void SetImage(Bitmap img)
        {
            original?.Dispose();
            original = new Bitmap(img);
            originalBox.Image = original;
            ApplyChanges();
        }
    }
}