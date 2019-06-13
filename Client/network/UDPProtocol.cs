using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class UDPProtocol
    {
        private System.Net.Sockets.UdpClient udpClient;
        Thread socketReceiveThread;

        public String serverIP = "14.9.118.64";
        public int serverPort = 8530;

        private int localPort;

        public bool isServerOver = false;

        private IPEndPoint serverIPEndPoint;
        public UDPProtocol(int localPort)
        {
            this.localPort = localPort;
            this.serverIPEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

            udpClient = new System.Net.Sockets.UdpClient(localPort);
            udpClient.AllowNatTraversal(true);
        }

        public void UdpSocketSend(IPEndPoint iPEndPoint, byte[] data)
        {
            if (isServerOver&& iPEndPoint.Address.ToString()!=serverIP)
            {
                data = new byte[] { 11 }.Concat(data).ToArray();
                iPEndPoint = serverIPEndPoint;
            }
            udpClient.Send(data, data.Length, iPEndPoint);
        }

        public void UdpSocketSend(String host, int port, byte[] data)
        {
            if (isServerOver && host!=serverIP)
            {
                data = new byte[] { 11 }.Concat(data).ToArray();
                host = serverIP;
                port = serverPort;
            }
            udpClient.Send(data, data.Length, host, port);
        }

        public void UdpSocketReceiveStart(Action<IPEndPoint, byte[]> action)
        {
            System.Net.IPEndPoint remoteEP = null;

            socketReceiveThread = new Thread(() =>
            {
                while (true)
                {
                    byte[] receiveBytes = udpClient.Receive(ref remoteEP);
                    Console.WriteLine("{0} {1}", remoteEP.Address, remoteEP.Port);
                    action(remoteEP, receiveBytes);
                    //Environment.Exit(0);
                }
            });

            socketReceiveThread.IsBackground = true;
            socketReceiveThread.Start();


            //udpClient.Close();
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void Close()
        {
            if (socketReceiveThread.IsAlive)
            {
                socketReceiveThread.Abort();
            }
            udpClient.Close();
        }
    }
}
