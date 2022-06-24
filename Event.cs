using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MyPaint
{
    abstract class Event
    {
        protected Pen pen;
        protected bool mouseDown;
        protected Point start;
        public Event()
        {
            pen = new Pen(Color.Black);
            mouseDown = false;
        }

        public Event(Event e)
        {
            pen = e.pen;
            mouseDown = false;
        }
        public Color Color
        {
            get { return pen.Color; }
            set { pen.Color = value; }
        }
        public int PenWidth
        {
            set
            {
                if (value <= 0) throw new Exception("Width of start pen cannot be less than or equal to zero.");
                pen.Width = value;
            }
        }
        public abstract void Update(IGraphics<MyGraphics> g, Point p);
        public virtual void Reset() { mouseDown = false; }
    }

    class Selection : Event
    {
        private Point end, a;
        private bool constructed;
        Action undo_callback;
        Action<Rectangle, int, int> move_callback;
        public Selection(Action undo, Action<Rectangle, int, int> move) : base()
        {
            constructed = false;
            undo_callback = undo;
            move_callback = move;
        }
        public Selection(Event e, Action undo, Action<Rectangle, int, int> move) : base(e)
        {
            constructed = false;
            undo_callback = undo;
            move_callback = move;
        }
        private void Draw(IGraphics<MyGraphics> g)
        {
            Pen pen = new Pen(Color.LightBlue);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            Point a = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point b = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
            g.DrawRectangle(pen, a.X, a.Y, b.X - a.X, b.Y - a.Y);
        }
        private bool Inside(Point p)
        {
            Rectangle rect = GetSelection();
            return p.X >= rect.X && p.Y >= rect.Y && p.X <= rect.X + rect.Width && p.Y <= rect.Y + rect.Height;
        }
        private Rectangle GetSelection() { return new Rectangle(start, new Size(end.X - start.X, end.Y - start.Y)); }
        public override void Update(IGraphics<MyGraphics> g, Point p)
        {
            if (constructed && Inside(p))
            {
                if (mouseDown) move_callback(GetSelection(), p.X - a.X, p.Y - a.Y);
                a = p;
                mouseDown = true;
            }
            else
            {
                if (constructed)
                {
                    constructed = false;
                    undo_callback();
                }
                if (mouseDown)
                {
                    end = p;
                    Draw(g);
                }
                else start = p;
                mouseDown = true;
            }
        }
        public override void Reset()
        {
            base.Reset();
            constructed = true;
            if (start.X > end.X) (start.X, end.X) = (end.X, start.X);
            if (start.Y > end.Y) (start.Y, end.Y) = (start.Y, end.Y);
        }
    }

    abstract class HandDrawing : Event
    {
        protected List<Point> path;
        public HandDrawing() : base()
        {
            path = new List<Point>();
        }

        public HandDrawing(Event e) : base(e)
        {
            path = new List<Point>();
        }
        protected abstract void Show(IGraphics<MyGraphics> g);
        public override void Update(IGraphics<MyGraphics> g, Point end)
        {
            if (mouseDown)
            {
                path.Add(end);
                Show(g);
            }
            mouseDown = true;
            start = end;
        }
        public override void Reset()
        {
            base.Reset();
            path.Clear();
        }
    }

    class Draw : HandDrawing
    {
        public Draw() : base() { }
        public Draw(Event e) : base(e) { }
        protected override void Show(IGraphics<MyGraphics> g)
        {
            g.DrawPath(pen, path.ToArray());
        }
    }
    class Erase : HandDrawing
    {
        public Erase() : base() { }
        public Erase(Event e) : base(e) { }
        protected override void Show(IGraphics<MyGraphics> g)
        {
            Color c = pen.Color;
            pen.Color = Color.FromArgb(240, 240, 240);
            g.DrawPath(pen, path.ToArray());
            pen.Color = c;
        }
    }

    abstract class DrawShape : Event
    {
        public DrawShape() : base() { }
        public DrawShape(Event e) : base(e) { }

        protected abstract void ExtendShape(IGraphics<MyGraphics> g, Point end);

        public override void Update(IGraphics<MyGraphics> g, Point end)
        {
            if (mouseDown) ExtendShape(g, end);
            else start = end;
            mouseDown = true;
        }
    }

    class StraightLine : DrawShape
    {
        public StraightLine() : base() { }
        public StraightLine(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point end)
        {
            g.DrawLine(pen, start, end);
        }
    }

    class MyRectangle : DrawShape
    {
        public MyRectangle() : base() { }
        public MyRectangle(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point end)
        {
            Point a = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point b = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
            g.DrawRectangle(pen, a.X, a.Y, b.X - a.X, b.Y - a.Y);
        }
    }

    class MyEllipse : DrawShape
    {
        public MyEllipse() : base() { }
        public MyEllipse(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point end)
        {
            g.DrawEllipse(pen, start.X, start.Y, end.X - start.X, end.Y - start.Y);
        }
    }
    class MyTriangle : DrawShape
    {
        public MyTriangle() : base() { }
        public MyTriangle(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point end)
        {
            Point[] points = { new Point((start.X + end.X) / 2, start.Y), new Point(start.X, end.Y), new Point(end.X, end.Y) };
            g.DrawPolygon(pen, points);
        }
    }

    class VerticalArrow : DrawShape
    {
        public VerticalArrow() : base() { }
        public VerticalArrow(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point end)
        {
            Point[] points = { new Point((start.X + end.X) / 2, start.Y), new Point(start.X, (start.Y + end.Y) / 2), new Point((3 * start.X + end.X) / 4, (start.Y + end.Y) / 2),
                               new Point((3 * start.X + end.X) / 4, end.Y), new Point((start.X + 3 * end.X) / 4, end.Y), new Point((start.X + 3 * end.X) / 4, (start.Y + end.Y) / 2),
                               new Point(end.X, (start.Y + end.Y) / 2) };
            g.DrawPolygon(pen, points);
        }
    }
    class HorizontalArrow : DrawShape
    {
        public HorizontalArrow() : base() { }
        public HorizontalArrow(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point end)
        {
            Point[] points = { new Point(end.X, (start.Y + end.Y) / 2), new Point((start.X + end.X) / 2, start.Y), new Point((start.X + end.X) / 2, (3 * start.Y + end.Y) / 4),
                               new Point(start.X, (3 * start.Y + end.Y) / 4),  new Point(start.X, (start.Y + 3 * end.Y) / 4), 
                               new Point((start.X + end.X) / 2, (start.Y + 3 * end.Y) / 4), new Point((start.X + end.X) / 2, end.Y) };
            g.DrawPolygon(pen, points);
        }
    }
    class Star : DrawShape
    {
        public Star() : base() { }
        public Star(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point end)
        {
            Point[] points = { new Point((start.X + end.X) / 2, start.Y), new Point((2 * start.X + end.X) / 3, (2 * start.Y + end.Y) / 3), new Point(start.X, (start.Y + end.Y) / 2),
                               new Point((2 * start.X + end.X) / 3, (start.Y + 2 * end.Y) / 3), new Point((start.X + end.X) / 2, end.Y),
                               new Point((start.X + 2 * end.X) / 3, (start.Y + 2 * end.Y) / 3), new Point(end.X, (start.Y + end.Y) / 2),
                               new Point((start.X + 2 * end.X) / 3, (2 * start.Y + end.Y) / 3) };
            g.DrawPolygon(pen, points);
        }
    }
    class Heart : DrawShape
    {
        public Heart() : base() { }
        public Heart(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point end)
        {
            Point a = new Point(Math.Min(start.X, end.X), start.Y);
            Point b = new Point(Math.Max(start.X, end.X), end.Y);
            g.DrawBezier(pen, (a.X + b.X) / 2, (13 * a.Y + 7 * b.Y) / 20, (5 * b.X - a.X) / 4, a.Y, b.X, (a.Y + 3 * b.Y) / 4, (a.X + b.X) / 2, b.Y);
            g.DrawBezier(pen, (a.X + b.X) / 2, (13 * a.Y + 7 * b.Y) / 20, (5 * a.X - b.X) / 4, a.Y, a.X, (a.Y + 3 * b.Y) / 4, (a.X + b.X) / 2, b.Y);
        }
    }
}