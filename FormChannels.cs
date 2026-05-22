using System;
using System.Drawing;
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

        CheckBox chkR;
        CheckBox chkG;
        CheckBox chkB;

        TrackBar trackR;
        TrackBar trackG;
        TrackBar trackB;

        Bitmap original;
        ChannelProcessor processor = new ChannelProcessor();

        // ✅ FIX: Debouncing Timer — منع المعالجة المتكررة عند تحريك الـ Slider
        private Timer _debounceTimer;


        public FormChannels()
        {
            // ✅ FIX: إعداد Timer للـ Debouncing (300ms)
            _debounceTimer = new Timer();
            _debounceTimer.Interval = 300;
            _debounceTimer.Tick += (s, e) => { _debounceTimer.Stop(); ApplyChanges(); };

            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "PixelLab - Channels Module";
            this.Size = new Size(1200, 700);

            originalBox = new PictureBox() { Location = new Point(20, 20), Size = new Size(300, 250), SizeMode = PictureBoxSizeMode.Zoom };
            resultBox = new PictureBox() { Location = new Point(340, 20), Size = new Size(300, 250), SizeMode = PictureBoxSizeMode.Zoom };

            redBox = new PictureBox() { Location = new Point(20, 300), Size = new Size(200, 200), SizeMode = PictureBoxSizeMode.Zoom };
            greenBox = new PictureBox() { Location = new Point(240, 300), Size = new Size(200, 200), SizeMode = PictureBoxSizeMode.Zoom };
            blueBox = new PictureBox() { Location = new Point(460, 300), Size = new Size(200, 200), SizeMode = PictureBoxSizeMode.Zoom };

            btnLoad = new Button() { Text = "Load", Location = new Point(700, 20) };
            btnLoad.Click += LoadImage;

            chkR = new CheckBox() { Text = "R", Location = new Point(700, 80), Checked = true };
            chkG = new CheckBox() { Text = "G", Location = new Point(700, 110), Checked = true };
            chkB = new CheckBox() { Text = "B", Location = new Point(700, 140), Checked = true };

            trackR = new TrackBar() { Minimum = 0, Maximum = 200, Value = 100, Location = new Point(700, 200) };
            trackG = new TrackBar() { Minimum = 0, Maximum = 200, Value = 100, Location = new Point(700, 260) };
            trackB = new TrackBar() { Minimum = 0, Maximum = 200, Value = 100, Location = new Point(700, 320) };

            chkR.CheckedChanged += Apply;
            chkG.CheckedChanged += Apply;
            chkB.CheckedChanged += Apply;

            // ✅ FIX: الـ TrackBars تستخدم Debouncing بدل Apply مباشرة
            trackR.Scroll += (s, e) => { _debounceTimer.Stop(); _debounceTimer.Start(); };
            trackG.Scroll += (s, e) => { _debounceTimer.Stop(); _debounceTimer.Start(); };
            trackB.Scroll += (s, e) => { _debounceTimer.Stop(); _debounceTimer.Start(); };

            this.Controls.Add(originalBox);
            this.Controls.Add(resultBox);
            this.Controls.Add(redBox);
            this.Controls.Add(greenBox);
            this.Controls.Add(blueBox);
            this.Controls.Add(btnLoad);

            this.Controls.Add(chkR);
            this.Controls.Add(chkG);
            this.Controls.Add(chkB);

            this.Controls.Add(trackR);
            this.Controls.Add(trackG);
            this.Controls.Add(trackB);
        }

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

        // ✅ FIX: دالة Apply الفورية (للـ CheckBoxes فقط)
        private void Apply(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            ApplyChanges();
        }

        // ✅ FIX: المعالجة الفعلية مع Dispose للصور القديمة
        private void ApplyChanges()
        {
            if (original == null) return;

            processor.EnableR = chkR.Checked;
            processor.EnableG = chkG.Checked;
            processor.EnableB = chkB.Checked;

            processor.RFactor = trackR.Value / 100f;
            processor.GFactor = trackG.Value / 100f;
            processor.BFactor = trackB.Value / 100f;

            // ✅ FIX: تحرير الصور القديمة قبل إنشاء الجديدة
            resultBox.Image?.Dispose();
            redBox.Image?.Dispose();
            greenBox.Image?.Dispose();
            blueBox.Image?.Dispose();

            // ✅ FIX: Process يُستدعى مرة واحدة فقط (كان يُستدعى مرتين!)
            Bitmap result = processor.Process(original);
            resultBox.Image = result;
            redBox.Image = processor.ExtractRed(original);
            greenBox.Image = processor.ExtractGreen(original);
            blueBox.Image = processor.ExtractBlue(original);

            ImageModified?.Invoke(result);
        }

        public void SetImage(Bitmap img)
        {
            // ✅ FIX: تحرير الصورة القديمة
            original?.Dispose();
            original = new Bitmap(img);
            originalBox.Image = original;
            ApplyChanges();
        }

    }
}