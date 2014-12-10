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
        public DoubleRandom() : base() { selectedValues = new HashSet<int>(); }
        public DoubleRandom(int seed) : base(seed) { selectedValues = new HashSet<int>(); }
        private HashSet<int> selectedValues;
        public double NextDouble(double d1, double d2)
        {
            double q = d1 + (d2 - d1) * this.NextDouble();
            return q;
        }
        
        public int nextExclusive(int minValue, int maxValue)
        {
            return nextExclusive(minValue, maxValue, 0);
        }

        private int nextExclusive(int minValue, int maxValue, int succesiveFailures)
        {
            if(succesiveFailures > 1000)
            {
                //throw new ArgumentException("No values left");
                return Next(minValue, maxValue);
            }
            int q = this.Next(minValue, maxValue);
            if (!selectedValues.Contains(q))
            {
                selectedValues.Add(q);
                return q;
            }
            else
            {
                return nextExclusive(minValue, maxValue, succesiveFailures+1);
            }
        }
    }
}
