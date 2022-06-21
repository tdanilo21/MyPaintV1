using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace MyPaint
{
    enum Tools { Pen, Eraser, Rectangle, Ellipse, Triangle, Arrow, Star }
    class Toolbox
    {
        Event state;
        Point p;
        int a;
        IButton[] shape_buttons, width_buttons;

        public Toolbox(IForm form, Point p, int a)
        {
            state = new Draw();
            this.p = p; this.a = a;
            form.PaintCallback = Paint;

            string exeFile = (new Uri(Assembly.GetEntryAssembly().CodeBase)).AbsolutePath;
            string exeDir = Path.GetDirectoryName(exeFile);
            string fullDirPath = exeDir.Substring(0, exeDir.Length - 20) + "\\Images";

            //
            // shape_buttons
            //
            string[] shape_img_paths = { "\\Rectangle.png", "\\Ellipse.png", "\\Triangle.png", "\\Arrow.png", "\\Star.png" };
            shape_buttons = new IButton[shape_img_paths.Length];
            for (int i = 0; i < shape_buttons.Length; i++)
            {
                shape_buttons[i] = new MyButton();
                shape_buttons[i].Location = new Point();
                shape_buttons[i].Size = new Size();
                shape_buttons[i].Image = Image.FromFile(fullDirPath + shape_img_paths[i]);
                shape_buttons[i].TabIndex = i;
                shape_buttons[i].Index = IControl.GetIndex();
                shape_buttons[i].ClickCallback = ShapeButtonClicked;
                form.AddControl(shape_buttons[i]);
            }
            //
            // width_buttons
            //
            string[] width_img_paths = { "\\Width1.png", "\\Width2.png", "\\Width2.png", "\\Width3.png" };
            width_buttons = new IButton[width_img_paths.Length];
            for (int i = 0; i < width_img_paths.Length; i++)
            {
                width_buttons[i] = new MyButton();
                width_buttons[i].Location = new Point();
                width_buttons[i].Size = new Size();
                width_buttons[i].Image = Image.FromFile(fullDirPath + width_img_paths[i]);
                width_buttons[i].TabIndex = shape_buttons.Length + i;
                width_buttons[i].Index = IControl.GetIndex();
                width_buttons[i].ClickCallback = WidthButtonClicked;
                form.AddControl(width_buttons[i]);
            }
            //
            // pen_button
            //
            IButton pen_button = new MyButton();
            pen_button.Location = new Point();
            pen_button.Size = new Size();
            pen_button.Image = Image.FromFile(fullDirPath + "\\Pen.png");
            pen_button.TabIndex = shape_buttons.Length + width_buttons.Length;
            pen_button.Index = IControl.GetIndex();
            pen_button.ClickCallback = PenButtonClicked;
            form.AddControl(pen_button);
            //
            // eraser_button
            //
            IButton eraser_button = new MyButton();
            eraser_button.Location = new Point();
            eraser_button.Size = new Size();
            eraser_button.Image = Image.FromFile(fullDirPath + "\\Eraser.png");
            eraser_button.TabIndex = pen_button.TabIndex + 1;
            eraser_button.Index = IControl.GetIndex();
            eraser_button.ClickCallback = EraserButtonClicked;
            form.AddControl(eraser_button);
            //
            // color_buttons
            //
            Color[] cols = { Color.Black, Color.Gray, Color.Brown, Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.LightBlue, Color.Blue, Color.Purple };
            IButton[] color_buttons = new IButton[cols.Length];
            for (int i = 0; i < cols.Length; i++)
            {
                color_buttons[i] = new MyButton();
                color_buttons[i].Location = new Point();
                color_buttons[i].Size = new Size();
                color_buttons[i].BackColor = cols[i];
                color_buttons[i].TabIndex = eraser_button.TabIndex + 1 + i;
                color_buttons[i].Index = IControl.GetIndex();
                color_buttons[i].ClickCallback = ColorButtonClicked;
                form.AddControl(color_buttons[i]);
            }
            //
            // more_colors_button
            //
            IButton more_colors_button = new MyButton();
            more_colors_button.Location = new Point();
            more_colors_button.Size = new Size();
            more_colors_button.Image = Image.FromFile(fullDirPath + "\\ColorDialog.png");
            more_colors_button.TabIndex = eraser_button.TabIndex + color_buttons.Length + 1;
            more_colors_button.Index = IControl.GetIndex();
            more_colors_button.ClickCallback = MoreColorsButtonClicked;
            form.AddControl(more_colors_button);
            //
            // checked_color_label
            //
            ILabel checked_color_label = new MyLabel();
            checked_color_label.Location = new Point();
            checked_color_label.Size = new Size();
            checked_color_label.Text = "Color";
            form.AddControl(more_colors_button);
            //
            // more_color_label
            //
            ILabel more_colors_label = new MyLabel();
            more_colors_label.Location = new Point();
            more_colors_label.Size = new Size();
            more_colors_label.Text = "More Colors";
            form.AddControl(more_colors_label);
        }

        private void ChangeTool(Tools tool)
        {
            switch (tool)
            {
                case Tools.Pen: state = new Draw(state); break;
                case Tools.Eraser: state = new Erase(state); break;
                case Tools.Rectangle: state = new MyRectangle(state); break;
                case Tools.Ellipse: state = new MyEllipse(state); break;
                case Tools.Triangle: state = new MyTriangle(state); break;
                case Tools.Arrow: state = new Arrow(state); break;
                case Tools.Star: state = new Star(state); break;
            }
        }
        private Bitmap Draw(Bitmap image, Point p)
        {
            state.Update(IGraphics<MyGraphics>.FromImage(image), p);
            return image;
        }
        private void Finish() { state.Reset(); }

        public int Height { get { return 3*a; } }
        public Func<Bitmap, Point, Bitmap> GetDraw() { return Draw; }
        public Action GetFinish() { return Finish; }

        #region EventHandlers

        private void Paint(IForm form, IPaintEventProps e)
        {

        }
        private void ShapeButtonClicked(IButton sender)
        {
            if (sender.Index == shape_buttons[0].Index) ChangeTool(Tools.Rectangle);
            if (sender.Index == shape_buttons[1].Index) ChangeTool(Tools.Ellipse);
            if (sender.Index == shape_buttons[2].Index) ChangeTool(Tools.Triangle);
            if (sender.Index == shape_buttons[3].Index) ChangeTool(Tools.Arrow);
            if (sender.Index == shape_buttons[4].Index) ChangeTool(Tools.Star);
        }
        private void WidthButtonClicked(IButton sender)
        {
            if (sender.Index == width_buttons[0].Index) state.ChangeWidth(1);
            if (sender.Index == width_buttons[1].Index) state.ChangeWidth(2);
            if (sender.Index == width_buttons[2].Index) state.ChangeWidth(4);
            if (sender.Index == width_buttons[3].Index) state.ChangeWidth(8);
        }
        private void PenButtonClicked(IButton sender) { ChangeTool(Tools.Pen); }
        private void EraserButtonClicked(IButton sender) { ChangeTool(Tools.Eraser); }
        private void ColorButtonClicked(IButton sender) { state.ChangeColor(sender.BackColor); }
        private void MoreColorsButtonClicked(IButton sender)
        {

        }
        #endregion
    }
}
