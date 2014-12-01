using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STLParserProject
{
    class Shadow
    {
        private ZBuffer depthBuffer;
        private double[] shadowMap;
        private Lightning light;
        private List<Figure> figureList;
        private Matrix3D shadowMatr;
        private Camera camera;
        private int width, height;
        protected List<TRIANGLE_VISIBILITY[]> visibilityList = new List<TRIANGLE_VISIBILITY[]>();

        public Shadow(Lightning light, List<Figure> figures, int width, int height)
        {
            this.figureList = figures;
            this.light = light;
            this.depthBuffer = new ZBuffer(width, height);
            this.shadowMap = depthBuffer.getZBits();
            this.width = depthBuffer.getWidth();
            this.height = depthBuffer.getHeight();
        }

        public void update(int yGroupsCount, Camera camera)
        {
            light.calculateFigureTransformatioMatrix();
            this.camera = camera;
            visibilityList = new List<TRIANGLE_VISIBILITY[]>();
            ZBufferData data = new ZBufferData(light, depthBuffer.getWidth(), depthBuffer.getHeight(), depthBuffer.getZBits());

            ConcurrentQueue<double> ymins = new ConcurrentQueue<double>();
            ConcurrentQueue<double> ymaxs = new ConcurrentQueue<double>();
//             Parallel.For(0, figureList.Count, i => figureList[i].transformForShadow(data, ymaxs, ymins));

            for (int i = 0; i < figureList.Count; i++)
            {
                TRIANGLE_VISIBILITY[] newArray = new TRIANGLE_VISIBILITY[figureList[i].getTriangles().Length];
                Array.Copy(figureList[i].getVisibility(), newArray, newArray.Length);
                visibilityList.Add(newArray);
            }

            int ymin = Triangle.round(ymins.Min());
            int ymax = Triangle.round(ymaxs.Max());
            depthBuffer.update(figureList, yGroupsCount, ymin, ymax);
            m1 = camera.getFigureTransformationMatrix().getInverstionMatrix();
            m2 = light.getFigureTransformationMatrix();
            shadowMatr = m1 * m2;
        }

        public TRIANGLE_VISIBILITY getVisibilityOfTriangleInShadow(int triangleID, int figureID)
        {
            return visibilityList[figureID][triangleID];
        }

        Matrix3D m1, m2;


                [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public bool checkPoint(Point3D point)
        {

            Point3D bufP = new Point3D(point);
            //             shadowMatr.transformPoint(bufP);   
            // //             shadowMatr.transformPoint(bufP);
            // //             light.trasnformPointAfterProjection(bufP);
            // 
            bufP = bufP.toInt();
            if (bufP.X < 0 || bufP.X >= width || bufP.Y < 0 || bufP.Y >= height)
                return true;
            bool res = shadowMap[(int)(bufP.X) + width * (height - (int)(bufP.Y) - 1)] + Triangle.Eps2 < bufP.Z;
            //             if (res == true)
            //             {
            //                 Trace.WriteLine(shadowMap[Triangle.round(bufP.X) + width * (height - Triangle.round(bufP.Y) - 1)] + " " + bufP.ToString());
            //             }
            //             
//             if (res == true)
//                 Debugger.Break();
            return res;
        }

        public static Point3D getUVPoint(Point3D f, Point3D p1, Point3D p2, Point3D p3, Point3D uv1, Point3D uv2, Point3D uv3)
        {
            Point3D f1 = p1 - f;
            Point3D f2 = p2 - f;
            Point3D f3 = p3 - f;
            // calculate the areas and factors (order of parameters doesn't matter):
            Point3D va = Point3D.vectorMultiplication(p1 - p2, p1 - p3); // main triangle area a
            Point3D va1 = Point3D.vectorMultiplication(f2, f3); // p1's triangle area / a
            Point3D va2 = Point3D.vectorMultiplication(f3, f1); // p2's triangle area / a 
            Point3D va3 = Point3D.vectorMultiplication(f1, f2); // p3's triangle area / a
            // find the uv corresponding to point f (uv1/uv2/uv3 are associated to p1/p2/p3):
            double a = va.getLength(),
                a1 = va1.getLength() / a * Math.Sign(va1 * va),
                a2 = va2.getLength() / a * Math.Sign(va2 * va),
                a3 = va3.getLength() / a * Math.Sign(va3 * va);

            
            return uv1 * a1 + uv2 * a2 + uv3 * a3;
        }

    }
}
