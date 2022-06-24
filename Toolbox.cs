using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace MyPaint
{
    enum Tools
    {
        Pen = 0,
        Eraser = 1,
        StraightLine = 2,
        Rectangle = 3,
        Ellipse = 4,
        Triangle = 5,
        VerticalArrow = 6,
        HorizontalArrow = 7,
        Star = 8,
        Heart = 9,
        Selection = 10
    }
    class Toolbox
    {
        Event state;
        Point p;
        IButton[] shape_buttons, width_buttons;
        Action undo_callback;
        Action<Rectangle, int, int> move_callback;

        public Toolbox(IForm form, Point p)
        {
            state = new Draw();
            this.p = p;
            form.PaintCallback = Paint;

            string exeFile = (new Uri(Assembly.GetEntryAssembly().CodeBase)).AbsolutePath;
            string exeDir = Path.GetDirectoryName(exeFile);
            string fullDirPath = exeDir.Substring(0, exeDir.Length - 20) + "\\Images";
            int tab_index = 0;

            //
            // selection_button
            //
            IButton selection_button = new MyButton();
            selection_button.Location = new Point(p.X + 20, p.Y + 15);
            selection_button.Size = new Size(50, 50);
            selection_button.TabIndex = tab_index++;
            selection_button.ClickCallback = SelectionButtonClicked;
            form.AddControl(selection_button);
            //
            // selection_label
            //
            ILabel selection_label = new MyLabel();
            selection_label.Location = new Point(p.X + 17, p.Y + 65);
            selection_label.Size = new Size(55, 20);
            selection_label.BackColor = Color.White;
            selection_label.Text = "Selection";
            selection_label.TextAlign = ContentAlignment.MiddleCenter;
            form.AddControl(selection_label);
            //
            // shape_buttons
            //
            string[] shape_img_paths = { "\\StraightLine.png", "\\Rectangle.png", "\\Ellipse.png", "\\Triangle.png", "\\VerticalArrow.png", "\\HorizontalArrow.png", "\\Star.png", "\\Heart.png" };
            shape_buttons = new IButton[shape_img_paths.Length];
            for (int i = 0; i < shape_buttons.Length; i++)
            {
                shape_buttons[i] = new MyButton();
                shape_buttons[i].Location = new Point(p.X + 85 + i * 60, p.Y + 25);
                shape_buttons[i].Size = new Size(50, 50);
                shape_buttons[i].Image = Image.FromFile(fullDirPath + shape_img_paths[i]);
                shape_buttons[i].TabIndex = tab_index++;
                shape_buttons[i].Index = IControl.GetIndex();
                shape_buttons[i].ClickCallback = ShapeButtonClicked;
                form.AddControl(shape_buttons[i]);
            }
            //
            // pen_button
            //
            IButton pen_button = new MyButton();
            pen_button.Location = new Point(p.X + 575, p.Y + 10);
            pen_button.Size = new Size(30, 30);
            pen_button.Image = Image.FromFile(fullDirPath + "\\Pen.png");
            pen_button.TabIndex = tab_index++;
            pen_button.ClickCallback = PenButtonClicked;
            form.AddControl(pen_button);
            //
            // eraser_button
            //
            IButton eraser_button = new MyButton();
            eraser_button.Location = new Point(p.X + 575, p.Y + 60);
            eraser_button.Size = new Size(30, 30);
            eraser_button.Image = Image.FromFile(fullDirPath + "\\Eraser.png");
            eraser_button.TabIndex = tab_index++;
            eraser_button.ClickCallback = EraserButtonClicked;
            form.AddControl(eraser_button);
            //
            // width_buttons
            //
            string[] width_img_paths = { "\\Width1.png", "\\Width2.png", "\\Width2.png", "\\Width3.png" };
            width_buttons = new IButton[width_img_paths.Length];
            for (int i = 0; i < width_img_paths.Length; i++)
            {
                width_buttons[i] = new MyButton();
                width_buttons[i].Location = new Point(p.X + 615, p.Y + 10 + i * 22);
                width_buttons[i].Size = new Size(70, 15);
                width_buttons[i].Image = Image.FromFile(fullDirPath + width_img_paths[i]);
                width_buttons[i].TabIndex = tab_index++;
                width_buttons[i].Index = IControl.GetIndex();
                width_buttons[i].ClickCallback = WidthButtonClicked;
                form.AddControl(width_buttons[i]);
            }
            //
            // color_buttons
            //
            Color[] cols = { Color.Black, Color.Gray, Color.Brown, Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.LightBlue, Color.Blue, Color.Purple };
            IButton[] color_buttons = new IButton[cols.Length];
            for (int i = 0; i < cols.Length; i++)
            {
                color_buttons[i] = new MyButton();
                color_buttons[i].Location = new Point(p.X + 755 + i * 40, p.Y + 35);
                color_buttons[i].Size = new Size(30, 30);
                color_buttons[i].BackColor = cols[i];
                color_buttons[i].TabIndex = tab_index++;
                color_buttons[i].ClickCallback = ColorButtonClicked;
                form.AddControl(color_buttons[i]);
            }
            //
            // more_colors_button
            //
            IButton more_colors_button = new MyButton();
            more_colors_button.Location = new Point(p.X + 1165, p.Y + 15);
            more_colors_button.Size = new Size(50, 40);
            more_colors_button.Image = Image.FromFile(fullDirPath + "\\ColorDialog.png");
            more_colors_button.TabIndex = tab_index++;
            more_colors_button.Index = IControl.GetIndex();
            more_colors_button.ClickCallback = MoreColorsButtonClicked;
            form.AddControl(more_colors_button);
            //
            // checked_color_label
            //
            ILabel checked_color_label = new MyLabel();
            checked_color_label.Location = new Point(p.X + 700, p.Y + 65);
            checked_color_label.Size = new Size(40, 15);
            checked_color_label.BackColor = Color.White;
            checked_color_label.Text = "Color";
            checked_color_label.TextAlign = ContentAlignment.MiddleCenter;
            form.AddControl(checked_color_label);
            //
            // more_color_label
            //
            ILabel more_colors_label = new MyLabel();
            more_colors_label.Location = new Point(p.X + 1165, p.Y + 55);
            more_colors_label.Size = new Size(50, 30);
            more_colors_label.BackColor = Color.White;
            more_colors_label.Text = "More Colors";
            more_colors_label.TextAlign = ContentAlignment.MiddleCenter;
            form.AddControl(more_colors_label);
        }

        private void ChangeTool(Tools tool)
        {
            switch (tool)
            {
                case Tools.Pen: state = new Draw(state); break;
                case Tools.Eraser: state = new Erase(state); break;
                case Tools.StraightLine: state = new StraightLine(state); break;
                case Tools.Rectangle: state = new MyRectangle(state); break;
                case Tools.Ellipse: state = new MyEllipse(state); break;
                case Tools.Triangle: state = new MyTriangle(state); break;
                case Tools.VerticalArrow: state = new VerticalArrow(state); break;
                case Tools.HorizontalArrow: state = new HorizontalArrow(state); break;
                case Tools.Star: state = new Star(state); break;
                case Tools.Heart: state = new Heart(state); break;
                case Tools.Selection: state = new Selection(state, undo_callback, move_callback); break;
            }
        }
        private Bitmap Draw(Bitmap image, Point p)
        {
            state.Update(IGraphics<MyGraphics>.FromImage(image), p);
            return image;
        }
        private void Finish() { state.Reset(); }

        public int Height { get { return 100; } }
        public Func<Bitmap, Point, Bitmap> GetDraw() { return Draw; }
        public Action GetFinish() { return Finish; }

        #region EventHandlers

        private void Paint(IForm form, IPaintEventProps e)
        {
            SolidBrush brush = new SolidBrush(Color.LightBlue);
            e.Graphics.FillRectangle(brush, p.X, p.Y, form.ClientRectangle.Width, Height);
            brush.Color = Color.White;
            Pen pen = new Pen(Color.Black);
            e.Graphics.FillRectangle(brush, p.X + 15, p.Y + 10, 60, 80);
            e.Graphics.DrawRectangle(pen, p.X + 15, p.Y + 10, 60, 80);
            e.Graphics.FillRectangle(brush, p.X + 695, p.Y + 10, 50, 80);
            e.Graphics.DrawRectangle(pen, p.X + 695, p.Y + 10, 50, 80);
            e.Graphics.FillRectangle(brush, p.X + 1160, p.Y + 10, 60, 80);
            e.Graphics.DrawRectangle(pen, p.X + 1160, p.Y + 10, 60, 80);
            brush.Color = state.Color;
            e.Graphics.FillRectangle(brush, p.X + 700, p.Y + 15, 40, 40);
            e.Graphics.DrawRectangle(pen, p.X + 700, p.Y + 15, 40, 40);
        }
        private void ShapeButtonClicked(IButton sender)
        {
            if (sender.Index == shape_buttons[0].Index) ChangeTool(Tools.StraightLine);
            if (sender.Index == shape_buttons[1].Index) ChangeTool(Tools.Rectangle);
            if (sender.Index == shape_buttons[2].Index) ChangeTool(Tools.Ellipse);
            if (sender.Index == shape_buttons[3].Index) ChangeTool(Tools.Triangle);
            if (sender.Index == shape_buttons[4].Index) ChangeTool(Tools.VerticalArrow);
            if (sender.Index == shape_buttons[5].Index) ChangeTool(Tools.HorizontalArrow);
            if (sender.Index == shape_buttons[6].Index) ChangeTool(Tools.Star);
            if (sender.Index == shape_buttons[7].Index) ChangeTool(Tools.Heart);
        }
        private void WidthButtonClicked(IButton sender)
        {
            if (sender.Index == width_buttons[0].Index) state.PenWidth = 1;
            if (sender.Index == width_buttons[1].Index) state.PenWidth = 2;
            if (sender.Index == width_buttons[2].Index) state.PenWidth = 4;
            if (sender.Index == width_buttons[3].Index) state.PenWidth = 8;
        }
        private void SelectionButtonClicked(IButton sender) { ChangeTool(Tools.Selection); }
        private void PenButtonClicked(IButton sender) { ChangeTool(Tools.Pen); }
        private void EraserButtonClicked(IButton sender) { ChangeTool(Tools.Eraser); }
        private void ColorButtonClicked(IButton sender) { state.Color = sender.BackColor; sender.GetForm().Refresh(); }
        private void MoreColorsButtonClicked(IButton sender)
        {
            IColorDialog colorDialog = new MyColorDialog();
            colorDialog.Color = state.Color;
            if (colorDialog.Show() == DialogResults.OK) state.Color = colorDialog.Color;
            sender.GetForm().Refresh();
        }
        #endregion

        public void RegisterUndoCallback(Action undo_callback) { this.undo_callback = undo_callback; }
        public void RegisterMoveCallback(Action<Rectangle, int, int> move_callback) { this.move_callback = move_callback; }
    }
}