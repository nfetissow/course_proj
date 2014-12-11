using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Painter
{
    enum ROTATE_AXIS { LEFT, RIGHT, UP, DOWN, UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT}

    class RotateRequest : Request
    {
        private double rotateAngle;
        private ROTATE_AXIS axis;

        public RotateRequest(int cameraNum, double rotateAngle, ROTATE_AXIS axis)
            : base(cameraNum)
        {
            this.rotateAngle = rotateAngle;
            this.axis = axis;
            //calculateMatrix();
        }

        public double getRotateAngle() { return rotateAngle; }

        public ROTATE_AXIS getRotateAxis() { return axis; }
    }
}
