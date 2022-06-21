using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyPaint
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MyForm form = new MyForm();
            Toolbox toolbox = new Toolbox(form, new Point(0, 0));
            Screen screen = new Screen(form, toolbox.GetDraw(), toolbox.GetFinish(), new Point(0, toolbox.Height), form.ClientRectangle.Width, form.ClientRectangle.Height - toolbox.Height);
            Application.Run(form);
        }
    }
}