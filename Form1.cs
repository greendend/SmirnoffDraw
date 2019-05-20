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
using System.Reflection;

namespace SmirnoffDraw
{
    public partial class Form1 : Form
    {
        List<Shape> shapeList = new List<Shape>();
        List<Shape> shapeListBuf = new List<Shape>();
        List<Type> typeList = new List<Type>();
        Type[] arrList;

        List<Shape> Factory = new List<Shape>();
        private List<IPlugin> plugins = new List<IPlugin>();

        Shape shapeCurr;
        Shape checkedShape;

        int currColor = Color.Red.ToArgb();
        int currPenWidth = 1;
        
        public PictureBox GetPictureBox()
        {
            return pictureBox1;
        }

        public Bitmap pic { get; set; }             
        int x1, y1;
        int xp, yp;
        int x2;
        int y2;
        int x, y;
        Pen checkedPen;
        bool shiftDown = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
        public Graphics g { get; set; }

        Pen p;
        bool isPluginsNull;

        public Form1()
        {
            InitializeComponent();
            RefreshPlugins();
            this.KeyPreview = true;
            //mode = "Pen";
            pic = new Bitmap(pictureBox1.Width, pictureBox1.Height);            
            g = Graphics.FromImage(pic);            

            checkedPen = new Pen(Color.Red, 1);
            //checkedRectangle = new Rectangle();

            p = new Pen(Color.Red);
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            p.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            List<Type> types = new List<Type>();
            foreach (Type t in typeof(Shape).Assembly.GetExportedTypes())
            {
                types.Add(t);
            }

            arrList = typeList.ToArray<Type>();            

            x1 = y1 = 0;
            xp = yp = 0;
            x = y = 0;

            RefreshFigureList();
        }

        //путь к папке с плагинами
        private readonly string pluginPath = System.IO.Path.Combine(
                                                        Directory.GetCurrentDirectory(),
                                                        "Plugins");

        private void RefreshPlugins()
        {
            isPluginsNull = true;
            arrList = null;
            typeList.Clear();
            plugins.Clear();
            cmbbShapes.Items.Clear();

            DirectoryInfo pluginDirectory = new DirectoryInfo(pluginPath);
            if (!pluginDirectory.Exists)
                pluginDirectory.Create();

            //берем из директории все файлы с расширением .dll      
            var pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
            foreach (var file in pluginFiles)
            {
                //загружаем сборку
                Assembly asm = Assembly.Load(File.ReadAllBytes(file));
                //ищем типы, имплементирующие наш интерфейс IPlugin,
                //чтобы не захватить лишнего
                var types = asm.GetTypes().
                                Where(t => t.GetInterfaces().
                                Where(i => i.FullName == typeof(IPlugin).FullName).Any());

                //заполняем экземплярами полученных типов коллекцию плагинов
                foreach (var type in types)
                {
                    var plugin = asm.CreateInstance(type.FullName) as IPlugin;
                    plugins.Add(plugin);
                }
            }

            foreach (var plugin in plugins)
            {
                cmbbShapes.Items.Add(plugin.Load(this).GetType());
                typeList.Add(plugin.Load(this).GetType());
            }
            arrList = typeList.ToArray<Type>();

            if (plugins.Count > 0)
            {
                isPluginsNull = false;
                cmbbShapes.SelectedItem = cmbbShapes.Items[0];
            }
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
            string textFromFile;
            shapeList.Clear();
            //Type type = shapeList.GetType();          
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Shape>), arrList);
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fs.Length];
                // считываем данные
                fs.Read(array, 0, array.Length);
                // декодируем байты в строку
                textFromFile = Encoding.Default.GetString(array);
                //ShapeList.Clear();
                //ShapeList = (List<Shape>)jsonFormatter.ReadObject(fs);       
            }               

            textFromFile.Remove(0, 1); 

            while (textFromFile.Length > 1)
            {

                int startIndex = textFromFile.IndexOf("{\"__type\"");                
                textFromFile = textFromFile.Remove(0, 9 + startIndex);
                
                int endIndex;

                endIndex = textFromFile.IndexOf("{\"__type\"");                    
                    if (endIndex == -1)
                    {
                        endIndex = textFromFile.IndexOf("]}]");
                        endIndex += 1;                        
                    } else
                        endIndex -= 2;

                //MessageBox.Show(textFromFile + "\n" + Convert.ToString(startIndex) + "    " + Convert.ToString(endIndex) + "\n");
                         
                string bufstr = "{\"__type\":" + textFromFile.Substring(startIndex, endIndex);
                //MessageBox.Show(bufstr);
                textFromFile = textFromFile.Remove(0, endIndex + 1);
                startIndex = 0;
                endIndex = -1;
                
                
                string bufFile = "bufFile.json";
                File.WriteAllText(bufFile, bufstr);

                Type type = shapeList.GetType();
                try
                {
                    jsonFormatter = new DataContractJsonSerializer(typeof(Shape), arrList);
                    using (FileStream fs = new FileStream(bufFile, FileMode.OpenOrCreate))
                    {
                        shapeList.Add((Shape)jsonFormatter.ReadObject(fs));
                    }
                }
                catch
                {

                }
                bufstr = String.Empty;
            }

            DrawShapes();
            RefreshFigureList();
        }
     

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isPluginsNull)
            { 
            x2 = e.Location.X;
            y2 = e.Location.Y;
              
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
                    shapeCurr.Draw(this, p, g);

                    shapeList.Add(shapeCurr);
                    RefreshFigureList();
                
            }

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
            if (!isPluginsNull)
            {
                shapeListBuf.Clear();
                currColor = button4.BackColor.ToArgb();
                p.Color = Color.FromArgb(currColor);
                p.Width = trackBar1.Value;
                currPenWidth = trackBar1.Value;

                x1 = e.X;
                y1 = e.Y;

                shapeCurr = plugins[cmbbShapes.SelectedIndex].Load(this);

                shapeCurr.X1 = x1;
                shapeCurr.Y1 = y1;
            }
        }

        private void lbFigures_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedShape = (Shape)lbFigures.SelectedItem;

            if (checkedShape is ISelectable)
            {

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

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (checkedShape is IEditable)
            {
                shapeList[lbFigures.SelectedIndex].X1 = Convert.ToInt32(tbX.Text);
                shapeList[lbFigures.SelectedIndex].Y1 = Convert.ToInt32(tbY.Text);
                shapeList[lbFigures.SelectedIndex].Width = Convert.ToInt32(tbW.Text);
                shapeList[lbFigures.SelectedIndex].Height = Convert.ToInt32(tbH.Text);
                shapeList[lbFigures.SelectedIndex].Color = tbColor.BackColor.ToArgb();
                shapeList[lbFigures.SelectedIndex].PenWidth = Convert.ToInt32(tbPenWidth.Text);
                shapeList[lbFigures.SelectedIndex].Calculate(shapeList[lbFigures.SelectedIndex].X1, shapeList[lbFigures.SelectedIndex].Y1, shapeList[lbFigures.SelectedIndex].Width, shapeList[lbFigures.SelectedIndex].Height);
                DrawShapes();
            }

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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left && !isPluginsNull)
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
                shapeCurr.Draw(this, p, g);               
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
                shape.Draw(this, p, g);
            }
            p.Width = currPenWidth;
            p.Color = Color.FromArgb(currColor);
            GetPictureBox().Image = pic;
        }
    }
}
