using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    unsafe class RasterizationData: ZBufferData
    {
        public int* bits;
        public List<Lightning> lightnings;

        public RasterizationData(Camera camera, int width, int height, double[] zbits, List<Lightning> lightnings, int* bits)
        :base(camera, width, height, zbits)
        {
            this.bits = bits;
            this.lightnings = lightnings;
        }
    }
}
