using System.Collections.Generic;
using System.ComponentModel;

namespace MBUtilities.Channel
{
    public class Channel
    {
        public Channel()
        {
            Name = "";
            Topic = "";
            Users = new BindingList<User>();
            Modes = new List<char>();
        }

        public string Name { get; set; }

        public string Topic { get; set; }

        public BindingList<User> Users { get; set; }

        public List<char> Modes { get; set; }

        public override string ToString()
        {
            return Name + " (" + Users.Count + ")";
        }
    }
}
