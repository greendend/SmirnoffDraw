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
        List<Shape> shapeList = new List<Shape>();
        List<Shape> shapeListBuf = new List<Shape>();

        Shape shapeCurr;
        Color currColor = Color.Red;
        int currPenWidth = 1;

        public PictureBox GetPictureBox()
        {
            return pictureBox1;
        }

        public Bitmap pic { get; set; }
        public Bitmap pic1 { get; set; }
        string mode;
        int x1, y1;
        int xp, yp;
        int x2;
        int y2;
        int x, y;
        bool shiftDown = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
        public Graphics g { get; set; }
        public Graphics g1 { get; set; }
        Pen p;
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            mode = "Pen";
            pic = new Bitmap(1000, 1000);
            pic1 = new Bitmap(1000, 1000);
            g = Graphics.FromImage(pic);
            g1 = Graphics.FromImage(pic1);

            p = new Pen(Color.Red);
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            p.EndCap = System.Drawing.Drawing2D.LineCap.Round;

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
            x2 = e.Location.X;
            y2 = e.Location.Y;

            {
                if (x2 < x1)
                {
                    shapeCurr.x1 = x2;
                }

                if (y2 < y1)
                {
                    shapeCurr.y1 = y2;
                }

                shapeCurr.width = Math.Abs(x2 - x1);
                shapeCurr.height = Math.Abs(y2 - y1);
                shapeCurr.color = currColor;
                shapeCurr.penWidth = currPenWidth;

                DrawShapes();
                shapeCurr.Draw(x1, y1, shapeCurr.width, shapeCurr.height, currColor, currPenWidth, this, p);

                shapeList.Add(shapeCurr);
            }

        }

        private void button19_Click(object sender, EventArgs e)
        {
            mode = "Romb";
        }

        //private void Form1_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Shift)
        //    {
        //        if (shapeCurr.width > shapeCurr.height)
        //        {
        //            shapeCurr.width = shapeCurr.height;
        //        }
        //        else
        //        {
        //            shapeCurr.height = shapeCurr.width;
        //        }
        //    }
        //}

        private void button17_Click(object sender, EventArgs e)
        {
            mode = "Line";
            button18.Image = Properties.Resources.line70x70;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            mode = "Triangle";
        }

        private void button21_Click(object sender, EventArgs e)
        {
            mode = "Star";
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            currColor = button4.BackColor;
            p.Color = currColor;
            p.Width = trackBar1.Value;
            currPenWidth = trackBar1.Value;

            x1 = e.X;
            y1 = e.Y;

            if (mode == "Romb")
            {
                shapeCurr = new Rhombus(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Rectangle")
            {
                shapeCurr = new Rect(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Ellipse")
            {
                shapeCurr = new Ellipse(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Line")
            {
                shapeCurr = new Line(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Pen")
            {
                shapeCurr = new Pencil(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Triangle")
            {
                shapeCurr = new Triangle(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Star")
            {
                shapeCurr = new Star(x1, y1, 0, 0, currColor, currPenWidth);
            }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                x2 = e.Location.X;
                y2 = e.Location.Y;
                
                shapeCurr.width = Math.Abs(x2 - x1);
                shapeCurr.height = Math.Abs(y2 - y1);
                shapeCurr.Calculate(new Point(x1, y1), e.Location);

                if (cbShift.Checked)
                {
                    if (shapeCurr.width > shapeCurr.height)
                    {
                        shapeCurr.width = shapeCurr.height;
                    }
                    else
                    {
                        shapeCurr.height = shapeCurr.width;
                    }
                }


                if (x2 < x1 && y2 > y1)
                {
                    shapeCurr.x1 = x2;
                    shapeCurr.Calculate(x2, y1, shapeCurr.width, shapeCurr.height);
                }
                if (x2 > x1 && y2 > y1)
                {
                    shapeCurr.Calculate(x1, y1, shapeCurr.width, shapeCurr.height);
                }
                if (x2 < x1 && y2 < y1)
                {
                    shapeCurr.x1 = x2;
                    shapeCurr.y1 = y2;
                    shapeCurr.Calculate(x2, y2, shapeCurr.width, shapeCurr.height);
                }
                if (x2 > x1 && y2 < y1)
                {
                    shapeCurr.y1 = y2;
                    shapeCurr.Calculate(x1, y2, shapeCurr.width, shapeCurr.height);
                }
                
                DrawShapes();
                shapeCurr.Draw(x1, y1, shapeCurr.width, shapeCurr.height, currColor, currPenWidth, this, p);               
            }
        }

        public void DrawShapes()
        {
            g.Clear(Color.White);
            foreach (Shape shape in shapeList)
            {
                p.Color = shape.color;
                p.Width = shape.penWidth;
                shape.Draw(x1, y1, shape.width, shape.height, shape.color, shape.penWidth, this, p);
            }
            p.Width = currPenWidth;
            p.Color = currColor;
            GetPictureBox().Image = pic;
        }
    }
}
