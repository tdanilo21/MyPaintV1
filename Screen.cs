using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;
using System.Drawing;

namespace MyPaint
{
    class Screen
    {
        Bitmap prev;
        LinkedList<Bitmap> undo, redo;
        Func<Bitmap, Point, Bitmap> Draw;
        Action Finish;
        IPictureBox screen;
        bool mouse_down;

        public Screen(IForm form, Func<Bitmap, Point, Bitmap> draw, Action finish, Point p, int w, int h)
        {
            undo = new LinkedList<Bitmap>();
            redo = new LinkedList<Bitmap>();
            Draw = draw; Finish = finish;
            prev = new Bitmap(w, h);
            undo.AddLast(new Bitmap(prev));
            mouse_down = false;

            screen = new MyPictureBox();
            screen.Location = p;
            screen.Size = new Size(w, h);
            screen.Image = new Bitmap(prev);
            screen.MouseDownCallback = MouseDown;
            screen.MouseMoveCallback = MouseMove;
            screen.MouseUpCallback = MouseUp;
            form.AddControl(screen);

            form.KeyDownCallback = KeyDown;
            form.ClientSizeChangedCallback = ClientSizeChanged;
        }

        private Bitmap ResizeImage(Bitmap bmp, Size new_size)
        {
            Bitmap new_bmp = new Bitmap(new_size.Width, new_size.Height);
            IGraphics<MyGraphics> g = IGraphics<MyGraphics>.FromImage(new_bmp);
            g.DrawImage(bmp, 0, 0);
            return new_bmp;
        }
        public void Undo()
        {
            if (undo.Count() == 1) return;
            redo.AddFirst(undo.Last());
            undo.RemoveLast();
            prev = new Bitmap(undo.Last());
            screen.Image = undo.Last();
        }
        private void Redo()
        {
            if (redo.Count() == 0) return;
            undo.AddLast(redo.First());
            redo.RemoveFirst();
            prev = new Bitmap(undo.Last());
            screen.Image = undo.Last();
        }
        public void MoveSelection(Rectangle rect, int dx, int dy)
        {
            Bitmap original = new Bitmap(screen.Image);
            Bitmap sel = original.Clone(rect, original.PixelFormat);
            IGraphics<MyGraphics> g = IGraphics<MyGraphics>.FromImage(original);
            g.FillRectangle(new SolidBrush(Color.White), rect);
            rect.X += dx; rect.Y += dy;
            g.DrawImage(sel, rect);
            screen.Image = original;
        }

        private void Save()
        {
            MySaveFileDialog dialog = new MySaveFileDialog();
            dialog.Filter = "Png Image|*.png";
            dialog.Title = "Save an Image File";
            dialog.Show();
            if (dialog.FileName != "")
            {
                FileStream fs = (FileStream)dialog.OpenFile();
                screen.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                fs.Close();
            }
        }
        private void Open()
        {
            MyOpenFileDialog dialog = new MyOpenFileDialog();
            dialog.Filter = "Png Image|*.png";
            dialog.Title = "Open an Image File";
            if (dialog.Show() == DialogResults.OK)
            {
                string path = dialog.FileName;
                Image img = Image.FromFile(path);
                img = ResizeImage(new Bitmap(img), screen.Image.Size);
                screen.Image = img;
                prev = new Bitmap(img);
                undo.AddLast(new Bitmap(img));
            }
        }


        private void MouseDown(object sender, IMouseEventProps e)
        {
            screen.Image = Draw(prev, e.Location);
            mouse_down = true;
        }
        private void MouseMove(object sender, IMouseEventProps e)
        {
            if (mouse_down) screen.Image = Draw(new Bitmap(prev), e.Location);
        }
        private void MouseUp(object sender, IMouseEventProps e)
        {
            prev = new Bitmap(screen.Image);
            Finish();
            undo.AddLast(new Bitmap(screen.Image));
            redo.Clear();
            if (undo.Count() > 100) undo.RemoveFirst();
            mouse_down = false;
        }
        private void KeyDown(object sender, IKeyEventProps e)
        {
            if (e.Ctrl)
            {
                if (e.KeyCode == Key.Z) Undo();
                if (e.KeyCode == Key.Y) Redo();
                if (e.KeyCode == Key.S) Save();
                if (e.KeyCode == Key.O) Open();
            }
        }

        private void ClientSizeChanged(IForm form)
        {
            Bitmap bmp = ResizeImage(new Bitmap(screen.Image), new Size(form.ClientRectangle.Width, form.ClientRectangle.Height - 100));
            screen.Image = bmp;
            screen.Size = screen.Image.Size;
        }
    }
}