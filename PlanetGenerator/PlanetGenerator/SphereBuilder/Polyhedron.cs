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
        public static PolyhedronMesh getDualPolyhedron(TriangleMesh icosahedron)
        {
            Corner[] corners = new Corner[icosahedron.faces.Length];
            Border[] borders = new Border[icosahedron.edges.Length];
            Tile[] tiles = new Tile[icosahedron.nodes.Length];


            Face f;
            for(int i = 0; i < icosahedron.faces.Length; ++i) 
            {
                f = icosahedron.faces[i];
                corners[i] = new Corner(i, centroid(f, icosahedron.nodes), f.e.Count, f.e.Count, f.n.Count);
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
                tiles[i] = new Tile(i, new Vector(n.p), n.f.Count, n.e.Count, n.e.Count);
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
                
			    for (var j = 0; j < node.e.Count; ++j)
			    {
				    var border = borders[node.e[j]];
				    if (border.tiles[0].Equals(tile))
				    {
                        bool found = false;
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
                            found = true;
						    break;
					    }
                        if (!found)
                        {
                            int a = 9;
                        }
				    }
				    else
				    {
                        bool found = false;
					    for (var k = 0; k < tile.corners.Length; ++k)
					    {
						    var corner0 = tile.corners[k];
						    var corner1 = tile.corners[(k + 1) % tile.corners.Length];
						    if (border.corners[0].Equals(corner0) && border.corners[1].Equals(corner1))
						    {
							    border.corners[1] = corner0;
							    border.corners[0] = corner1;
						    }
						    else if (!border.corners[1].Equals(corner0) || !border.corners[0].Equals(corner1.id))
						    {
							    continue;
						    }
						    tile.borders[k] = border;
						    tile.tiles[k] = border.oppositeTile(tile);
                            found = true;
						    break;
					    }
                        if (!found)
                        {
                            int a = 9;
                        }

				    }
			    }
                
			    tile.averagePos = new Vector(0, 0, 0);
			    for (var j = 0; j < tile.corners.Length; ++j)
			    {
                    tile.averagePos += tile.corners[j].pos;
			    }
			    tile.averagePos /= tile.corners.Length;
			
			    double maxDistanceToCorner = 0;
			    for (var j = 0; j < tile.corners.Length; ++j)
			    {
				    maxDistanceToCorner = Math.Max(maxDistanceToCorner, 
                        (tile.corners[j].pos - tile.averagePos).Magnitude());
                        
			    }
                tile.maxDistanceToCorner = maxDistanceToCorner;
                tile.normal = tile.pos.Normalize();
               // tile.color = new RayTracer.Color(rng.NextDouble(), rng.NextDouble(), rng.NextDouble());
                tile.D = -tile.pos.Dot(tile.normal);
                tiles[i] = tile;
            }
            #endregion

            var mesh =  new PolyhedronMesh(tiles, borders, corners);
            return verifyIntegrity(mesh);
        }

        public static PolyhedronMesh verifyIntegrity(PolyhedronMesh mesh)
        {
            //foreach(Tile t in mesh.tiles)
            //{
            //    int adjTileIndex = 0;
            //    foreach(Border b in t.borders)
            //    {
            //        foreach(Tile conT in mesh.tiles)
            //        {
            //            if(!conT.Equals(t))
            //            {
            //                foreach(Border conB in conT.borders)
            //                {
            //                    if(conB.Equals(b))
            //                    {
            //                        //t.tiles[adjTileIndex++] = conT;
            //                        adjTileIndex++;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    if(adjTileIndex < t.borders.Length)
            //    {
            //        throw new FormatException("Not enough adjacent tiles for selected tile!");
            //    }

            //}
            int[] tilesIndexes = new int[mesh.tiles.Length];
            foreach(Border b in mesh.borders)
            {
                b.tiles[0].tiles[tilesIndexes[b.tiles[0].id]++] = b.tiles[1];
                b.tiles[1].tiles[tilesIndexes[b.tiles[1].id]++] = b.tiles[0];
            }
            return mesh;
        }


        static Vector centroid(Face f, Node[] nodes)
        {
            return calculateFaceCentroid(nodes[f.n[0]].p, nodes[f.n[1]].p, nodes[f.n[2]].p);
        }

        public static Vector calculateFaceCentroid(Vector pa, Vector pb, Vector pc)
        {
            var vabHalf = (pb - pa) / 2;
            var pabHalf = pa + vabHalf;
            var centroid = (pc - pabHalf) / 3 + pabHalf;
            return centroid;
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
