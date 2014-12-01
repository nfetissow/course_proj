using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanetGenerator.SphereBuilder;
using System.Drawing;


namespace PlanetGenerator.Painter
{
    class Scene
    {
        private List<IDrawable> figures = new List<IDrawable>();
        public Graphics output { get; set; }

        public void AddFigure(IDrawable figure) 
        {
            figures.Add(figure);
        }

        public void rotate(double  angle)
        {
            foreach(IDrawable d in figures)
            {
                d.rotate(angle);
            }
        }

        public void draw()
        {
            output.Clear(System.Drawing.Color.Black);
            Pen p = new Pen(System.Drawing.Color.White, 1);
            foreach(IDrawable f in figures)
            {
                f.draw(output, p);
            }
        }
    }
}
