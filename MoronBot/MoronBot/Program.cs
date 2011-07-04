using System;

using MBUtilities;

namespace MoronBot
{
    static class Program
    {
        public static MoronBot moronBot;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            moronBot = new MoronBot();

            if (Settings.Instance.ShowForm)
            {
                FormStarter.Start();
            }
            else
            {
                ConsoleStarter.Start();
            }
        }
    }
}
