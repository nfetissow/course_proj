﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Painter
{
    class DeleteObjectRequest: Request
    {
        public DeleteObjectRequest(int cameraNum)
            : base(cameraNum)
        {
        }
    }
}
