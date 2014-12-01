using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    unsafe class CylindricalTexture
    {
        FastBitmap bmp;
        int* bits;
        int Width{get;set;}
        int Height {get;set;}

        public CylindricalTexture(FastBitmap bmp)
        {
            this.bmp = bmp;
            bits = bmp.lockBits();
            Width = bmp.Width;
            Height = bmp.Height;
        }

        public CylindricalTexture(CylindricalTexture texture)
        {
            texture.bmp.unlockBits();
            this.bmp = new FastBitmap((Image)(new Bitmap(texture.bmp.getBitmap())), texture.bmp.getBitmap().Size);
            texture.bits= texture.bmp.lockBits();

            bits = bmp.lockBits();
            Width = bmp.Width;
            Height = bmp.Height;
        }

        public void finalize()
        {
            bmp.unlockBits();
        }

        public int* getTextureBits() { return bits; }

        public void resizeUV(Point3D[] uvpoints)
        {
            double maxY = uvpoints.Max(elem => elem.Y);
            double minY = uvpoints.Min(elem => elem.Y);
            double maxX = uvpoints.Max(elem => elem.X);
            double minX = uvpoints.Min(elem => elem.X);

            double deltaX = maxX - minX,
                deltaY = maxY - minY;

            double scaleX = (Width-1) / deltaX;
            double scaleY = (Height-1) / deltaY;

            for (int i = 0; i < uvpoints.Length; i++)
            {
                uvpoints[i].X *= scaleX;
                uvpoints[i].Y *= scaleY;
                if (uvpoints[i].Y >= Height) uvpoints[i].Y = Height-1;
                if (uvpoints[i].X >= Width) uvpoints[i].X = Width - 1;

            }
            

        }

        public int getPixel(ref Point3DS uvPoint)
        {
            return bits[Width * (Height - 1 - (int)uvPoint.Y) + (int)uvPoint.X];

        }

    }
}
