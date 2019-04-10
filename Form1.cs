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
        Bitmap pic;
        Bitmap pic1;
        string mode;
        int x1, y1;
        int x, y;
        int xclick1, yclick1;
        public Form1()
        {
            mode = "Pen";
            InitializeComponent();
            pic = new Bitmap(1000, 1000);
            pic1 = new Bitmap(1000, 1000);
            x1 = y1 = 0;
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
                g.DrawLine(p, x1, y1, e.X, e.Y);
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

            g1.DrawImage(pic, 0, 0);
                
                pictureBox1.Image = pic1;

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
                    g.DrawLine(p, x1, y1, e.X, e.Y);
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

                g1.DrawImage(pic, 0, 0);
                
                pictureBox1.Image = pic1;
            }
            x1 = e.X;
            y1 = e.Y;
        }
    }
}
