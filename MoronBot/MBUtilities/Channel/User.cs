using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBUtilities.Channel
{
    public class User
    {
        public User()
        {
            Nick = "";
            Hostmask = "";
            Modes = new List<char>();
            Symbols = "";
        }

        public string Nick { get; set; }

        public string Hostmask { get; set; }

        public List<char> Modes { get; set; }

        public string Symbols { get; set; }
    }
}
