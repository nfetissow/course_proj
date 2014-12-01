using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    class ZBufferData :TransmittingData
    {
        public Camera camera;
        public int width;
        public int height;
        public double[] zbits;

        public ZBufferData(Camera camera, int width, int height, double[] zbits)
        {
            this.camera = camera;
            this.width = width;
            this.height = height;
            this.zbits = zbits;
        }
    }
}
