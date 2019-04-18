using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmirnoffDraw
{
    public partial class Form1 : Form
    {
        //public PictureBox GetPictureBox()
        //{
        //    return pic;
        //}

        List<Shape> shapeList = new List<Shape>();

        Shape shapeCurr;

        public Bitmap pic { get; set; }
        public Bitmap pic1 { get; set; }
        string mode;
        int x1, y1;
        int xp, yp;
        int x2;
        int y2;
        int x, y;
        int xclick1, yclick1;
        public Graphics g { get; set; }
        public Graphics g1 { get; set; }
        public Form1()
        {
            mode = "Pen";
            InitializeComponent();
            pic = new Bitmap(1000, 1000);
            pic1 = new Bitmap(1000, 1000);
            g = Graphics.FromImage(pic);
            g1 = Graphics.FromImage(pic1);

            x1 = y1 = 0;
            xp = yp = 0;
            x = y = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            button4.BackColor = b.BackColor;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
                pic.Save(saveFileDialog1.FileName);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                pic = (Bitmap)Image.FromFile(openFileDialog1.FileName);
                pictureBox1.Image = pic;
            }    
        }

        private void button14_Click(object sender, EventArgs e)
        {
            mode = "Pen";
            button18.Image = Properties.Resources.pencil70x701;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            mode = "Rectangle";
            button18.Image = Properties.Resources.rectangle70x70;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            mode = "Ellipse";
            button18.Image = Properties.Resources.Ellipsefirst70x70;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Pen p;
            p = new Pen(button4.BackColor, trackBar1.Value);

            Graphics g;
            g = Graphics.FromImage(pic);

            Graphics g1;
            g1 = Graphics.FromImage(pic1);

            if (mode == "Pen")
            {
                g.DrawLine(p, xp, yp, e.X, e.Y);
            }

            if (mode == "Rectangle")
            {
                x = xclick1;
                y = yclick1;

                if (x > e.X) x = e.X;
                if (y > e.Y) y = e.Y;
                g.DrawRectangle(p, x, y, Math.Abs(e.X - xclick1), Math.Abs(e.Y - yclick1));
                //g.DrawRectangle(p, xclick1, yclick1, e.X - xclick1, e.Y - yclick1);
            }

            if (mode == "Ellipse")
            {
                g.DrawEllipse(p, xclick1, yclick1, e.X - xclick1, e.Y - yclick1);
            }

            if (mode == "Line")
            {
                g.DrawLine(p, xclick1, yclick1, e.X, e.Y);
            }

            if (mode == "Romb")
            {
                shapeList.Add(shapeCurr);
            }

            g1.DrawImage(pic, 0, 0);
                
                pictureBox1.Image = pic1;

        }

        private void button19_Click(object sender, EventArgs e)
        {
            mode = "Romb";
        }

        private void button17_Click(object sender, EventArgs e)
        {
            mode = "Line";
            button18.Image = Properties.Resources.line70x70;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            xclick1 = e.X;
            yclick1 = e.Y;
            x = e.X;
            y = e.Y;

            x1 = e.X;
            y1 = e.Y;

            if (mode == "Romb")
            {
                shapeCurr = new Rhombus(x1, y1, 0, 0);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Pen p;
            p = new Pen(button4.BackColor, trackBar1.Value);
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            p.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            Graphics g;
            g = Graphics.FromImage(pic);
            Graphics g1;
            g1 = Graphics.FromImage(pic1);

            if (e.Button == MouseButtons.Left)
            {
                if (mode == "Pen")
                {
                    g.DrawLine(p, xp, yp, e.X, e.Y);
                }

                if (mode == "Rectangle")
                {
                    g1.Clear(Color.White);
                    x = xclick1;
                    y = yclick1;

                    if (x > e.X) x = e.X;
                    if (y > e.Y) y = e.Y;
                    g1.DrawRectangle(p, x, y, Math.Abs(e.X - xclick1), Math.Abs(e.Y - yclick1));
                }

                if (mode == "Ellipse")
                {
                    g1.Clear(Color.White);
                    g1.DrawEllipse(p, xclick1, yclick1, e.X - xclick1, e.Y - yclick1);
                }

                if (mode == "Line")
                {
                    g1.Clear(Color.White);
                    g1.DrawLine(p, xclick1, yclick1, e.X, e.Y);
                }

                // Это всё было без классов, теперь с классамі

                x2 = e.Location.X;
                y2 = e.Location.Y;

                if (mode == "Romb")
                {

                    if (x2 < x1 && y2 > y1)
                    {
                        shapeCurr.x1 = x2;
                        shapeCurr.Calculate(x2, y1, Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                    }
                    if (x2 > x1 && y2 > y1)
                    {
                        //    rectCurr.x1 = x2;
                        shapeCurr.Calculate(x1, y1, Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                    }
                    if (x2 < x1 && y2 < y1)
                    {
                        shapeCurr.x1 = x2;
                        shapeCurr.y1 = y2;
                        shapeCurr.Calculate(x2, y2, Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                    }
                    if (x2 > x1 && y2 < y1)
                    {
                        shapeCurr.y1 = y2;
                        shapeCurr.Calculate(x1, y2, Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                    }

                    shapeCurr.width = Math.Abs(x2 - x1);
                    shapeCurr.height = Math.Abs(y2 - y1);

                    DrawShapes();
                    shapeCurr.Draw(x1, y1, 0, 0, this, p);
                }

                    g1.DrawImage(pic, 0, 0);
                
                pictureBox1.Image = pic1;
            }
            //x1 = e.X;
            //y1 = e.Y;

            xp = e.X;
            yp = e.Y;
        }

        public void DrawShapes()
        {
            g.Clear(Color.White);
            foreach (Shape shape in shapeList)
            {
                Pen p;
                p = new Pen(button4.BackColor, trackBar1.Value);
                p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                p.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                shape.Draw(x1, y1, 0, 0, this, p);
            }
        }
    }
}
