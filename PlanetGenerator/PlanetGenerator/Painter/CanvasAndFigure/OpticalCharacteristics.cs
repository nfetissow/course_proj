using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    public class OpticalCharacteristics
    {
        private double ka, kd, ks, ns;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ka">Коэффициент фоновой интенсивности</param>
        /// <param name="kd">Коэффициент рассеивания</param>
        /// <param name="ks">Коэффициент отражения</param>
        /// <param name="ns">Коэффициент вида отражения</param>
        public OpticalCharacteristics(double ka, double kd, double ks, double ns)
        {
            this.ka = ka;
            this.kd = kd;
            this.ks = ks;
            this.ns = ns;
        }

        public double getKa() { return ka; }
        public double getKd() { return kd; }
        public double getKs() { return ks; }
        public double getNs() { return ns; }
    }
}
