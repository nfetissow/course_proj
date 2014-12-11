using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using PlanetGenerator.PlanetGeneration;
using Painter;

namespace PlanetGenerator.SphereBuilder
{
    interface IDrawable
    {
        void draw(Graphics g, Pen p);
        void rotate(double angle);
    }


    public class Node
    {
        public Vector p;
        public List<int> e;
        public List<int> f;
        public Node(Vector v)
        {
            p = v;
            e = new List<int>();
            f = new List<int>();
        }

        public Node(Vector v, int e1, int e2)
        {
            p = v;
            e = new List<int>();
            e.Add(e1); e.Add(e2);
            f = new List<int>();
        }
    }

    public class Edge
    {
        public List<int> n;
        public List<int> f;

        public List<int> subdivided_n;
        public List<int> subdivided_e; 
        public Edge(int a, int b)
        {
            n = new List<int>();
            n.Add(a); n.Add(b);
            
            f = new List<int>();

            subdivided_e = null;
            subdivided_n = null;
        }
    }

    public class Face
    {
        public List<int> n;
        public List<int> e;
        public Vector centroid;
        public Face(int n1, int n2, int n3, int e1, int e2, int e3)
        {
            e = new List<int>();
            e.Add(e1); e.Add(e2); e.Add(e3);
            n = new List<int>();
            n.Add(n1); n.Add(n2); n.Add(n3); 
        }
    }

    public class Corner
    {
        public int id;
        public Vector pos;
        public Corner[] corners;
        public Border[] borders;
        public Tile[] tiles;
        public double area;
        public double distanceToRoot;
        public double distanceToPlateBoundary;
        public double pressure;
        public double shear;
        public double elevation;
        public bool betweenPlates;
        public Vector airCurrent;
        public double airCurrentSpeed;
        public double[] airCurrentOutflows;

        public double airHeat;
        public double heat;
        public double maxHeat;
        public double heatAbsorption;
        public double newAirHeat;

        public double airMoisture;
        public double newAirMoisture;
        public double precipitation;
        public double precipitationRate;
        public double maxPrecipitation;
        public double moisture;

        public double temperature;
        public Corner(int id, Vector position, int cornerCount, int borderCount, int tileCount)
        {
            this.id = id;
            pos = position;
            corners = new Corner[cornerCount];
            borders = new Border[borderCount];
            tiles = new Tile[tileCount];
            distanceToRoot = Double.NaN;
            distanceToPlateBoundary = Double.NaN;
            elevation = Double.NaN;
            shear = Double.NaN;
            pressure = Double.NaN;
            betweenPlates = false;
            airCurrentSpeed = Double.NaN;
            airHeat = Double.NaN;
            heat = Double.NaN;
            area = Double.NaN;
            maxHeat = Double.NaN;
            heatAbsorption = Double.NaN;
            newAirHeat = Double.NaN;
            temperature = Double.NaN;

        }   
        public Vector vectorTo(Corner corner)
        {
            return corner.pos - this.pos;
        }
    }

    public class Border
    {
        public int id;
        public Vector midpoint;
        public Corner[] corners;
        public Border[] borders;
        public Tile[] tiles;
        public bool betweenPlates;

        public Border(int id, int cornerCount, int borderCount, int tileCount)
        {
            midpoint = null;
            this.id = id;
            corners = new Corner[cornerCount];
            borders = new Border[borderCount];
            tiles = new Tile[tileCount];
            betweenPlates = false;
        }

        public Corner oppositeCorner(Corner corner)
        {
            return (this.corners[0].Equals(corner)) ? this.corners[1] : this.corners[0];
        }

        public Tile oppositeTile(Tile tile)
        {
            return (this.tiles[0].Equals(tile)) ? this.tiles[1] : this.tiles[0];
        }

        public double Length()
        {
            return (corners[0].pos - corners[1].pos).Magnitude();
        }
        
    }

    public class Tile
    {
        public int id;
        public double D;
        public Vector pos;
        public Vector averagePos;
        public Corner[] corners;
        public Border[] borders;
        public Tile[] tiles;
        public Vector normal;
        public Color color;
        public Plate plate;
        public double area;
        public double elevation;
        public double temperature;
        public double moisture;
        public string biome;
        public Tile(int id, Vector position, int cornerCount, int borderCount, int tileCount)
        {
            this.id = id;
            pos = position;
            averagePos = null;
            corners = new Corner[cornerCount];
            borders = new Border[borderCount];
            tiles = new Tile[tileCount];
            normal = null;
            D = 0;
            color = Color.Black;
            plate = null;
            temperature = Double.NaN;
        }

        public void sortCorners()
        {
            Corner temp;
            trans tr = makeTrans();
            for (int i = 0; i < corners.Length; i++)
            {
                for (int j = i + 1; j < corners.Length; j++)
                {
                    if (less(tr(corners[j].pos), tr(corners[i].pos), tr(averagePos)))
                    {
                        temp = corners[i];
                        corners[i] = corners[j];
                        corners[j] = temp;
                    }
                }
            }
        }

        private trans makeTrans()
        {
            trans curr;
            double deltaXmax , deltaYmax, deltaZmax;
            deltaXmax = deltaYmax = deltaZmax = -1;
            double tempX, tempY, tempZ;
            for(int i = 0; i < corners.Length; ++i)
            {
                for(int j = i + 1; j < corners.Length -1; ++j)
                {
                    tempX = Math.Abs(corners[i].pos.x - corners[j].pos.x);
                    tempY = Math.Abs(corners[i].pos.y - corners[j].pos.y);
                    tempZ = Math.Abs(corners[i].pos.z - corners[j].pos.z);
                    if (tempX > deltaXmax) { deltaXmax = tempX; }
                    if (tempY > deltaYmax) { deltaYmax = tempY; }
                    if (tempZ > deltaZmax) { deltaZmax = tempZ; }
                }
            }
            if (deltaXmax < deltaYmax && deltaXmax < deltaZmax)
            {
                curr = (Vector v) =>
                    {
                        return new Vector(v.y, v.z, 0);
                    };
            }
            else if (deltaYmax < deltaXmax && deltaYmax < deltaZmax)
            {
                curr = (Vector v) =>
                {
                    return new Vector(v.x, v.z, 0);
                };
            }
            else //if (deltaZmax < deltaYmax && deltaZmax < deltaXmax)
            {
                curr = (Vector v) =>
                {
                    return new Vector(v.x, v.y, 0);
                };
            } 

            return curr;
        }

        delegate Vector trans(Vector v);
        static private bool less(Vector a, Vector b, Vector center)
        {
           if(a.x == b.x && a.y == b.y)
           {
               int s = 5;
           }
            if (a.x - center.x >= 0 && b.x - center.x < 0)
                return true;
            if (a.x - center.x < 0 && b.x - center.x >= 0)
                return false;
            if (a.x - center.x == 0 && b.x - center.x == 0)
            {
                if (a.y - center.y >= 0 || b.y - center.y >= 0)
                    return a.y > b.y;
                return b.y > a.y;
            }

            // compute the cross product of vectors (center -> a) x (center -> b)
            double det = (a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y);
            if (det < 0)
                return true;
            if (det > 0)
                return false;

            // points a and b are on the same line from the center
            // check which point is closer to the center
            double d1 = (a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y);
            double d2 = (b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y);
            return d1 > d2;
        }
    }




    public class TriangleMesh : IDrawable
    {
        public Node[] nodes { get; set; }
        public Edge[] edges { get; set; }
        public Face[] faces { get; set; }

        public TriangleMesh(Node[] n, Edge[] e, Face[] f)
        {
            nodes = n;
            edges = e;
            faces = f;
        }

        void IDrawable.draw(Graphics g, Pen p)
        {
            int d = 150;
            int sum = 300;
            foreach (Edge e in this.edges)
            {
                g.DrawLine(p, sum + (int)(this.nodes[e.n[0]].p.x * d), sum + (int)(this.nodes[e.n[0]].p.y * d), sum + (int)(this.nodes[e.n[1]].p.x * d), sum + (int)(this.nodes[e.n[1]].p.y * d));
                
            }
        }

        void IDrawable.rotate(double angle)
        {
            foreach(Node n in nodes)
            {
                n.p.Rotate(angle);
            }
        }
    }
}
