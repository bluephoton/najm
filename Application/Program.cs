using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Najm.UI;

namespace Najm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#if  !DEBUG
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
#endif
            Application.Run(new MainAppForm());
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // TODO: investigate this in details and see if we can do better handling or if we missed some possible
            //       sources of exceptions (e.g.; not from UI)
            MessageBox.Show(e.Exception.Message);
        }
    }
}
