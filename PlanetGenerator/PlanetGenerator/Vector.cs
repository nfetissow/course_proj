using System;
using System.Collections.Generic;
using System.Text;

namespace PlanetGenerator
{
    /// <summary>
    /// a 3D vector class, used to calculate some basic trigonomitry
    /// </summary>
    public class Vector
    {
        public double x;
        public double y;
        public double z;

        static public Vector Null;
        static public Vector Infinate;



        static Vector()
        {
            Null = new Vector(0, 0, 0);
            Infinate = new Vector(double.MaxValue, double.MaxValue, double.MaxValue);
        }

        public Vector()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="v"></param>
        public Vector(Vector v) : this(v.x, v.y, v.z)
        {
        }

        
        public static Vector Lerp(Vector v1, Vector v2, double t)
        {
            return (v1 + (v2 - v1) * t);
        }

        public Vector Normalize()
        {
            double t = (double) this.Magnitude();
            return new Vector(x / t, y / t, z / t);
        }

        static public Vector operator +(Vector v, Vector w)
        {
            return new Vector(w.x + v.x, w.y + v.y, w.z + v.z);
        }

        static public Vector operator -(Vector v, Vector w)
        {
            return new Vector(v.x - w.x, v.y - w.y, v.z - w.z);
        }

        static public Vector operator *(Vector v, Vector w)
        {
            return new Vector(v.x * w.x, v.y * w.y, v.z * w.z);
        }

        static public Vector operator *(Vector v, double f)
        {
            return new Vector(v.x * f, v.y * f, v.z * f);
        }

        static public Vector operator /(Vector v, double f)
        {
            return new Vector(v.x / f, v.y / f, v.z / f);
        }

        public double Dot(Vector w)
        {
            return this.x*w.x + this.y*w.y + this.z * w.z;
        }
        
        public Vector Clone()
        {
            return new Vector(this);
        }

        public Vector Cross(Vector w)
        {
            return new Vector(-this.z * w.y + this.y * w.z,
                               this.z * w.x - this.x * w.z, 
                              -this.y * w.x + this.x * w.y);
        }

        public double Magnitude()
        {
            return Math.Sqrt((double)((x * x) + (y * y) + (z * z)));
        }


        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", this.x, this.y, this.z);
        }

        public void Rotate(double angle) 
        {
            double x, y;
            x = this.x*Math.Cos(angle)+this.y*Math.Sin(angle);
            y = this.y*Math.Cos(angle)+this.x*Math.Sin(angle);
            this.x = x;
            this.y = y;
        }
        public Vector projectOnVector(Vector vector)
        {
            return vector.Normalize() * vector.Normalize().Dot(this);
        }
    }
}
