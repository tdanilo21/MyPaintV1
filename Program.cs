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

        static MyForm form;
        static Toolbox toolbox;
        static Screen screen;

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Initialize();
            Application.Run(form);
        }
        static void Initialize()
        {
            form = new MyForm();
            form.KeyPreview = true;
            form.WindowState = FormWindowState.Maximized;
            //form.ClientSize = new Size(1920, 1017);
            toolbox = new Toolbox(form, new Point(0, 0));
            screen = new Screen(form, toolbox.GetDraw(), toolbox.GetFinish(), new Point(0, toolbox.Height), form.ClientRectangle.Width, form.ClientRectangle.Height - toolbox.Height);
            toolbox.RegisterUndoCallback(screen.Undo);
            toolbox.RegisterMoveCallback(screen.MoveSelection);
        }
        static Size ScreenSize() { return new Size(form.ClientRectangle.Width, form.ClientRectangle.Height - toolbox.Height); }
    }
}