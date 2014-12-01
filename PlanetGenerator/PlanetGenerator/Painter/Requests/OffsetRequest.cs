
namespace STLParserProject
{
    enum OFFSET_DIRECTION{LEFT, RIGHT, UP, DOWN}
    class OffsetRequest :Request
    {
        private OFFSET_DIRECTION direction;
        private double d;

        public OffsetRequest(int cameraNum, OFFSET_DIRECTION direction, double d)
            : base(cameraNum)
        {
            this.direction = direction;
            this.d = d;
        }

        public double getD() { return d; }
        public OFFSET_DIRECTION getOffsetDirection() { return direction; }

//         public override Matrix3D calculateMatrix()
//         {
//             double dx = 0, dy = 0, dz = 0;
//             switch (direction)
//             {
//                 case (OFFSET_DIRECTION.DOWN):
//                     dy = -d;
//                     break;
//                 case (OFFSET_DIRECTION.UP):
//                     dy = d;
//                     break;
//                 case (OFFSET_DIRECTION.LEFT):
//                     dx = -d;
//                     break;
//                 case (OFFSET_DIRECTION.RIGHT):
//                     dx = d;
//                     break;
//             }
//             transformMatrix = Matrix3D.getOffsetMatrix(dx, dy, dz);
//             return transformMatrix;
//         }
    }
}
