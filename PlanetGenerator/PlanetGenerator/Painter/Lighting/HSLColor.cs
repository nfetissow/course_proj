using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Painter
{
    public class HSLColor
    {
        int H; // 0..360
        int S, L; // 0..255
        int minL;
        // S=[0..255], L as S too, H = [0..6]
        public HSLColor(int color, int minL)
        {
            int R = ((color & 0x00FF0000) >> 16);
            int G = ((color & 0x0000FF00) >> 8);
            int B = (color & 0x000000FF);
            int Cmax = max(R, G, B), Cmin = min(R, G, B);
            int delta = Cmax - Cmin;

            L = (Cmax + Cmin) / 2;

            if (delta == 0)
            {
                H = 0;
                S = 0;
            }
            else
            {
                if (L == 0)
                    S = 0;
                if (L <= 127)
                    S = (int)(255*(delta / (2*L+0.0)));
                else
                    S = (int)(255*delta / (510 - 2 * L));
            }
            if (Cmax == R)
                H = (int)(60 * (((G - B + 0.0) / delta)));
            else if (Cmax == G)
                H = (int)(60 * ((B - R) / delta)) + 120;
            else if (Cmax == B)
                H = (int)(60 * ((R - G) / delta)) + 240;
            if (H < 0) H += 360;

            if (H < 0) H = 360;
            if (H > 360) H = 0;
            if (S < 0) S = 0;
            if (S > 255) S = 255;

            this.minL = minL;
            
        }

        public HSLColor(HSLColor copy)
        {
            this.H = copy.H;
            this.L = copy.L;
            this.S = copy.S;
            this.minL = copy.minL;
        }

        public void setLightness(int L)
        {
            L = (int)(minL + L * (255 - minL + 0.0) / 255);
            if (L < minL)
                this.L = minL;
            if (L > 255)
                this.L = 255;
            this.L = L;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public int convertToRGB()
        {
            int res = unchecked((int)0xFF000000);
            int C = L > 127 ? ((510 - 2 * L) * S )/255 : (2 * L * S)/255;
            int m = L - C / 2;
            int X = (int)(C * (1 - Math.Abs((H + 0.0) / 60 % 2.0 - 1)) + m);        
            C += m;
            int R, G, B;
            if (H < 60) { R = C; G = X; B = m; }
            else if (H < 120) { R = X; G = C; B = m; }
            else if (H < 180) { R = m; G = C; B = X; }
            else if (H < 240) { R = m; G = X; B = C; }
            else if (H < 300) { R = X; G = m; B = C; }
            else { R = C; G = 0; B = X; }
            if (R > 255) 
                R = 255;
            if (R < 0)
                R = 0;
            if (G > 255) 
                G = 255;
            if (G < 0) 
                G = 0;
            if (B > 255) 
                B = 255;
            if (B < 0)
                B = 0; 
            res = (int)(0x000000 | ((int)R) << 16 | ((int)G) << 8 | ((int)B));
            return res;
        }

        public static int max(params int[] elems)
        {
            if (elems.Length<1)
                return -1;
            int max = elems[0];
            for (int i = 1; i < elems.Length; i++)
                max = max < elems[i] ? elems[i] : max;
            return max;
        }

        public static int min(params int[] elems)
        {
            if (elems.Length < 1)
                return -1;
            int min = elems[0];
            for (int i = 1; i < elems.Length; i++)
                min = min > elems[i] ? elems[i] : min;
            return min;
        }
    }




    struct HSLM
    {
        double H, S, L, minL;
        int color;

        public HSLM(int color)
        {
            this.color = color;
            double R = ((color & 0xFF0000) >> 16) / 255.0;
            double G = ((color & 0x00FF00) >> 8) / 255.0;
            double B = (color & 0x0000FF) / 255.0;

            double MIN = min(R, G, B);
            double MAX = max(R, G, B);
            double delta = MAX - MIN;

            L = 0.5 * (MAX + MIN);

            if (L == 0 || L == 1)
            {
                H = 0; S = 0;
            }
            else
            {
                S = L <= 0.5 ? (delta) / (MAX + MIN) : (delta) / (2 - MAX - MIN);
                if (MAX == R)
                {
                    H = (G - B) / (delta);
                    if (G < B)
                        H += 6;
                }
                else if (MAX == G)
                    H = 2 + (B - R) / (delta);
                else
                    H = 4 + (R - G) / (delta);
                H /= 6;
                //                 H = H < 0 ? H + 6 : H;
                //                 H *= 60;
            }

            if (H < 0) H = 0; if (H > 360) H = 360;
            if (S < 0) S = 0; if (S > 1) S = 1;


            minL = L;
        }

        static double hue2rgb(double p, double q, double t)
        {
            if(t < 0) t += 1;
            if(t > 1) t -= 1;
            if(t < 1.0/6) return p + (q - p) * 6 * t;
            if(t < 1.0/2) return q;
            if(t < 2.0/3) return p + (q - p) * (2.0/3 - t) * 6;
            return p;
        }

        public int fromHSL(double L0)
        {
            this.L = minL + L0 * (1 - minL);
            if (L < 0) L = 0;
            if (L > 1) L = 1;

            double R, G, B;
            int res = 0;

            double q = L < 0.5 ? L * (1 + S) : L + S - L * S;
            double p = 2 * L - q;
            R = hue2rgb(p, q, H + 1.0/3);
            G = hue2rgb(p, q, H);
            B = hue2rgb(p, q, H - 1.0 / 3);


            R *= 255;
            G *= 255;
            B *= 255;
            if (R < 0) R = 0; if (R > 255) R = 255;
            if (G < 0) G = 0; if (G > 255) G = 255;
            if (B < 0) B = 0; if (B > 255) B = 255;

            res = (int)(0xFF000000 | ((int)R) << 16 | ((int)G) << 8 | ((int)B));
            return res;

        }

        public static double min(double a, double b, double c)
        {
            double res = a;
            if (b < res)
                res = b;
            if (c < res)
                res = c;
            return res;
        }

        public static double max(double a, double b, double c)
        {
            double res = a;
            if (b > res)
                res = b;
            if (c > res)
                res = c;
            return res;
        }
    }
}
