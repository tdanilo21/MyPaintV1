using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MyPaint
{
    interface IControl
    {
        public static Dictionary<int, IControl> IndexMap = new Dictionary<int, IControl>();
        public static int GetIndex()
        {
            int index = 0;
            while (IndexMap.ContainsKey(index)) index++;
            return index;
        }
        public abstract int Index { get; set; }
        public abstract Control ToControl();
    }
    class MyButton : Button, IButton
    {
        int index;
        List<Action<MyButton> > click_callback;
        public MyButton() : base()
        {
            click_callback = new List<Action<MyButton>>();
        }
        private void ClickEventHandler(object sender, EventArgs e) { foreach (var f in click_callback) f(this); }
        public int Index
        {
            get { return index; }
            set
            {
                if (IControl.IndexMap.ContainsKey(value)) throw new Exception("Control index " + value + " already exists.");
                index = value;
                IControl.IndexMap.Add(value, this);
            }
        }
        public Control ToControl() { return this; }
        public Action<IButton> ClickCallback
        { set { click_callback.Add(value); if (click_callback.Count == 1) Click += new EventHandler(ClickEventHandler); } }
    }
    class MyLabel : Label, ILabel
    {
        int index;
        public Control ToControl() { return this; }
        public int Index
        {
            get { return index; }
            set
            {
                if (IControl.IndexMap.ContainsKey(value)) throw new Exception("Control index " + value + " already exists.");
                index = value;
                IControl.IndexMap.Add(value, this);
            }
        }
    }
    class MyPictureBox : PictureBox, IPictureBox, IClickable
    {
        int index;
        List<Action<MyPictureBox> > click_callback;
        List<Action<MyPictureBox, IMouseEventProps> > mouseClick_callback, mouseDoubleClick_callback, mouseMove_callback, mouseDown_callback, mouseUp_callback;
        List<Action<MyPictureBox, IKeyEventProps> > keyDown_callback, keyUp_callback;

        public MyPictureBox() : base()
        {
            click_callback = new List<Action<MyPictureBox>>();
            mouseClick_callback = new List<Action<MyPictureBox, IMouseEventProps>>();
            mouseDoubleClick_callback = new List<Action<MyPictureBox, IMouseEventProps>>();
            mouseMove_callback = new List<Action<MyPictureBox, IMouseEventProps>>();
            mouseDown_callback = new List<Action<MyPictureBox, IMouseEventProps>>();
            mouseUp_callback = new List<Action<MyPictureBox, IMouseEventProps>>();
            keyDown_callback = new List<Action<MyPictureBox, IKeyEventProps>>();
            keyUp_callback = new List<Action<MyPictureBox, IKeyEventProps>>();
        }

        private void ClickEventHandler(object sender, EventArgs e) { foreach (var f in click_callback) f(this); }
        private void MouseClickEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseClick_callback) f(this, new MouseEventProps(e)); }
        private void MouseDoubleClickEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseDoubleClick_callback) f(this, new MouseEventProps(e)); }
        private void MouseMoveEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseMove_callback) f(this, new MouseEventProps(e)); }
        private void MouseDownEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseDown_callback) f(this, new MouseEventProps(e)); }
        private void MouseUpEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseUp_callback) f(this, new MouseEventProps(e)); }
        private void KeyDownEventHandler(object sender, KeyEventArgs e) { foreach (var f in keyDown_callback) f(this, new KeyEventProps(e)); }
        private void KeyUpEventHandler(object sender, KeyEventArgs e) { foreach (var f in keyUp_callback) f(this, new KeyEventProps(e)); }

        public Control ToControl() { return this; }
        public int Index
        {
            get { return index; }
            set
            {
                if (IControl.IndexMap.ContainsKey(value)) throw new Exception("Control index " + value + " already exists.");
                index = value;
                IControl.IndexMap.Add(value, this);
            }
        }

        public Action<object> ClickCallback
        { set { click_callback.Add(value); if (click_callback.Count == 1) Click += new EventHandler(ClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseClickCallback
        { set { mouseClick_callback.Add(value); if (mouseClick_callback.Count == 1) MouseClick += new MouseEventHandler(MouseClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseDoubleClickCallback
        { set { mouseDoubleClick_callback.Add(value); if (mouseDoubleClick_callback.Count == 1) MouseDoubleClick += new MouseEventHandler(MouseDoubleClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseMoveCallback
        { set { mouseMove_callback.Add(value); if (mouseMove_callback.Count == 1) MouseMove += new MouseEventHandler(MouseMoveEventHandler); } }
        public Action<object, IMouseEventProps> MouseDownCallback
        { set { mouseDown_callback.Add(value); if (mouseDown_callback.Count == 1) MouseDown += new MouseEventHandler(MouseDownEventHandler); } }
        public Action<object, IMouseEventProps> MouseUpCallback
        { set { mouseUp_callback.Add(value); if (mouseUp_callback.Count == 1) MouseUp += new MouseEventHandler(MouseUpEventHandler); } }
        public Action<object, IKeyEventProps> KeyDownCallback
        { set { keyDown_callback.Add(value); if (keyDown_callback.Count == 1) KeyDown += new KeyEventHandler(KeyDownEventHandler); } }
        public Action<object, IKeyEventProps> KeyUpCallback
        { set { keyUp_callback.Add(value); if (keyUp_callback.Count == 1) KeyUp += new KeyEventHandler(KeyUpEventHandler); } }
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
        void IGraphics<MyGraphics>.DrawPath(Pen pen, Point[] points)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            for (int i = 0; i < points.Length - 1; i++) path.AddLine(points[i], points[i + 1]);
            g.DrawPath(pen, path);
        }
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
        List<Action<MyForm> > load_callback, click_callback;
        List<Action<MyForm, IPaintEventProps> > paint_callback;
        List<Action<MyForm, IMouseEventProps> > mouseClick_callback, mouseDoubleClick_callback, mouseMove_callback, mouseDown_callback, mouseUp_callback;
        List<Action<MyForm, IKeyEventProps> > keyDown_callback, keyUp_callback;

        public MyForm() : base()
        {
            load_callback = new List<Action<MyForm>>();
            click_callback = new List<Action<MyForm>>();
            paint_callback = new List<Action<MyForm, IPaintEventProps>>();
            mouseClick_callback = new List<Action<MyForm, IMouseEventProps>>();
            mouseDoubleClick_callback = new List<Action<MyForm, IMouseEventProps>>();
            mouseMove_callback = new List<Action<MyForm, IMouseEventProps>>();
            mouseDown_callback = new List<Action<MyForm, IMouseEventProps>>();
            mouseUp_callback = new List<Action<MyForm, IMouseEventProps>>();
            keyDown_callback = new List<Action<MyForm, IKeyEventProps>>();
            keyUp_callback = new List<Action<MyForm, IKeyEventProps>>();
        }

        private void LoadEventHandler(object sender, EventArgs e) { foreach(var f in load_callback) f(this); }
        private void PaintEventHandler(object sender, PaintEventArgs e) { foreach (var f in paint_callback) f(this, new PaintEventProps(e)); }
        private void ClickEventHandler(object sender, EventArgs e) { foreach (var f in click_callback) f(this); }
        private void MouseClickEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseClick_callback) f(this, new MouseEventProps(e)); }
        private void MouseDoubleClickEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseDoubleClick_callback) f(this, new MouseEventProps(e)); }
        private void MouseMoveEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseMove_callback) f(this, new MouseEventProps(e)); }
        private void MouseDownEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseDown_callback) f(this, new MouseEventProps(e)); }
        private void MouseUpEventHandler(object sender, MouseEventArgs e) { foreach (var f in mouseUp_callback) f(this, new MouseEventProps(e)); }
        private void KeyDownEventHandler(object sender, KeyEventArgs e) { foreach (var f in keyDown_callback) f(this, new KeyEventProps(e)); }
        private void KeyUpEventHandler(object sender, KeyEventArgs e) { foreach (var f in keyUp_callback) f(this, new KeyEventProps(e)); }

        public IGraphics<MyGraphics> GetGraphics() { return new MyGraphics(CreateGraphics()); }
        public void AddControl(IControl control) { Controls.Add(control.ToControl()); }

        public Action<IForm> LoadCallback
        { set { load_callback.Add(value); if (load_callback.Count == 1) Load += new EventHandler(LoadEventHandler); } }
        public Action<IForm, IPaintEventProps> PaintCallback
        { set { paint_callback.Add(value); if (paint_callback.Count == 1) Paint += new PaintEventHandler(PaintEventHandler); } }
        public Action<object> ClickCallback
        { set { click_callback.Add(value); if (click_callback.Count == 1) Click += new EventHandler(ClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseClickCallback
        { set { mouseClick_callback.Add(value); if (click_callback.Count == 1) MouseClick += new MouseEventHandler(MouseClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseDoubleClickCallback
        { set { mouseDoubleClick_callback.Add(value); if (mouseDoubleClick_callback.Count == 1) MouseDoubleClick += new MouseEventHandler(MouseDoubleClickEventHandler); } }
        public Action<object, IMouseEventProps> MouseMoveCallback
        { set { mouseMove_callback.Add(value); if (mouseMove_callback.Count == 1) MouseMove += new MouseEventHandler(MouseMoveEventHandler); } }
        public Action<object, IMouseEventProps> MouseDownCallback
        { set { mouseDown_callback.Add(value); if (mouseDown_callback.Count == 1) MouseDown += new MouseEventHandler(MouseDownEventHandler); } }
        public Action<object, IMouseEventProps> MouseUpCallback
        { set { mouseUp_callback.Add(value); if (mouseDown_callback.Count == 1) MouseUp += new MouseEventHandler(MouseUpEventHandler); } }
        public Action<object, IKeyEventProps> KeyDownCallback
        { set { keyDown_callback.Add(value); if (keyDown_callback.Count == 1) KeyDown += new KeyEventHandler(KeyDownEventHandler); } }
        public Action<object, IKeyEventProps> KeyUpCallback
        { set { keyUp_callback.Add(value); if (keyUp_callback.Count == 1) KeyUp += new KeyEventHandler(KeyUpEventHandler); } }
    }
}