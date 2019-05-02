using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace SmirnoffDraw
{
    [Serializable]
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

        public virtual void Draw(Form1 form, Pen pen) { }
        public abstract Shape FactoryMethod(int x1, int y1, int width, int height, int color, int penWidth);
    }
}