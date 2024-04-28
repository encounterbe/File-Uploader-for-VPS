using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            label3.Visible = false;
            label4.Visible = true;
            progressBar1.Visible = true;
            label5.Visible = true;
            label6.Visible = true;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    using (TcpClient client = new TcpClient("SERVERIPADRESS", 20089))
                    {
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(Path.GetFileName(filePath));
                            byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
                            await stream.WriteAsync(fileNameLengthBytes, 0, fileNameLengthBytes.Length);
                            await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);

                            FileInfo fileInfo = new FileInfo(filePath);
                            long fileLength = fileInfo.Length;
                            byte[] fileLengthBytes = BitConverter.GetBytes(fileLength);
                            await stream.WriteAsync(fileLengthBytes, 0, fileLengthBytes.Length);

                            progressBar1.Maximum = (int)fileLength;
                            progressBar1.Value = 0;

                            DateTime startTime = DateTime.Now;

                            byte[] buffer = new byte[8192];
                            long totalBytesRead = 0;

                            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                int bytesRead;
                                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await stream.WriteAsync(buffer, 0, bytesRead);
                                    totalBytesRead += bytesRead;

                                    progressBar1.Value = (int)totalBytesRead;

                                    TimeSpan elapsedTime = DateTime.Now - startTime;
                                    double transferRate = totalBytesRead / elapsedTime.TotalSeconds;

                                    double transferRateMbits = transferRate * 8 / 1_000_000;

                                    if (transferRateMbits >= 1)
                                    {
                                        label6.Text = $"Upload speed: {transferRateMbits:0.00} Mbits/s";
                                    }

                                    double remainingBytes = fileLength - totalBytesRead;
                                    double estimatedTimeRemaining = remainingBytes / transferRate;

                                    label5.Text = $"Estimated time remaining: {estimatedTimeRemaining:0.00} seconds";

                                    Application.DoEvents();
                                }
                            }
                        }
                    }

                    button1.Visible = true;
                    label3.Visible = true;
                    label4.Visible = false;
                    progressBar1.Visible = false;
                    label5.Visible = false;
                    label6.Visible = false;

                    MessageBox.Show($"Uploaded '{Path.GetFileName(filePath)}' successfully.");
                }
            }
        }




        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
