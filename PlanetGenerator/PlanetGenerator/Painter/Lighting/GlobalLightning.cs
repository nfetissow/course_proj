using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Painter
{
    class GlobalLightning :Lightning
    {

        public GlobalLightning(Point3D lightSource, Point3D rotateCenter, Point3D turningPoint, double amp)
            : base(amp, lightSource, rotateCenter, turningPoint)
        {
        }

        public override Point3D getL(Point3D p)
        {
            return rotateCenter - center;
        }

        public virtual void trasnformPointAfterProjection(Point3D point)
        {
            point.X += rotateCenter.X;
            point.Y += rotateCenter.Y;
            point.Z *= 100;
        }

        public virtual void reverseTrasnformPointAfterDrawing(Point3D point)
        {
            point.Z /= 100;
            point.X-= rotateCenter.X;
            point.Y -= rotateCenter.Y;
        }
//         public override double calculateIntensivity(Point3D p, Point3D pNormal, OpticalCharacteristics optics, Point3D camera)
//         {
//             double cosTeta = Math.Cos(pNormal, lightDirection);
//             Point3D R = lightDirection + 2*cosTeta*pNormal;
//             Point3D V = camera - p;
//             double res = optics.getKa() + amp*(optics.getKd()*cosTeta + optics.getKs()*Math.Pow(Math.Cos(R, V), optics.getNs());
//         }

    }
}
