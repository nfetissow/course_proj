using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Painter
{
    public class Plane3D:Point3D
    {
        public double A { get { return coordinates[0]; } set { coordinates[0] = value; } }
        public double B { get { return coordinates[1]; } set { coordinates[1] = value; } }
        public double C { get { return coordinates[2]; } set { coordinates[2] = value; } }
        public double D { get { return coordinates[3]; } set { coordinates[3] = value; } }
        public double dzx {get;set;}
        public double dzy {get;set;}


        public Plane3D(double a, double b, double c, Point3D p) : base(a, b, c) 
        {
            D = -(p.X * a + p.Y * b + p.Z * c);
        }

        public Plane3D(Point3D p1, Point3D p2, Point3D p3)
        {
            recalculation(p1, p2, p3);
        }

        public Plane3D(Plane3D p)
        {
            this.A = p.A;
            this.B = p.B;
            this.C = p.C;
            this.D = p.D;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public void recalculation(Point3D p1, Point3D p2, Point3D p3)
        {
            A = (p3.Y - p1.Y) * (p2.Z - p1.Z) -
                (p2.Y - p1.Y) * (p3.Z - p1.Z);
            B = -(p3.X - p1.X) * (p2.Z - p1.Z) +
                (p2.X - p1.X) * (p3.Z - p1.Z);
            C = (p3.X - p1.X) * (p2.Y - p1.Y) -
                (p2.X - p1.X) * (p3.Y - p1.Y);
            D = -p1.X * A - p1.Y * B - p1.Z * C;
            dzx = -A / C;
            dzy = -B / C;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public double getZ(double x, double y)
        {
            return ((A * x + B * y + D) / -C);
        }

        public static Plane3D operator* (Plane3D curPlane, double param)
        {
            Plane3D res = new Plane3D(curPlane);
            res.A *= param;
            res.B *= param;
            res.C *= param;
            res.D *= param;
            return res;
        }

        public void recalculationD(Point3D p)
        {
            D = -(p.X * A + p.Y * B + p.Z * C);
        }

        public static double ScalarMultiple(Plane3D plane, Point3D p)
        {
            return (plane.A * p.X + plane.B * p.Y + plane.C * p.Z + plane.D);
        }

        public Point3D getIntersectionPoint(Point3D p1, Point3D p2)
        {
            double m = p2.X - p1.X,
                n = p2.Y - p1.Y,
                p = p2.Z - p1.Z;
            double buf = (A*m + B*n + C*p);
		    double t = 0;
            if (buf != 0)
            {
                t = -(A * p1.X + B * p1.Y + C * p1.Z + D) / buf;
                Point3D res = new Point3D(p1.X + m * t, p1.Y + n * t, p1.Z + p * t);
                return res;
            }
            return null;
        }


    }
}
