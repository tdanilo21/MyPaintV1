using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MyPaint
{
    interface IControl
    {
        public abstract Control ToControl();
    }
    class MyButton : Button, IButton
    {
        Action<MyButton> click_callback;
        private void ClickEventHandler(object sender, EventArgs e) { click_callback(this); }
        public Control ToControl() { return this; }
        public Action<IButton> ClickCallback { set { click_callback = value; Click += new EventHandler(ClickEventHandler); } }
    }
    class MyLabel : Label, ILabel
    {
        public Control ToControl() { return this; }
    }
    class MyPictureBox : PictureBox, IPictureBox, IClickable
    {
        Action<MyPictureBox> click_callback;
        Action<MyPictureBox, IMouseEventProps> mouseClick_callback, mouseDoubleClick_callback, mouseMove_callback, mouseDown_callback, mouseUp_callback;
        Action<MyPictureBox, KeyEventProps> keyDown_callback, keyUp_callback;

        private void ClickEventHandler(object sender, EventArgs e) { click_callback(this); }
        private void MouseClickEventHandler(object sender, MouseEventArgs e) { mouseClick_callback(this, new MouseEventProps(e)); }
        private void MouseDoubleClickEventHandler(object sender, MouseEventArgs e) { mouseDoubleClick_callback(this, new MouseEventProps(e)); }
        private void MouseMoveEventHandler(object sender, MouseEventArgs e) { mouseMove_callback(this, new MouseEventProps(e)); }
        private void MouseDownEventHandler(object sender, MouseEventArgs e) { mouseDown_callback(this, new MouseEventProps(e)); }
        private void MouseUpEventHandler(object sender, MouseEventArgs e) { mouseUp_callback(this, new MouseEventProps(e)); }
        private void KeyDownEventHandler(object sender, KeyEventArgs e) { keyDown_callback(this, new KeyEventProps(e)); }
        private void KeyUpEventHandler(object sender, KeyEventArgs e) { keyUp_callback(this, new KeyEventProps(e)); }

        public Control ToControl() { return this; }

        public Action<object> ClickCallback { set { click_callback = value; Click += new EventHandler(ClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseClickCallback { set { mouseClick_callback = value; MouseClick += new MouseEventHandler(MouseClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseDoubleClickCallback { set { mouseDoubleClick_callback = value; MouseDoubleClick += new MouseEventHandler(MouseDoubleClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseMoveCallback { set { mouseMove_callback = value; MouseMove += new MouseEventHandler(MouseMoveEventHandler); } }
        public Action<object, IMouseEventProps> MouseDownCallback { set { mouseDown_callback = value; MouseDown += new MouseEventHandler(MouseDownEventHandler); } }
        public Action<object, IMouseEventProps> MouseUpCallback { set { mouseUp_callback = value; MouseUp += new MouseEventHandler(MouseUpEventHandler); } }
        public Action<object, IKeyEventProps> KeyDownCallback { set { keyDown_callback = value; KeyDown += new KeyEventHandler(KeyDownEventHandler); } }
        public Action<object, IKeyEventProps> KeyUpCallback { set { keyUp_callback = value; KeyUp += new KeyEventHandler(KeyUpEventHandler); } }
    }

    class MyGraphics : IGraphics<MyGraphics>
    {
        Graphics g;
        public MyGraphics() { }
        public MyGraphics(Graphics g) { this.g = g; }
        MyGraphics IGraphics<MyGraphics>.GetFromImage(Bitmap img) { return new MyGraphics(Graphics.FromImage(img)); }
        void IGraphics<MyGraphics>.DrawLine(Pen pen, Point a, Point b) { g.DrawLine(pen, a, b); }
        void IGraphics<MyGraphics>.DrawLine(Pen pen, int x1, int y1, int x2, int y2) { g.DrawLine(pen, x1, y1, x2, y2); }
        void IGraphics<MyGraphics>.DrawRectangle(Pen pen, Point a, int w, int h) { g.DrawRectangle(pen, a.X, a.Y, w, h); }
        void IGraphics<MyGraphics>.DrawRectangle(Pen pen, int x, int y, int w, int h) { g.DrawRectangle(pen, x, y, w, h); }
        void IGraphics<MyGraphics>.DrawEllipse(Pen pen, Point a, int w, int h) { g.DrawEllipse(pen, a.X, a.Y, w, h); }
        void IGraphics<MyGraphics>.DrawEllipse(Pen pen, int x, int y, int w, int h) { g.DrawEllipse(pen, x, y, w, h); }
        void IGraphics<MyGraphics>.DrawPolygon(Pen pen, Point[] points) { g.DrawPolygon(pen, points); }
    }

    class MouseEventProps : IMouseEventProps
    {
        MouseButton button;
        Point location;
        int delta;
        public MouseEventProps(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None) button = MouseButton.None;
            if (e.Button == MouseButtons.Left) button = MouseButton.Left;
            if (e.Button == MouseButtons.Middle) button = MouseButton.Middle;
            if (e.Button == MouseButtons.Right) button = MouseButton.Right;
            location = e.Location;
            delta = e.Delta;
        }

        public MouseButton Button { get { return button; } }
        public Point Location { get { return location; } }
        public int Delta { get { return delta; } }
    }

    class KeyEventProps : IKeyEventProps
    {
        Key keyCode;
        bool alt, ctrl, shift;
        public KeyEventProps(KeyEventArgs e)
        {
            keyCode = (Key)((int)e.KeyCode);
            alt = e.Alt;
            ctrl = e.Control;
            shift = e.Shift;
        }

        public Key KeyCode { get { return keyCode; } }
        public bool Alt { get { return alt; } }
        public bool Ctrl { get { return ctrl; } }
        public bool Shift { get { return shift; } }
    }

    class PaintEventProps : IPaintEventProps
    {
        MyGraphics graphics;
        public PaintEventProps(PaintEventArgs e) { graphics = new MyGraphics(e.Graphics); }
        public IGraphics<MyGraphics> Graphics { get { return graphics; } }
    }
    class MyForm : Form1, IForm
    {
        Action<MyForm> load_callback, click_callback;
        Action<MyForm, PaintEventProps> paint_callback;
        Action<MyForm, MouseEventProps> mouseClick_callback, mouseDoubleClick_callback, mouseMove_callback, mouseDown_callback, mouseUp_callback;
        Action<MyForm, KeyEventProps> keyDown_callback, keyUp_callback;

        private void LoadEventHandler(object sender, EventArgs e) { load_callback(this); }
        private void PaintEventHandler(object sender, PaintEventArgs e) { paint_callback(this, new PaintEventProps(e)); }
        private void ClickEventHandler(object sender, EventArgs e) { click_callback(this); }
        private void MouseClickEventHandler(object sender, MouseEventArgs e) { mouseClick_callback(this, new MouseEventProps(e)); }
        private void MouseDoubleClickEventHandler(object sender, MouseEventArgs e) { mouseDoubleClick_callback(this, new MouseEventProps(e)); }
        private void MouseMoveEventHandler(object sender, MouseEventArgs e) { mouseMove_callback(this, new MouseEventProps(e)); }
        private void MouseDownEventHandler(object sender, MouseEventArgs e) { mouseDown_callback(this, new MouseEventProps(e)); }
        private void MouseUpEventHandler(object sender, MouseEventArgs e) { mouseUp_callback(this, new MouseEventProps(e)); }
        private void KeyDownEventHandler(object sender, KeyEventArgs e) { keyDown_callback(this, new KeyEventProps(e)); }
        private void KeyUpEventHandler(object sender, KeyEventArgs e) { keyUp_callback(this, new KeyEventProps(e)); }

        public IGraphics<MyGraphics> GetGraphics() { return new MyGraphics(CreateGraphics()); }
        public void AddControl(IControl control) { Controls.Add(control.ToControl()); }

        public Action<IForm> LoadCallback { set { load_callback = value; Load += new EventHandler(LoadEventHandler); } }
        public Action<IForm, IPaintEventProps> PaintCallback { set { paint_callback = value; Paint += new PaintEventHandler(PaintEventHandler); } }
        public Action<object> ClickCallback { set { click_callback = value; Click += new EventHandler(ClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseClickCallback { set { mouseClick_callback = value; MouseClick += new MouseEventHandler(MouseClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseDoubleClickCallback { set { mouseDoubleClick_callback = value; MouseDoubleClick += new MouseEventHandler(MouseDoubleClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseMoveCallback { set { mouseMove_callback = value; MouseMove += new MouseEventHandler(MouseMoveEventHandler); } }
        public Action<object, IMouseEventProps> MouseDownCallback { set { mouseDown_callback = value; MouseDown += new MouseEventHandler(MouseDownEventHandler); } }
        public Action<object, IMouseEventProps> MouseUpCallback { set { mouseUp_callback = value; MouseUp += new MouseEventHandler(MouseUpEventHandler); } }
        public Action<object, IKeyEventProps> KeyDownCallback { set { keyDown_callback = value; KeyDown += new KeyEventHandler(KeyDownEventHandler); } }
        public Action<object, IKeyEventProps> KeyUpCallback { set { keyUp_callback = value; KeyUp += new KeyEventHandler(KeyUpEventHandler); } }
    }
}