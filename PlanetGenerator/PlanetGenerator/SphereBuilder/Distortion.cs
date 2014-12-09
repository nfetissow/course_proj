using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetGenerator.SphereBuilder
{
    class Distortion
    {
        private static int getEdgeOppositeFaceIndex(Edge edge, int faceIndex)
        {
    	    if (edge.f[0] == faceIndex) return edge.f[1];
	        if (edge.f[1] == faceIndex) return edge.f[0];
            throw new ArgumentOutOfRangeException("faceIndex");
        }
        private static int getFaceOppositeNodeIndex(Face face, Edge edge)
        {
	        if (face.n[0] != edge.n[0] && face.n[0] != edge.n[1]) return 0;
	        if (face.n[1] != edge.n[0] && face.n[1] != edge.n[1]) return 1;
	        if (face.n[2] != edge.n[0] && face.n[2] != edge.n[1]) return 2;
            throw new ArgumentOutOfRangeException("edge");
        }
        private static int findNextFaceIndex(TriangleMesh mesh, int nodeIndex, int faceIndex)
        {
	        var node = mesh.nodes[nodeIndex];
	        var face = mesh.faces[faceIndex];
	        var nodeFaceIndex = face.n.IndexOf(nodeIndex);
	        var edge = mesh.edges[face.e[(nodeFaceIndex + 2) % 3]];
	        return getEdgeOppositeFaceIndex(edge, faceIndex);
        }

        private delegate bool Predicate(Node oldNode0, Node oldNode1, Node newNode0, Node newNode1);

        private static bool conditionalRotateEdge(TriangleMesh mesh, int edgeIndex, Predicate predicate)
        {
	        var edge = mesh.edges[edgeIndex];
	        var face0 = mesh.faces[edge.f[0]];
	        var face1 = mesh.faces[edge.f[1]];
	        var farNodeFaceIndex0 = getFaceOppositeNodeIndex(face0, edge);
	        var farNodeFaceIndex1 = getFaceOppositeNodeIndex(face1, edge);
	        var newNodeIndex0 = face0.n[farNodeFaceIndex0];
	        var oldNodeIndex0 = face0.n[(farNodeFaceIndex0 + 1) % 3];
	        var newNodeIndex1 = face1.n[farNodeFaceIndex1];
	        var oldNodeIndex1 = face1.n[(farNodeFaceIndex1 + 1) % 3];
	        var oldNode0 = mesh.nodes[oldNodeIndex0];
	        var oldNode1 = mesh.nodes[oldNodeIndex1];
	        var newNode0 = mesh.nodes[newNodeIndex0];
	        var newNode1 = mesh.nodes[newNodeIndex1];
	        var newEdgeIndex0 = face1.e[(farNodeFaceIndex1 + 2) % 3];
	        var newEdgeIndex1 = face0.e[(farNodeFaceIndex0 + 2) % 3];
	        var newEdge0 = mesh.edges[newEdgeIndex0];
	        var newEdge1 = mesh.edges[newEdgeIndex1];
	
	        if (!predicate(oldNode0, oldNode1, newNode0, newNode1)) return false;
            oldNode0.e.RemoveAt(oldNode0.e.IndexOf(edgeIndex));
            oldNode1.e.RemoveAt(oldNode1.e.IndexOf(edgeIndex));
            newNode0.e.Add(edgeIndex);
            newNode1.e.Add(edgeIndex);

	        edge.n[0] = newNodeIndex0;
	        edge.n[1] = newNodeIndex1;

            newEdge0.f.RemoveAt(newEdge0.f.IndexOf(edge.f[1]));
            newEdge1.f.RemoveAt(newEdge1.f.IndexOf(edge.f[0]));
            newEdge0.f.Add(edge.f[0]);
            newEdge1.f.Add(edge.f[1]);

            oldNode0.f.RemoveAt(oldNode0.f.IndexOf(edge.f[1]));
            oldNode1.f.RemoveAt(oldNode1.f.IndexOf(edge.f[0]));
	        newNode0.f.Add(edge.f[1]);
	        newNode1.f.Add(edge.f[0]);
	
	        face0.n[(farNodeFaceIndex0 + 2) % 3] = newNodeIndex1;
	        face1.n[(farNodeFaceIndex1 + 2) % 3] = newNodeIndex0;

	        face0.e[(farNodeFaceIndex0 + 1) % 3] = newEdgeIndex0;
	        face1.e[(farNodeFaceIndex1 + 1) % 3] = newEdgeIndex1;
	        face0.e[(farNodeFaceIndex0 + 2) % 3] = edgeIndex;
	        face1.e[(farNodeFaceIndex1 + 2) % 3] = edgeIndex;
	
	        return true;
        }

        public static bool distortMesh(TriangleMesh mesh, int degree, DoubleRandom random)
        {
	        var totalSurfaceArea = 4 * Math.PI;
	        var idealFaceArea = totalSurfaceArea / mesh.faces.Length;
	        var idealEdgeLength = Math.Sqrt(idealFaceArea * 4 / Math.Sqrt(3));
	        var idealFaceHeight = idealEdgeLength * Math.Sqrt(3) / 2;

	        Predicate rotationPredicate = (oldNode0, oldNode1, newNode0, newNode1) =>
	            {
		            if (newNode0.f.Count >= 7 ||
			            newNode1.f.Count >= 7 ||
			            oldNode0.f.Count <= 5 ||
			            oldNode1.f.Count <= 5) return false;
		            var oldEdgeLength = (oldNode0.p - oldNode1.p).Magnitude();
		            var newEdgeLength = (newNode0.p - newNode1.p).Magnitude();
		            var ratio = oldEdgeLength / newEdgeLength;
		            if (ratio >= 2 || ratio <= 0.5) return false;
		            Vector v0 = (oldNode1.p - oldNode0.p)/(oldEdgeLength);
		            Vector v1 = (newNode0.p - oldNode0.p).Normalize();
		            Vector v2 = (newNode1.p - oldNode0.p).Normalize();
		            if (v0.Dot(v1) < 0.2 || v0.Dot(v2) < 0.2) return false;
		            v0 = v0 * (-1);
		            var v3 = (newNode0.p -oldNode1.p).Normalize();
		            var v4 = (newNode1.p - oldNode1.p).Normalize();
		            if (v0.Dot(v3) < 0.2 || v0.Dot(v4) < 0.2) return false;
		            return true;
	            };
            for(int i = 0; i < degree; ++i)
            {
                int consecutiveFailedAttempts = 0;
                int edgeIndex = random.nextExclusive(0, mesh.edges.Length);
                //Здесь был excluding random!!
                while (!conditionalRotateEdge(mesh, edgeIndex, rotationPredicate))
                {
	                if ( ++consecutiveFailedAttempts >= mesh.edges.Length) return false;
	                edgeIndex = (edgeIndex + 1) % mesh.edges.Length;
                }
            }
	        return true;
        }

        public static double relaxMesh(TriangleMesh mesh, double multiplier)
        {
	        var totalSurfaceArea = 4 * Math.PI;
	        var idealFaceArea = totalSurfaceArea / mesh.faces.Length;
	        var idealEdgeLength = Math.Sqrt(idealFaceArea * 4 / Math.Sqrt(3));
	        var idealDistanceToCentroid = idealEdgeLength * Math.Sqrt(3) / 3 * 0.9;

            Vector[] pointShifts = new Vector[mesh.nodes.Length];
            for (var i = 0; i < mesh.nodes.Length; ++i)
            {
                pointShifts[i] = new Vector(0,0,0);
            }
	        
            for(int i = 0; i <mesh.faces.Length; ++i)
            {
                var face = mesh.faces[i];
		        var n0 = mesh.nodes[face.n[0]];
		        var n1 = mesh.nodes[face.n[1]];
		        var n2 = mesh.nodes[face.n[2]];
		        var p0 = n0.p;
		        var p1 = n1.p;
		        var p2 = n2.p;
		        var centroid = calculateFaceCentroid(p0, p1, p2).Normalize();
		        Vector v0 = centroid - p0;
		        Vector v1 = centroid - p1;
		        Vector v2 = centroid - p2;
		        var length0 = v0.Magnitude();
		        var length1 = v1.Magnitude();
		        var length2 = v2.Magnitude();
                
		        v0 = v0*(multiplier * (length0 - idealDistanceToCentroid) / length0);
		        v1 = v1*(multiplier * (length1 - idealDistanceToCentroid) / length1);
		        v2 = v2*(multiplier * (length2 - idealDistanceToCentroid) / length2);
		        pointShifts[face.n[0]] = pointShifts[face.n[0]] + v0;
		        pointShifts[face.n[1]] = pointShifts[face.n[1]] + v1;
		        pointShifts[face.n[2]] = pointShifts[face.n[2]] + v2;
            }




            var origin = new Vector(0, 0, 0);
            for (var i = 0; i < mesh.nodes.Length; ++i)
            {
                origin = mesh.nodes[i].p;
                pointShifts[i] = mesh.nodes[i].p + ProjectPoint(pointShifts[i], mesh.nodes[i].p, origin).Normalize();
            }
                       
            var rotationSupressions = new double[mesh.nodes.Length];
	
            for(int i = 0; i < mesh.edges.Length; ++i) 
            {
                var edge = mesh.edges[i];
		        var oldPoint0 = mesh.nodes[edge.n[0]].p;
		        var oldPoint1 = mesh.nodes[edge.n[1]].p;
		        var newPoint0 = pointShifts[edge.n[0]];
		        var newPoint1 = pointShifts[edge.n[1]];
		        var oldVector = (oldPoint1 - oldPoint0).Normalize();
		        var newVector = (newPoint1 - newPoint0).Normalize();
		        double suppression = (1 - oldVector.Dot(newVector)) * 0.5;

                //Сравенение даблов и интов, обрати внимание!
		        rotationSupressions[edge.n[0]] = Math.Max(rotationSupressions[edge.n[0]], suppression);
		        rotationSupressions[edge.n[1]] = Math.Max(rotationSupressions[edge.n[1]], suppression);
            }

	        
	
	        double totalShift = 0;
	        
		    for (var i = 0; i < mesh.nodes.Length; ++i)
		    {
			    var node = mesh.nodes[i];
			    //var point = node.p;
                var delta = node.p.Clone();
                node.p = Vector.Lerp(node.p, pointShifts[i], 1 - Math.Sqrt(rotationSupressions[i])).Normalize();
                
                //point = point.Normalize();
                delta = delta - node.p;
                
			    totalShift += delta.Magnitude();
		    }
	        
	
	        return totalShift;
        }


        public static void distortAndRelaxMesh(TriangleMesh mesh, double distortionRate, DoubleRandom random)
        {
            var totalDistortion = Math.Ceiling(mesh.edges.Length * distortionRate);
            for(int remainingIterations = 6; remainingIterations > -1; --remainingIterations)
            {
                int iterationDistortion = (int) Math.Floor(totalDistortion / remainingIterations);
                totalDistortion -= iterationDistortion;
                distortMesh(mesh, iterationDistortion, random);
                relaxMesh(mesh, 0.5);
            }

            var averageNodeRadius = Math.Sqrt(4 * Math.PI / mesh.nodes.Length);
		    var minShiftDelta = averageNodeRadius / 50000 * mesh.nodes.Length;
		    var maxShiftDelta = averageNodeRadius / 50 * mesh.nodes.Length;

            double priorShift;
            double currentShift = relaxMesh(mesh, 0.5);
            double shiftDelta;
            int numOfIterations = 0;
            do
            {
                if (numOfIterations++ > 1000)
                    break;
                priorShift = currentShift;
                currentShift = relaxMesh(mesh, 0.5);
                shiftDelta = Math.Abs(currentShift - priorShift);
            } while (shiftDelta >= minShiftDelta);

            for (var i = 0; i < mesh.faces.Length; ++i)
            {
                var face = mesh.faces[i];
                var p0 = mesh.nodes[face.n[0]].p;
                var p1 = mesh.nodes[face.n[1]].p;
                var p2 = mesh.nodes[face.n[2]].p;
                face.centroid = calculateFaceCentroid(p0, p1, p2).Normalize();
            }

            for (var i = 0; i < mesh.nodes.Length; ++i)
            {
                var node = mesh.nodes[i];
                var faceIndex = node.f[0];
                for (var j = 1; j < node.f.Count - 1; ++j)
                {
                    faceIndex = findNextFaceIndex(mesh, i, faceIndex);
                    var k = node.f.IndexOf(faceIndex);
                    node.f[k] = node.f[j];
                    node.f[j] = faceIndex;
                }
            }
            
        }

        private static Vector ProjectPoint(Vector point, Vector normal, Vector origin)
        {
            Vector v = point - origin;
            double dist = v.Dot(normal.Normalize());
            return point - normal.Normalize()*dist;
        }
        private static Vector calculateFaceCentroid(Vector pa, Vector pb, Vector pc)
        {
            var vabHalf = (pb - pa) / 2;
            var pabHalf = pa + vabHalf;
            var centroid = (pc - pabHalf) / 3 + pabHalf;
            return centroid;
        }

    }
}
