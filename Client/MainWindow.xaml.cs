using Client.model;
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
        private int controlPort = 0;

        const int localPort = 54756;

        static int ID = 0;

        private Thread sendIPPortThread;

        private ViewScreenManager viewScreenManager;
        private ChatWindowManager chatWindowManager;

        private UDPProtocol udpProtocol;

        public MainWindow()
        {
            InitializeComponent();

            udpProtocol = new UDPProtocol(localPort);
            udpProtocol.UdpSocketReceiveStart(RunCommand);

            ID = Properties.Settings.Default.ID;
            while (ID == 0)
            {
                udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 1 });
                Thread.Sleep(5000);
            }

            sendIPPortThread = new Thread(() =>
            {
                while (true)
                {
                    if (ID == 0) continue;
                    udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 3, (byte)ID });
                    Thread.Sleep(10000);
                }
            });

            viewScreenManager = new ViewScreenManager(ID, controlIP, controlPort, udpProtocol);
            chatWindowManager = new ChatWindowManager(ID, controlIP, controlPort, udpProtocol);

            sendIPPortThread.IsBackground = true;
            sendIPPortThread.Start();
        }

        void GetControlIPPort()
        {
            do
            {
                udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 6, 1 });
                Thread.Sleep(5000);
            } while (controlPort == 0);
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

                case 6:
                    String str = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    IPPort ipPort = IPPort.getIPPort(str);

                    if (command[1] == 1)
                    {
                        controlIP = ipPort.IP;
                        controlPort = ipPort.Port;
                    }
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

                    //udpProtocol.UdpSocketSend(controlIP, controlPort, new byte[] { 7, (byte)ID });

                    break;

                case 8:
                    String mess = System.Text.Encoding.UTF8.GetString(command, 3, command.Length - 3);

                    chatWindowManager.PushMessage(mess, false);

                    break;

                case 10:
                    viewScreenManager.StartThread();

                    break;

                case 11:

                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sendIPPortThread.IsAlive)
            {
                sendIPPortThread.Abort();
            }
        }
    }
}
