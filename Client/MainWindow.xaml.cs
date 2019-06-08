using System;
using System.Collections.Generic;
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

        const int localPort = 5656;

        static int ID = 0;

        static Thread sendIPPortThread;
        static Thread registerIDThread;

        ChatWindow chatWindow;

        UDPProtocol udpProtocol;
        public MainWindow()
        {
            InitializeComponent();

            //udpProtocol = new UDPProtocol(localPort);
            //udpProtocol.UdpSocketReceiveStart(RunCommand);

            chatWindow = new ChatWindow((mess)=> { });
            chatWindow.Show();

            //ID = Properties.Settings.Default.ID;

            //sendIPPortThread = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        if (ID == 0) continue;
            //        udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 3, (byte)ID });
            //        Thread.Sleep(10000);
            //    }
            //});

            //registerIDThread = new Thread(() =>
            //{
            //    while (ID == 0)
            //    {
            //        udpProtocol.UdpSocketSend(serverIP, serverPort, new byte[] { 1 });
            //        Thread.Sleep(5000);
            //    }
            //});

            //sendIPPortThread.IsBackground = true;
            //registerIDThread.IsBackground = true;

            //sendIPPortThread.Start();
            //registerIDThread.Start();
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

                    String ip = split[0];
                    int port = int.Parse(split[1]);

                    udpProtocol.UdpSocketSend(ip, port, new byte[] { 7, (byte)ID });
                    break;

                case 8:
                    String mess = System.Text.Encoding.UTF8.GetString(command, 1, command.Length - 1);
                    chatWindow.PushMessage(mess, false);
                    break;
            }
        }
    }
}
