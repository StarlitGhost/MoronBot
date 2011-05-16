using System;
using System.Windows.Forms;

namespace MoronBot
{
    static class Program
    {
        public static formMoronBot form;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new formMoronBot();
            Application.Run(form);
        }
    }
}
