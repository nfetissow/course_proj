﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using PlanetGenerator.SphereBuilder;

using System.IO;

namespace STLParserProject
{
    class Figure
    {
        protected Triangle[] triangles;
        protected Point3D[] points;
        protected Point3D[] pointsCopy;
        protected Point3D[] pointNormals;
        //protected Point3D[] uvpoints;
        protected double[] intensivities;
        protected Matrix3D prevMatrix;
        protected double xWidth, yWidth, zWidth;
        protected TRIANGLE_VISIBILITY[] visibleTriangles;
        protected List<TRIANGLE_VISIBILITY[]> visibleTrianglesDiffCamera = new List<TRIANGLE_VISIBILITY[]>();
        protected ConcurrentQueue<Matrix3D> motions = new ConcurrentQueue<Matrix3D>();
        protected bool transformed = false;
        public Point3D middlePoint { get; set; }

        //
        private CylindricalTexture texture;
        protected OpticalCharacteristics optics;
        //public Material material;

        public Figure(Triangle[] triangles, Point3D[] points, Point3D[] pointNormals, Point3D[] uvpoints, Material material)
        {
            this.triangles = triangles;
            this.points = points;
            this.pointNormals = pointNormals;
            //this.uvpoints = uvpoints;
            prevMatrix = new Matrix3D();

            xWidth = points.Max(p => p.X) - points.Min(p => p.X);
            yWidth = points.Max(p => p.Y) - points.Min(p => p.Y);
            zWidth = points.Max(p => p.Z) - points.Min(p => p.Z);

            this.optics = optics;
            intensivities = new double[points.Length];
            pointsCopy = new Point3D[points.Length];
            middlePoint= new Point3D(0, 0, 0);

            for (int i = 0; i < points.Length; i++)
            {
                pointsCopy[i] = new Point3D(points[i]);
                middlePoint += points[i];
            }
            middlePoint /= points.Length;
            //middlePoint = new Point3D(points.Min(p => p.X), points.Min(p => p.Y),0);
//             Image img = Image.FromFile("C:\\Users\\Али\\Documents\\Visual Studio 2010\\Projects\\STLParserProject Финальная\\Textures\\e1.jpg");
//             
            //             TriplanarTexture texture = new TriplanarTexture(img, this);
            //this.material = material;
            //texture = new CylindricalTexture(material.Texture);
            optics = new OpticalCharacteristics(0.1, 0.9, 0.5, 40);
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i].setUVVerts(uvpoints);
                triangles[i].setTexture(texture);
                triangles[i].setIntensivities(intensivities);
                triangles[i].setNormals(pointNormals);
                triangles[i].setRealVerts(pointsCopy);
            }

        }

        public Figure(Figure obj)
        {
            triangles = new Triangle[obj.triangles.Length];
            points = new Point3D[obj.points.Length];
            for (int i = 0; i < points.Length; i++)
                points[i] = new Point3D(obj.points[i]);
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = new Triangle(obj.triangles[i]);
                triangles[i].setVerts(points);
            }
        }

        public void finalize() { texture.finalize(); }
        /// <summary>
        /// This function does nothing
        /// </summary>
        /// <param name="m"></param>
        public void setMaterial(Material m)
        {
            //material = m;
            //texture = new CylindricalTexture(material.Texture);
            //optics = material.Optics;
            //for (int i = 0; i < triangles.Length; i++)
            //    triangles[i].setTexture(texture);
        }

        public Triangle[] getTriangles() { return triangles; }

        public OpticalCharacteristics getOptics() { return optics; }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private bool[] getVisibleArray(Point3D watchVector)
        {
            bool[] res = new bool[triangles.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = true;
            return res;
        }
        // 
        //         public void draw(Graphics g, int height)
        //         {
        //             for (int i = 0; i < triangles.Length; i++)
        //                 if (visibleTriangles[i] != TRIANGLE_VISIBILITY.NO)
        //                     triangles[i].draw(g, height);
        //         }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public void transform(Matrix3D matr)
        {
            for (int i = 0; i < points.Length; i++)
                matr.transformPoint(pointsCopy[i]);
            matr.transformPoint(middlePoint);
        }


        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public void transformForFrame(RasterizationData data, ConcurrentQueue<double> ymaxs, ConcurrentQueue<double> ymins)
        {
            transformed = false;
            Camera camera = data.camera;
            Matrix3D figureMatr, cameraMatr = camera.getFigureTransformationMatrix();
            List<Lightning> lightningList = data.lightnings;
            motions.TryDequeue(out figureMatr);

            double zBuf;
            double ymax = -10000; double ymin = 10000;
            if (figureMatr != null)
                figureMatr.transformPoint(middlePoint);
            for (int i = 0; i < points.Length; i++)
            {
                if (figureMatr != null)
                    figureMatr.transformPoint(pointsCopy[i]);
                // преобразование точек к СК камеры
                points[i] = new Point3D(pointsCopy[i]);
                cameraMatr.transformPoint(points[i]);
                camera.trasnformPointAfterProjection(points[i]);

                if (ymax < points[i].Y)
                    ymax = points[i].Y;
                if (ymin > points[i].Y)
                    ymin = points[i].Y;
                

                // вычисление интенсивности света в каждой вершине
                Point3D normal = new Point3D(pointNormals[i]);
                intensivities[i] = optics.getKa();
                Point3D V = camera.getCenterPoint() - pointsCopy[i];
                for (int j = 0; j < lightningList.Count; j++)
                {
                    Point3D L = lightningList[j].getL(pointsCopy[i]);
                    double cosTeta = (normal * L) / (normal.getLength() * L.getLength());
                    L *= -1;
                    Point3D R = L - normal * 2 * (normal * L);
                    intensivities[i] += lightningList[j].Amp
                         * (optics.getKd() * cosTeta + optics.getKs() * Math.Pow(R*V/R.getLength()/V.getLength(), optics.getNs()));
                }
                if (intensivities[i] > 1)
                    intensivities[i] = 1;
                if (intensivities[i] < 0)
                    intensivities[i] = 0;
            };
            // вот тут внимательно слушаем:
            // алгоритм отбрасывания нелицевых граней создан из 2-х: первый этап Робертса как преобработка и z буфер 
            // на этом этапе определяется видимость грани относительно камеры(тот самый первый этап)
            visibleTriangles = new TRIANGLE_VISIBILITY[triangles.Length];
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i].transformForFrame(data);
                visibleTriangles[i] = triangles[i].getVisibility();
                if (visibleTriangles[i] != TRIANGLE_VISIBILITY.NO)
                    triangles[i].precalculation();
            };

            visibleTrianglesDiffCamera.Add(visibleTriangles);

            ymins.Enqueue((ymin));
            ymaxs.Enqueue((ymax));

        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public virtual unsafe void updateZBufferAndImageFrame(int y1, int y2, int figureID)
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                if (visibleTriangles[i] != TRIANGLE_VISIBILITY.NO && !(y1 > triangles[i].p3.Y || y2 < triangles[i].p1.Y))
                    if (visibleTriangles[i] == TRIANGLE_VISIBILITY.FULL)
                        triangles[i].updateFrame(y1, y2);
                    else
                        triangles[i].updateFrameWithChecking(y1, y2);
            }
        }


        // 
        //         [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public virtual unsafe void updateOnlyZBufferFrame(int y1, int y2)
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                if (visibleTriangles[i] != TRIANGLE_VISIBILITY.NO && !(y1 > triangles[i].p3.Y || y2 < triangles[i].p1.Y))
                    if (visibleTriangles[i] == TRIANGLE_VISIBILITY.FULL)
                        triangles[i].updateOnlyZBuffer(y1, y2);
                    else
                        triangles[i].updateOnlyZBufferWithChecking(y1, y2);
            }
        }

        public ConcurrentQueue<Matrix3D> getMotionQueue() { return motions; }

        public double getXWidth()
        {
            return xWidth = pointsCopy.Max(p => p.X) - pointsCopy.Min(p => p.X);
        }
        public double getYWidth()
        {
            return yWidth = pointsCopy.Max(p => p.Y) - pointsCopy.Min(p => p.Y);
        }
        public double getZWidth()
        {
            return zWidth = pointsCopy.Max(p => p.Z) - pointsCopy.Min(p => p.Z); 
        }

        public TRIANGLE_VISIBILITY[] getVisibility() { return visibleTriangles; }

        public static Figure fromTriangleMesh(TriangleMesh mesh)
        {
            Random rng = new Random();
            Distortion.distortMesh(mesh, 10, rng);

            var triangles = new Triangle[mesh.faces.Length];
            var points = new Point3D[mesh.nodes.Length];
            double mult = 50;
            double offset = 100;
            for (int i = 0; i < mesh.nodes.Length; ++i)
            {
                points[i] = new Point3D(offset+ mesh.nodes[i].p.x * mult,offset+ mesh.nodes[i].p.y * mult,offset+ mesh.nodes[i].p.z * mult);
            }
            for (int i = 0; i < mesh.faces.Length; ++i)
            {
                triangles[i] = new Triangle(points, mesh.faces[i].n[0], mesh.faces[i].n[1], mesh.faces[i].n[2], rng.Next(0, 255));
            }

            var figure = new Figure(triangles, points, points, null, null);
            figure.transform(Matrix3D.getOffsetMatrix(-160, -160, 0)* Matrix3D.getOffsetMatrix(320, 320, 0));
            return figure;
        }

        public static Figure fromPolyhedronMesh(PolyhedronMesh mesh, double x1, double y1, double z1)
        {
            Random rng = new Random();
            var points = new Point3D[mesh.corners.Length];
            var triangles = new List<Triangle>();
            double mult = 100;
            double offset = 200;
            for (int i = 0; i < mesh.corners.Length; ++i)
            {
                points[i] = new Point3D(offset + mesh.corners[i].pos.x * mult, offset + mesh.corners[i].pos.y * mult, mesh.corners[i].pos.z * mult);
            }
            HashSet<int> q = new HashSet<int>();
            foreach(Tile t in mesh.tiles)
            {
                
                int k = rng.Next(-4048, 1024);
                t.sortCorners();
                for(int i = t.corners.Length -2; i > 0; --i)
                {

                    triangles.Add(new Triangle(points, t.corners[t.corners.Length - 1].id, t.corners[i].id, t.corners[i - 1].id, k));//t.plate.oceanic? 255: 125));
                }

            }

            var figure = new Figure(triangles.ToArray(), points, points, null, null);
            figure.transform(/*Matrix3D.getOffsetMatrix(-160, -160, 0) */ Matrix3D.getOffsetMatrix(x1, y1, z1));
            return figure;
        }
        
    }
}
