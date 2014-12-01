using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    struct Point3DS
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double D { get; set; }

        public bool Equals(Point3D obj)
        {
            return (Math.Abs(X - obj.X) < Triangle.Eps && Math.Abs(Y - obj.Y) < Triangle.Eps && Math.Abs(Z - obj.Z)<Triangle.Eps);
        }

        public override int GetHashCode()
        {
            return (int)(X * 1000 + Y * 100 + Z * 10);
        }

        public override string ToString()
        {
            return X + " " + Y + " " + Z;
        }

        public Point3DS(double x, double y, double z, double k)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
            D = k;
        }

        public static Point3DS getEmpty()
        {
            return new Point3DS(0, 0, 0, 1);
        }

        public Point3DS(double x, double y, double z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
            D = 1;
        }

        public Point3DS(Point3D p):this()
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
            D = 1;
        }

        public Point3DS(Point3DS p)
            : this()
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
            D = 1;
        }

        public static Point3DS operator + (Point3DS p1, Point3DS p2)
        {
            Point3DS newP = new Point3DS(p1);
            newP.X += p2.X;
            newP.Y += p2.Y;
            newP.Z += p2.Z;
            return newP;
        }

        public static Point3DS operator -(Point3DS p1, Point3DS p2)
        {
            Point3DS newP = new Point3DS(p1);
            newP.X -= p2.X;
            newP.Y -= p2.Y;
            newP.Z -= p2.Z;
            return newP;
        }

        public static Point3DS operator -(Point3DS p1, double d)
        {
            Point3DS newP = new Point3DS(p1);
            newP.X -= d;
            newP.Y -= d;
            newP.Z -= d;
            return newP;
        }

        public static Point3DS operator +(Point3DS p1, double d)
        {
            Point3DS newP = new Point3DS(p1);
            newP.X += d;
            newP.Y += d;
            newP.Z += d;
            return newP;
        }

        public static Point3DS operator *(Point3DS p1, double m)
        {
            Point3DS newP = new Point3DS(p1);
            newP.X *= m;
            newP.Y *= m;
            newP.Z *= m;
            return newP;
        }

        public static Point3DS operator /(Point3DS p1, double m)
        {
            Point3DS newP = new Point3DS(p1);
            newP.X /= m;
            newP.Y /= m;
            newP.Z /= m;
            return newP;
        }

        public static double operator *(Point3DS p1, Point3DS p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
        }

        public double getLength()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Point3DS normalize()
        {
            double len = getLength();
            this /= len;
            return this;
        }

        public void normalizeXY()
        {
            double len = Math.Sqrt(X * X + Y * Y);
            this /= len;
        }

        public Point3DS toInt()
        {
            return new Point3DS(Triangle.round(X), Triangle.round(Y), Z);
        }

        public Point3DS toIntAll()
        {
            return new Point3DS(Triangle.round(X), Triangle.round(Y), Triangle.round(Z));
        }

        public Point3DS toTruncateInt()
        {
            return new Point3DS((int)(X), (int)(Y), Z);
        }
    }
}
