using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmirnoffDraw
{
    class OctaCreator : Creator
    {
        public override Shape FactoryMethod(int x1, int y1, int width, int height, int color, int penWidth)
        {
            return new Octavian(x1, y1, width, height, color, penWidth);
        }
    }
}
