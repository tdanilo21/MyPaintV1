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
        protected Point a;
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
        public abstract void Update(IGraphics<MyGraphics> g, Point p);
        public virtual void Reset() { mouseDown = false; }
        public void ChangeColor(Color c) { pen.Color = c; }
        public void ChangeWidth(int w) { pen.Width = w; }
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
        public override void Update(IGraphics<MyGraphics> g, Point p)
        {
            if (mouseDown)
            {
                path.Add(p);
                Show(g);
            }
            mouseDown = true;
            a = p;
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
            pen.Color = Color.White;
            g.DrawPath(pen, path.ToArray());
            pen.Color = c;
        }
    }
    
    abstract class DrawShape : Event
    {
        public DrawShape() : base() { }
        public DrawShape(Event e) : base(e) { }

        protected abstract void ExtendShape(IGraphics<MyGraphics> g, Point p);

        public override void Update(IGraphics<MyGraphics> g, Point p)
        {
            if (mouseDown) ExtendShape(g, p);
            else a = p;
            mouseDown = true;
        }
    }

    class MyRectangle : DrawShape
    {
        public MyRectangle() : base() { }
        public MyRectangle(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point p)
        {
            Point b = new Point(Math.Min(a.X, p.X), Math.Min(a.Y, p.Y));
            Point c = new Point(Math.Max(a.X, p.X), Math.Max(a.Y, p.Y));
            g.DrawRectangle(pen, b.X, b.Y, c.X - b.X, c.Y - b.Y);
        }
    }

    class MyEllipse : DrawShape
    {
        public MyEllipse() : base() { }
        public MyEllipse(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point p)
        {
            g.DrawEllipse(pen, a.X, a.Y, p.X - a.X, p.Y - a.Y);
        }
    }
    class MyTriangle : DrawShape
    {
        public MyTriangle() : base() { }
        public MyTriangle(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point p)
        {
            Point[] points = { new Point((a.X + p.X) / 2, a.Y), new Point(a.X, p.Y), new Point(p.X, p.Y) };
            g.DrawPolygon(pen, points);
        }
    }

    class Arrow : DrawShape
    {
        public Arrow() : base() { }
        public Arrow(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point p)
        {
            Point[] points = { new Point((a.X + p.X) / 2, a.Y), new Point(a.X, (a.Y + p.Y) / 2), new Point((3 * a.X + p.X) / 4, (a.Y + p.Y) / 2),
                               new Point((3 * a.X + p.X) / 4, p.Y), new Point((a.X + 3 * p.X) / 4, p.Y), new Point((a.X + 3 * p.X) / 4, (a.Y + p.Y) / 2),
                               new Point(p.X, (a.Y + p.Y) / 2) };
            g.DrawPolygon(pen, points);
        }
    }
    class Star : DrawShape
    {
        public Star() : base() { }
        public Star(Event e) : base(e) { }

        protected override void ExtendShape(IGraphics<MyGraphics> g, Point p)
        {
            Point[] points = { new Point((a.X + p.X) / 2, a.Y), new Point((2 * a.X + p.X) / 3, (2 * a.Y + p.Y) / 3), new Point(a.X, (a.Y + p.Y) / 2),
                               new Point((2 * a.X + p.X) / 3, (a.Y + 2 * p.Y) / 3), new Point((a.X + p.X) / 2, p.Y), new Point((a.X + 2 * p.X) / 3, (a.Y + 2 * p.Y) / 3),
                               new Point(p.X, (a.Y + p.Y) / 2), new Point((a.X + 2 * p.X) / 3, (2 * a.Y + p.Y) / 3) };
            g.DrawPolygon(pen, points);
        }
    }
}