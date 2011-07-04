using System.Windows.Forms;

namespace MoronBot
{
    class FormStarter
    {
        public static void Start()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new formMoronBot());
        }
    }
}
