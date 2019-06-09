using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const String serverIP = "14.9.118.64";
        const int serverPort = 8530;

        private String controlIP;
        private int controlPort;

        const int localPort = 5656;

        static int ID = 0;

        private Thread sendIPPortThread;
        private Thread registerIDThread;
        private Thread sendScreenImageThread;

        ChatWindow chatWindow;

        UDPProtocol udpProtocol;
        public MainWindow()
        {
            InitializeComponent();

            SendScreenImage();

            udpProtocol = new UDPProtocol(localPort);
            udpProtocol.UdpSocketReceiveStart(RunCommand);

            chatWindow = new ChatWindow((mess) =>
            {
                SendMessage(controlIP, controlPort, mess);
            });
            chatWindow.Show();

            ID = Properties.Settings.Default.ID;

            sendIPPortThread = new Thread(() =>
            {
                while (true)
                {
                    if (ID == 0) continue;
                    udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 3, (byte)ID });
                    Thread.Sleep(10000);
                }
            });

            registerIDThread = new Thread(() =>
            {
                while (ID == 0)
                {
                    udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 1 });
                    Thread.Sleep(5000);
                }
            });

            sendScreenImageThread = new Thread(() =>
            {
                while (true)
                {
                    SendScreenImage();
                    Thread.Sleep(500);
                }
            });

            sendIPPortThread.IsBackground = true;
            registerIDThread.IsBackground = true;
            sendScreenImageThread.IsBackground = true;

            sendIPPortThread.Start();
            registerIDThread.Start();
        }

        private byte[] BitmapSourceToArray(BitmapSource bitmapSource)
        {
            // Stride = (width) x (bytes per pixel)
            int stride = (int)bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)bitmapSource.PixelHeight * stride];

            bitmapSource.CopyPixels(pixels, stride, 0);

            return pixels;
        }

        private void SendMessage(String controlIP, int controlPort, String message)
        {
            byte[] messagebyte = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] sendbyte = new byte[messagebyte.Length + 2];

            sendbyte[0] = 8;
            sendbyte[1] = (byte)ID;


            Array.Copy(messagebyte, 0, sendbyte, 2, messagebyte.Length);
            udpProtocol.UdpSocketSend(controlIP, controlPort, sendbyte);
        }

        void RunCommand(IPEndPoint ipEndPoint, byte[] command)
        {
            switch (command[0])
            {
                case 1:
                    Properties.Settings.Default.ID = ID = command[1];
                    Properties.Settings.Default.Save();
                    break;

                case 2:

                    break;

                case 3:

                    break;

                case 7:
                    String ipport = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    String[] split = ipport.Split('|');

                    controlIP = split[0];
                    controlPort = int.Parse(split[1]);

                    for (int i = 0; i < 5000; i++)
                    {
                        udpProtocol.UdpSocketSend(controlIP, controlPort, new byte[] { 0 });
                    }

                    udpProtocol.UdpSocketSend(controlIP, controlPort, new byte[] { 7, (byte)ID });

                    chatWindow.Dispatcher.Invoke(() =>
                    {
                        chatWindow.Show();
                    });

                    break;

                case 8:
                    String mess = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    chatWindow.Dispatcher.Invoke(() =>
                    {
                        chatWindow.Show();
                        chatWindow.PushMessage(mess, false);
                    });

                    break;

                case 10:
                    String ipportViewScreen = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    String[] splitViewScreen = ipportViewScreen.Split('|');

                    controlIP = splitViewScreen[0];
                    controlPort = int.Parse(splitViewScreen[1]);

                    if (sendScreenImageThread.ThreadState == ThreadState.Suspended)
                    {
                        sendScreenImageThread.Resume();
                    }
                    else
                    {
                        sendScreenImageThread.Start();
                    }

                    break;
            }
        }

        private void SendScreenImage()
        {
            //BitmapSource screen = CopyScreen();
            //byte[] imageData = BitmapSourceToArray(screen);

            Bitmap screen = CopyScreen();
            byte[] width = BitConverter.GetBytes(screen.Width);
            byte[] height = BitConverter.GetBytes(screen.Height);
            byte[] imageData = BitmapToByteArray(screen);
            byte[] sendData = new byte[] { 10, (byte)ID };
            sendData = sendData.Concat(width).Concat(height).Concat(imageData).ToArray();

            udpProtocol.UdpSocketSend(controlIP, controlPort, sendData);
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }
        private Bitmap CopyScreen()
        {
            var screenBmp = new Bitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

            using (var bmpGraphics = Graphics.FromImage(screenBmp))
            {
                bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                //return Imaging.CreateBitmapSourceFromHBitmap(
                //    screenBmp.GetHbitmap(),
                //    IntPtr.Zero,
                //    Int32Rect.Empty,
                //    BitmapSizeOptions.FromEmptyOptions());

                return screenBmp;
            }

        }

        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
