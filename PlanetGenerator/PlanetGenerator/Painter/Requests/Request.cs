using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STLParserProject
{
    public abstract class Request
    {
        protected Request(int cameraNum)
        {
            this.cameraNum = cameraNum;
        }

        protected int cameraNum;

        public int getCameraNum() { return cameraNum; }
    }
}
