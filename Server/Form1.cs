using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        private TcpListener listener;
        private Thread serverThread;
        private string downloadFolder = "downloads";
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Timer pingTimer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartServer();
            UpdateRecentFiles();
            UpdatePing();
            UpdateLocalIPAddress();
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 60 * 1000; 
            updateTimer.Tick += (s, args) => UpdateRecentFiles();
            updateTimer.Start();

            pingTimer = new System.Windows.Forms.Timer();
            pingTimer.Interval = 5000; 
            pingTimer.Tick += (s, args) => UpdatePing();
            pingTimer.Start();
        }

        private void StartServer()
        {
            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }

            listener = new TcpListener(IPAddress.Any, 20089);
            listener.Start();
            serverThread = new Thread(HandleClients);
            serverThread.Start();
            Invoke((Action)(() =>
            {
                string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                textBox3.AppendText($"[{dateTimeNow}] Server started. Waiting for connections.{Environment.NewLine}");
            }));
        }

        private void HandleClients()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        private void HandleClient(TcpClient client)
        {
            string fileName = null;
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[4];
                stream.Read(buffer, 0, buffer.Length);
                int fileNameLength = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[fileNameLength];
                stream.Read(buffer, 0, buffer.Length);
                fileName = System.Text.Encoding.UTF8.GetString(buffer);

                buffer = new byte[8];
                stream.Read(buffer, 0, buffer.Length);
                long fileLength = BitConverter.ToInt64(buffer, 0);

                buffer = new byte[8192];
                string filePath = Path.Combine(downloadFolder, fileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    int bytesRead;
                    long totalBytesRead = 0;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;

                        if (totalBytesRead >= fileLength)
                        {
                            break;
                        }
                    }
                }
            }

            client.Close();

            Invoke((Action)(() =>
            {
                string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                textBox3.AppendText($"[{dateTimeNow}] File \"{fileName}\" received and saved in \"{downloadFolder}\".{Environment.NewLine}");
            }));

            UpdateRecentFiles();
        }



        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            listener.Stop();
            serverThread.Abort();
            updateTimer.Stop();
            pingTimer.Stop();
        }

        private void UpdateRecentFiles()
        {
            DateTime cutoffTime = DateTime.Now.AddDays(-1);

            var recentFiles = Directory.GetFiles(downloadFolder)
                .Select(f => new FileInfo(f))
                .Where(f => f.LastWriteTime >= cutoffTime)
                .OrderByDescending(f => f.LastWriteTime)
                .ToList();

            textBox1.Clear();

            foreach (var file in recentFiles)
            {
                textBox1.AppendText(file.Name + Environment.NewLine);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void UpdatePing()
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = ping.Send("212.132.108.241");

                    if (reply.Status == IPStatus.Success)
                    {
                        label8.Text = $"Ping: {reply.RoundtripTime} ms";
                    }
                    else
                    {
                        label8.Text = $"Ping failed: {reply.Status}";
                    }
                }
                catch (Exception ex)
                {
                    label8.Text = $"Error at sending Ping: {ex.Message}";
                }
            }
        }

        private void UpdateLocalIPAddress()
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());

                IPAddress localIP = hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip));

                if (localIP != null)
                {
                    label5.Text = $"Server IP Address: {localIP}";
                }
                else
                {
                    label5.Text = "Server IP Address: Not found";
                }
            }
            catch (Exception ex)
            {
                label5.Text = $"Error getting IP address: {ex.Message}";
            }
        }


        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
