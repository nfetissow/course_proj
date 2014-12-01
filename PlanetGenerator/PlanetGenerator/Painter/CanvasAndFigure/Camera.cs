using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STLParserProject
{
    public class Camera
    {
        protected Point3D center;
        //private Point3D[] cameraRect;
        protected Point3D rotateCenter;
        protected Point3D turningPoint;
        protected double distance;
        protected double projectionPlane;
        protected double zf = 1000;
        protected Matrix3D figureTransformationMatrix;

        public Camera(Point3D center, Point3D rotateCenter, Point3D turningPoint)
        {
            this.center = center;
            this.rotateCenter = rotateCenter;
            this.distance = (rotateCenter - center).getLength();
            this.turningPoint = turningPoint;
            this.projectionPlane = distance / 4;
        }

        public Camera(Camera camera)
        {
            this.center = new Point3D(camera.center);
            this.rotateCenter = new Point3D(camera.rotateCenter);
            this.distance = camera.distance;
            this.turningPoint = new Point3D(camera.turningPoint);
        }

        public Point3D getCenterPoint() { return center; }
        public Point3D getRotateCenter() { return rotateCenter; }

        public void transform(Matrix3D matr)
        {
            matr.transformPoint(center);
            matr.transformPoint(rotateCenter);
            matr.transformPoint(turningPoint);
            //this.vectorMultMark = Point3D.vectorMultiplicationMark(center - rotateCenter, Cartesian.k);
        }

        public void transformAroundCenter(Matrix3D matr)
        {
            Matrix3D resMatr = new Matrix3D();
            resMatr = Matrix3D.getOffsetMatrix(-rotateCenter.X, -rotateCenter.Y, -rotateCenter.Z) * matr * Matrix3D.getOffsetMatrix(rotateCenter.X, rotateCenter.Y, rotateCenter.Z);
            resMatr.transformPoint(center);
            resMatr.transformPoint(turningPoint);
            resMatr.transformPoint(rotateCenter);
            // this.vectorMultMark = Point3D.vectorMultiplicationMark(center - rotateCenter, Cartesian.k);
        }

        public bool canTransform(Matrix3D matr)
        {
            Camera bufCamera = new Camera(this);
            bufCamera.transformAroundCenter(matr);
            if (bufCamera.center.Z > 0)
                return true;
            return false;
        }

        public Matrix3D getCameraTransformationMatrix(Request request)
        {
            if (request is RotateRequest)
            {
                RotateRequest rotationRequest = request as RotateRequest;

                if (rotationRequest.getRotateAxis() == ROTATE_AXIS.UP)
                {
                    Point3D vector = turningPoint - center;
                    vector /= vector.getLength();
                    Matrix3D matr = Matrix3D.getRotationOnSimpleAxisMatrix(rotationRequest.getRotateAngle(), vector);
                    return matr;
                }

                if (rotationRequest.getRotateAxis() == ROTATE_AXIS.DOWN)
                {
                    Point3D vector = turningPoint - center;
                    vector /= vector.getLength();
                    Matrix3D matr = Matrix3D.getRotationOnSimpleAxisMatrix(-rotationRequest.getRotateAngle(), vector);
                    return matr;
                }

                if (rotationRequest.getRotateAxis() == ROTATE_AXIS.LEFT)
                {
                    Matrix3D matr = Matrix3D.getRotationZMatrix(-rotationRequest.getRotateAngle());
                    return matr;
                }

                if (rotationRequest.getRotateAxis() == ROTATE_AXIS.RIGHT)
                {
                    Matrix3D matr = Matrix3D.getRotationZMatrix(rotationRequest.getRotateAngle());
                    return matr;
                }
            }
            if (request is ScaleRequest)
            {
//                 Point3D vector = (center - rotateCenter) * (request as ScaleRequest).getKoef(),
//                     arm = turningPoint - center;
// 
//                 center = rotateCenter + vector;
//                 turningPoint = center + arm;
//                 distance = vector.getLength();
//                 projectionPlane = distance / 4;

                projectionPlane *= (request as ScaleRequest).getKoef();

                if (projectionPlane < 20 || projectionPlane > 200)
                    projectionPlane /= (request as ScaleRequest).getKoef();
            }
            if (request is OffsetRequest)
            {
                OffsetRequest offsetData = request as OffsetRequest;
                Matrix3D matr = null;
                Point3D directionVector;
                switch (offsetData.getOffsetDirection())
                {
                    case OFFSET_DIRECTION.DOWN:
                        directionVector = Point3D.vectorMultiplication(turningPoint - center,rotateCenter - center);
                        directionVector.normalizeXY();
                        matr = Matrix3D.getOffsetMatrix(-directionVector.X, -directionVector.Y, 0);
                        break;
                    case OFFSET_DIRECTION.UP:
                        directionVector = Point3D.vectorMultiplication(turningPoint - center,rotateCenter - center);
                        directionVector.normalizeXY();
                        matr = Matrix3D.getOffsetMatrix(directionVector.X, directionVector.Y, 0);
                        break;
                    case OFFSET_DIRECTION.LEFT:
                        directionVector = turningPoint - center;
                        directionVector.normalizeXY();
                        matr = Matrix3D.getOffsetMatrix(-directionVector.X, -directionVector.Y, 0);
                        break;
                    case OFFSET_DIRECTION.RIGHT:
                        directionVector = turningPoint - center;
                        directionVector.normalizeXY();
                        matr = Matrix3D.getOffsetMatrix(directionVector.X, directionVector.Y, 0);
                        break;
                }
                return matr;
            }
            
            return new Matrix3D();
        }


        Matrix3D invertedMatrix;
        /// <summary>
        /// Производит операции по получению матрицы для преобразования фигуры(говоря проще - представления фигуры в координатах камеры)
        /// ЛУЧШЕ СЮДА НЕ ЛЕЗТЬ! ВСЕ РАБОТАЕТ! другой вопрос как? я сам хз)) как-то шаманил в начале июня 14 года))
        /// </summary>
        /// <returns></returns>
        public Matrix3D calculateFigureTransformatioMatrix()
        {
            Matrix3D resMatrix = new Matrix3D();
            Point3D vector = center - rotateCenter;

            double mark1 = Point3D.vectorMultiplicationMarkXZ(new Point3D(vector.X, 0, vector.Z), Cartesian.k);
            
            double angleXZK = Point3D.angleBetween2Points(new Point3D(vector.X, 0, vector.Z), Cartesian.k);

            if (mark1 < 0)
                angleXZK = -angleXZK;

            Camera bufCamera = new Camera(this);
            resMatrix *= Matrix3D.getOffsetMatrix(-rotateCenter.X, -rotateCenter.Y, -rotateCenter.Z)
                * Matrix3D.getRotationYMatrix(angleXZK);

            bufCamera.transform(resMatrix);

            vector = bufCamera.center;
            double angleYZK = Point3D.angleBetween2Points(Cartesian.k, new Point3D(0, vector.Y, vector.Z));
            double mark2 = Point3D.vectorMultiplicationMarkZY(new Point3D(0, vector.Y, vector.Z), Cartesian.k);
            if (mark2 < 0)
                angleYZK = -angleYZK;

            bufCamera.transform(Matrix3D.getRotationXMatrix(-angleYZK));

            resMatrix = Matrix3D.getOffsetMatrix(-rotateCenter.X, -rotateCenter.Y, -rotateCenter.Z)
                * Matrix3D.getRotationYMatrix(angleXZK)
                * Matrix3D.getRotationXMatrix(angleYZK);

            Camera bufCamera2 = new Camera(this);
            bufCamera2.transform(resMatrix);

            vector = bufCamera2.turningPoint;
            double angleXYI = Point3D.angleBetween2Points(Cartesian.i, new Point3D(vector.X, vector.Y, 0));
            double mark3 = Point3D.vectorMultiplicationMarkXY(new Point3D(vector.X, vector.Y, 0), Cartesian.i);
            
            if (mark3 < 0)
                angleXYI = -angleXYI;

            resMatrix *= Matrix3D.getRotationZMatrix(angleXYI) *
                          Matrix3D.getRotationYMatrix(-180) *
                          Matrix3D.getOffsetMatrix(0, 0, distance) *
                Matrix3D.getPreProjectionMatrix(120, 1, projectionPlane) *
             Matrix3D.getScaleMatrix(6, 6, 1);

//                          
//             secondMatrix =
                //                          Matrix3D.getCentralProjectionMatrixV1(projectionPlane);
//                         Matrix3D.getCentralProjectionMatrixV2(120, 1, projectionPlane, zf)*
//                         Matrix3D.getCentralProjectionMatrixV3(120, 1, projectionPlane, zf)*
//                         Matrix3D.getScaleMatrix(6, 6, 1) *
//                         Matrix3D.getOffsetMatrix(rotateCenter.X, rotateCenter.Y, rotateCenter.Z);

            Camera bufCamera3 = new Camera(this);
            bufCamera3.transform(resMatrix);

            figureTransformationMatrix = resMatrix;

            return resMatrix;
        }

        public virtual void trasnformPointAfterProjection(Point3D point)
        {

            point.X = point.X / point.Z + rotateCenter.X;
            point.Y = point.Y / point.Z + rotateCenter.Y;
// //             point.Z *= 100;
        }

        public virtual void reverseTrasnformPointAfterDrawing(Point3D point)
        {
//             point.Z /= 100;
//             point.X = (point.X - rotateCenter.X) * point.Z;
//             point.Y = (point.Y - rotateCenter.Y) * point.Z;
        }

        public double getZn() { return projectionPlane; }
        public double getZf() { return zf; }
        public double getDistance() { return distance; }
        public Matrix3D getFigureTransformationMatrix() { return figureTransformationMatrix; }

        public Matrix3D getInvertedMatrix() { return invertedMatrix; }
    }
}

