using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    unsafe class  ZBuffer
    {
        protected double[] zbits;
        protected int frameWidth, frameHeight;

        public ZBuffer(int frameWidth, int frameHeight)
        {
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            zbits = new double[frameHeight * frameWidth];
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private unsafe void clearZBuffer()
        {
            int color = Color.White.ToArgb();
            for (int i = 0; i < frameHeight; i++)
                for (int j = 0; j < frameWidth; j++)
                    zbits[i * frameWidth + j] = 1000000;
        }


        public virtual void update(List<Figure> figures, int yGroups, double ymin, double ymax)
        {
            clearZBuffer();
            ymax++;
            if (ymax > frameHeight) ymax = frameHeight;
            if (ymin < 0) ymin = 0;

            double yGroupWidth = (ymax - ymin + 0.0) / yGroups;

            Parallel.For(0, yGroups, j =>
            {
                for (int i = figures.Count - 1; i >= 0; i--)
                    figures[i].updateOnlyZBufferFrame(Triangle.round(ymin + (j * yGroupWidth)), Triangle.round(ymin + ((j + 1) * yGroupWidth) - 1));
            });
        }

        public int getWidth() { return frameWidth; }
        public int getHeight() { return frameHeight; }
        public double[] getZBits() { return zbits; }

        public ZBufferData getData(Camera camera) { return new ZBufferData(camera, frameWidth, frameHeight, zbits); }
    }
}
