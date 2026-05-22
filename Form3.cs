using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Homwore
{
    public partial class Form3 : Form
    {
        public event Action<Bitmap> ImageModified;
        Bitmap originalImage;
        public Form3()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";

            if (open.ShowDialog() == DialogResult.OK)
            {
                //  تحميل الصورة الأصلية
                originalImage = new Bitmap(open.FileName);
                pictureBoxOriginal.Image = originalImage; // عرض الصورة في مربع الأصل

                trackBarColors.Value = 256;

                lblColorCount.Text = "عدد المستويات: 256";

                // عرض الصورة الأصلية في مربع النتيجة أيضاً عند البداية
                pictureBoxResult.Image = (Bitmap)originalImage.Clone();
            }
        }

        private void trackBarColors_Scroll(object sender, EventArgs e)
        {
            if (originalImage == null) return;

            int levels = trackBarColors.Value;
            lblColorCount.Text = "عدد المستويات: " + levels.ToString();

            // ✅ FIX: تحرير الصورة القديمة قبل إنشاء الجديدة (منع تسريب الذاكرة)
            pictureBoxResult.Image?.Dispose();

            // ✅ FIX: استدعاء ReduceColors مرة واحدة فقط (كان يُستدعى مرتين!)
            Bitmap result = ImageProcessor.ReduceColors(originalImage, levels);
            pictureBoxResult.Image = result;
            ImageModified?.Invoke(result);
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        public void SetImage(Bitmap img)
        {
            // ✅ FIX: تحرير الصور القديمة
            originalImage?.Dispose();
            pictureBoxResult.Image?.Dispose();

            originalImage = new Bitmap(img);
            pictureBoxOriginal.Image = originalImage;
            pictureBoxResult.Image = (Bitmap)originalImage.Clone();
            trackBarColors.Value = 256;
            lblColorCount.Text = "عدد المستويات: 256";
        }
    }
}