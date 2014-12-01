using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    unsafe class TriplanarTexture
    {
        private FastBitmap bmpX, bmpY, bmpZ;
        private Size sX, sY, sZ, s;
        private int* bitsX, bitsY, bitsZ;
        Figure fig;
        private int xWidth, yWidth, zWidth;

        public TriplanarTexture(Image img, Figure fig)
        {
            this.fig = fig;
            xWidth = Triangle.round(fig.getXWidth()+1);
            yWidth = Triangle.round(fig.getYWidth()+1);
            zWidth = Triangle.round(fig.getZWidth()+1);
            sX = new Size(yWidth, zWidth);
            sY = new Size(xWidth, zWidth);
            sZ = new Size(xWidth, yWidth);
            s = img.Size;
//             if (sX.Height == 0) sX.Height = 1; if (sX.Width == 0) sX.Width = 1;
//             if (sY.Height == 0) sY.Height = 1; if (sY.Width == 0) sY.Width = 1;
//             if (sZ.Height == 0) sZ.Height = 1; if (sZ.Width == 0) sZ.Width = 1;
//             
            bmpZ = new FastBitmap(img, sZ);
            bmpY = new FastBitmap(img, sY);

            img.RotateFlip(RotateFlipType.RotateNoneFlipX);

            bmpX = new FastBitmap(img, sX);            



            bitsX = bmpX.lockBits();
            bitsY = bmpY.lockBits();
            bitsZ = bmpZ.lockBits();
        }

        ~TriplanarTexture()
        {
            bmpX.unlockBits();
            bmpY.unlockBits();
            bmpZ.unlockBits();
        }

        public RGBColor getTexturePixel(Point3D worldPoint, Point3D normal)
        {
            RGBColor cx, cy, cz;
            Point3D newWP = new Point3D(worldPoint), absnormal;
            newWP -= fig.middlePoint;
            newWP = newWP.toIntAll();
            double sum;

//             if (fig as ChessBoard == null)
//             {
//                 absnormal = new Point3D(Math.Abs(normal.X), Math.Abs(normal.Y), Math.Abs(normal.Z));
//                 absnormal.normalizeXY();
//                 sum = absnormal.X + absnormal.Y;
//                 absnormal /= sum;
//                 if (newWP.Z >= sX.Height) newWP.Z = sX.Height - 1;
//                 if (newWP.Z >= sY.Height) newWP.Z = sY.Height - 1;
//                 if (newWP.Y >= sZ.Height) newWP.Y = sZ.Height - 1;
//                 if (newWP.Y >= sX.Width) newWP.Y = sX.Width - 1;
//                 if (newWP.X >= sY.Width) newWP.X = sX.Width - 1;
//                 if (newWP.X >= sZ.Width) newWP.X = sX.Width - 1;
// 
//                 //             RGBColor cx = new RGBColor(bitsX[(int)(s.Width * (s.Height -1- newWP.Z) + newWP.Y)]);
//                 //             RGBColor cy = new RGBColor(bitsY[(int)(s.Width * (s.Height -1- newWP.Z) + newWP.X)]);
//                 //             RGBColor cz = new RGBColor(bitsZ[(int)(s.Width * (s.Height -1- newWP.Y) + newWP.X)]);
//                 cx = new RGBColor(bitsX[(int)(sX.Width * (sX.Height - 1 - newWP.Z) + newWP.Y)]);
//                 cy = new RGBColor(bitsY[(int)(sY.Width * (sY.Height - 1 - newWP.Z) + newWP.X)]);
// //                 RGBColor cz = new RGBColor(bitsZ[(int)(sZ.Width * (sZ.Height - 1 - newWP.Y) + newWP.X)]);
//                 return RGBColor.blendColors(absnormal, cx, cy);
//             }

//             newWP *= ts;/

            absnormal = new Point3D(Math.Abs(normal.X), Math.Abs(normal.Y), Math.Abs(normal.Z));
            absnormal = absnormal.normalize();
            sum = absnormal.X + absnormal.Y + absnormal.Z;
            absnormal /= sum;

            if (newWP.Z >= sX.Height) newWP.Z = sX.Height-1;
            if (newWP.Z >= sY.Height) newWP.Z = sY.Height - 1;
            if (newWP.Y >= sZ.Height) newWP.Y = sZ.Height - 1;
            if (newWP.Y >= sX.Width) newWP.Y = sX.Width - 1;
            if (newWP.X >= sY.Width) newWP.X = sX.Width - 1;
            if (newWP.X >= sZ.Width) newWP.X = sX.Width - 1; 

//             RGBColor cx = new RGBColor(bitsX[(int)(s.Width * (s.Height -1- newWP.Z) + newWP.Y)]);
//             RGBColor cy = new RGBColor(bitsY[(int)(s.Width * (s.Height -1- newWP.Z) + newWP.X)]);
//             RGBColor cz = new RGBColor(bitsZ[(int)(s.Width * (s.Height -1- newWP.Y) + newWP.X)]);
            cx = new RGBColor(bitsX[(int)(sX.Width * (sX.Height -1- newWP.Z) + newWP.Y)]);
            cy = new RGBColor(bitsY[(int)(sY.Width * (sY.Height -1- newWP.Z) + newWP.X)]);
            cz = new RGBColor(bitsZ[(int)(sZ.Width * (sZ.Height -1- newWP.Y) + newWP.X)]);


//             bitsX[worldPoint.X
            return RGBColor.blendColors(absnormal, cx, cy, cz);
        }
        int ts = 1;//textureScale
    }
}
