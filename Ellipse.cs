﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmirnoffDraw
{
    class Ellipse : Shape
    {
        public List<Point> points = new List<Point>();

        public Ellipse(int x1, int y1, int width, int height, Color color, int penWidth)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.width = width;
            this.height = height;
            this.color = color;
            this.penWidth = penWidth;

            Calculate(this.x1, this.y1, this.width, this.height);
        }

        public override void Calculate(int x1, int y1, int width, int height)
        {
            points.Clear();

            Point center = new Point(x1 + width / 2, y1 + height / 2);

            for (int i = 0; i < 360; i++)
            {
                double psi = (((i) % 360) * 3.14159f / 180.0f);
                double fi = Math.Atan2(width * Math.Sin(psi), height * Math.Cos(psi));
                float x = (float)((width / 2 * Math.Cos(fi)) + center.X);
                float y = (float)(height / 2 * Math.Sin(fi) + center.Y);
                points.Add(new Point(Convert.ToInt32(x), Convert.ToInt32(y)));
            }
        }

        public override void Draw(int x1, int y1, int width, int height, Color color, int penWidth, Form1 form, Pen pen)
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