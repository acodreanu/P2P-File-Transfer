using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    class NetworkManager
    {
        static int CHAT_PORT = 50000, FILE_TRANSFER_PORT = 50001;
        static string IP = "192.168.0.107";

        public ConcurrentQueue<string> threadSendQueue, threadRecvQueue;
        Thread sender, receiver;
        IPAddress ip;
        Socket socket, listeningSocket, fileTransferSocket;
        Form1 form;
        bool isSenderStarted = false;

        public enum AcceptFilePermision { UNDEFINED, YES, NO };

        public NetworkManager(Form1 app)
        {
            form = app;
            threadSendQueue = new ConcurrentQueue<string>();
            threadRecvQueue = new ConcurrentQueue<string>();
            
            receiver = new Thread(() =>
            {
                bool error = false;
                while (!error)
                {
                    IPAddress[] ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                    IPAddress ipAddress = IPAddress.Parse(IP);
                    IPEndPoint ep;

                    try
                    {
                        ep = new IPEndPoint(IPAddress.Any, CHAT_PORT);
                        listeningSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        listeningSocket.Bind(ep);
                        listeningSocket.Listen(50);
                    }
                    catch (Exception e)
                    {
                        form.AppendTextBox("System: " + e.Message);
                        listeningSocket.Close();
                        while (true)
                        {
                            try
                            {
                                System.Threading.SpinWait.SpinUntil(() => form.Connection);
                                Receive();
                            }
                            catch (Exception ex)
                            {
                                form.AppendTextBox("System: " + ex.Message);
                                form.Connection = false;
                                socket.Close();
                                break;
                            }
                        }
                        continue;
                    }

                    //socket = listeningSocket.Accept();
                    //listeningSocket.Close();
                    System.Threading.SpinWait.SpinUntil(() => form.Connection);

                    ip = (socket.RemoteEndPoint as IPEndPoint).Address;

                    form.Connection = true;

                    if (!isSenderStarted)
                    {
                        sender.Start();
                    }

                    while (true)
                    {
                        try
                        {
                            Receive();
                        }
                        catch (Exception e)
                        {
                            form.AppendTextBox("System: " + e.Message);
                            form.Connection = false;
                            socket.Close();
                            break;
                        }
                    }
                }
            });
            receiver.IsBackground = true;

            sender = new Thread(() =>
            {
                string msg;
                isSenderStarted = true;
                try
                {
                    while (CheckConnection())
                    {
                        System.Threading.SpinWait.SpinUntil(() => !threadSendQueue.IsEmpty);
                        threadSendQueue.TryDequeue(out msg);
                        socket.Send(Encoding.ASCII.GetBytes(msg), SocketFlags.None);
                        form.AppendTextBox("Me: " + msg);
                    }
                } catch(Exception ex)
                {
                    form.AppendTextBox("System: " + ex.Message);
                }
                isSenderStarted = false;
                form.Connection = false;
            });
            sender.IsBackground = true;

            receiver.Start();
        }

        public void Connect(string ip_string)
        {
            ip = IPAddress.Parse(ip_string);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ip, CHAT_PORT);

            form.Connection = true;

            sender.Start();
        }

        private void Receive()
        {
            byte[] bytes = new byte[1024];
            int byteCount = socket.Receive(bytes, SocketFlags.None);
            if (byteCount > 0)
            {
                string msg = Encoding.ASCII.GetString(bytes, 0, byteCount);
                if (0 == String.Compare(msg, 0, Form1.FILETRANSFER, 0, Form1.FILETRANSFER.Length))
                {
                    IPAddress ipAddress = IPAddress.Parse(IP);
                    listeningSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, FILE_TRANSFER_PORT);
                    listeningSocket.Bind(ep);
                    listeningSocket.Listen(1);
                    fileTransferSocket = listeningSocket.Accept();

                    byteCount = fileTransferSocket.Receive(bytes, SocketFlags.None);
                    msg = Encoding.ASCII.GetString(bytes, 0, byteCount);

                    ReceiveFile(msg, byteCount);
                }
                else if (0 == String.Compare(msg, 0, Form1.FILETRANSFER_ENCRYPTED, 0, Form1.FILETRANSFER_ENCRYPTED.Length)) {
                    IPAddress ipAddress = IPAddress.Parse(IP);
                    listeningSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, FILE_TRANSFER_PORT);
                    listeningSocket.Bind(ep);
                    listeningSocket.Listen(1);
                    fileTransferSocket = listeningSocket.Accept();

                    byteCount = fileTransferSocket.Receive(bytes, SocketFlags.None);
                    msg = Encoding.ASCII.GetString(bytes, 0, byteCount);

                    ReceiveEncryptedFile(msg, byteCount);
                } else
                {
                    threadRecvQueue.Enqueue(msg);
                    form.AppendTextBox("Friend: " + msg);
                }
            }
        }

        public void ReceiveFile(string Header, long ByteCount)
        {
            string fileName = new string('c', 5);
            long fileSize = 0;

            StringBuilder sb = new StringBuilder(Header);

            //sb.Remove(0, Form1.FILETRANSFER.Length + 1);
            //ByteCount -= Form1.FILETRANSFER.Length + 1;

            fileName = sb.ToString(0, sb.ToString().IndexOf('\0'));

            ByteCount -= sb.ToString().IndexOf('\0') + 1;
            sb.Remove(0, sb.ToString().IndexOf('\0') + 1);

            fileSize = Int32.Parse(sb.ToString(0, sb.ToString().IndexOf('\0')));

            ByteCount -= sb.ToString().IndexOf('\0') + 1;

            form.FileTransferWindow(fileName);

            System.Threading.SpinWait.SpinUntil(() => form.AcceptedFile != (int)AcceptFilePermision.UNDEFINED);

            if (form.AcceptedFile == (int)AcceptFilePermision.NO)
            {
                form.AppendTextBox("You refused the file transfer");
                fileTransferSocket.Send(Encoding.ASCII.GetBytes("NO"));
                fileTransferSocket.Close();
                return;
            } else
            {
                form.AppendTextBox("Receiving file...");
                fileTransferSocket.Send(Encoding.ASCII.GetBytes("YES"));
            }
            
            byte[] buffer = new byte[4096];
            int bytesRead;
            long totalBytes = fileSize;
            var outFile = File.Create(fileName);

            if (ByteCount > 0)
            {
                form.AppendTextBox("System: Something went wrong with the file transfer!");
            }

            while (fileSize > 0)
            {
                if (fileSize < 4096)
                {
                    bytesRead = fileTransferSocket.Receive(buffer, (int)fileSize, SocketFlags.None);
                } else
                {
                    bytesRead = fileTransferSocket.Receive(buffer, 4096, SocketFlags.None);
                }

                outFile.Write(buffer, 0, bytesRead);
                fileSize -= bytesRead;
                form.SetPercentage(((int)(((double)(totalBytes - fileSize) / (double)totalBytes) * 100.0)).ToString() + "%");
            }

            outFile.Close();
            fileTransferSocket.Close();
            form.SetPercentage("");
            form.AppendTextBox("File " + fileName + " received.");
        }

        public void ReceiveEncryptedFile(string Header, long ByteCount) {
            string fileName = new string('c', 5);
            long fileSize = 0;

            StringBuilder sb = new StringBuilder(Header);

            fileName = sb.ToString(0, sb.ToString().IndexOf('\0'));

            ByteCount -= sb.ToString().IndexOf('\0') + 1;
            sb.Remove(0, sb.ToString().IndexOf('\0') + 1);

            fileSize = Int32.Parse(sb.ToString(0, sb.ToString().IndexOf('\0')));

            ByteCount -= sb.ToString().IndexOf('\0') + 1;

            form.FileTransferWindow(fileName);

            System.Threading.SpinWait.SpinUntil(() => form.AcceptedFile != (int)AcceptFilePermision.UNDEFINED);

            if (form.AcceptedFile == (int)AcceptFilePermision.NO) {
                form.AppendTextBox("You refused the file transfer");
                fileTransferSocket.Send(Encoding.ASCII.GetBytes("NO"));
                fileTransferSocket.Close();
                return;
            }
            else {
                form.AppendTextBox("Receiving file...");
                fileTransferSocket.Send(Encoding.ASCII.GetBytes("YES"));
            }

            byte[] buffer = new byte[fileSize];
            int bytesRead;
            long totalBytes = fileSize;
            //var outFile = File.Create(fileName);

            if (ByteCount > 0) {
                form.AppendTextBox("System: Something went wrong with the file transfer!");
            }

            fileTransferSocket.Receive(buffer, (int)fileSize, SocketFlags.None);
            
            string password = "abcd1234";
            
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(buffer, passwordBytes);
            
            File.WriteAllBytes(fileName, bytesDecrypted);

            //while (fileSize > 0) {
            //    if (fileSize < 4096) {
            //        bytesRead = fileTransferSocket.Receive(buffer, (int)fileSize, SocketFlags.None);
            //    }
            //    else {
            //        bytesRead = fileTransferSocket.Receive(buffer, 4096, SocketFlags.None);
            //    }

            //    outFile.Write(buffer, 0, bytesRead);
            //    fileSize -= bytesRead;
            //    form.SetPercentage(((int)(((double)(totalBytes - fileSize) / (double)totalBytes) * 100.0)).ToString() + "%");
            //}

            //outFile.Close();
            fileTransferSocket.Close();
            form.SetPercentage("");
            form.AppendTextBox("File " + fileName + " received.");
        }

        private bool CheckConnection()
        {
            bool part1 = socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);
            if ((part1 && part2) || !socket.Connected)
                return false;
            else
                return true;
        }

        public void SendFile(string path)
        {
            if (form.Connection)
            {
                try
                {
                    string buffer = (new System.IO.FileInfo(path).Name) + "\0" +
                        (new System.IO.FileInfo(path).Length.ToString()) + "\0";
                    byte[] bytesEncrypted = { 0 };

                    if (form.encryptTraffic) {
                        if(String.IsNullOrEmpty(form.password)) {
                            form.AppendTextBox("Type a password.");
                            return;
                        }

                        socket.Send(Encoding.ASCII.GetBytes(Form1.FILETRANSFER_ENCRYPTED + "\0"));
                    }
                    else {
                        socket.Send(Encoding.ASCII.GetBytes(Form1.FILETRANSFER + "\0"));
                    }


                    fileTransferSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IAsyncResult result = fileTransferSocket.BeginConnect(ip, FILE_TRANSFER_PORT, null, null);

                    bool success = result.AsyncWaitHandle.WaitOne(CHAT_PORT, true);

                    if (!success)
                    {
                        fileTransferSocket.Close();
                        throw new ApplicationException("Failed to connect to file transfer socket (timeout)");
                    }

                    if(form.encryptTraffic) {
                        form.AppendTextBox("Sending with encryption.");
                       
                        string password = "abcd1234";

                        byte[] bytesToBeEncrypted = File.ReadAllBytes(path);
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                        // Hash the password with SHA256
                        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                        bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

                        buffer = (new System.IO.FileInfo(path).Name) + "\0" +
                        bytesEncrypted.Length.ToString() + "\0";
                    }

                    fileTransferSocket.Send(Encoding.ASCII.GetBytes(buffer));

                    string recvBuf;
                    byte[] bytes = new byte[16];
                    int bytesRead = fileTransferSocket.Receive(bytes, SocketFlags.None);
                    recvBuf = Encoding.ASCII.GetString(bytes, 0, bytesRead);

                    if (recvBuf.Equals("YES"))
                    {
                        form.AppendTextBox("Starting file transfer...");
                        if (form.encryptTraffic) {
                            fileTransferSocket.Send(bytesEncrypted);
                        }
                        else {
                            fileTransferSocket.SendFile(path);
                        }
                    } else if (recvBuf.Equals("NO"))
                    {
                        form.AppendTextBox("The file transfer was refused.");
                    } else
                    {
                        form.AppendTextBox("Something went terribly wrong");
                    }

                    fileTransferSocket.Close();
                } catch(Exception ex)
                {
                    form.AppendTextBox("System: " + ex.Message);
                    fileTransferSocket.Close();
                }
            }
        }

        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes) {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream()) {
                using (RijndaelManaged AES = new RijndaelManaged()) {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Padding = PaddingMode.PKCS7;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write)) {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes) {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream()) {
                using (RijndaelManaged AES = new RijndaelManaged()) {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Padding = PaddingMode.PKCS7;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write)) {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}
