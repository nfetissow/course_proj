using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Painter
{
    class RGBColor
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        private int a;
        public int A { get {return a; } set { a = value > 0 ? (value < 255 ? value : 255) : 0; } }
        public RGBColor(int color)
        {
            R = ((color & 0x00FF0000) >> 16);
            G = ((color & 0x0000FF00) >> 8);
            B = (color & 0x000000FF);
            A = (int)((color & 0xFF000000) >> 24);
        }

        public int toInt()
        {
            A = (int)(A * 0.40);

            A = 255 - A;
            return (int)(A << 24 | R << 16 | G << 8 | B);
        }

        public static RGBColor blendColors(Point3D f, RGBColor c1, RGBColor c2, RGBColor c3)//cx, cy, cz
        {
            //RGBColor res
            int r =(int)(f.X * c1.R + f.Y * c2.R + f.Z * c3.R);
            int g = (int)(f.X * c1.G + f.Y * c2.G + f.Z * c3.G);
            int b = (int)(f.X * c1.B + f.Y * c2.B + f.Z * c3.B);
            return new RGBColor((int)(0xFF000000 | r << 16 | g << 8 | b));
        }

        public static RGBColor blendColors(Point3D f, RGBColor c1, RGBColor c2)//cx, cy, cz
        {
            //RGBColor res
            int r = (int)(f.X * c1.R + f.Y * c2.R);
            int g = (int)(f.X * c1.G + f.Y * c2.G);
            int b = (int)(f.X * c1.B + f.Y * c2.B);
            return new RGBColor((int)(0xFF000000 | r << 16 | g << 8 | b));
        }


    }
}
