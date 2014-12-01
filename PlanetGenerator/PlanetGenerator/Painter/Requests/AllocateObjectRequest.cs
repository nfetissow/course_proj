using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLParserProject
{
    class AllocateObjectRequest :Request
    {
        private Point3D selectedPoint;

        public AllocateObjectRequest(int cameraNum, Point3D selectedPoint)
            : base(cameraNum)
        {
            this.selectedPoint = selectedPoint;
        }

        public Point3D getSelectedPoint() { return selectedPoint; }
    }
}
