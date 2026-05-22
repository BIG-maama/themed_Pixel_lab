// ===================================================================
//  مثال على طريقة الدمج مع باقي المجموعة
//  ضع هذا الكود في الـ Form الرئيسي للمشروع
// ===================================================================
//
//  using PixelLab.ColorSystems;
//  using PixelLab.ColorSystems.Models;
//
//
//  public partial class MainForm : Form
//  {
//      // ① أنشئ instance واحد في الـ Form
//      private readonly ColorSystemManager _colorManager = new();
//
//      private void MainForm_Load(object sender, EventArgs e)
//      {
//          // ② ابنِ الـ ComboBox تلقائياً من الأنظمة المتاحة
//          comboBoxSystems.Items.AddRange(
//              _colorManager.AvailableSystems.ToArray());
//          comboBoxSystems.SelectedIndex = 0;
//      }
//
//      // ③ عند الضغط على بكسل في الصورة
//      private void pictureBox_MouseClick(object sender, MouseEventArgs e)
//      {
//          Color clickedColor = currentBitmap.GetPixel(e.X, e.Y);
//
//          // تحويل إلى جميع الأنظمة مرة واحدة (المتطلب 5)
//          var allResults = _colorManager.ConvertAll(clickedColor);
//          foreach (ColorResult result in allResults)
//              Console.WriteLine(result);
//          // مثال الطباعة:
//          //   CMY   → C: 135, M: 175, B: 55
//          //   HSV   → H: 260.00, S: 0.6000, V: 0.7800
//          //   YUV   → Y: 97.73, U: 28.11, V: -38.77
//          //   YCbCr → Y: 97.73, Cb: 142.06, Cr: 89.23
//          //   L*a*b*→ L*: 42.84, a*: 27.43, b*: -48.91
//      }
//
//      // ④ عند تحريك الـ Slider (المتطلب 3 و 6)
//      private void sliderHue_ValueChanged(object sender, EventArgs e)
//      {
//          string system = comboBoxSystems.SelectedItem.ToString();
//
//          double ch1 = sliderChannel1.Value;
//          double ch2 = sliderChannel2.Value / 100.0; // تطبيع 0-1
//          double ch3 = sliderChannel3.Value / 100.0;
//
//          // تحويل القيم الجديدة لـ RGB وعرض الصورة
//          Color newColor = _colorManager.UpdateChannel(system, ch1, ch2, ch3);
//          pictureBox.BackColor = newColor;
//      }
//
//      // ⑤ عند تغيير النظام من الـ ComboBox
//      private void comboBoxSystems_SelectedIndexChanged(object sender, EventArgs e)
//      {
//          string system = comboBoxSystems.SelectedItem.ToString();
//
//          // تسمية الـ Sliders تلقائياً
//          string[] names = _colorManager.GetChannelNames(system);
//          labelCh1.Text = names[0];
//          labelCh2.Text = names[1];
//          labelCh3.Text = names[2];
//      }
//  }
