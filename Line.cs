using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SmirnoffDraw
{
    [Serializable]
    class Line : Shape
    {
        public int counter = 0;
        public List<Point> points = new List<Point>();
        public Line(int x1, int y1, int width, int height, Color color, int penWidth)
        {

            this.x1 = x1;
            this.y1 = y1;
            this.width = width;
            this.height = height;
            this.color = color;
            this.penWidth = penWidth;

            Calculate(this.x1, this.y1, this.width, this.height);
            Calculate(new Point(this.x1, this.y1), new Point(this.x1, this.y1));
        }

        public override void Calculate(int x1, int y1, int width, int height)
        {
            pointList.Clear();
            pointList.Add(new float[4] { x1, y1, x1 + width, y1 + height });
        }

        public override void Calculate(Point from, Point to)
        {
            points.Clear();
            points.Add(from);
            points.Add(to);
        }

        public override void Draw(int x1, int y1, int width, int height, Color color, int penWidth, Form1 form, Pen pen)
        {
            form.g.DrawLine(pen, points[0], points[1]);
            form.GetPictureBox().Image = form.pic;
        }
    }
}
