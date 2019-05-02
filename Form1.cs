using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace SmirnoffDraw
{
    public partial class Form1 : Form
    {
        List<Shape> shapeList = new List<Shape>();
        List<Shape> shapeListBuf = new List<Shape>();
        List<Type> typeList = new List<Type>();
        Type[] arrList;

        Shape shapeCurr;
        int currColor = Color.Red.ToArgb();
        int currPenWidth = 1;

        Rectangle checkedRectangle;
        Shape checkedShape;        

        public PictureBox GetPictureBox()
        {
            return pictureBox1;
        }

        public Bitmap pic { get; set; }        
        string mode;
        int x1, y1;
        int xp, yp;
        int x2;
        int y2;
        int x, y;
        Pen checkedPen;
        bool shiftDown = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
        public Graphics g { get; set; }
        
        Pen p;

        LineCreator LineC = new LineCreator();
        RectCreator RectC = new RectCreator();
        ElpCreator ElpC = new ElpCreator();
        RhoCreator RhoC = new RhoCreator();
        StarCreator StarC = new StarCreator();
        TriCreator TriC = new TriCreator();
        OctaCreator OctaC = new OctaCreator();
        PenCreator PenC = new PenCreator();
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            mode = "Pen";
            pic = new Bitmap(pictureBox1.Width, pictureBox1.Height);            
            g = Graphics.FromImage(pic);            

            checkedPen = new Pen(Color.Red, 1);
            checkedRectangle = new Rectangle();

            p = new Pen(Color.Red);
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            p.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            shapeCurr = new Rect();
            typeList.Add(shapeCurr.GetType());
            shapeCurr = new Octavian();
            typeList.Add(shapeCurr.GetType());
            shapeCurr = new Rhombus();
            typeList.Add(shapeCurr.GetType());
            shapeCurr = new Star();
            typeList.Add(shapeCurr.GetType());
            shapeCurr = new Ellipse();
            typeList.Add(shapeCurr.GetType());
            shapeCurr = new Triangle();
            typeList.Add(shapeCurr.GetType());
            shapeCurr = new Line();
            typeList.Add(shapeCurr.GetType());
            shapeCurr = new Pencil();
            typeList.Add(shapeCurr.GetType());

            arrList = typeList.ToArray<Type>();            

            x1 = y1 = 0;
            xp = yp = 0;
            x = y = 0;

            RefreshFigureList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            button4.BackColor = b.BackColor;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName + ".json";

            Type type = shapeList.GetType();
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Shape>), arrList);
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                jsonFormatter.WriteObject(fs, shapeList);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;

            Type type = shapeList.GetType();
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Shape>), arrList);
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                shapeList.Clear();
                shapeList = (List<Shape>)jsonFormatter.ReadObject(fs);
            }
            DrawShapes();
            RefreshFigureList();
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
                    shapeCurr.X1 = x2;
                }

                if (y2 < y1)
                {
                    shapeCurr.Y1 = y2;
                }

                shapeCurr.Width = Math.Abs(x2 - x1);
                shapeCurr.Height = Math.Abs(y2 - y1);
                shapeCurr.Color = currColor;
                shapeCurr.PenWidth = currPenWidth;

                if (cbShift.Checked)  //это надо для выделения, если не будет ифа, проблемы выделения с Shift
                {
                    if (shapeCurr.Width > shapeCurr.Height)
                    {
                        shapeCurr.Width = shapeCurr.Height;
                    }
                    else
                    {
                        shapeCurr.Height = shapeCurr.Width;
                    }
                }

                DrawShapes();
                shapeCurr.Draw(this, p);

                shapeList.Add(shapeCurr);               
                RefreshFigureList();
            }

        }

        private void button19_Click(object sender, EventArgs e)
        {
            mode = "Romb";
            button18.Image = Properties.Resources.rhombus64;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            mode = "Line";
            button18.Image = Properties.Resources.line70x70;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            mode = "Triangle";
            button18.Image = Properties.Resources.triangle64;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            mode = "Star";
            button18.Image = Properties.Resources.star64;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            mode = "Octavian";
            button18.Image = Properties.Resources.octagon64;
        }

        private void buttonUNDO_Click(object sender, EventArgs e)
        {
            try
            {
                shapeListBuf.Add(shapeList[shapeList.Count - 1]);
                shapeList.RemoveAt(shapeList.Count - 1);
                DrawShapes();
                RefreshFigureList();
            }
            catch { MessageBox.Show("Отмена невозможна"); }
        }

        private void buttonRETURN_Click(object sender, EventArgs e)
        {
            try
            {
                shapeList.Add(shapeListBuf[shapeListBuf.Count - 1]);
                shapeListBuf.RemoveAt(shapeListBuf.Count - 1);
                DrawShapes();
                RefreshFigureList();
            }
            catch { MessageBox.Show("Восстановление невозможно"); }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            shapeListBuf.Clear();
            currColor = button4.BackColor.ToArgb();
            p.Color = Color.FromArgb(currColor);
            p.Width = trackBar1.Value;
            currPenWidth = trackBar1.Value;

            x1 = e.X;
            y1 = e.Y;

            if (mode == "Romb")
            {
                shapeCurr = RhoC.FactoryMethod(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Rectangle")
            {
                shapeCurr = RectC.FactoryMethod(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Ellipse")
            {
                shapeCurr = ElpC.FactoryMethod(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Line")
            {
                shapeCurr = LineC.FactoryMethod(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Pen")
            {
                shapeCurr = PenC.FactoryMethod(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Triangle")
            {
                shapeCurr = TriC.FactoryMethod(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Star")
            {
                shapeCurr = StarC.FactoryMethod(x1, y1, 0, 0, currColor, currPenWidth);
            }
            else if (mode == "Octavian")
            {
                shapeCurr = OctaC.FactoryMethod(x1, y1, 0, 0, currColor, currPenWidth);
            }

        }

        private void lbFigures_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedShape = (Shape)lbFigures.SelectedItem;

            DrawShapes();


            if (checkedShape == null)
            {
                MessageBox.Show("Figure was not selected");
            }
            else
            {
                SelectedShape.SetSel(checkedShape.X1, checkedShape.Y1, checkedShape.X1 + checkedShape.Width,
              checkedShape.Y1 + checkedShape.Height, checkedPen);
                g.DrawRectangle(checkedPen, SelectedShape.GetX(), SelectedShape.GetY(), SelectedShape.GetW(), SelectedShape.GetH());



                tbX.Clear();
                tbY.Clear();
                tbH.Clear();
                tbW.Clear();
                tbPenWidth.Clear();

                tbX.AppendText(checkedShape.X1.ToString());
                tbY.AppendText(checkedShape.Y1.ToString());
                tbW.AppendText(checkedShape.Width.ToString());
                tbH.AppendText(checkedShape.Height.ToString());
                tbColor.BackColor = Color.FromArgb(checkedShape.Color);
                tbPenWidth.AppendText(checkedShape.PenWidth.ToString());
            }

            GetPictureBox().Image = pic;

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            shapeList[lbFigures.SelectedIndex].X1 = Convert.ToInt32(tbX.Text);
            shapeList[lbFigures.SelectedIndex].Y1 = Convert.ToInt32(tbY.Text);
            shapeList[lbFigures.SelectedIndex].Width = Convert.ToInt32(tbW.Text);
            shapeList[lbFigures.SelectedIndex].Height = Convert.ToInt32(tbH.Text);
            shapeList[lbFigures.SelectedIndex].Color = tbColor.BackColor.ToArgb();
            shapeList[lbFigures.SelectedIndex].PenWidth = Convert.ToInt32(tbPenWidth.Text);
            shapeList[lbFigures.SelectedIndex].Calculate(shapeList[lbFigures.SelectedIndex].X1, shapeList[lbFigures.SelectedIndex].Y1, shapeList[lbFigures.SelectedIndex].Width, shapeList[lbFigures.SelectedIndex].Height);
            DrawShapes();

            SelectedShape.SetSel(checkedShape.X1, checkedShape.Y1, checkedShape.X1 + checkedShape.Width,
              checkedShape.Y1 + checkedShape.Height, checkedPen);
            g.DrawRectangle(checkedPen, SelectedShape.GetX(), SelectedShape.GetY(), SelectedShape.GetW(), SelectedShape.GetH());
            GetPictureBox().Image = pic;
        }

        private void tbColor_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            if (MyDialog.ShowDialog() == DialogResult.OK)
                tbColor.BackColor = MyDialog.Color;
            checkedShape.Color = MyDialog.Color.ToArgb();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                x2 = e.Location.X;
                y2 = e.Location.Y;
                
                shapeCurr.Width = Math.Abs(x2 - x1);
                shapeCurr.Height = Math.Abs(y2 - y1);
                shapeCurr.Calculate(new Point(x1, y1), e.Location);

                if (cbShift.Checked)
                {
                    if (shapeCurr.Width > shapeCurr.Height)
                    {
                        shapeCurr.Width = shapeCurr.Height;
                    }
                    else
                    {
                        shapeCurr.Height = shapeCurr.Width;
                    }
                }


                if (x2 < x1 && y2 > y1)
                {
                    shapeCurr.X1 = x2;
                    shapeCurr.Calculate(x2, y1, shapeCurr.Width, shapeCurr.Height);
                }
                if (x2 > x1 && y2 > y1)
                {
                    shapeCurr.Calculate(x1, y1, shapeCurr.Width, shapeCurr.Height);
                }
                if (x2 < x1 && y2 < y1)
                {
                    shapeCurr.X1 = x2;
                    shapeCurr.Y1 = y2;
                    shapeCurr.Calculate(x2, y2, shapeCurr.Width, shapeCurr.Height);
                }
                if (x2 > x1 && y2 < y1)
                {
                    shapeCurr.Y1 = y2;
                    shapeCurr.Calculate(x1, y2, shapeCurr.Width, shapeCurr.Height);
                }
                
                DrawShapes();
                shapeCurr.Draw(this, p);               
            }
        }

        private void RefreshFigureList()
        {
            lbFigures.Items.Clear();
            foreach (Shape shape in shapeList)
            {
                lbFigures.Items.Add(shape);
            }
        }

        public void DrawShapes()
        {
            g.Clear(Color.White);
            foreach (Shape shape in shapeList)
            {
                p.Color = Color.FromArgb(shape.Color);
                p.Width = shape.PenWidth;
                shape.Draw(this, p);
            }
            p.Width = currPenWidth;
            p.Color = Color.FromArgb(currColor);
            GetPictureBox().Image = pic;
        }
    }
}
