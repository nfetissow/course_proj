using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace Painter
{
    class Canvas
    {
        private PictureBox canvasPictureBox;
        
        public Canvas(PictureBox box)
        {
            canvasPictureBox = box;
            canvasPictureBox.Image = new Bitmap(canvasPictureBox.Width, canvasPictureBox.Height);
        }

        public int Width { get { return canvasPictureBox.Width; } }

        public int Height { get { return canvasPictureBox.Height; } }

        public Graphics getGraphics() { return Graphics.FromImage(canvasPictureBox.Image); }

        public void setBitmap(Bitmap bmp)
        {
            canvasPictureBox.Image = bmp;
        }
    }
}
