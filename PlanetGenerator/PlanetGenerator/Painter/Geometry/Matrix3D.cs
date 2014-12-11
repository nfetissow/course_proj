using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Painter
{
    public class Matrix3D
    {
        double[,] m = new double[4, 4];

        public Matrix3D()
        {
            for (int i=0; i<4; i++)
            {
                for (int j = 0; j < 4; j++)
                    m[i, j] = 0;
                m[i, i] = 1;
            }
        }

        public Matrix3D(Matrix3D matr)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    m[i, j] = matr.m[i, j];
        }
        
        public static Matrix3D getRotationZMatrix(double angle)
        {
            Matrix3D matr = new Matrix3D();
            angle *= Math.PI / 180; 
            matr.m[0, 0] = Math.Cos(angle);
            matr.m[0, 1] = Math.Sin(angle);
            matr.m[1, 0] = -Math.Sin(angle);
            matr.m[1, 1] = Math.Cos(angle);
            return matr;
        }

        public static Matrix3D getRotationYMatrix(double angle)
        {
            Matrix3D matr = new Matrix3D();
            angle *= Math.PI / 180;
            matr.m[0, 0] = Math.Cos(angle);
            matr.m[0, 2] = -Math.Sin(angle);
            matr.m[2, 0] = Math.Sin(angle);
            matr.m[2, 2] = Math.Cos(angle);
            return matr;
        }

        public static Matrix3D getRotationXMatrix(double angle)
        {
            Matrix3D matr = new Matrix3D();
            angle *= Math.PI / 180;
            matr.m[1, 1] = Math.Cos(angle);
            matr.m[1, 2] = Math.Sin(angle);
            matr.m[2, 1] = -Math.Sin(angle);
            matr.m[2, 2] = Math.Cos(angle);
            return matr;
        }

        public static Matrix3D getOffsetMatrix(double x, double y, double z)
        {
            Matrix3D matr = new Matrix3D();
            matr.m[3, 0] = x;
            matr.m[3, 1] = y;
            matr.m[3, 2] = z;
            return matr;
        }

        public static Matrix3D getScaleMatrix(double koef)
        {
            Matrix3D matr = new Matrix3D();
            matr.m[0, 0] = koef;
            matr.m[1, 1] = koef;
            matr.m[2, 2] = koef;
            return matr;
        }

        public static Matrix3D getScaleMatrix(double koef1, double koef2, double koef3)
        {
            Matrix3D matr = new Matrix3D();
            matr.m[0, 0] = koef1;
            matr.m[1, 1] = koef2;
            matr.m[2, 2] = koef3;
            return matr;
        }


        public static Matrix3D getRotationOnSimpleAxisMatrix(double angle, Point3D axis)
        {
            Matrix3D matr = new Matrix3D();
            angle *= Math.PI / 180;
            matr.m[0, 0] = Math.Cos(angle) + (1 - Math.Cos(angle)) * axis.X * axis.X;
            matr.m[1, 1] = Math.Cos(angle) + (1 - Math.Cos(angle)) * axis.Y * axis.Y;
            matr.m[2, 2] = Math.Cos(angle) + (1 - Math.Cos(angle)) * axis.Z * axis.Z;
            
            matr.m[1, 0] = (1 - Math.Cos(angle)) * axis.Y * axis.X - Math.Sin(angle) * axis.Z;
            matr.m[2, 0] = (1 - Math.Cos(angle)) * axis.Z * axis.X + Math.Sin(angle) * axis.Y;
            matr.m[2, 1] = (1 - Math.Cos(angle)) * axis.Z * axis.Y - Math.Sin(angle) * axis.X;

            matr.m[0, 1] = (1 - Math.Cos(angle)) * axis.Y * axis.X + Math.Sin(angle) * axis.Z;
            matr.m[0, 2] = (1 - Math.Cos(angle)) * axis.Z * axis.X - Math.Sin(angle) * axis.Y;
            matr.m[1, 2] = (1 - Math.Cos(angle)) * axis.Z * axis.Y + Math.Sin(angle) * axis.X;

            return matr;
        }

        public static Matrix3D operator *(Matrix3D m1, Matrix3D m2)
        {
            Matrix3D res = new Matrix3D();

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    res.m[i, j] = 0;
                    for (int k = 0; k < 4; k++)
                        res.m[i, j] += m1.m[i, k] * m2.m[k, j];
                }
            return res;
        }

        public static Point3D operator *(Point3D p, Matrix3D matr)
        {
            Point3D res = new Point3D();
            double[] resCor = res.coordinates, cor = p.coordinates;
            for (int i=0; i<4; i++)
                for (int j=0; j<4; j++)
                    resCor[i] += matr.m[j,i] * cor[j];
            resCor[3] = 1;
            return res;
        }

        public double determinant2X2(int i1, int i2, int j1, int j2)
        {
            int p1 = -1, p2 = -1, p3 = -1, p4 = -1;
            for (int i = 0; i < 4; i++)
                if (i != i1 && i != i2)
                    if (p1 == -1)
                        p1 = i;
                    else
                        p2 = i;
            for (int j = 0; j < 4; j++)
                if (j != j1 && j != j2)
                    if (p3 == -1)
                        p3 = j;
                    else
                        p4 = j;

            double res = m[p1, p3] * m[p2, p4] - m[p2, p3] * m[p1, p4];
            return res;
        }

        public double determinant3X3(int i1, int j1)
        {
            double res = 0;
            int sign = 1;
            for (int i = 0; i < 4; i++)
                if (i != i1)
                {
                    for (int j = 0; j < 4; j++)
                        if (j != j1)
                        {
                            res += determinant2X2(i1, i, j1, j) * m[i, j] * sign;
                            sign *= -1;
                        }
                    break;
                }
            if ((i1 + j1) % 2 != 0)
                res *= -1;
            return res;
        }

        public double determinantOfMatrix()
        {
            double res = 0;
            res += determinant3X3(0, 0) * m[0, 0];
            res += determinant3X3(0, 1) * m[0, 1];
            res += determinant3X3(0, 2) * m[0, 2];
            res += determinant3X3(0, 3) * m[0, 3];
            return res;
        }

        public Matrix3D getInverstionMatrix()
        {
            Matrix3D buf = new Matrix3D(this);
            double det = determinantOfMatrix();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    buf.m[j, i] = determinant3X3(i, j) / det;
            return buf;
        }

        public static Matrix3D getBasisMatrix(Point3D p1, Point3D p2, Point3D p3)
        {
            Matrix3D matr = new Matrix3D();
            matr.m[0, 0] = p1.X;
            matr.m[1, 0] = p1.Y;
            matr.m[2, 0] = p1.Z;
            matr.m[0, 1] = p2.X;
            matr.m[1, 1] = p2.Y;
            matr.m[2, 1] = p2.Z;
            matr.m[0, 2] = p3.X;
            matr.m[1, 2] = p3.Y;
            matr.m[2, 2] = p3.Z;
            return matr;
        }

        // тр
        public static Point3D operator *(Matrix3D matr, Point3D point)
        {
            Point3D res = new Point3D();
            double[] resCor = res.coordinates, cor = point.coordinates;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    resCor[i] += matr.m[i, j] * cor[j];
            for (int i = 0; i < 4; i++)
                cor[i] = resCor[i] / resCor[3];
            return res;
        }

        public void transformPoint(Point3D point)
        {
            Point3D res = new Point3D();
            double[] resCor = res.coordinates, cor = point.coordinates;
            resCor[3] = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    resCor[i] += m[j, i] * cor[j];
            for (int i = 0; i < 3; i++)
                cor[i] = resCor[i] / resCor[3];
            cor[3] = 1;

        }

        public void transformPlane(Plane3D plane)
        {
            Point3D res = new Point3D();
            double[] resCor = res.coordinates, cor = plane.coordinates;
            resCor[3] = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    resCor[i] += m[j, i] * cor[j];
            for (int i = 0; i < 4; i++)
                cor[i] = resCor[i];
        }

        public static Matrix3D getCentralProjectionMatrixV2(double fov, double aspect, double zn, double zf)
        {
            Matrix3D matr = new Matrix3D(); 
            matr.m[3, 3] = 0;
            matr.m[0, 0] = 1 / Math.Tan(fov * Math.PI / 360) * zn;
            matr.m[1, 1] = matr.m[0, 0] / aspect;
            matr.m[2, 2] = zf / (zf - zn);
            matr.m[2, 3] = 1;
            matr.m[3, 2] = - zn * zf / (zf - zn);
            return matr;
        }

        public static Matrix3D getCentralProjectionMatrixV3(double fov, double aspect, double zn, double zf)
        {
            Matrix3D matr = new Matrix3D();
            matr.m[3, 3] = 0;
            matr.m[0, 0] = 1 / Math.Tan(fov * Math.PI / 360)*zn;
            matr.m[1, 1] = matr.m[0, 0] / aspect;
            matr.m[2, 2] = 1;
            matr.m[2, 3] = 1;
            return matr;
        }

        public static Matrix3D getPreProjectionMatrix(double fov, double aspect, double zn)
        {
            Matrix3D matr = new Matrix3D();
            matr.m[0, 0] = 1 / Math.Tan(fov * Math.PI / 360) * zn;
            matr.m[1, 1] = matr.m[0, 0] / aspect;
            return matr;
        }

        public static Matrix3D getCentralProjectionMatrixV1(double d)
        {
            Matrix3D matr = new Matrix3D();
            matr.m[2, 3] = 1 / d;
            matr.m[3, 3] = 0;
            return matr;
        }


    }
}
