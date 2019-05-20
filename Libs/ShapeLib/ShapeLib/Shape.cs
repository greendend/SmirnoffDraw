using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

[DataContract]
public abstract class Shape
{
    [DataMember]
    public int X1 { get; set; }
    [DataMember]
    public int Y1 { get; set; }
    [DataMember]
    public int Width { get; set; }
    [DataMember]
    public int Height { get; set; }
    [DataMember]
    public int PenWidth { get; set; }
    [DataMember]
    public int Color { get; set; }
    [DataMember]
    public List<float[]> pointList = new List<float[]>();
        
        public virtual void Calculate(int x1, int y1, int width, int height) { }
        public virtual void Calculate(Point from, Point to) { }

        public virtual void Draw(Form form, Pen pen, Graphics g) { }
        public abstract Shape FactoryMethod();
    }


