using System;
using System.Windows.Forms;

namespace MGSDatless
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MGS4DatlessForm());
        }
    }
}