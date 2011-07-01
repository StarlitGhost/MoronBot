using System.Collections.Generic;

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

        public override string ToString()
        {
            return Symbols + Nick;
        }
    }
}
