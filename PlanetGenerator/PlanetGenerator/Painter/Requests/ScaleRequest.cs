using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Painter
{
    enum SCALE_SIGN {PLUS, MINUS}
    class ScaleRequest: Request
    {
        private SCALE_SIGN sign;
        private double koef;

        public ScaleRequest(int cameraNum, SCALE_SIGN sign, double koef)
            : base(cameraNum)
        {
            this.sign = sign;
            this.koef = koef;
            this.koef = (sign == SCALE_SIGN.PLUS) ? 1 + koef : 1 - koef;
        }

        public double getKoef() { return koef; }
        public SCALE_SIGN getScaleSign() { return sign; }
    }
}
