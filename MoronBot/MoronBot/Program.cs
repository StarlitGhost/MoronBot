using System;
using System.Windows.Forms;

using MBUtilities;

namespace MoronBot
{
    static class Program
    {
        public static MoronBot moronBot;
        public static formMoronBot form;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            moronBot = new MoronBot();

            if (Settings.Instance.ShowForm)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                form = new formMoronBot();
                Application.Run(form);
            }
            else
            {
                string text = "";
                while (true)
                {
                    text = Console.ReadLine();
                    if (text == "quit")
                        break;
                }
            }
        }
    }
}
