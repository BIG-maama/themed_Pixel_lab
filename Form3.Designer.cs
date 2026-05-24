namespace Homwore
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblColorCount = new System.Windows.Forms.Label();
            this.trackBarColors = new System.Windows.Forms.TrackBar();
            this.pictureBoxOriginal = new System.Windows.Forms.PictureBox();
            this.pictureBoxResult = new System.Windows.Forms.PictureBox();
            this.btnUpload = new System.Windows.Forms.Button();
            this.cmbColorSystem = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarColors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).BeginInit();
            this.SuspendLayout();
            // 
            // lblColorCount
            // 
            this.lblColorCount.AutoSize = true;
            this.lblColorCount.Location = new System.Drawing.Point(204, 43);
            this.lblColorCount.Name = "lblColorCount";
            this.lblColorCount.Size = new System.Drawing.Size(32, 17);
            this.lblColorCount.TabIndex = 0;
            this.lblColorCount.Text = "256";
            // 
            // trackBarColors
            // 
            this.trackBarColors.Location = new System.Drawing.Point(452, 43);
            this.trackBarColors.Maximum = 256;
            this.trackBarColors.Minimum = 2;
            this.trackBarColors.Name = "trackBarColors";
            this.trackBarColors.Size = new System.Drawing.Size(908, 56);
            this.trackBarColors.TabIndex = 1;
            this.trackBarColors.Value = 256;
            this.trackBarColors.Scroll += new System.EventHandler(this.trackBarColors_Scroll);
            // 
            // pictureBoxOriginal
            // 
            this.pictureBoxOriginal.Location = new System.Drawing.Point(927, 200);
            this.pictureBoxOriginal.Name = "pictureBoxOriginal";
            this.pictureBoxOriginal.Size = new System.Drawing.Size(701, 575);
            this.pictureBoxOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxOriginal.TabIndex = 2;
            this.pictureBoxOriginal.TabStop = false;
            // 
            // pictureBoxResult
            // 
            this.pictureBoxResult.Location = new System.Drawing.Point(170, 209);
            this.pictureBoxResult.Name = "pictureBoxResult";
            this.pictureBoxResult.Size = new System.Drawing.Size(719, 566);
            this.pictureBoxResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxResult.TabIndex = 3;
            this.pictureBoxResult.TabStop = false;
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(1419, 829);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(183, 52);
            this.btnUpload.TabIndex = 4;
            this.btnUpload.Text = "Load Image";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // cmbColorSystem
            // 
            this.cmbColorSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColorSystem.FormattingEnabled = true;
            this.cmbColorSystem.Items.AddRange(new object[] {
            "RGB",
            "CMY",
            "HSV",
            "YUV",
            "LAB",
            "YCbCr"});
            this.cmbColorSystem.Location = new System.Drawing.Point(204, 80);
            this.cmbColorSystem.Name = "cmbColorSystem";
            this.cmbColorSystem.Size = new System.Drawing.Size(200, 24);
            this.cmbColorSystem.TabIndex = 5;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1714, 925);
            this.Controls.Add(this.cmbColorSystem);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.pictureBoxResult);
            this.Controls.Add(this.pictureBoxOriginal);
            this.Controls.Add(this.trackBarColors);
            this.Controls.Add(this.lblColorCount);
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarColors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblColorCount;
        private System.Windows.Forms.TrackBar trackBarColors;
        private System.Windows.Forms.PictureBox pictureBoxOriginal;
        private System.Windows.Forms.PictureBox pictureBoxResult;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.ComboBox cmbColorSystem;
    }
}