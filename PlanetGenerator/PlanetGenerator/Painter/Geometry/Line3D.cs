using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Painter
{
    class Line3D
    {
        private double m, n, p;
        private Point3D p1;

        public Line3D(Point3D p1, Point3D p2)
        {
            m = p2.X - p1.X;
            n = p2.Y - p1.Y;
            p = p2.Z - p1.Z;
            this.p1 = p1;
        }

        public static unsafe void drawLine2D(Point3D p1, Point3D p2, int* bits, double[] zbits, int width, int height, int color)
        {
            double dx = (p2.X - p1.X), dy = (p2.Y - p1.Y);
            double zt = p1.Z-1;
            double dz = (p2.Z - p1.Z);
            double dzx = (dx != 0) ? (p2.Z - p1.Z) / dx : 0;
            double dzy = (dy != 0) ? (p2.Z - p1.Z) / dy : 0;

            int l;
            double xt = p1.X, yt = p1.Y;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                l = (int)Math.Abs(dx);
            }
            else
            {
                l = (int)Math.Abs(dy);
            }
            dx /= l;
            dy /= l;
            dz /= l;
            for (int i = 0; i < l + 1; i++)
            {
                if (Triangle.round(xt) < width && Triangle.round(xt) >= 0 && Triangle.round(yt) >= 0 && Triangle.round(yt) < height)
                {
                    bits[Triangle.round(yt) * width + Triangle.round(xt)] = color;
                    zbits[Triangle.round(yt) * width + Triangle.round(xt)] = zt;
                }
                if (Triangle.round(xt) + 1 < width && Triangle.round(xt) + 1 >= 0 && Triangle.round(yt) >= 0 && Triangle.round(yt) < height)
                {
                    bits[Triangle.round(yt) * width + Triangle.round(xt)+1] = color;
                    zbits[Triangle.round(yt) * width + Triangle.round(xt)+1] = zt+dzx;
                }
                if (Triangle.round(xt) - 1 < width && Triangle.round(xt) - 1 >= 0 && Triangle.round(yt) >= 0 && Triangle.round(yt) < height)
                {
                    bits[Triangle.round(yt) * width + Triangle.round(xt)-1] = color;
                    zbits[Triangle.round(yt) * width + Triangle.round(xt)-1] = zt-dzx;
                }
                xt += dx;
                yt += dy;
                zt += dz;
            }
        }
    }
}
