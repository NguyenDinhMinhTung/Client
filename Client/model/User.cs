using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.model
{
    class User
    {
        private int id;
        private String name;

        public User(int id, String name)
        {
            this.ID = id;
            this.Name = name;
        }

        public static ObservableCollection<User> GetListUser(String strList)
        {
            String[] split = strList.Split('|');

            ObservableCollection<User> result = new ObservableCollection<User>();

            int i = 0;
            while (i < split.Length)
            {
                int id = int.Parse(split[i++]);
                String name = split[i++];

                User user = new User(id, name);
                result.Add(user);
            }

            return result;
        }

        public static User getUser(String str)
        {
            return null;
        }

        public int ID { get { return id; } set { id = value; } }
        public String Name { get { return name; } set { name = value; } }
    }
}
