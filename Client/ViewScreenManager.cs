using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Client
{
    class ViewScreenManager
    {
        private double windowsScaling;

        private int imageWidth = 200;
        private int imageHeight = 200;

        private int screenWidth;
        private int screenHeight;

        private int blockWidth;
        private int blockHeight;

        private int ID;
        private String controlIP;
        private int controlPort;

        private UDPProtocol udpProtocol;

        private Thread sendScreenImageThread;

        public double WindowsScaling { get { return windowsScaling; } }
        public int ImageWidth { get { return imageWidth; } set { imageWidth = value; } }
        public int ImageHeight { get { return imageHeight; } set { imageHeight = value; } }
        public int ScreenWidth { get { return screenWidth; } set { screenWidth = value; } }
        public int ScreenHeight { get { return screenHeight; } set { screenHeight = value; } }
        public int BlockWidth { get { return blockWidth; } set { blockWidth = value; } }
        public int BlockHeight { get { return blockHeight; } set { blockHeight = value; } }

        public ViewScreenManager(int ID, String controlIP, int controlPort, UDPProtocol udpProtocol)
        {
            this.windowsScaling = GetWindowsScaling();
            this.screenWidth = Screen.PrimaryScreen.Bounds.Width;
            this.ScreenHeight = Screen.PrimaryScreen.Bounds.Height;

            this.blockWidth = (int)Math.Ceiling((double)screenWidth / imageWidth);
            this.blockHeight = (int)Math.Ceiling((double)ScreenHeight / ImageHeight);

            this.ID = ID;
            this.udpProtocol = udpProtocol;
            this.controlIP = controlIP;
            this.controlPort = controlPort;

            sendScreenImageThread = new Thread(() =>
            {
                while (true)
                {
                    for (int i = 0; i < BlockWidth; i++)
                    {
                        for (int j = 0; j < BlockHeight; j++)
                        {
                            SendScreenImage(i, j);
                        }
                    }
                    Console.WriteLine(new Random().Next(1000));
                    //Thread.Sleep(50);
                }
            });
            sendScreenImageThread.IsBackground = true;
        }

        public void StartThread()
        {
            sendScreenImageThread.Start();
        }

        private void SendScreenImage(int x, int y)
        {
            byte[] bx = BitConverter.GetBytes(x);
            byte[] by = BitConverter.GetBytes(y);
            byte[] imageData = GetScreenAtPos(x, y);
            byte[] sendData = new byte[] { 10, (byte)ID };
            sendData = sendData.Concat(bx).Concat(by).Concat(imageData).ToArray();

            udpProtocol.UdpSocketSend(controlIP, controlPort, sendData);

            //Console.WriteLine("send" + new Random().Next(100));
        }

        private double GetWindowsScaling()
        {
            return Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;
        }

        private BitmapSource CopyScreen(int x, int y)
        {
            using (var screenBmp = new Bitmap(imageWidth, imageHeight,
                System.Drawing.Imaging.PixelFormat.Format16bppRgb555))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(imageWidth * x, imageHeight * y, 0, 0, screenBmp.Size);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        public byte[] GetScreenAtPos(int x, int y)
        {
            BitmapSource bitmapSource = CopyScreen(x, y);
            //bitmapSource.Freeze();

            return getJPGFromBitmapSource(bitmapSource);
        }

        private byte[] getJPGFromBitmapSource(BitmapSource imageC)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageC));
                imageC = null;
                encoder.Save(memStream);
                return memStream.ToArray();
            }
        }
    }
}
