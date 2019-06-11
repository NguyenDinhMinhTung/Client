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
                SendMessage(ControlIP, ControlPort, mess);
            });
            SetControlIPPort(ControlIP, ControlPort);

            chatWindow.Show();
        }

        private void SendMessage(String controlIP, int controlPort, String message)
        {
            byte[] messagebyte = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] sendbyte = null;
            if (udpProtocol.isServerOver)
            {
                sendbyte = new byte[messagebyte.Length + 4];

                sendbyte[0] = 11;
                sendbyte[1] = 8;
                sendbyte[2] = (byte)ID;
                sendbyte[3] = 1;

                Array.Copy(messagebyte, 0, sendbyte, 4, messagebyte.Length);

                udpProtocol.UdpSocketSend(udpProtocol.serverIP, udpProtocol.serverPort, sendbyte);
            }
            else
            {
                sendbyte = new byte[messagebyte.Length + 3];

                sendbyte[0] = 8;
                sendbyte[1] = (byte)ID;
                sendbyte[2] = 1;

                Array.Copy(messagebyte, 0, sendbyte, 3, messagebyte.Length);

                udpProtocol.UdpSocketSend(controlIP, controlPort, sendbyte);

            }
            
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
