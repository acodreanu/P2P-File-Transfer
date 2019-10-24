using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.IO;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        public static string FILETRANSFER = "FTRANS090";
        public static string FILETRANSFER_ENCRYPTED = "ENCRYPT090_FTRANS_";

        NetworkManager networkManager;

        private bool _connection = false;
        public bool encryptTraffic = false;
        public string password;
        public enum AcceptFilePermision { UNDEFINED, YES, NO };

        public int AcceptedFile = (int)AcceptFilePermision.UNDEFINED;

        public bool Connection {
            get { return _connection; }
            set {
                _connection = value;
                SetConnected(_connection);
            }
        }

        public Form1()
        {
            InitializeComponent();
            Connection = false;
            networkManager = new NetworkManager(this);
            FileTransferWorker.WorkerReportsProgress = true;
            FileTransferWorker.WorkerSupportsCancellation = true;
            ProgressLabel.Visible = true;
            ProgressLabel.Text = "";
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }

            int i;

            for (i = 0; i < value.Length; i++)
            {
                if (value[i].Equals('\0'))
                {
                    break;
                }
            }

            StringBuilder sb = new StringBuilder(value);
            sb.Remove(i, value.Length - i);
            sb.Insert(sb.Length, Environment.NewLine);
            value = sb.ToString();

            textBox3.AppendText(value);
        }

        public void SetConnected(bool connected)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetConnected), new object[] { connected });
                return;
            }

            if (connected)
            {
                label2.Text = "Connected";
                button2.Visible = true;
            } else
            {
                label2.Text = "Not Connected";
                button2.Visible = true;
            }
        }

        public void SetPercentage(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(SetPercentage), new object[] { value });
                return;
            }

            ProgressLabel.Text = value;
        }

        public void FileTransferWindow(string Filename)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(FileTransferWindow), new object[] { Filename });
                return;
            }

            if (MessageBox.Show("Do you want to receive file " + Filename + " ?", "Incoming file transfer!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                AcceptedFile = (int)AcceptFilePermision.YES;
            } else
            {
                AcceptedFile = (int)AcceptFilePermision.NO;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            Connection = false;
            //if (MessageBox.Show("Do you really want to quit?", "Whatever...", MessageBoxButtons.YesNo) == DialogResult.No)
            //{
            //    e.Cancel = true;
            //}
        }

        private void FileTransferWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (((FileTransferInfo)e.Argument).isSending())
            {
                networkManager.SendFile(((FileTransferInfo)e.Argument).getPath());
            } else
            {

            }
        }

        private void FileTransferWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressLabel.Text = (e.ProgressPercentage.ToString() + "%");
        }

        private void FileTransferWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                AppendTextBox("File transfer error!");
            }
            else if (e.Cancelled)
            {
                AppendTextBox("File transfer canceled!");
            }
            else
            {
                AppendTextBox("File transfer complete!");
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            networkManager.Connect(IP_textBox.Text);
            IP_textBox.Clear();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            networkManager.threadSendQueue.Enqueue(Chat_textBox.Text);
            Chat_textBox.Clear();
        }

        private void IP_textBox_TextChanged(object sender, EventArgs e)
        {
            if (IP_textBox.Text.Length > 0 && IP_textBox.Text[IP_textBox.Text.Length - 1].Equals('\n'))
            {
                ConnectButton_Click(this, null);
            }
        }

        private void Chat_textBox_TextChanged(object sender, EventArgs e)
        {
            if (Chat_textBox.Text.Length > 0 && Chat_textBox.Text[Chat_textBox.Text.Length - 1].Equals('\n'))
            {
                SendButton_Click(this, null);
            }
        }

        private void BigChat_textBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void FileTransfer_button_Click(object sender, EventArgs e)
        {
            Stream stream = null;
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.InitialDirectory = "c:\\";
            fileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            fileDialog.FilterIndex = 2;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((stream = fileDialog.OpenFile()) != null)
                    {
                        FileTransferInfo ftInfo = new FileTransferInfo(fileDialog.FileName, true);
                        stream.Close();
                        FileTransferWorker.RunWorkerAsync(ftInfo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void encrypt_checkbox_CheckedChanged(object sender, EventArgs e) {
            if (encrypt_checkbox.Checked) {
                encryptTraffic = true;
                password_textBox.Visible = pass_label.Visible = true;
            } else {
                encryptTraffic = false;
                password_textBox.Visible = pass_label.Visible = false;
            }
        }

        private void password_textBox_TextChanged(object sender, EventArgs e) {
            password = password_textBox.Text;
        }
    }

    public class FileTransferInfo
    {
        string Path;
        bool Sending;

        public FileTransferInfo(string filePath, bool isSending)
        {
            Path = filePath;
            Sending = isSending;
        }

        public string getPath()
        {
            return Path;
        }

        public bool isSending()
        {
            return Sending;
        }
    }
}
