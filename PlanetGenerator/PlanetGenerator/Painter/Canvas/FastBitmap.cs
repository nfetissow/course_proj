using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    class FastBitmap
    {
        private Bitmap bmp;
        private BitmapData bmpData;
        private PixelFormat pxf = PixelFormat.Format32bppArgb;
        private Rectangle rect;
        public static int initializeColor = Color.White.ToArgb();

        public FastBitmap(int width, int height)
        {
            bmp = new Bitmap(width, height);

            rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        }

        public FastBitmap(Image img, Size size)
        {
            bmp = new Bitmap(img, size);
            rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        }

        public int Width { get { return bmp.Width; } }
        public int Height { get { return bmp.Height; } }

        public unsafe int* lockBits()
        {
            bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            int* ptr = (int*)bmpData.Scan0.ToPointer();

            return ptr;
        }

        public void unlockBits()
        {
            bmp.UnlockBits(bmpData);
        }

        public Bitmap getBitmap() { return bmp; }
    }
}
