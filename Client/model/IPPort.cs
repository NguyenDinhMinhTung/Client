using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.model
{
    class IPPort
    {
        private int id;
        private int userid;
        private String ip;
        private int port;
        private DateTime? dateTime;

        public IPPort(int id, int userid, String ip, int port, DateTime? dateTime = null)
        {
            this.ID = id;
            this.UserId = userid;
            this.IP = ip;
            this.Port = port;
            this.DateTime = dateTime;
        }

        public static IPPort getIPPort(String str)
        {
            String[] split = str.Split('|');

            int id = int.Parse(split[0]);
            int userid = int.Parse(split[1]);
            String ip = split[2];
            int port = int.Parse(split[3]);
            DateTime dateTime = System.DateTime.Parse(split[4]);

            return new IPPort(id, userid, ip, port, dateTime);
        }

        public static ObservableCollection<IPPort> GetListIPPort(String strList)
        {
            String[] split = strList.Split('|');
            ObservableCollection<IPPort> result = new ObservableCollection<IPPort>();

            int i = 0;
            while (i < split.Length)
            {
                int id = int.Parse(split[i++]);
                int userid = int.Parse(split[i++]);
                String ip = split[i++];
                int port = int.Parse(split[i++]);
                DateTime dateTime = System.DateTime.Parse(split[i++]);

                IPPort ipPort = new IPPort(id, userid, ip, port, dateTime);
                result.Add(ipPort);
            }

            return result;
        }

        public DateTime? DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public String IP
        {
            get { return ip; }
            set { ip = value; }
        }

        public int UserId
        {
            get { return userid; }
            set { userid = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
