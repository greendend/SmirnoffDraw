﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SmirnoffDraw
{
    [Serializable]
    public class Octavian : Shape
    {
        public List<Point> points = new List<Point>();
        public Octavian(int x1, int y1, int width, int height, int color, int penWidth)
        {
            X1 = x1;
            Y1 = y1;
            Width = width;
            Height = height;
            Color = color;
            PenWidth = penWidth;
            Calculate(X1, Y1, Width, Height);
        }

        public Octavian() { }

        public override void Calculate(int x1, int y1, int width, int height)
        {
            Point center = new Point(x1 + width / 2, y1 + height / 2);
            pointList.Clear();
            points.Clear();
            for (int i = 0; i <= 360; i += 360 / 8)
            {
                double psi = (((i - 90) % 360) * 3.14159f / 180.0f);
                double fi = Math.Atan2(width * Math.Sin(psi), height * Math.Cos(psi));
                float x = (float)((width / 2 * Math.Cos(fi)) + center.X);
                float y = (float)(height / 2 * Math.Sin(fi) + center.Y);
                points.Add(new Point(Convert.ToInt32(x), Convert.ToInt32(y)));
            }
        }

        public override void Draw(Form1 form, Pen pen)
        {
            Graphics canvas = form.g;
            for (int i = 0; i < points.Count; i++)
            {
                canvas.DrawLine(pen, points[i], points[((i + 1) % points.Count)]);
            }
            form.GetPictureBox().Image = form.pic;
        }
    }
}
