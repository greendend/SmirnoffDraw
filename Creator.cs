using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmirnoffDraw
{
    abstract class Creator
    {
        public abstract Shape FactoryMethod(int x1, int y1, int width, int height, int color, int penWidth);
    }
}
