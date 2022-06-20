﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MyPaint
{

    interface IClickable
    {
        public abstract Action<object> ClickCallback { set; }
        public abstract Action<object, IMouseEventProps> MouseClickCallback { set; }
        public abstract Action<object, IMouseEventProps> MouseDoubleClickCallback { set; }
        public abstract Action<object, IMouseEventProps> MouseMoveCallback { set; }
        public abstract Action<object, IMouseEventProps> MouseDownCallback { set; }
        public abstract Action<object, IMouseEventProps> MouseUpCallback { set; }
        public abstract Action<object, IKeyEventProps> KeyDownCallback { set; }
        public abstract Action<object, IKeyEventProps> KeyUpCallback { set; }
    }
    interface IButton : IControl
    {
        public abstract Point Location { get; set; }
        public abstract Size Size { get; set; }
        public abstract Image Image { get; set; }
        public abstract Color BackColor { get; set; }
        public abstract int TabIndex { get; set; }
        public abstract Action<IButton> ClickCallback { set; }
    }
    interface ILabel : IControl
    {
        public abstract Point Location { get; set; }
        public abstract Size Size { get; set; }
        public abstract Color BackColor { get; set; }
        public abstract string Text { get; set; }
    }
    interface IPictureBox : IControl, IClickable
    {
        public abstract Point Location { get; set; }
        public abstract Size Size { get; set; }
        public abstract Image Image { get; set; }
    }
    interface IGraphcis<T> where T : IGraphcis<T>, new()
    {
        private static T _instance = new T();
        protected abstract T GetFromImage(Bitmap img);
        public static T FromImage(Bitmap img) { return _instance.GetFromImage(img); }

        public abstract void DrawLine(Pen pen, Point a, Point b);
        public abstract void DrawLine(Pen pen, int x1, int y1, int x2, int y2);
        public abstract void DrawRectangle(Pen pen, Point a, int w, int h);
        public abstract void DrawRectangle(Pen pen, int x, int y, int w, int h);
        public abstract void DrawEllipse(Pen pen, Point a, int w, int h);
        public abstract void DrawEllipse(Pen pen, int x, int y, int w, int h);
        public abstract void DrawPolygon(Pen pen, Point[] points);
    }
    public enum MouseButton
    {
        None,
        Left,
        Middle,
        Right
    }
    interface IMouseEventProps
    {
        public abstract MouseButton Button { get; }
        public abstract Point Location { get; }
        public abstract int Delta { get; }
    }
    public enum Key
    {
        None = 0,
        Tab = 9,
        Enter = 13,
        CapsLock = 20,
        Space = 32,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        LShift = 160,
        RShift = 161,
        LControl = 162,
        RControl = 163,
        LAlt = 164,
        RAlt = 165,
    }
    interface IKeyEventProps
    {
        public abstract bool Alt { get; }
        public abstract bool Ctrl { get; }
        public abstract bool Shift { get; }
        public abstract Key KeyCode { get; }
    }
    interface IPaintEventProps
    {
        public abstract MyGraphics Graphics { get; }
    }
    interface IForm : IClickable
    {
        public abstract IGraphcis<MyGraphics> GetGraphics();
        public abstract void AddControl(IControl control);
        public abstract Action<IForm> LoadCallback { set; }
        public abstract Action<IForm, IPaintEventProps> PaintCallback { set; }
    }
}