using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace PlanetGenerator.SphereBuilder
{
    class Polyhedron
    {
        public delegate int determineColor(Tile tile);

        private static Dictionary<string, int> biomes;

        public static determineColor showBiomes = (Tile tile) =>
        {
            if(biomes == null)
            {
                biomes = new Dictionary<string, int>();
                biomes.Add("ocean", 0x0000FB);
                biomes.Add("deepOcean", 0x0000CD);
                biomes.Add("veryDeepOcean", 0x0000AB);
                biomes.Add("oceanGlacier", Color.White.ToArgb());
                biomes.Add("desert", 0xE0E03E);
                biomes.Add("rainForest", 0x0EA313);
                biomes.Add("rocky", 0x705954);
                biomes.Add("plains", 0x054A02);
                biomes.Add("swamp", 0x466E45);
                biomes.Add("grassland", 0x099817);
                biomes.Add("deciduousForest", 0x429809);
                biomes.Add("tundra", 0x6F8E82);
                biomes.Add("landGlacier", Color.White.ToArgb());
                biomes.Add("coniferForest", 0x15A924);
                biomes.Add("mountain", 0x545F5B);
                biomes.Add("snowyMountain", Color.White.ToArgb());
                biomes.Add("land", 0x66B564);
            }
            if (biomes.ContainsKey(tile.biome))
                return biomes[tile.biome];
            else
                return 0xFF0000;
        };

        public static determineColor random = (Tile tile) =>
        {
            Random rng = new Random();
            return rng.Next(0, 0xFFFFFF);
        };

        public static determineColor showElevation = (Tile tile) =>
        {
            if(tile.elevation < 0)
            {
                if (tile.elevation > -0.3) return 0x0000FB;
                else if (tile.elevation > -0.4) return 0x0000CD;
                else return 0x0000AB;
                    
            } 
            else if (tile.elevation < 0.2)
            {
                return 0x7a5230;
            } else if(tile.elevation < 0.4)
            {
                return 0x614126;
            }
            else
            {
                return 0x49311c;
            }
        };

        public static determineColor showPlates = (Tile tile) =>
        {
            return tile.plate.color;
        };


        public static PolyhedronMesh getDualPolyhedron(TriangleMesh icosahedron)
        {
            Corner[] corners = new Corner[icosahedron.faces.Length];
            Border[] borders = new Border[icosahedron.edges.Length];
            Tile[] tiles = new Tile[icosahedron.nodes.Length];


            Face f;
            for(int i = 0; i < icosahedron.faces.Length; ++i) 
            {
                f = icosahedron.faces[i];
                corners[i] = new Corner(i, f.centroid * 1000, f.e.Count, f.e.Count, f.n.Count);
            }

            Edge e;
            for (int i = 0; i < icosahedron.edges.Length; ++i )
            {
                e = icosahedron.edges[i];
                borders[i] = new Border(i, 2, 4, 2);
            }

            Node n;
            for(int i = 0; i < icosahedron.nodes.Length; ++i)
            {
                n = icosahedron.nodes[i];
                tiles[i] = new Tile(i, new Vector(n.p) * 1000, n.f.Count, n.e.Count, n.e.Count);
            }

            #region corners
            for (var i = 0; i < corners.Length; ++i)
		    {
			    var corner = corners[i];
			    var face = icosahedron.faces[i];

			    for (var j = 0; j < face.e.Count; ++j)
			    {
				    corner.borders[j] = borders[face.e[j]];
			    }
			    for (var j = 0; j < face.n.Count; ++j)
			    {
				     corner.tiles[j] = tiles[face.n[j]];
			    }
            }
            #endregion            

            #region borders
            for (var i = 0; i < borders.Length; ++i)
		    {
			    var border = borders[i];
			    var edge = icosahedron.edges[i];
			    var averageCorner = new Vector(0, 0, 0);
			    int num = 0;
			    for (var j = 0; j < edge.f.Count; ++j)
			    {
                    var corner = corners[edge.f[j]];
                    averageCorner = averageCorner + corner.pos;
				    border.corners[j] = corner;
				    for (var k = 0; k < corner.borders.Length; ++k)
				    {
					    if (corner.borders[k].Equals(border)) border.borders[num++] = corner.borders[k];
				    }
			    }
			    border.midpoint = averageCorner * (1 / border.corners.Length);
			    for (var j = 0; j < edge.n.Count; ++j)
			    {
				    border.tiles[j] = tiles[edge.n[j]];
			    }
            }
            #endregion

            for (var i = 0; i < corners.Length; ++i)
            {
                var corner = corners[i];
                for (var j = 0; j < corner.borders.Length; ++j)
                {
                    corner.corners[j] = corner.borders[j].oppositeCorner(corner);
                }
            }
            
            //------------------------------
            //Random rng = new Random();
            #region tiles

            for (var i = 0; i < tiles.Length; ++i)
		    {
			    var tile = tiles[i];


			    var node = icosahedron.nodes[i];
                for (var j = 0; j < node.f.Count; ++j)
                {
                    tile.corners[j] = corners[node.f[j]];
                }


                tile.averagePos = new Vector(0, 0, 0);
                for (var j = 0; j < tile.corners.Length; ++j)
                {
                    tile.averagePos += tile.corners[j].pos;
                }
                tile.averagePos /= tile.corners.Length;

                //tile.sortCorners();
                
			    for (var j = 0; j < node.e.Count; ++j)
			    {
				    var border = borders[node.e[j]];
				    if (border.tiles[0].Equals(tile))
				    {
					    for (var k = 0; k < tile.corners.Length; ++k)
					    {
						    var corner0 = tile.corners[k];
						    var corner1 = tile.corners[(k + 1) % tile.corners.Length];
						    if (border.corners[1].Equals(corner0) && border.corners[0].Equals(corner1))
						    {
							    border.corners[0] = corner0;
							    border.corners[1] = corner1;
						    }
						    else if (!border.corners[0].Equals(corner0) || !border.corners[1].Equals(corner1))
						    {
							    continue;
						    }
						    tile.borders[k] = border;
						    tile.tiles[k] = border.oppositeTile(tile);
						    break;
					    }
				    }
				    else
				    {
                        for (var k = 0; k < tile.corners.Length; ++k)
                        {
                            var corner0 = tile.corners[k];
                            var corner1 = tile.corners[(k + 1) % tile.corners.Length];
                            if (border.corners[0].Equals(corner0) && border.corners[1].Equals(corner1))
                            {
                                border.corners[0] = corner0;
                                border.corners[1] = corner1;
                            }
                            else if (!border.corners[1].Equals(corner0) || !border.corners[0].Equals(corner1))
                            {
                                continue;
                            }
                            tile.borders[k] = border;
                            tile.tiles[k] = border.oppositeTile(tile);
                            break;
                        }
				    }
			    }



                double area = 0;
                for (int j = 0; j < tile.borders.Length; ++j)
                {
                    area += calculateTriangleArea(tile.pos, tile.borders[j].corners[0].pos, tile.borders[j].corners[1].pos);
                }
                tile.area = area;
                tile.normal = tile.pos.Normalize();
                tile.D = -tile.pos.Dot(tile.normal);
            }
            #endregion

            foreach(Corner corner in corners)
            {
                corner.area = 0;
                foreach(Tile t in corner.tiles)
                {
                    corner.area += t.area / t.corners.Length;
                }
            }

            var mesh =  new PolyhedronMesh(tiles, borders, corners);
            return mesh;//verifyIntegrity(mesh);
        }

        private static double calculateTriangleArea(Vector p1, Vector p2, Vector p3)
        {
            double A = (p1 - p2).Magnitude();
            double B = (p2 - p3).Magnitude();
            double C = (p3 - p1).Magnitude();
            double p = (A + B + C) / 2;
            return Math.Sqrt(p * (p - A) * (p - B) * (p - C));
        }
        
    }

    public class PolyhedronMesh:  IDrawable
    {
        public Tile[] tiles;
        public Border[] borders;
        public Corner[] corners;

        public Vector Position { get; set; }
        public double Radius { get; set; }


        public PolyhedronMesh(Tile[] t, Border[] b, Corner[] c)
        {
            tiles = t;
            borders = b;
            corners = c;
            Position = new Vector(0, 0, 0);
            Radius = 2;
        }
        #region IDrawable implementation
        void IDrawable.draw(Graphics g, Pen p)
        {
            int mult = 400;
            foreach(Border b in borders)
            {
                g.DrawLine(p, 300 + (int)(b.corners[0].pos.x * mult), 300 + (int)(b.corners[0].pos.y * mult), 300 + (int)(b.corners[1].pos.x * mult), 300 + (int)(b.corners[1].pos.y * mult));
            }
            //foreach(Corner c in corners)
            //{
            //    g.DrawRectangle(p, 300 + (int)(c.pos.x * 50), 300 + (int)(c.pos.y * 50), 1, 1);
            //}
        }

        void IDrawable.rotate(double angle)
        {
            foreach(Tile t in tiles)
            {
                t.pos.Rotate(angle);
                //t.averagePos.Rotate(angle);
            }
            foreach(Corner c in corners)
            {
                c.pos.Rotate(angle);
            }
        }
        #endregion

       
    }
}
