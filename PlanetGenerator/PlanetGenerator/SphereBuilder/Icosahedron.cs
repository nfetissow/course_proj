using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetGenerator.SphereBuilder
{
    class Icosahedron
    {
        public static TriangleMesh generateIcosahedron()
        {
            var phi = (1.0 + Math.Sqrt(5.0)) / 2.0;
            var du = 1.0 / Math.Sqrt(phi * phi + 1.0);
            var dv = phi * du;

            Node[] nodes = {new Node(new Vector(0, +dv, +du)), new Node(new Vector(0, +dv, -du)),new Node(new Vector(0, -dv, +du)), 
                        new Node(new Vector(0, -dv, -du)),new Node(new Vector(+du, 0, +dv)), new Node(new Vector(-du, 0, +dv)),
                        new Node(new Vector(+du, 0, -dv)),new Node(new Vector(-du, 0, -dv)),new Node(new Vector(+dv, +du, 0)),
                        new Node(new Vector(+dv, -du, 0)),new Node(new Vector(-dv, +du, 0)),new Node(new Vector(-dv, -du, 0))};
            Edge[] edges = {
                            new Edge( 0,  1),
		                    new Edge( 0,  4),
		                    new Edge( 0,  5),
		                    new Edge( 0,  8),
		                    new Edge( 0, 10),
		                    new Edge( 1,  6),
		                    new Edge( 1,  7),
		                    new Edge( 1,  8),
		                    new Edge( 1, 10),
		                    new Edge( 2,  3),
		                    new Edge( 2,  4),
		                    new Edge( 2,  5),
		                    new Edge( 2,  9),
		                    new Edge( 2, 11),
		                    new Edge( 3,  6),
		                    new Edge( 3,  7),
		                    new Edge( 3,  9),
		                    new Edge( 3, 11),
		                    new Edge( 4,  5),
		                    new Edge( 4,  8),
		                    new Edge( 4,  9),
		                    new Edge( 5, 10),
		                    new Edge( 5, 11),
		                    new Edge( 6,  7),
		                    new Edge( 6,  8),
		                    new Edge( 6,  9),
		                    new Edge( 7, 10),
		                    new Edge( 7, 11),
		                    new Edge( 8,  9),
		                    new Edge(10, 11),
                        };
            Face[] faces = {
	                        new Face( 0,  1,  8, 0,  7,  3),
		                    new Face( 0,  4,  5, 1, 18,  2),
		                    new Face( 0,  5, 10, 2, 21,  4),
		                    new Face( 0,  8,  4, 3, 19,  1),
		                    new Face( 0, 10,  1, 4,  8,  0),
		                    new Face( 1,  6,  8, 5, 24,  7),
		                    new Face( 1,  7,  6, 6, 23,  5),
		                    new Face( 1, 10,  7, 8, 26,  6),
		                    new Face( 2,  3, 11, 9, 17, 13),
		                    new Face( 2,  4,  9,10, 20, 12),
		                    new Face( 2,  5,  4,11, 18, 10),
		                    new Face( 2,  9,  3,12, 16,  9),
		                    new Face( 2, 11,  5,13, 22, 11),
		                    new Face( 3,  6,  7,14, 23, 15),
		                    new Face( 3,  7, 11,15, 27, 17),
		                    new Face( 3,  9,  6,16, 25, 14),
		                    new Face( 4,  8,  9,19, 28, 20),
		                    new Face( 5, 11, 10,22, 29, 21),
		                    new Face( 6,  9,  8,25, 28, 24),
		                    new Face( 7, 10, 11,26, 29, 27), 
                       };
            
            
            return new TriangleMesh(nodes, edges, faces);
        }

        private static Vector slerp(Vector p0, Vector p1, double t)
        {
	        var omega = Math.Acos(p0.Dot(p1));
            return (p0 * (Math.Sin((1-t)*omega)) + p1 *  Math.Sin(t * omega)) / Math.Sin(omega);   
        }

        delegate int Deleg(int k);





        //----------------------------------------------------------------------------------------------------------------------
        public static TriangleMesh generateSubdividedIcosahedron(int degree)
        {
            var icosahedron = generateIcosahedron();

            var nodes = new List<Node>();
            for (var i = 0; i < icosahedron.nodes.Length; ++i)
            {
                nodes.Add(new Node(icosahedron.nodes[i].p));
            }

    

            var edges = new List<Edge>();
            for (var i = 0; i < icosahedron.edges.Length; ++i)
            {
                var edge = icosahedron.edges[i];
                icosahedron.edges[i].subdivided_e = new List<int>();
                icosahedron.edges[i].subdivided_n = new List<int>();
                var n0 = icosahedron.nodes[icosahedron.edges[i].n[0]];
                var n1 = icosahedron.nodes[icosahedron.edges[i].n[1]];
                Vector p0 = n0.p;
                Vector p1 = n1.p;
                nodes[icosahedron.edges[i].n[0]].e.Add(edges.Count);
                var priorNodeIndex = icosahedron.edges[i].n[0];
                for (var s = 1; s < degree; ++s)
                {
                    var edgeIndex = edges.Count;
                    var nodeIndex = nodes.Count;
                    icosahedron.edges[i].subdivided_e.Add(edgeIndex);
                    icosahedron.edges[i].subdivided_n.Add(nodeIndex);
                    edges.Add(new Edge(priorNodeIndex, nodeIndex));
                    priorNodeIndex = nodeIndex;
                    nodes.Add(new Node(slerp(p0, p1, (double)s / degree), edgeIndex, edgeIndex + 1));//p: slerp(p0, p1, s / degree), e: [ edgeIndex, edgeIndex + 1 ], f: [] });
                }
                icosahedron.edges[i].subdivided_e.Add(edges.Count);
                nodes[icosahedron.edges[i].n[1]].e.Add(edges.Count);
                edges.Add(new Edge(priorNodeIndex, icosahedron.edges[i].n[1]));//{ priorNodeIndex, edge.n[1] ], f: [] });
            }

            var faces = new List<Face>();
            for (var i = 0; i < icosahedron.faces.Length; ++i)
            {
                var face = icosahedron.faces[i];
                var edge0 = icosahedron.edges[face.e[0]];
                var edge1 = icosahedron.edges[face.e[1]];
                var edge2 = icosahedron.edges[face.e[2]];
                var point0 = icosahedron.nodes[face.n[0]].p;
                var point1 = icosahedron.nodes[face.n[1]].p;

                Deleg getEdgeNode0 = (face.n[0] == edge0.n[0])
                    ? (Deleg) ((k) => { return edge0.subdivided_n[k]; }) 
                    : ((k) => { return edge0.subdivided_n[degree - 2 - k];});

                Deleg getEdgeNode1 =  (face.n[1] == edge1.n[0])
                    ? (Deleg) ((k) => { return  edge1.subdivided_n[k]; }) 
                    : ((k) => { return edge1.subdivided_n[degree - 2 - k];});

                Deleg getEdgeNode2 =  (face.n[0] == edge2.n[0])
                    ? (Deleg) ((k) => { return edge2.subdivided_n[k]; }) 
                    : ((k) => { return edge2.subdivided_n[degree - 2 - k];});
                
                var faceNodes =  new List<int>();
                faceNodes.Add(face.n[0]);
                for (var j = 0; j < edge0.subdivided_n.Count; ++j)
                    faceNodes.Add(getEdgeNode0(j));
                faceNodes.Add(face.n[1]);
                for (var s = 1; s < degree; ++s)
                {
                    faceNodes.Add(getEdgeNode2(s - 1));
                    var p0 = nodes[getEdgeNode2(s - 1)].p;
                    var p1 = nodes[getEdgeNode1(s - 1)].p;
                    for (var t = 1; t < degree - s; ++t)
                    {
                        faceNodes.Add(nodes.Count);
                        nodes.Add( new Node(slerp(p0, p1, (double) t/ (degree - s))));
                    }
                    faceNodes.Add(getEdgeNode1(s - 1));
                }
                faceNodes.Add(face.n[2]);

                Deleg getEdgeEdge0 = (face.n[0] == edge0.n[0])
                    ? (Deleg) ((k) => { return edge0.subdivided_e[k]; }) 
                    : ((k) => { return edge0.subdivided_e[degree - 1 - k];});

                Deleg getEdgeEdge1 =  (face.n[1] == edge1.n[0])
                    ? (Deleg) ((k) => { return  edge1.subdivided_e[k]; }) 
                    : ((k) => { return edge1.subdivided_e[degree - 1 - k];});

                Deleg getEdgeEdge2 =  (face.n[0] == edge2.n[0])
                    ? (Deleg) ((k) => { return edge2.subdivided_e[k]; }) 
                    : ((k) => { return edge2.subdivided_e[degree - 1 - k];});

                var faceEdges0 = new List<int>();
                for (var j = 0; j < degree; ++j)
                    faceEdges0.Add(getEdgeEdge0(j));
                var nodeIndex = degree + 1;
                for (var s = 1; s < degree; ++s)
                {
                    for (var t = 0; t < degree - s; ++t)
                    {
                        faceEdges0.Add(edges.Count);
                        var edge = new Edge(faceNodes[nodeIndex], faceNodes[nodeIndex + 1]);
                        nodes[edge.n[0]].e.Add(edges.Count);
                        nodes[edge.n[1]].e.Add(edges.Count);
                        edges.Add(edge);
                        ++nodeIndex;
                    }
                    ++nodeIndex;
                }

                var faceEdges1 = new List<int>();
                nodeIndex = 1;
                for (var s = 0; s < degree; ++s)
                {
                    for (var t = 1; t < degree - s; ++t)
                    {
                        faceEdges1.Add(edges.Count);
                        var edge = new Edge(faceNodes[nodeIndex], faceNodes[nodeIndex + degree - s]);
                        nodes[edge.n[0]].e.Add(edges.Count);
                        nodes[edge.n[1]].e.Add(edges.Count);
                        edges.Add(edge);
                        ++nodeIndex;
                    }
                    faceEdges1.Add(getEdgeEdge1(s));
                    nodeIndex += 2;
                }

                var faceEdges2 = new List<int>();;
                nodeIndex = 1;
                for (var s = 0; s < degree; ++s)
                {
                    faceEdges2.Add(getEdgeEdge2(s));
                    for (var t = 1; t < degree - s; ++t)
                    {
                        faceEdges2.Add(edges.Count);
                        var edge = new Edge(faceNodes[nodeIndex], faceNodes[nodeIndex + degree - s + 1]);
                        nodes[edge.n[0]].e.Add(edges.Count);
                        nodes[edge.n[1]].e.Add(edges.Count);
                        edges.Add(edge);
                        ++nodeIndex;
                    }
                    nodeIndex += 2;
                }

                nodeIndex = 0;
                int edgeIndex = 0;
                for (var s = 0; s < degree; ++s)
                {
                    for (int t = 1; t < degree - s + 1; ++t)
                    {
                        var subFace = new Face( faceNodes[nodeIndex], faceNodes[nodeIndex + 1], faceNodes[nodeIndex + degree - s + 1],
                            faceEdges0[edgeIndex], faceEdges1[edgeIndex], faceEdges2[edgeIndex]);
                        nodes[subFace.n[0]].f.Add(faces.Count);
                        nodes[subFace.n[1]].f.Add(faces.Count);
                        nodes[subFace.n[2]].f.Add(faces.Count);
                        edges[subFace.e[0]].f.Add(faces.Count);
                        edges[subFace.e[1]].f.Add(faces.Count);
                        edges[subFace.e[2]].f.Add(faces.Count);
                        faces.Add(subFace);
                        ++nodeIndex;
                        ++edgeIndex;
                    }
                    ++nodeIndex;
                }

                nodeIndex = 1;
                edgeIndex = 0;
                for (var s = 1; s < degree; ++s)
                {
                    for (int t = 1; t < degree - s + 1; ++t)
                    {
                        var subFace = new Face( faceNodes[nodeIndex], faceNodes[nodeIndex + degree - s + 2], faceNodes[nodeIndex + degree - s + 1],
                            faceEdges2[edgeIndex + 1], faceEdges0[edgeIndex + degree - s + 1], faceEdges1[edgeIndex]);
                        nodes[subFace.n[0]].f.Add(faces.Count);
                        nodes[subFace.n[1]].f.Add(faces.Count);
                        nodes[subFace.n[2]].f.Add(faces.Count);
                        edges[subFace.e[0]].f.Add(faces.Count);
                        edges[subFace.e[1]].f.Add(faces.Count);
                        edges[subFace.e[2]].f.Add(faces.Count);
                        faces.Add(subFace);
                        ++nodeIndex;
                        ++edgeIndex;
                    }
                    nodeIndex += 2;
                    edgeIndex += 1;
                }
            }
            return new TriangleMesh(nodes.ToArray(), edges.ToArray(), faces.ToArray()); 
        }


    }
}
