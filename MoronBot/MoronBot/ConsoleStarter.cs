using System;

namespace MoronBot
{
    class ConsoleStarter
    {
        static MoronBot moronBot;

        static ConsoleStarter()
        {
            moronBot = Program.moronBot;

            moronBot.NickChanged += moronBot_NickChanged;
            moronBot.NewFormattedIRC += moronBot_NewFormattedIRC;
        }

        public static void Start()
        {
            string text = "";
            while (true)
            {
                text = Console.ReadLine();
                if (text == "quit")
                    break;
                string[] msg = text.Split(' ');
                string target = msg[0].StartsWith("#") ? msg[0] : "#" + msg[0];
                string message = text.Substring(target.Length);
                moronBot.Say(message, target);
            }
        }

        static void moronBot_NickChanged(object sender, string nick)
        {
            Console.Title = nick;
        }

        static void moronBot_NewFormattedIRC(object sender, string text)
        {
            Console.WriteLine(text);
        }
    }
}
