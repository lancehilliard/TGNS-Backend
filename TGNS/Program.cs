using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace TGNS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // http://stackoverflow.com/a/184143/116895
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "TGNS Recording Helper - 71fee96c-8dcd-4103-be3e-46afc641ed4f", out createdNew))
            {
                if (createdNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
                else
                {
                    MessageBox.Show("Another instance of TGNS Recording Helper is already running.\n\nIt might be hiding in your system tray.\n\nThis second instance will exit now.");
                }
            }
        }
    }
}
