using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetGenerator
{
    //Classic Random class does not contain method to generate Random double value within specified range
    class DoubleRandom: Random
    {
        public DoubleRandom() : base() { }
        public DoubleRandom(int seed) : base(seed) { }

        public double NextDouble(double d1, double d2)
        {
            return d1 + (d2 - d1) * this.NextDouble();
        }
        public double Unit()
        {
            return (double) this.Next() / 0x80000000;
        }
    }
}
