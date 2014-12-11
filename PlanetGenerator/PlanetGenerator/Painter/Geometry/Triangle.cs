using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
namespace Painter
{

    enum TRIANGLE_VISIBILITY { FULL, PARTLY, NO }

    unsafe class Triangle : IEquatable<Triangle>
    {
        private Point3D[] verts;//transformed coordinates
        private int id1, id2, id3, tid1, tid2, tid3;
        private Plane3D plane = null;
        private double[] intensivities;
        private Point3D[] normals;
        private Point3D[] realVerts;//real coordinates
        public static double Eps = 1E-3;
        public static double Eps2 = 5;

        public Point3D p1, p2, p3;
        private double dx21, dx31, dx32, dzx, dzy, _dx31, _dx32;
        private double i1, i2, i3, di21, di31, di32, _di31, _di32;
        private double z1, z2, z3, dz21, dz31, dz32, _dz31, _dz32;
        private HSLColor color;
        private int intColor;
        private double[] zbits;
        private int height;
        private int width;
        private int* bits;
        private Camera camera;
        private List<Shadow> shadows;
        private List<int> colorsForShadow;
        int colorForShadow;
        Plane3D shadowPlane;

        public Triangle(Point3D[] verts, int id1, int id2, int id3, int color)
        {
            this.verts = verts;
            this.id1 = id1;
            this.id2 = id2;
            this.id3 = id3;
            this.intColor = color;
            p1 = verts[id1].toInt();
            p2 = verts[id2].toInt();
            p3 = verts[id3].toInt();
        }

        public Triangle(Triangle obj)
        {
            this.verts = obj.verts;
            this.id1 = obj.id1;
            this.id2 = obj.id2;
            this.id3 = obj.id3;
            this.color = obj.color;
        }

        public bool containVert(Point3D vert)
        {
            return vert.Equals(verts[id1]) || vert.Equals(verts[id2]) || vert.Equals(verts[id3]);
        }

        // поиск D. плоскость видима если скалярное произведение уравнения плоскости (a,b,c,d) на точку камеры 
        // а она находится в начале координат будет (0,0,0,1). т.е. проще говоря d must be greater than zero
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public TRIANGLE_VISIBILITY getVisibility()
        {
            Point3D p1 = verts[id1], p2 = verts[id2], p3 = verts[id3];

            if ((plane * new Point3D(0, 0, 1)) > -Eps)
                if (p1.X >= 0 && p2.X >= 0 && p3.X >= 0 && p1.Y >= 0 && p2.Y >= 0 && p3.Y >= 0 &&
                p1.X < width && p2.X < width && p3.X < width && p1.Y < height && p2.Y < height && p3.Y < height)
                    return TRIANGLE_VISIBILITY.FULL;
                else
                    if ((p1.X < 0 && p2.X < 0 && p3.X < 0) ||
                         (p1.Y < 0 && p2.Y < 0 && p3.Y < 0) ||
                         (p1.X >= width && p2.X >= width && p3.X >= width) ||
                         (p1.Y >= height && p2.Y >= height && p3.Y >= height))
                        return TRIANGLE_VISIBILITY.NO;
                    else
                        return TRIANGLE_VISIBILITY.PARTLY;
            else
                return TRIANGLE_VISIBILITY.PARTLY;
        }

        
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public void setVerts(Point3D[] verts)
        {
            this.verts = verts;
            plane = new Plane3D(verts[id1], verts[id2], verts[id3]);
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public void setRealVerts(Point3D[] verts)
        {
            this.realVerts = verts;
        }

        public void setIntensivities(double[] intensivities)
        {
            this.intensivities = intensivities;
        }

        public void setNormals(Point3D[] normals)
        {
            this.normals = normals;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Triangle obj)
        {
            return (verts[id1].Equals(obj.verts[id1]) && verts[id2].Equals(obj.verts[id2]) && verts[id3].Equals(obj.verts[id3]));
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public void transformForFrame(RasterizationData data)
        {
            zbits = data.zbits;
            height = data.height;
            width = data.width;
            camera = data.camera;
            bits = data.bits;
            plane = new Plane3D(verts[id1], verts[id2], verts[id3]);
            plane.normalize();
            plane.recalculationD(verts[id1]);
        }

        public Plane3D getPlane() { return plane; }

        public void draw(Graphics g, int height)
        {
            g.DrawLine(new Pen(Color.Green), verts[id1].toPoint(height), verts[id2].toPoint(height));
            g.DrawLine(new Pen(Color.Green), verts[id2].toPoint(height), verts[id3].toPoint(height));
            g.DrawLine(new Pen(Color.Green), verts[id3].toPoint(height), verts[id1].toPoint(height));
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public unsafe void swap(int* p1, int* p2)
        {
            int t = *p1;
            *p1 = *p2;
            *p2 = t;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static unsafe void swap(double* p1, double* p2)
        {
            double t = *p1;
            *p1 = *p2;
            *p2 = t;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static void swap<T>(ref T p1, ref T p2)
        {
            T t = p1;
            p1 = p2;
            p2 = t;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static int round(double d)
        {
            return (int)(d + 0.5);
        }
        

        public bool isLine { get; set; }

        public unsafe void precalculationForShadow()
        {
            z1 = p1.Z;
            z2 = p2.Z;
            z3 = p3.Z;
            isLine = false;

            if (p3.Y != p1.Y)
            {
                dx31 = (p3.X - p1.X) / (p3.Y - p1.Y);
                dz31 = (z3 - z1) / (p3.Y - p1.Y);
            }
            else { dx31 = 0; dz31 = 0; }

            if (p2.Y != p1.Y)
            {
                dx21 = (p2.X - p1.X) / (p2.Y - p1.Y);
                dz21 = (z2 - z1) / (p2.Y - p1.Y);
            }
            else { dx21 = 0; dz21 = 0; }

            if (p3.Y != p2.Y)
            {
                dx32 = (p3.X - p2.X) / (p3.Y - p2.Y);
                dz32 = (z3 - z2) / (p3.Y - p2.Y);
            }
            else { dx32 = 0; dz32 = 0; }

            _dx32 = -dx32; _dx31 = -dx31;
            _dz32 = -dz32; _dz31 = -dz31;
            if (dx31 < dx21)
            {
                swap(ref dx31, ref dx21);
                swap(ref dz31, ref dz21);
            }
            if (_dx31 < _dx32)
            {
                swap(ref _dx32, ref _dx31);
                swap(ref _dz32, ref _dz31);
            }

            if (p3.Y - p1.Y == 0)
            {
                isLine = true;
                if (p2.X < p1.X)
                    swap(ref p1, ref p2);
                if (p3.X < p1.X)
                    swap(ref p1, ref p3);
                if (p2.X > p3.X)
                    swap(ref p3, ref p2);
                z1 = p1.Z;
                z2 = p2.Z;
                z3 = p3.Z;
            }
        }

        public unsafe void precalculation()
        {

            i1 = intensivities[id1];
            i2 = intensivities[id2];
            i3 = intensivities[id3];

            p1 = verts[id1].toInt();
            p2 = verts[id2].toInt();
            p3 = verts[id3].toInt();

            if (p2.Y < p1.Y)
            {
                swap(ref p1, ref p2);
                swap(ref i1, ref i2);
            }
            if (p3.Y < p1.Y)
            {
                swap(ref p3, ref p1);
                swap(ref i3, ref i1);
            }
            if (p2.Y > p3.Y)
            {
                swap(ref p2, ref p3);
                swap(ref i2, ref i3);
            }

            if (p3.Y - p1.Y == 0)
            {
                if (p2.X < p1.X)
                {
                    swap(ref p1, ref p2);
                    swap(ref i1, ref i2);
                }
                if (p3.X < p1.X)
                {
                    swap(ref p1, ref p3);
                    swap(ref i1, ref i3);

                }
                if (p2.X > p3.X)
                {
                    swap(ref p3, ref p2);
                    swap(ref i3, ref i2);
                }
            }

            z1 = p1.Z;
            z2 = p2.Z;
            z3 = p3.Z;
            if (p3.Y != p1.Y)
            {
                double deltaY = p3.Y - p1.Y;
                dx31 = (p3.X - p1.X) / deltaY;
                dz31 = (z3 - z1) / deltaY;
                di31 = (i3 - i1) / deltaY;
            }
            else { dx31 = 0; dz31 = 0; di31 = 0; }

            if (p2.Y != p1.Y)
            {
                double deltaY = p2.Y - p1.Y;
                dx21 = (p2.X - p1.X) / deltaY;
                dz21 = (z2 - z1) / deltaY;
                di21 = (i2 - i1) / deltaY;
            }
            else { dx21 = 0; dz21 = 0; di21 = 0; }

            if (p3.Y != p2.Y)
            {
                double deltaY = p3.Y - p2.Y;
                dx32 = (p3.X - p2.X) / deltaY;
                dz32 = (z3 - z2) / deltaY;
                di32 = (i3 - i2) / deltaY;
            }
            else { dx32 = 0; dz32 = 0; di32 = 0; }

            _dx32 = -dx32; _dx31 = -dx31;
            _dz32 = -dz32; _dz31 = -dz31;
            _di32 = -di32; _di31 = -di31;
            if (dx31 < dx21)
            {
                swap(ref dx31, ref dx21);
                swap(ref dz31, ref dz21);
                swap(ref di31, ref di21);
            }
            if (_dx31 < _dx32)
            {
                swap(ref _dx32, ref _dx31);
                swap(ref _dz32, ref _dz31);
                swap(ref _di32, ref _di31);            
            }
        }


        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public unsafe void updateFrame(int y1, int y2)
        {
            int k,
                maxY = (int)p3.Y <= y2 ? (int)p3.Y : y2,
                minY = (int)p1.Y >= y1 ? (int)p1.Y : y1,
                midY;

            double z, wx1, wx2, i, wi1, wi2, dix, dzx, wz1, wz2;
            int wx1Int, wx2Int;


            if (y2 >= (int)p2.Y)
            {
                midY = y1 >= (int)p2.Y ? y1 : (int)p2.Y;

                if (midY == maxY && p3.Y == p2.Y)
                {
                    if (p3.X < p2.X)
                    {
                        wx1 = p3.X; wx2 = p2.X;
                        wz1 = z3; wz2 = z2;
                        wi1 = i3; wi2 = i2;
                    }
                    else
                    {
                        wx1 = p2.X; wx2 = p3.X;
                        wz1 = z2; wz2 = z3;
                        wi1 = i2; wi2 = i3;
                    }
                }
                else
                {
                    wx1 = p3.X + (-maxY + (int)p3.Y) * _dx32; wx2 = p3.X + (-maxY + (int)p3.Y) * _dx31;
                    wz1 = z3 + (-maxY + (int)p3.Y) * _dz32; wz2 = z3 + (-maxY + (int)p3.Y) * _dz31;
                    wi1 = i3 + (-maxY + (int)p3.Y) * _di32; wi2 = i3 + (-maxY + (int)p3.Y) * _di31;
                }


                // растеризуем верхний полутреугольник
                for (int p = maxY; p >= midY; p--)
                {
                    wx1Int = round(wx1); wx2Int = round(wx2);

                    k = (height - p - 1) * width + wx1Int;
                    z = wz1; dzx = (wz2 - wz1) / (wx2Int - wx1Int);
                    i = wi1; dix = (wi2 - wi1) / (wx2Int - wx1Int);
                    for (int j = wx1Int; j <= wx2Int; j++, z += dzx, i += dix, k++)
                        if (zbits[k] > z + Eps)
                        {
                            zbits[k] = z;
                            HSLM color = new HSLM(intColor);
                            bits[k] = color.fromHSL(i);
                        }
                    wx1 += _dx32; wx2 += _dx31;
                    wz1 += _dz32; wz2 += _dz31;
                    wi1 += _di32; wi2 += _di31;
                }
            }

            if (y1 < (int)p2.Y)
            {
                midY = y2 <= (int)p2.Y ? y2 : (int)p2.Y;

                if (midY == minY && p1.Y == p2.Y)
                {
                    if (p1.X < p2.X)
                    {
                        wx1 = p1.X; wx2 = p2.X;
                        wz1 = z1; wz2 = z2;
                        wi1 = i1; wi2 = i2;
                    }
                    else
                    {
                        wx1 = p2.X; wx2 = p1.X;
                        wz1 = z2; wz2 = z1;
                        wi1 = i2; wi2 = i1;
                    }

                }
                else
                {
                    wx1 = p1.X + (minY - (int)p1.Y) * dx21; wx2 = p1.X + (minY - (int)p1.Y) * dx31;
                    wz1 = z1 + (minY - (int)p1.Y) * dz21; wz2 = z1 + (minY - (int)p1.Y) * dz31;
                    wi1 = i1 + (minY - (int)p1.Y) * di21; wi2 = i1 + (minY - (int)p1.Y) * di31;
                }

                for (int p = minY; p <= midY; p++)
                {
                    wx1Int = round(wx1); wx2Int = round(wx2);

                    k = (height - p - 1) * width + wx1Int;
                    z = wz1; dzx = (wz2 - wz1) / (wx2Int - wx1Int);
                    i = wi1; dix = (wi2 - wi1) / (wx2Int - wx1Int);
                    for (int j = wx1Int; j <= wx2Int; j++, z += dzx, i += dix, k++)
                        if (zbits[k] > z + Eps)
                        {
                            zbits[k] = z;
                            HSLM color = new HSLM(intColor);
                            bits[k] = color.fromHSL(i);
                        }
                    wx1 += dx21; wx2 += dx31;
                    wz1 += dz21; wz2 += dz31;
                    wi1 += di21; wi2 += di31;
                }
            }

        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public unsafe void updateFrameWithChecking(int y1, int y2)
        {
            int k,
                maxY = (int)p3.Y <= y2 ? (int)p3.Y : y2,
                minY = (int)p1.Y >= y1 ? (int)p1.Y : y1,
                midY;

            double z, wx1, wx2, i, wi1, wi2, dix, dzx, wz1, wz2;
            int wx1Int, wx2Int;


            if (y2 >= (int)p2.Y)
            {
                midY = y1 >= (int)p2.Y ? y1 : (int)p2.Y;

                if (midY == maxY && p3.Y == p2.Y)
                {
                    if (p3.X < p2.X)
                    {
                        wx1 = p3.X; wx2 = p2.X;
                        wz1 = z3; wz2 = z2;
                        wi1 = i3; wi2 = i2;
                    }
                    else
                    {
                        wx1 = p2.X; wx2 = p3.X;
                        wz1 = z2; wz2 = z3;
                        wi1 = i2; wi2 = i3;
                    }
                }
                else
                {
                    wx1 = p3.X + (-maxY + (int)p3.Y) * _dx32; wx2 = p3.X + (-maxY + (int)p3.Y) * _dx31;
                    wz1 = z3 + (-maxY + (int)p3.Y) * _dz32; wz2 = z3 + (-maxY + (int)p3.Y) * _dz31;
                    wi1 = i3 + (-maxY + (int)p3.Y) * _di32; wi2 = i3 + (-maxY + (int)p3.Y) * _di31;
                }


                // растеризуем верхний полутреугольник
                for (int p = maxY; p >= midY; p--)
                    if (p >= 0 && p < height)
                    {
                        wx1Int = round(wx1); wx2Int = round(wx2);

                        k = (height - p - 1) * width + wx1Int;
                        z = wz1; dzx = (wz2 - wz1) / (wx2Int - wx1Int);
                        i = wi1; dix = (wi2 - wi1) / (wx2Int - wx1Int);
                        for (int j = wx1Int; j <= wx2Int; j++, z += dzx, i += dix, k++)
                            if (j >= 0 && j < width && zbits[k] > z)
                                if (zbits[k] > z + Eps)
                                {
                                    zbits[k] = z;
                                    HSLM color = new HSLM(intColor);
                                    bits[k] = color.fromHSL(i);
                                }
                        wx1 += _dx32; wx2 += _dx31;
                        wz1 += _dz32; wz2 += _dz31;
                        wi1 += _di32; wi2 += _di31;
                    }
            }

            if (y1 < (int)p2.Y)
            {
                midY = y2 <= (int)p2.Y ? y2 : (int)p2.Y;

                if (midY == minY && p1.Y == p2.Y)
                {
                    if (p1.X < p2.X)
                    {
                        wx1 = p1.X; wx2 = p2.X;
                        wz1 = z1; wz2 = z2;
                        wi1 = i1; wi2 = i2;
                    }
                    else
                    {
                        wx1 = p2.X; wx2 = p1.X;
                        wz1 = z2; wz2 = z1;
                        wi1 = i2; wi2 = i1;
                    }

                }
                else
                {
                    wx1 = p1.X + (minY - (int)p1.Y) * dx21; wx2 = p1.X + (minY - (int)p1.Y) * dx31;
                    wz1 = z1 + (minY - (int)p1.Y) * dz21; wz2 = z1 + (minY - (int)p1.Y) * dz31;
                    wi1 = i1 + (minY - (int)p1.Y) * di21; wi2 = i1 + (minY - (int)p1.Y) * di31;
                }

                for (int p = minY; p <= midY; p++)
                    if (p >= 0 && p < height)
                    {
                        wx1Int = round(wx1); wx2Int = round(wx2);

                        k = (height - p - 1) * width + wx1Int;
                        z = wz1; dzx = (wz2 - wz1) / (wx2Int - wx1Int);
                        i = wi1; dix = (wi2 - wi1) / (wx2Int - wx1Int);
                        for (int j = wx1Int; j <= wx2Int; j++, z += dzx, i += dix, k++)
                            if (j >= 0 && j < width && zbits[k] > z)
                                if (zbits[k] > z + Eps)
                                {
                                    zbits[k] = z;
                                    HSLM color = new HSLM(intColor);
                                    bits[k] = color.fromHSL(i);
                                }
                        wx1 += dx21; wx2 += dx31;
                        wz1 += dz21; wz2 += dz31;
                        wi1 += di21; wi2 += di31;
                    }
            }

        }

        //-------------------------------------------------------------------------------------------------
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public unsafe void updateOnlyZBuffer(int y1, int y2)
        {

            int k,
                maxY = (int)p3.Y <= y2 ? (int)p3.Y : y2,
                minY = (int)p1.Y >= y1 ? (int)p1.Y : y1,
                midY;

            double z, wx1, wx2, dzx, wz1, wz2;
            int wx1Int, wx2Int;

            if (y2 >= (int)p2.Y)
            {
                midY = y1 >= (int)p2.Y ? y1 : (int)p2.Y;

                if (midY == maxY && p3.Y == p2.Y)
                {
                    if (p3.X < p2.X)
                    {
                        wx1 = p3.X; wx2 = p2.X;
                        wz1 = z3; wz2 = z2;
                    }
                    else
                    {
                        wx1 = p2.X; wx2 = p3.X;
                        wz1 = z2; wz2 = z3;
                    }
                }
                else
                {
                    wx1 = p3.X + (-maxY + (int)p3.Y) * _dx32; wx2 = p3.X + (-maxY + (int)p3.Y) * _dx31;
                    wz1 = z3 + (-maxY + (int)p3.Y) * _dz32; wz2 = z3 + (-maxY + (int)p3.Y) * _dz31;
                }


                // растеризуем верхний полутреугольник
                for (int i = maxY; i >= midY; i--)
                {
                    wx1Int = round(wx1); wx2Int = round(wx2);

                    k = (height - i - 1) * width + wx1Int;
                    z = wz1; dzx = (wz2 - wz1) / (wx2Int - wx1Int);
                    for (int j = wx1Int; j <= wx2Int; j++, k++, z += dzx)
                        if (zbits[k] > z + Eps)
                            zbits[k] = z;
                    wx1 += _dx32; wx2 += _dx31;
                    wz1 += _dz32; wz2 += _dz31;
                }
            }

            if (y1 < (int)p2.Y)
            {
                midY = y2 <= (int)p2.Y ? y2 : (int)p2.Y;

                if (midY == minY && p1.Y == p2.Y)
                {
                    if (p1.X < p2.X)
                    {
                        wx1 = p1.X; wx2 = p2.X;
                        wz1 = z1; wz2 = z2;
                    }
                    else
                    {
                        wx1 = p2.X; wx2 = p1.X;
                        wz1 = z2; wz2 = z1;
                    }
                }
                else
                {
                    wx1 = p1.X + (minY - (int)p1.Y) * dx21; wx2 = p1.X + (minY - (int)p1.Y) * dx31;
                    wz1 = z1 + (minY - (int)p1.Y) * dz21; wz2 = z1 + (minY - (int)p1.Y) * dz31;
                }

                // растеризуем нижний полутреугольник
                for (int i = minY; i <= midY; i++)
                {
                    wx1Int = round(wx1); wx2Int = round(wx2);

                    k = (height - i - 1) * width + wx1Int;

                    z = wz1; dzx = (wz2 - wz1) / (wx2Int - wx1Int);
                    for (int j = wx1Int; j <= wx2Int; j++, z += dzx, k++)
                        if (zbits[k] > z + Eps)
                            zbits[k] = z;
                    wx1 += dx21; wx2 += dx31;
                    wz1 += dz21; wz2 += dz31;
                }
            }

        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public unsafe void updateOnlyZBufferWithChecking(int y1, int y2)
        {

            int k,
                maxY = (int)p3.Y <= y2 ? (int)p3.Y : y2,
                minY = (int)p1.Y >= y1 ? (int)p1.Y : y1,
                midY;

            double z, wx1, wx2, dzx, wz1, wz2;
            int wx1Int, wx2Int;

            if (y2 >= (int)p2.Y)
            {
                midY = y1 >= (int)p2.Y ? y1 : (int)p2.Y;

                if (midY == maxY && p3.Y == p2.Y)
                {
                    if (p3.X < p2.X)
                    {
                        wx1 = p3.X; wx2 = p2.X;
                        wz1 = z3; wz2 = z2;
                    }
                    else
                    {
                        wx1 = p2.X; wx2 = p3.X;
                        wz1 = z2; wz2 = z3;
                    }
                }
                else
                {
                    wx1 = p3.X + (-maxY + (int)p3.Y) * _dx32; wx2 = p3.X + (-maxY + (int)p3.Y) * _dx31;
                    wz1 = z3 + (-maxY + (int)p3.Y) * _dz32; wz2 = z3 + (-maxY + (int)p3.Y) * _dz31;
                }


                // растеризуем верхний полутреугольник
                for (int i = maxY; i >= midY; i--)
                    if (i >= 0 && i < height)
                    {
                        wx1Int = round(wx1); wx2Int = round(wx2);

                        k = (height - i - 1) * width + wx1Int;
                        z = wz1; dzx = (wz2 - wz1) / (wx2Int - wx1Int);
                        for (int j = wx1Int; j <= wx2Int; j++, k++, z += dzx)
                            if (j >= 0 && j < width && zbits[k] > z + Eps)
                                zbits[k] = z;
                        wx1 += _dx32; wx2 += _dx31;
                        wz1 += _dz32; wz2 += _dz31;
                    }
            }

            if (y1 < (int)p2.Y)
            {
                midY = y2 <= (int)p2.Y ? y2 : (int)p2.Y;

                if (midY == minY && p1.Y == p2.Y)
                {
                    if (p1.X < p2.X)
                    {
                        wx1 = p1.X; wx2 = p2.X;
                        wz1 = z1; wz2 = z2;
                    }
                    else
                    {
                        wx1 = p2.X; wx2 = p1.X;
                        wz1 = z2; wz2 = z1;
                    }
                }
                else
                {
                    wx1 = p1.X + (minY - (int)p1.Y) * dx21; wx2 = p1.X + (minY - (int)p1.Y) * dx31;
                    wz1 = z1 + (minY - (int)p1.Y) * dz21; wz2 = z1 + (minY - (int)p1.Y) * dz31;
                }

                // растеризуем нижний полутреугольник
                for (int i = minY; i <= midY; i++)
                    if (i >= 0 && i < height)
                    {
                        wx1Int = round(wx1); wx2Int = round(wx2);

                        k = (height - i - 1) * width + wx1Int;

                        z = wz1; dzx = (wz2 - wz1) / (wx2Int - wx1Int);
                        for (int j = wx1Int; j <= wx2Int; j++, z += dzx, k++)
                            if (j >= 0 && j < width && zbits[k] > z + Eps)
                                zbits[k] = z;
                        wx1 += dx21; wx2 += dx31;
                        wz1 += dz21; wz2 += dz31;
                    }
            }

        }
    }
}
