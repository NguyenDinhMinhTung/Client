using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ChatWindowManager
    {
        private ChatWindow chatWindow;

        private String ControlIP;
        private int ControlPort;
        private int ID;

        private UDPProtocol udpProtocol;

        public ChatWindowManager(int ID, String ControlIP, int ControlPort, UDPProtocol udpProtocol)
        {
            this.udpProtocol = udpProtocol;
            this.ID = ID;
            chatWindow = new ChatWindow(mess =>
            {
                SendMessage(mess);
            });
            SetControlIPPort(ControlIP, ControlPort);
        }

        private void SendMessage(String message)
        {
            byte[] messagebyte = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] sendbyte = new byte[messagebyte.Length + 3];
            sendbyte[0] = 8;
            sendbyte[1] = (byte)ID;
            sendbyte[2] = 1;
            sendbyte = sendbyte.Concat(messagebyte).ToArray();
            udpProtocol.UdpSocketSend(ControlIP, ControlPort, sendbyte);
        }

        public void PushMessage(String mess, Boolean isMe)
        {
            chatWindow.Dispatcher.Invoke(() =>
            {
                chatWindow.Show();
                chatWindow.PushMessage(mess, false);
            });
        }

        public void SetControlIPPort(String ControlIP, int ControlPort)
        {
            this.ControlIP = ControlIP;
            this.ControlPort = ControlPort;
        }
    }
}
