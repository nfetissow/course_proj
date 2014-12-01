using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace STLParserProject
{
    public class Point3D : IEquatable<Point3D>
    {
        public double[] coordinates = new  double[4];

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)] 
        public bool Equals(Point3D obj)
        {
            return (Math.Abs(X - obj.X) < Triangle.Eps && Math.Abs(Y - obj.Y) < Triangle.Eps && Math.Abs(Z - obj.Z)<Triangle.Eps);
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)] 
        public override int GetHashCode()
        {
            return (int)(X * 1000 + Y * 100 + Z * 10);
        }

        public override string ToString()
        {
            return X + " " + Y + " " + Z;
        }

        public Point3D(double x, double y, double z, double k)
        {
            coordinates[0] = x;
            coordinates[1] = y;
            coordinates[2] = z;
            coordinates[3] = k;
        }

        public Point3D()
        {
            coordinates[0] = 0;
            coordinates[1] = 0;
            coordinates[2] = 0;
            coordinates[3] = 1;
        }

        public Point3D(double x, double y, double z)
        {
            coordinates[0] = x;
            coordinates[1] = y;
            coordinates[2] = z;
            coordinates[3] = 1;
        }

        public Point3D(Point3D p)
        {
            coordinates[0] = p.coordinates[0];
            coordinates[1] = p.coordinates[1];
            coordinates[2] = p.coordinates[2];
            coordinates[3] = 1;
        }

        public double X
        {
            get { return coordinates[0]; }
            set { coordinates[0] = value; }
        }

        public double Y
        {
            get { return coordinates[1]; }
            set { coordinates[1] = value; }
        }

        public double Z
        {
            get { return coordinates[2]; }
            set { coordinates[2] = value; }
        }

        public double D
        {
            get { return coordinates[3]; }
            set { coordinates[3] = value; }
        }

        public Point toPoint()
        {
            return new Point((int)Math.Round(X), (int)Math.Round(Y));
        }

        public Point toPoint(int height)
        {
            return new Point((int)X, height - (int)Y);
        }

        public static Point3D operator + (Point3D p1, Point3D p2)
        {
            Point3D newP = new Point3D(p1);
            newP.coordinates[0] += p2.coordinates[0];
            newP.coordinates[1] += p2.coordinates[1];
            newP.coordinates[2] += p2.coordinates[2];
            return newP;
        }

        public static Point3D operator -(Point3D p1, Point3D p2)
        {
            Point3D newP = new Point3D(p1);
            newP.coordinates[0] -= p2.coordinates[0];
            newP.coordinates[1] -= p2.coordinates[1];
            newP.coordinates[2] -= p2.coordinates[2];
            return newP;
        }

        public static Point3D operator -(Point3D p1, double d)
        {
            Point3D newP = new Point3D(p1);
            newP.coordinates[0] -= d;
            newP.coordinates[1] -= d;
            newP.coordinates[2] -= d;
            return newP;
        }

        public static Point3D operator +(Point3D p1, double d)
        {
            Point3D newP = new Point3D(p1);
            newP.coordinates[0] += d;
            newP.coordinates[1] += d;
            newP.coordinates[2] += d;
            return newP;
        }

        public static Point3D operator *(Point3D p1, double m)
        {
            Point3D newP = new Point3D(p1);
            newP.coordinates[0] *= m;
            newP.coordinates[1] *= m;
            newP.coordinates[2] *= m;
            return newP;
        }

        public static Point3D operator /(Point3D p1, double m)
        {
            Point3D newP = new Point3D(p1);
            newP.coordinates[0] /= m;
            newP.coordinates[1] /= m;
            newP.coordinates[2] /= m;
            return newP;
        }

        public static double operator * (Point3D p1, Point3D p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
        }

        public double getLength()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public static double angleBetween2Points(Point3D p1, Point3D p2)
        {
            double l1 = p1.getLength(), l2 = p2.getLength();
            if (l1 != 0 && l2 != 0)
                return Math.Acos((p1 * p2) / (l1 * l2)) / Math.PI * 180.0; 
            return 0;
        }

        public static double vectorMultiplicationMarkXY(Point3D p1, Point3D p2)
        {
            return (p1.X * p2.Y - p1.Y * p2.X);
        }

        public static double vectorMultiplicationMarkXZ(Point3D p1, Point3D p2)
        {
            return (p1.Z * p2.X - p1.X * p2.Z);
        }

        public static double vectorMultiplicationMarkZY(Point3D p1, Point3D p2)
        {
            return (p1.Y * p2.Z - p1.Z * p2.Y);
        }

        public static Point3D vectorMultiplication(Point3D a, Point3D b)
        {
            Point3D resVector = new Point3D();
            resVector.X = a.Y * b.Z - a.Z * b.Y;
            resVector.Y = a.Z * b.X - a.X * b.Z;
            resVector.Z = a.X * b.Y - a.Y * b.X;
            return resVector;
        }

        public Point3D normalize()
        {
            double len = getLength();
            for (int i = 0; i < 3; i++)
                coordinates[i] /= len;
            return this;
        }

        public void normalizeXY()
        {
            double len = Math.Sqrt(X * X + Y * Y);
            for (int i = 0; i < 3; i++)
                coordinates[i] /= len;
        }

        public Point3D toInt()
        {
            return new Point3D(Triangle.round(X), Triangle.round(Y), Z);
        }

        public Point3D toIntAll()
        {
            return new Point3D(Triangle.round(X), Triangle.round(Y), Triangle.round(Z));
        }

        public Point3D toTruncateInt()
        {
            return new Point3D((int)(X), (int)(Y), Z);
        }
    }
}
