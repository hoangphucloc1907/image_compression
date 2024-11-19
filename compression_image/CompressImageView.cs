using K4os.Compression.LZ4;
using Npgsql;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
namespace compression_image
{
    public partial class CompressImageView : Form
    {
        private string selectedImagePath = string.Empty;
        private readonly BackgroundWorker backgroundWorker;
        private readonly ImageCompressor imageCompressor;
        private readonly DatabaseHelper databaseHelper;

        public CompressImageView()
        {
            InitializeComponent();
            cmbCompressionMethod.Items.AddRange(new string[] { "LZ4", "DEFLATE" });
            cmbCompressionMethod.SelectedIndex = 0;  // Default to LZ4

            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            imageCompressor = new ImageCompressor();
            databaseHelper = new DatabaseHelper();
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.tiff;*.tif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedImagePath = openFileDialog.FileName;
                txtImagePath.Text = selectedImagePath;

                // Load the selected image into the PictureBox
                pictureBox.Image = Image.FromFile(selectedImagePath);
            }
        }

        private void txtImagePath_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtImagePath.Text))
            {
                selectedImagePath = txtImagePath.Text;
                pictureBox.Image = Image.FromFile(selectedImagePath);
            }
            else
            {
                pictureBox.Image = null;
            }
        }

        private void btnCompressLZW_Click(object sender, EventArgs e)
        {
            StartBackgroundOperation("LZ4");
        }

        private void btnCompressDeflate_Click(object sender, EventArgs e)
        {
            StartBackgroundOperation("DEFLATE");
        }

        private void btnDecompress_Click(object sender, EventArgs e)
        {
            StartBackgroundOperation("DECOMPRESS");
        }

        private void StartBackgroundOperation(string operation)
        {
            if (string.IsNullOrEmpty(selectedImagePath) && operation != "DECOMPRESS")
            {
                MessageBox.Show("Vui lòng chọn một hình ảnh.");
                return;
            }

            if (!backgroundWorker.IsBusy)
            {
                DisableButtons();
                backgroundWorker.RunWorkerAsync(operation);
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string operation = e.Argument.ToString();
            byte[] imageBytes = null;
            byte[] resultBytes = null;
            string compressionMethod = string.Empty;
            string message = string.Empty;
            long elapsedTimeMs = 0;

            if (operation == "LZ4" || operation == "DEFLATE")
            {
                compressionMethod = operation;

                if (databaseHelper.ImageExistsInDatabase(Path.GetFileName(selectedImagePath), compressionMethod))
                {
                    e.Result = "Hình ảnh đã tồn tại trong cơ sở dữ liệu với phương pháp nén được chỉ định.";
                    return;
                }

                imageBytes = File.ReadAllBytes(selectedImagePath);
                resultBytes = imageCompressor.Compress(imageBytes, compressionMethod, out elapsedTimeMs);

                int compressedSize = resultBytes.Length;
                databaseHelper.SaveToDatabase(resultBytes, compressionMethod, Path.GetFileName(selectedImagePath), imageBytes.Length, compressedSize, (int)elapsedTimeMs, 0);
                message = $"Hình ảnh được nén bằng {compressionMethod}.\nKích thước nén: {GetSizeInMB(resultBytes):F2} MB\nThời gian nén: {elapsedTimeMs} ms.";
            }
            else if (operation == "DECOMPRESS")
            {
                compressionMethod = cmbCompressionMethod.InvokeRequired
                    ? (string)cmbCompressionMethod.Invoke(new Func<string>(() => cmbCompressionMethod.SelectedItem.ToString()))
                    : cmbCompressionMethod.SelectedItem.ToString();

                if (!databaseHelper.ImageExistsInDatabase(Path.GetFileName(selectedImagePath), compressionMethod))
                {
                    e.Result = "Không tìm thấy dữ liệu nén cho phương pháp đã chọn.";
                    return;
                }

                var (compressedData, originalFileName) = databaseHelper.GetCompressedDataAndFileNameFromDatabase(compressionMethod);

                resultBytes = imageCompressor.Decompress(compressedData, compressionMethod, out elapsedTimeMs);

                string originalExtension = Path.GetExtension(originalFileName);
                string decompressedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{originalFileName}");

                File.WriteAllBytes(decompressedPath, resultBytes);

                // Update the database with decompression time and decompressed file size
                databaseHelper.UpdateDecompressionTime(compressionMethod, (int)elapsedTimeMs, resultBytes.Length);

                message = $"Hình ảnh được giải nén bằng {compressionMethod}.\nThời gian giải nén: {elapsedTimeMs} ms.\nTệp được lưu: {decompressedPath}";
            }

            for (int i = 0; i <= 100; i++)
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                backgroundWorker.ReportProgress(i);
                System.Threading.Thread.Sleep(10); // Simulate work
            }

            e.Result = message;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Operation was canceled.");
            }
            else if (e.Error != null)
            {
                MessageBox.Show($"An error occurred: {e.Error.Message}");
            }
            else
            {
                MessageBox.Show(e.Result.ToString());
            }
            progressBar.Value = 0;
            EnableButtons();
        }

        private void DisableButtons()
        {
            btnLoadImage.Enabled = false;
            btnCompressLZW.Enabled = false;
            btnCompressDeflate.Enabled = false;
            btnDecompress.Enabled = false;
        }

        private void EnableButtons()
        {
            btnLoadImage.Enabled = true;
            btnCompressLZW.Enabled = true;
            btnCompressDeflate.Enabled = true;
            btnDecompress.Enabled = true;
        }

        private double GetSizeInMB(byte[] data)
        {
            return data.Length / (1024.0 * 1024.0); // Convert bytes to MB
        }
    }
}

