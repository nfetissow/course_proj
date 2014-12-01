using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    public class LocalLightning: Lightning 
    {
        private Point3D lightSource;

        public LocalLightning(Point3D lightSource, Point3D rotateCenter, Point3D turningPoint ,double amp)
            :base(amp, lightSource, rotateCenter, turningPoint)
        {
            this.lightSource = lightSource;
        }

        public override Point3D getL(Point3D p)
        {
            return lightSource - p;
        }
    }
}
