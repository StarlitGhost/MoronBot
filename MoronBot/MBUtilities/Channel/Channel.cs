using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
    }
}
