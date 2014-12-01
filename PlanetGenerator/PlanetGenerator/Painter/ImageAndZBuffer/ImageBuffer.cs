using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    class ImageBuffer: ZBuffer
    {
        private FastBitmap bmp;

        public ImageBuffer(int frameWidth, int frameHeight)
            : base(frameHeight, frameWidth)
        {
            bmp = new FastBitmap(frameWidth, frameHeight);
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private unsafe void clearBuffers(int* bits)
        {
            int color = Color.White.ToArgb();
            for (int i = 0; i < frameHeight; i++)
                for (int j = 0; j < frameWidth; j++)
                {
                    bits[i * frameWidth + j] = color;
                    zbits[i * frameWidth + j] = 1000000;
                }
        }

        public unsafe void update(List<Figure> figures, List<Lightning> lightnings, Camera camera)
        {
            // предварительные преобразования фигур в координаты камеры
            // и поиск minY and maxY
            camera.calculateFigureTransformatioMatrix();
            bmp = new FastBitmap(frameWidth, frameHeight);
            int* bits = bmp.lockBits();
            clearBuffers(bits);
            RasterizationData data = new RasterizationData(camera, frameWidth, frameHeight, zbits, lightnings, bits);
            ConcurrentQueue<double> ymins = new ConcurrentQueue<double>();
            ConcurrentQueue<double> ymaxs = new ConcurrentQueue<double>();

            Parallel.For(0, figures.Count, i => figures[i].transformForFrame(data, ymaxs, ymins));
            int ymin = Triangle.round(ymins.Min());
            int ymax = Triangle.round(ymaxs.Max());
            //-----------------------------------Растеризация


            ymax++;
            if (ymax > frameHeight) ymax = frameHeight;
            if (ymin < 0) ymin = 0;

            int yGroups = UIData.ThreadCounts;
            double yGroupWidth = (ymax - ymin + 0.0) / yGroups;
            //(figures[0] as ChessBoard).drawBorderOfAllocatedCell(bits, zbits, frameHeight, frameWidth);
            Parallel.For(0, yGroups, j =>
            {
                for (int i = figures.Count - 1; i >= 0; i--)
                    figures[i].updateZBufferAndImageFrame(Triangle.round(ymin + (j * yGroupWidth)), Triangle.round(ymin + ((j + 1) * yGroupWidth) - 1), i);
            });
            bmp.unlockBits();

            // отображение линий потоков
            if (UIData.ShowThreadLines == true)
            {
                Graphics g = Graphics.FromImage(bmp.getBitmap());
                for (int j = 0; j <= yGroups; j++)
                    g.DrawLine(new Pen(Color.Blue), new Point(0, frameHeight - 1 - Triangle.round(ymin + (j * yGroupWidth))), new Point(frameWidth, frameHeight - 1 - Triangle.round(ymin + (j * yGroupWidth))));
            }
        }

        public Bitmap getBitmap() { return bmp.getBitmap(); }
    }
}
