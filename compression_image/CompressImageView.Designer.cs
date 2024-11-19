namespace compression_image
{
    partial class CompressImageView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLoadImage = new Button();
            btnCompressLZW = new Button();
            btnCompressDeflate = new Button();
            btnDecompress = new Button();
            txtImagePath = new TextBox();
            cmbCompressionMethod = new ComboBox();
            progressBar = new ProgressBar();
            label1 = new Label();
            pictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // btnLoadImage
            // 
            btnLoadImage.Location = new Point(86, 59);
            btnLoadImage.Name = "btnLoadImage";
            btnLoadImage.Size = new Size(177, 61);
            btnLoadImage.TabIndex = 0;
            btnLoadImage.Text = "Tải ảnh";
            btnLoadImage.UseVisualStyleBackColor = true;
            btnLoadImage.Click += btnLoadImage_Click;
            // 
            // btnCompressLZW
            // 
            btnCompressLZW.Location = new Point(1329, 236);
            btnCompressLZW.Name = "btnCompressLZW";
            btnCompressLZW.Size = new Size(274, 60);
            btnCompressLZW.TabIndex = 1;
            btnCompressLZW.Text = "Nén với LZ4";
            btnCompressLZW.UseVisualStyleBackColor = true;
            btnCompressLZW.Click += btnCompressLZW_Click;
            // 
            // btnCompressDeflate
            // 
            btnCompressDeflate.Location = new Point(1329, 336);
            btnCompressDeflate.Name = "btnCompressDeflate";
            btnCompressDeflate.Size = new Size(274, 63);
            btnCompressDeflate.TabIndex = 2;
            btnCompressDeflate.Text = "Nén với Deflate";
            btnCompressDeflate.UseVisualStyleBackColor = true;
            btnCompressDeflate.Click += btnCompressDeflate_Click;
            // 
            // btnDecompress
            // 
            btnDecompress.Location = new Point(996, 732);
            btnDecompress.Name = "btnDecompress";
            btnDecompress.Size = new Size(274, 63);
            btnDecompress.TabIndex = 3;
            btnDecompress.Text = "Giải nén";
            btnDecompress.UseVisualStyleBackColor = true;
            btnDecompress.Click += btnDecompress_Click;
            // 
            // txtImagePath
            // 
            txtImagePath.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtImagePath.Location = new Point(287, 70);
            txtImagePath.Name = "txtImagePath";
            txtImagePath.Size = new Size(1336, 50);
            txtImagePath.TabIndex = 4;
            txtImagePath.TextChanged += txtImagePath_TextChanged;
            // 
            // cmbCompressionMethod
            // 
            cmbCompressionMethod.Font = new Font("Segoe UI", 12F);
            cmbCompressionMethod.FormattingEnabled = true;
            cmbCompressionMethod.Location = new Point(655, 735);
            cmbCompressionMethod.Name = "cmbCompressionMethod";
            cmbCompressionMethod.Size = new Size(274, 53);
            cmbCompressionMethod.TabIndex = 5;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(86, 829);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1537, 46);
            progressBar.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(287, 737);
            label1.Name = "label1";
            label1.Size = new Size(334, 45);
            label1.TabIndex = 7;
            label1.Text = "Phương pháp giải nén";
            // 
            // pictureBox
            // 
            pictureBox.Location = new Point(287, 159);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(1007, 532);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex = 8;
            pictureBox.TabStop = false;
            // 
            // CompressImageView
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1697, 916);
            Controls.Add(label1);
            Controls.Add(progressBar);
            Controls.Add(cmbCompressionMethod);
            Controls.Add(txtImagePath);
            Controls.Add(btnDecompress);
            Controls.Add(btnCompressDeflate);
            Controls.Add(btnCompressLZW);
            Controls.Add(btnLoadImage);
            Controls.Add(pictureBox);
            Name = "CompressImageView";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Nén hình ảnh";
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion


        private Button btnLoadImage;
        private Button btnCompressLZW;
        private Button btnCompressDeflate;
        private Button btnDecompress;
        private TextBox txtImagePath;
        private ComboBox cmbCompressionMethod;
        private ProgressBar progressBar;
        private Label label1;
        private PictureBox pictureBox;
    }
}
