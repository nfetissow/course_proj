using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using PlanetGenerator.SphereBuilder;

namespace PlanetGenerator.PlanetGeneration
{
    class Plates
    {

        public static PolyhedronMesh createPlanet(int subDivisionCount, int plateCount, double oceanicRate, DoubleRandom rng)
        {
            PolyhedronMesh topology = Polyhedron.getDualPolyhedron(Icosahedron.generateSubdividedIcosahedron(subDivisionCount));
            createPlates(topology, plateCount, oceanicRate, rng);
            return topology;
        }

        public static void fixTiles(Tile[] tiles)
        {
            for(int i = 0; i < tiles.Length; ++i)
            {
                for(int j = 0; j < tiles[i].corners.Length; ++j)
                {
                    for(int k = j + 1; k < tiles[i].corners.Length -1; ++k)
                    {

                    }
                }
            }
        }

        public static Plate[] createPlates(PolyhedronMesh topology, int plateCount, double oceanicRate, DoubleRandom rng)
        {
            List<Plate> plates = new List<Plate>();
       
            var platelessTiles = new List<Tile>();
            var platelessTilePlates = new List<Plate>();
            var failedCount = 0;
            HashSet<int> alreadyChosenNumbers = new HashSet<int>();
            int numOfCorner;
            while (plates.Count < plateCount && failedCount < 10000)
            {
                numOfCorner = rng.Next(0, topology.corners.Length);
                while(alreadyChosenNumbers.Contains(numOfCorner)) 
                {
                    numOfCorner = rng.Next(0, topology.corners.Length);
                }
                alreadyChosenNumbers.Add(numOfCorner);
                var corner = topology.corners[numOfCorner];
                var adjacentToExistingPlate = false;
                for (var i = 0; i < corner.tiles.Length; ++i)
                {
                    if (corner.tiles[i].plate != null)
                    {
                        adjacentToExistingPlate = true;
                        failedCount += 1;
                        break;
                    }
                }
                if (adjacentToExistingPlate) continue;
			
                failedCount = 0;
			
                var oceanic = (rng.NextDouble() < oceanicRate);
                var plate = new Plate(
                    Color.FromArgb(rng.Next(0, 255), rng.Next(0, 255), rng.Next(0, 255)),
                    randomUnitVector(rng),
                    rng.NextDouble(-Math.PI / 30, Math.PI / 30),
                    rng.NextDouble(-Math.PI / 30, Math.PI / 30),
                    oceanic ? rng.NextDouble(-0.8, -0.3) : rng.NextDouble(0.1, 0.5),
                    oceanic,
                    corner);
				
                plates.Add(plate);

                for (var i = 0; i < corner.tiles.Length; ++i)
                {
                    corner.tiles[i].plate = plate;
                    plate.tiles.Add(corner.tiles[i]);
                }

                for (var i = 0; i < corner.tiles.Length; ++i)
                {
                    var tile = corner.tiles[i];
                    for (var j = 0; j < tile.tiles.Length; ++j)
                    {
                        var adjacentTile = tile.tiles[j];
                        if (adjacentTile.plate == null)
                        {
                            platelessTiles.Add(adjacentTile);
                            platelessTilePlates.Add(plate);
                        }
                    }
                }
            }
	
            while (platelessTiles.Count > 0)
            {
                int tileIndex = (int)Math.Floor(Math.Pow(rng.NextDouble(), 2) * platelessTiles.Count);
                //rng.Next(0, platelessTilePlates.Count);//
                var tile = platelessTiles[tileIndex];
                var plate = platelessTilePlates[tileIndex];
                platelessTiles.RemoveAt(tileIndex);
                platelessTilePlates.RemoveAt(tileIndex);
                if (tile.plate == null)
                {
                    tile.plate = plate;
                    plate.tiles.Add(tile);
                    for (var j = 0; j < tile.tiles.Length; ++j)
                    {
                        if (tile.tiles[j].plate == null)
                        {
                            //platelessTiles.Add(tile.tiles[j]);
                            //platelessTilePlates.Add(plate);
                            if (!platelessTiles.Contains(tile.tiles[j]))
                            {
                                platelessTiles.Add(tile.tiles[j]);
                                platelessTilePlates.Add(plate);
                            }
                            //if(!platelessTilePlates.Contains(plate))
                            //{
                            //    platelessTilePlates.Add(plate);
                            //}
                        }
                    }
                }
            }
            calculateCornerDistancesToPlateRoot(plates);
            return plates.ToArray();
        }

        static void calculateCornerDistancesToPlateRoot(List<Plate> plates)
        {
            List<CornerDistance> distanceCornerQueue = new List<CornerDistance>();
            for(int i = 0; i < plates.Count; ++i)
            {
                var corner = plates[i].root;
                corner.distanceToRoot = 0;
                for(int j = 0; j < corner.corners.Length; ++j)
                {
                    distanceCornerQueue.Add(new CornerDistance(corner.corners[j], corner.borders[j].Length()));
                }
            }

            //distanceCornerQueue.Sort
            while(distanceCornerQueue.Count != 0)
            {
                int iEnd = distanceCornerQueue.Count;
		        for (var i = 0; i < iEnd; ++i)
		        {
			        var front = distanceCornerQueue[i];
			        var corner = front.corner;
			        var distanceToPlateRoot = front.distanceToPlateRoot;
			        if (corner.distanceToRoot == -1 || corner.distanceToRoot > distanceToPlateRoot)
			        {
				        corner.distanceToRoot = distanceToPlateRoot;
				        for (var j = 0; j < corner.corners.Length; ++j)
				        {
					        distanceCornerQueue.Add(new CornerDistance(corner.corners[j], distanceToPlateRoot + corner.borders[j].Length()));
				        }
			        }
		        }
                distanceCornerQueue.RemoveRange(0, iEnd);
		        //distanceCornerQueue.splice(0, iEnd);
                distanceCornerQueue.Sort();
            }
        }

        static Vector randomUnitVector(DoubleRandom rng)
        {
            double theta = rng.NextDouble(0, Math.PI * 2);
            double phi = Math.Acos(rng.NextDouble(-1, 1));
            double sinPhi = Math.Sin(phi);
            return new Vector(Math.Cos(theta) * sinPhi, Math.Sin(theta) * sinPhi, Math.Cos(phi));
        }
        
    }

    public class Plate
    {
        public Color color;
        public Vector driftAxis;
        public double driftRate;
        public double spinRate;
        public double elevation;
        public bool oceanic;
        public Corner root;
        public List<Tile> tiles;
        public List<Corner> boudaryCorners;
        public List<Border> boundaryBorders;
        public Plate(Color c, Vector driftAxis, double driftRate, double spinRate, double elevation, bool oceanic, Corner root)
        {
            this.color = c;
            this.driftAxis = driftAxis;
            this.driftRate = driftRate;
            this.spinRate = spinRate;
            this.elevation = elevation;
            this.oceanic = oceanic;
            this.root = root;
            tiles = new List<Tile>();
            boudaryCorners = new List<Corner>();
            boundaryBorders = new List<Border>();
        }
    }

    public class CornerDistance: IComparable
    {
        public Corner corner;
        public double distanceToPlateRoot;
        public CornerDistance(Corner corner, double distance)
        {
            this.corner = corner;
            this.distanceToPlateRoot = distance;
        }
        public int CompareTo(object other)
        {
            CornerDistance otherDistance = other as CornerDistance;
            return (int)(this.distanceToPlateRoot - otherDistance.distanceToPlateRoot);
        }
    }
}
