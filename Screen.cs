using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            screen.MouseDownCallback = MouseDown;
            screen.MouseMoveCallback = MouseMove;
            screen.MouseUpCallback = MouseUp;
            screen.KeyDownCallback = KeyDown;
            form.AddControl(screen);
        }

        private void Undo()
        {
            redo.AddFirst(undo.Last());
            undo.RemoveLast();
            prev = new Bitmap(undo.Last());
            screen.Image = undo.Last();
        }
        private void Redo()
        {
            undo.AddLast(redo.First());
            redo.RemoveFirst();
            prev = new Bitmap(undo.Last());
            screen.Image = undo.Last();
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
            if (undo.Count() > 20) undo.RemoveFirst();
            mouse_down = false;
        }
        private void KeyDown(object sender, IKeyEventProps e)
        {
            if (e.Ctrl)
            {
                if (e.KeyCode == Key.Z) Undo();
                if (e.KeyCode == Key.Y) Redo();
            }
        }
    }
}