using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanetGenerator.SphereBuilder;

namespace PlanetGenerator.PlanetGeneration
{
    class Elevation
    {
        public static void GeneratePlanetElevation(PolyhedronMesh topology, Plate[] plates)
        {
            IdentifyBoundaryBorders(topology.borders);
            var boundaryCorners = CollectBoundaryCorners(topology.corners);
            var boundaryCornerInnerBorderIndexes = calculatePlateBoundaryStress(boundaryCorners);
            BlurPlateBoundaryStress(boundaryCorners, 3, 0.4);
            var elevationBorderQueue = PopulateElevationBorderQueue(boundaryCorners, boundaryCornerInnerBorderIndexes);
            ProcessElevationBorderQueue(elevationBorderQueue);
            calculateTilesElevation(topology.tiles);
        }


        private static void IdentifyBoundaryBorders(Border[] borders)
        {
            foreach(Border border in borders)
            {
                if(!border.tiles[0].plate.Equals(border.tiles[1].plate))
                {
                    border.betweenPlates = true;
                    border.corners[0].betweenPlates = true;
                    border.corners[1].betweenPlates = true;
                    border.tiles[0].plate.boundaryBorders.Add(border);
                    border.tiles[1].plate.boundaryBorders.Add(border);
                    
                }
            }
        }
        private static List<Corner> CollectBoundaryCorners(Corner[] corners)
        {
            var boundaryCorners = new List<Corner>();
            foreach(Corner corner in corners)
            {
                if(corner.betweenPlates)
                {
                    boundaryCorners.Add(corner);
                    corner.tiles[0].plate.boundaryCorners.Add(corner);
                    if (!corner.tiles[1].plate.Equals(corner.tiles[0].plate)) 
                    {
                        corner.tiles[1].plate.boundaryCorners.Add(corner);
                    }
			        if (!corner.tiles[2].plate.Equals(corner.tiles[0].plate) && !corner.tiles[2].plate.Equals(corner.tiles[1].plate)) 
                    {
                        corner.tiles[2].plate.boundaryCorners.Add(corner);
                    }
                }
            }
            return boundaryCorners;
        }

        private static int[] calculatePlateBoundaryStress(List<Corner> boundaryCorners)
        {
            var boundaryCornerInnerBorderIndexes = new int[boundaryCorners.Count];
	        for (var i = 0; i < boundaryCorners.Count; ++i)
	        {
		        var corner = boundaryCorners[i];
		        corner.distanceToPlateBoundary = 0;
	
		        Border innerBorder = null;
		        int innerBorderIndex = -1;
		        for (var j = 0; j < corner.borders.Length; ++j)
		        {
			        var border = corner.borders[j];
			        if (!border.betweenPlates)
			        {
				        innerBorder = border;
				        innerBorderIndex = j;
				        break;
			        }
		        }
		
		        if (innerBorder != null)
		        {
			        boundaryCornerInnerBorderIndexes[i] = innerBorderIndex;
			        var outerBorder0 = corner.borders[(innerBorderIndex + 1) % corner.borders.Length];
			        var outerBorder1 = corner.borders[(innerBorderIndex + 2) % corner.borders.Length];
			        var farCorner0 = outerBorder0.oppositeCorner(corner);
			        var farCorner1 = outerBorder1.oppositeCorner(corner);
			        var plate0 = innerBorder.tiles[0].plate;
			        var plate1 = !outerBorder0.tiles[0].plate.Equals(plate0) ? outerBorder0.tiles[0].plate : outerBorder0.tiles[1].plate;
                    Vector boundaryVector = farCorner0.vectorTo(farCorner1);
			        var boundaryNormal = boundaryVector.Cross(corner.pos);
			        var stress = new Stress(plate0.CalculateMovement(corner.pos), plate1.CalculateMovement(corner.pos), boundaryVector, boundaryNormal);
			        corner.pressure = stress.pressure;
			        corner.shear = stress.shear;
		        }
		        else
		        {
			        boundaryCornerInnerBorderIndexes[i] = -1;
			        var plate0 = corner.tiles[0].plate;
			        var plate1 = corner.tiles[1].plate;
			        var plate2 = corner.tiles[2].plate;
			        var boundaryVector0 = corner.corners[0].vectorTo(corner);
			        var boundaryVector1 = corner.corners[1].vectorTo(corner);
			        var boundaryVector2 = corner.corners[2].vectorTo(corner);
			        var boundaryNormal0 = boundaryVector0.Cross(corner.pos);
			        var boundaryNormal1 = boundaryVector1.Cross(corner.pos);
			        var boundaryNormal2 = boundaryVector2.Cross(corner.pos);
			        var stress0 = new Stress(plate0.CalculateMovement(corner.pos), plate1.CalculateMovement(corner.pos), boundaryVector0, boundaryNormal0);
                    var stress1 = new Stress(plate1.CalculateMovement(corner.pos), plate2.CalculateMovement(corner.pos), boundaryVector1, boundaryNormal1);
                    var stress2 = new Stress(plate2.CalculateMovement(corner.pos), plate0.CalculateMovement(corner.pos), boundaryVector2, boundaryNormal2);
			
			        corner.pressure = (stress0.pressure + stress1.pressure + stress2.pressure) / 3;
			        corner.shear = (stress0.shear + stress1.shear + stress2.shear) / 3;
		        }
	        }
	
	        return boundaryCornerInnerBorderIndexes;
        }

        private static void BlurPlateBoundaryStress(List<Corner> boundaryCorners, int stressBlurIterations, double stressBlurCenterWeighting)
        {
            var newCornerPressure = new double[boundaryCorners.Count];
            var newCornerShear = new double[boundaryCorners.Count];
            for (var i = 0; i < stressBlurIterations; ++i)
            {
                for (var j = 0; j < boundaryCorners.Count; ++j)
                {
                    var corner = boundaryCorners[j];
                    double averagePressure = 0;
                    double averageShear = 0;
                    int neighborCount = 0;
                    for (var k = 0; k < corner.corners.Length; ++k)
                    {
                        var neighbor = corner.corners[k];
                        if (neighbor.betweenPlates)
                        {
                            averagePressure += neighbor.pressure;
                            averageShear += neighbor.shear;
                            ++neighborCount;
                        }
                    }
                    newCornerPressure[j] = corner.pressure * stressBlurCenterWeighting + (averagePressure / neighborCount) * (1 - stressBlurCenterWeighting);
                    newCornerShear[j] = corner.shear * stressBlurCenterWeighting + (averageShear / neighborCount) * (1 - stressBlurCenterWeighting);
                }

                for (var j = 0; j < boundaryCorners.Count; ++j)
                {
                    var corner = boundaryCorners[j];
                    if (corner.betweenPlates)
                    {
                        corner.pressure = newCornerPressure[j];
                        corner.shear = newCornerShear[j];
                    }
                }
            }
        }



        #region CalculateElevation delegates
        delegate double calculateElevation(double distanceToPlateBoundary, double distanceToPlateRoot, double boundaryElevation, double plateElevation, double pressure, double shear);


        static calculateElevation calculateCollidingElevation = (double distanceToPlateBoundary, double distanceToPlateRoot, double boundaryElevation, double plateElevation, double pressure, double shear) =>
        {
            double t = distanceToPlateBoundary / (distanceToPlateBoundary + distanceToPlateRoot);
            if(t < 0.5)
            {
                t = t / 0.5;
                return plateElevation + Math.Pow(t - 1, 2) * (boundaryElevation - plateElevation);
            }
            else
            {
                return plateElevation;
            }
        };

        static calculateElevation calculateSuperductingElevation = (double distanceToPlateBoundary, double distanceToPlateRoot, double boundaryElevation, double plateElevation, double pressure, double shear) =>
        {
            double t = distanceToPlateBoundary / (distanceToPlateBoundary + distanceToPlateRoot);
            if (t < 0.2)
            {
                t = t / 0.2;
                return boundaryElevation + t * (plateElevation - boundaryElevation + pressure / 2);
            }
            else if (t < 0.5)
            {
                t = (t - 0.2) / 0.3;
                return plateElevation + Math.Pow(t - 1, 2) * pressure / 2;
            }
            else
            {
                return plateElevation;
            }
        };

        static calculateElevation calculateSubductingElevation = (double distanceToPlateBoundary, double distanceToPlateRoot, double boundaryElevation, double plateElevation, double pressure, double shear) =>
        {
            double t = distanceToPlateBoundary / (distanceToPlateBoundary + distanceToPlateRoot);
            return plateElevation + Math.Pow(t - 1, 2) * (boundaryElevation - plateElevation);
        };

        static calculateElevation calculateDivergingElevation = (double distanceToPlateBoundary, double distanceToPlateRoot, double boundaryElevation, double plateElevation, double pressure, double shear) =>
        {
            var t = distanceToPlateBoundary / (distanceToPlateBoundary + distanceToPlateRoot);
            if (t < 0.3)
            {
                t = t / 0.3;
                return plateElevation + Math.Pow(t - 1, 2) * (boundaryElevation - plateElevation);
            }
            else
            {
                return plateElevation;
            }
        };

        static calculateElevation calculateShearingElevation = (double distanceToPlateBoundary, double distanceToPlateRoot, double boundaryElevation, double plateElevation, double pressure, double shear) =>
        {
            var t = distanceToPlateBoundary / (distanceToPlateBoundary + distanceToPlateRoot);
            if (t < 0.2)
            {
                t = t / 0.2;
                return plateElevation + Math.Pow(t - 1, 2) * (boundaryElevation - plateElevation);
            }
            else
            {
                return plateElevation;
            }
        };

        static calculateElevation calculateDormantElevation = (double distanceToPlateBoundary, double distanceToPlateRoot, double boundaryElevation, double plateElevation, double pressure, double shear) =>
        {
            var t = distanceToPlateBoundary / (distanceToPlateBoundary + distanceToPlateRoot);
            var elevationDifference = boundaryElevation - plateElevation;
            var a = 2 * elevationDifference;
            var b = -3 * elevationDifference;
            return t * t * elevationDifference * (2 * t - 3) + boundaryElevation;
        };
        #endregion
        private class Stress
        {
            public double pressure;
            public double shear;
            public Stress(Vector movement0, Vector movement1, Vector boundaryVector, Vector boundaryNormal)
            {
                Vector relativeMovement = movement0 - movement1;
                Vector pressureVector = relativeMovement.projectOnVector(boundaryNormal);
                double pressure = pressureVector.Magnitude();
                if (pressureVector.Dot(boundaryNormal) > 0) pressure = -pressure;
                double shear = relativeMovement.projectOnVector(boundaryVector).Magnitude();
                this.pressure = 2 / (1 + Math.Exp(-pressure / 30)) - 1;
                this.shear = 2 / (1 + Math.Exp(-shear / 30)) - 1;
            }
        }

        private class ElevationBorder : IComparable<ElevationBorder>
        {
            public class Origin
            {
                public Corner corner;
                public double pressure;
                public double shear;
                public Plate plate;
                public calculateElevation findElevation;
                public Origin(Corner corner, double pressure, double shear, Plate plate, calculateElevation findElevation)
                {
                    this.corner = corner;
                    this.pressure = pressure;
                    this.shear = shear;
                    this.plate = plate;
                    this.findElevation = findElevation;
                }
            }
            public Origin origin;
            public Border border;
            public Corner corner;
            public Corner nextCorner;
            public double distanceToPlateBoundary;
            public ElevationBorder(Origin origin, Border border, Corner corner, Corner nextCorner, double distanceToPlateBoundary)
            {
                this.origin = origin;
                this.border = border;
                this.corner = corner;
                this.nextCorner = nextCorner;
                this.distanceToPlateBoundary = distanceToPlateBoundary;
            }
            public int CompareTo(ElevationBorder other)
            {
                return Math.Sign(this.distanceToPlateBoundary - other.distanceToPlateBoundary);
            }
        }

        private static List<ElevationBorder> PopulateElevationBorderQueue(List<Corner> boundaryCorners, int[] boundaryCornerInnerBorderIndexes)
        {
            List<ElevationBorder> elevationBorderQueue = new List<ElevationBorder>();
            for (var i = 0; i < boundaryCorners.Count; ++i)
	        {
		        var corner = boundaryCorners[i];
		
		        var innerBorderIndex = boundaryCornerInnerBorderIndexes[i];
		        if (innerBorderIndex != -1)
		        {
			        var innerBorder = corner.borders[innerBorderIndex];
			        var outerBorder0 = corner.borders[(innerBorderIndex + 1) % corner.borders.Length];
			        var plate0 = innerBorder.tiles[0].plate;
			        var plate1 = !outerBorder0.tiles[0].plate.Equals(plate0) ? outerBorder0.tiles[0].plate : outerBorder0.tiles[1].plate;
			
			        calculateElevation calculateElevation;
			
			        if (corner.pressure > 0.3)
			        {
				        corner.elevation = Math.Max(plate0.elevation, plate1.elevation) + corner.pressure;
				        if (plate0.oceanic == plate1.oceanic)
					        calculateElevation = calculateCollidingElevation;
				        else if (plate0.oceanic)
					        calculateElevation = calculateSubductingElevation;
				        else
					        calculateElevation = calculateSuperductingElevation;
			        }
			        else if (corner.pressure < -0.3)
			        {
				        corner.elevation = Math.Max(plate0.elevation, plate1.elevation) - corner.pressure / 4;
				        calculateElevation = calculateDivergingElevation;
			        }
			        else if (corner.shear > 0.3)
			        {
				        corner.elevation = Math.Max(plate0.elevation, plate1.elevation) + corner.shear / 8;
				        calculateElevation = calculateShearingElevation;
			        }
			        else
			        {
				        corner.elevation = (plate0.elevation + plate1.elevation) / 2;
				        calculateElevation = calculateDormantElevation;
			        }
			
			        var nextCorner = innerBorder.oppositeCorner(corner);
			        if (!nextCorner.betweenPlates)
    		        {
                        elevationBorderQueue.Add(new ElevationBorder(new ElevationBorder.Origin(corner, corner.pressure, corner.shear, plate0, calculateElevation),
                            innerBorder, corner, nextCorner, innerBorder.Length()));
			        }
		        }
		        else
		        {
			        var plate0 = corner.tiles[0].plate;
			        var plate1 = corner.tiles[1].plate;
			        var plate2 = corner.tiles[2].plate;
			
			        if (corner.pressure > 0.3)
			        {
				        corner.elevation = Max(plate0.elevation, plate1.elevation, plate2.elevation) + corner.pressure;
			        }
			        else if (corner.pressure < -0.3)
			        {
				        corner.elevation = Max(plate0.elevation, plate1.elevation, plate2.elevation) + corner.pressure / 4;
			        }
			        else if (corner.shear > 0.3)
			        {
				        corner.elevation = Max(plate0.elevation, plate1.elevation, plate2.elevation) + corner.shear / 8;
			        }
			        else
			        {
				        corner.elevation = (plate0.elevation + plate1.elevation + plate2.elevation) / 3;
			        }
		        }
	        }
                
            return elevationBorderQueue;
        }



        static double Max(double d1, double d2, double d3)
        {
            return Math.Max(Math.Max(d1, d2), Math.Max(d3, d2));
        }

        private static void ProcessElevationBorderQueue(List<ElevationBorder> elevationBorderQueue)
        {

            while (elevationBorderQueue.Count != 0)
            {
                int iEnd = elevationBorderQueue.Count;
                for (var i = 0; i < iEnd; ++i)
                {
                    var front = elevationBorderQueue[i];
                    var corner = front.nextCorner;
                    if (Double.IsNaN(corner.elevation))
                    {
                        corner.distanceToPlateBoundary = front.distanceToPlateBoundary;
                        corner.elevation = front.origin.findElevation(
                            corner.distanceToPlateBoundary,
                            corner.distanceToRoot,
                            front.origin.corner.elevation,
                            front.origin.plate.elevation,
                            front.origin.pressure,
                            front.origin.shear);
                        for (var j = 0; j < corner.borders.Length; ++j)
                        {
                            var border = corner.borders[j];
                            if (!border.betweenPlates)
                            {
                                var nextCorner = corner.corners[j];
                                var distanceToPlateBoundary = corner.distanceToPlateBoundary + border.Length();
                                if (Double.IsNaN(nextCorner.distanceToPlateBoundary) || nextCorner.distanceToPlateBoundary > distanceToPlateBoundary)
                                {
                                    elevationBorderQueue.Add(new ElevationBorder(front.origin, border, corner, nextCorner, distanceToPlateBoundary));
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < iEnd; ++i)
                {
                    ElevationBorder b = elevationBorderQueue[i];
                }

                    elevationBorderQueue.RemoveRange(0, iEnd);
                elevationBorderQueue.Sort();
            }
        }

        private static void calculateTilesElevation(Tile[] tiles)
        {
            foreach(Tile tile in tiles)
            {
                double elevation = 0;
                for(int i = 0; i < tile.corners.Length; ++i)
                {
                    elevation += tile.corners[i].elevation;
                }
                tile.elevation = elevation / tile.corners.Length;
            }
        }
    }
}
