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

        const int localPort = 54756;

        static int ID = 0;

        private Thread sendIPPortThread;
        private Thread registerIDThread;
        private Thread sendScreenImageThread;

        ChatWindow chatWindow;

        ViewScreenManager viewScreenManager;

        UDPProtocol udpProtocol;
        public MainWindow()
        {
            InitializeComponent();

            viewScreenManager = new ViewScreenManager();
            //MessageBox.Show(viewScreenManager.WindowsScaling + "");

            udpProtocol = new UDPProtocol(localPort);
            udpProtocol.UdpSocketReceiveStart(RunCommand);

            chatWindow = new ChatWindow((mess) =>
            {
                SendMessage(controlIP, controlPort, mess);
            });
            //chatWindow.Show();

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
                    for (int i = 0; i < viewScreenManager.BlockWidth; i++)
                    {
                        for (int j = 0; j < viewScreenManager.BlockHeight; j++)
                        {
                            SendScreenImage(i, j);
                        }
                    }
                    Console.WriteLine(new Random().Next(1000));
                    //Thread.Sleep(50);
                }
            });

            sendIPPortThread.IsBackground = true;
            registerIDThread.IsBackground = true;
            //sendScreenImageThread.IsBackground = true;

            sendIPPortThread.Start();
            registerIDThread.Start();
            //sendScreenImageThread.Start();
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

                    Console.WriteLine(controlIP + " " + controlPort);

                    for (int i = 0; i < 5; i++)
                    {
                        udpProtocol.UdpSocketSend(controlIP, controlPort, new byte[] { 0 });
                    }

                    udpProtocol.UdpSocketSend(controlIP, controlPort, new byte[] { 7, (byte)ID });

                    //chatWindow.Dispatcher.Invoke(() =>
                    //{
                    //    chatWindow.Show();
                    //});

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
                    //String ipportViewScreen = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    //String[] splitViewScreen = ipportViewScreen.Split('|');

                    //controlIP = splitViewScreen[0];
                    //controlPort = int.Parse(splitViewScreen[1]);

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

        private void SendScreenImage(int x, int y)
        {
            byte[] bx = BitConverter.GetBytes(x);
            byte[] by = BitConverter.GetBytes(y);
            byte[] imageData = viewScreenManager.GetScreenAtPos(x, y);
            byte[] sendData = new byte[] { 10, (byte)ID };
            sendData = sendData.Concat(bx).Concat(by).Concat(imageData).ToArray();

            udpProtocol.UdpSocketSend(controlIP, controlPort, sendData);

            //Console.WriteLine("send" + new Random().Next(100));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Thread thread = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        image.Dispatcher.Invoke(() => {
            //            image.Source = CopyScreen();
            //        });

            //        Thread.Sleep(10);
            //    }
            //});

            //thread.IsBackground = true;
            //thread.Priority = ThreadPriority.Highest;
            //thread.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sendIPPortThread.IsAlive)
            {
                sendIPPortThread.Abort();
            }

            if (sendScreenImageThread.IsAlive)
            {
                sendScreenImageThread.Abort();
            }
        }
    }
}
