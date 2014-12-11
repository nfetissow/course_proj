using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Painter
{
    public abstract class Lightning :Camera
    {
        public double Amp { get; set; }

        protected Lightning(double amp, Point3D lightSource, Point3D rotateCenter, Point3D turningPoint)
        :base (lightSource, rotateCenter, turningPoint)
        {
            this.Amp = amp;
        }

        //public virtual double calculateIntensivity(Point3D p, Point3D pNormal, OpticalCharacteristics optics, Point3D camera);
        public abstract Point3D getL(Point3D p);
    }
}
