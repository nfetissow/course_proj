using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanetGenerator.SphereBuilder;

namespace PlanetGenerator.PlanetGeneration
{
    class Weather
    {
        public static void generatePlanetWeather(PolyhedronMesh topology, double heatLevel, double moistureLevel, DoubleRandom random, double planetRadius)
        {
            
            List<Whorl> whorls = new List<Whorl>();
            List<Corner> activeCorners = new List<Corner>();
            double totalHeat;
            double remainingHeat;
            double totalMoisture;
            double remainingMoisture;

            whorls = GenerateAirCurrentWhorls(planetRadius, random);

            CalculateAirCurrents(topology.corners, whorls, planetRadius);

            activeCorners = initializeAirHeat(topology.corners, heatLevel);

            remainingHeat = 0;
            foreach(Corner corner in topology.corners)
            {
                remainingHeat += corner.airHeat;
            }
            double consumedHeat;
            do
            {
                consumedHeat = processAirHeat(activeCorners);
                remainingHeat -= consumedHeat;
            }
            while (remainingHeat > 0 && consumedHeat >= 0.0001);

            calculateTemperature(topology.corners, topology.tiles, planetRadius);

            activeCorners = InitializeAirMoisture(topology.corners, moistureLevel);
            remainingMoisture = 0;
            foreach (Corner corner in topology.corners)
            {
                remainingMoisture += corner.airMoisture;
            }
            double consumedMoisture;
            do
            {
                consumedMoisture = ProcessAirMoisture(activeCorners);
                remainingMoisture -= consumedMoisture;
            }
            while (remainingMoisture > 0 && consumedMoisture >= 0.0001);

            CalculateMoisture(topology.corners, topology.tiles);

        }

        private static List<Whorl> GenerateAirCurrentWhorls(double planetRadius, DoubleRandom random)
        {
            List<Whorl> whorls = new List<Whorl>();
            int direction = 0;
            while (direction == 0) direction = random.Next(-1, 1);
            int layerCount = random.Next(4, 7);
            double circumference = Math.PI * 2 * planetRadius;
            double fullRevolution = Math.PI * 2;
            double baseWhorlRadius = circumference / (2 * (layerCount - 1));
            whorls.Add(
                new Whorl(new Vector(0, planetRadius, 0)
                    .applyAxisAngle(new Vector(1, 0, 0), random.NextDouble(0, fullRevolution/(2 *(layerCount + 4))))
                    .applyAxisAngle(new Vector(0, 1, 0), random.NextDouble(0, fullRevolution)), 
                random.NextDouble(fullRevolution / 36, fullRevolution / 24)*direction, 
                random.NextDouble(baseWhorlRadius*0.8, baseWhorlRadius*1.2))
                );
            for (int i = 1; i < layerCount - 1; ++i)
            {
                direction = -direction;
                double baseTilt = i / (layerCount - 1) * fullRevolution / 2;
                int layerWhorlCount = (int) Math.Ceiling((Math.Sin(baseTilt) * planetRadius * fullRevolution) / baseWhorlRadius);
                for(int j = 0; j < layerWhorlCount; ++j)
                {
                    whorls.Add(
                        new Whorl(new Vector(0, planetRadius, 0)
                            .applyAxisAngle(new Vector(1, 0, 0), random.NextDouble(0, fullRevolution / (2 * (layerCount + 4))))
                            .applyAxisAngle(new Vector(0, 1, 0), random.NextDouble(0, fullRevolution))
                            .applyAxisAngle(new Vector(1, 0, 0), baseTilt)
                            .applyAxisAngle(new Vector(0, 1, 0), fullRevolution * (j + (i % 2) / 2) / layerWhorlCount),
                            random.NextDouble(fullRevolution / 48, fullRevolution / 32) * direction,
                            random.NextDouble(baseWhorlRadius * 0.8, baseWhorlRadius * 1.2))
                        );
                }
            }
            direction = -direction;
            whorls.Add(
                new Whorl(new Vector(0, planetRadius, 0)
                .applyAxisAngle(new Vector(1, 0, 0), random.NextDouble(0, fullRevolution / (2 * (layerCount + 4))))
                .applyAxisAngle(new Vector(0, 1, 0), random.NextDouble(0, fullRevolution))
                .applyAxisAngle(new Vector(1, 0, 0), fullRevolution / 2),
                random.NextDouble(fullRevolution/36, fullRevolution/24)*direction,
                random.NextDouble(baseWhorlRadius * 0.8, baseWhorlRadius * 1.2))
                );
            return whorls;
        }

        private static void CalculateAirCurrents(Corner[] corners, List<Whorl> whorls, double planetRadius)
        {

            for(int i = 0; i < corners.Length; ++i)
            {
                var corner = corners[i];
                var airCurrent = new Vector(0, 0, 0);
                double weight = 0;
                for (var j = 0; j < whorls.Count; ++j)
                {
                    var whorl = whorls[j];
                    var angle = whorl.center.angleTo(corner.pos);
                    if (Double.IsNaN(angle))
                    {
                        int b = 1;
                    }//ANGLETO - HANDMADE FUNCTION
                    var distance = Math.Cos(angle) * planetRadius;
                    if (distance < whorl.radius)
                    {
                        var normalizedDistance = distance / whorl.radius;
                        var whorlWeight = 1 - normalizedDistance;
                        var whorlStrength = planetRadius * whorl.strength * whorlWeight * normalizedDistance;
                        var whorlCurrent = whorl.center.Cross(corner.pos).Normalize() * whorlStrength;
                        airCurrent = airCurrent + whorlCurrent;
                        weight += whorlWeight;
                    }
                }
                if(weight == 0)
                {
                    int b = 1;
                }
                airCurrent = airCurrent / weight;
                corner.airCurrent = airCurrent;
                corner.airCurrentSpeed = airCurrent.Magnitude(); //kilometers per hour

                corner.airCurrentOutflows = new double[corner.borders.Length];
                var airCurrentDirection = airCurrent.Normalize();
                double outflowSum = 0;
                for (var j = 0; j < corner.corners.Length; ++j)
                {
                    var vector = corner.vectorTo(corner.corners[j]).Normalize();
                    var dot = vector.Dot(airCurrentDirection);
                    if (dot > 0)
                    {
                        corner.airCurrentOutflows[j] = dot;
                        outflowSum += dot;
                    }
                    else
                    {
                        corner.airCurrentOutflows[j] = 0;
                    }
                }

                if (outflowSum > 0)
                {
                    for (var j = 0; j < corner.borders.Length; ++j)
                    {
                        corner.airCurrentOutflows[j] /= outflowSum;
                    }
                }
            }
        }

        private static List<Corner> initializeAirHeat(Corner[] corners, double heatLevel)
        {
            List<Corner> activeCorners = new List<Corner>();
            double airHeat = 0;
            for(int i = 0; i < corners.Length; ++i)
            {
                var corner = corners[i];
                corner.airHeat = corner.area * heatLevel;
                corner.heat = 0;
                corner.newAirHeat = 0;

                corner.heatAbsorption = 0.1 * corner.area / Math.Max(0.1, Math.Min(corner.airCurrentSpeed, 1));
                if (corner.elevation <= 0)
                {
                    corner.maxHeat = corner.area;
                }
                else
                {
                    corner.maxHeat = corner.area;
                    corner.heatAbsorption *= 2;
                }

                activeCorners.Add(corner);
                airHeat += corner.airHeat;
            }
            return activeCorners;
        }

        private static double processAirHeat(List<Corner> activeCorners)
        {
            double consumedHeat = 0;
	        var activeCornerCount = activeCorners.Count;
	        for (var i = 0; i < activeCornerCount; ++i)
	        {
		        var corner = activeCorners[i];
		        if (corner.airHeat == 0) continue;
		
		        var heatChange = Math.Max(0, Math.Min(corner.airHeat, corner.heatAbsorption * (1 - corner.heat / corner.maxHeat)));
		        corner.heat += heatChange;
		        consumedHeat += heatChange;
		        var heatLoss = corner.area * (corner.heat / corner.maxHeat) * 0.02;
		        heatChange = Math.Min(corner.airHeat, heatChange + heatLoss);
		
		        var remainingCornerAirHeat = corner.airHeat - heatChange;
		        corner.airHeat = 0;
		
		        for (var j = 0; j < corner.corners.Length; ++j)
		        {
			        var outflow = corner.airCurrentOutflows[j];
			        if (outflow > 0)
			        {
				        corner.corners[j].newAirHeat += remainingCornerAirHeat * outflow;
				        activeCorners.Add(corner.corners[j]);
			        }
		        }
	        }
	
	        activeCorners.RemoveRange(0, activeCornerCount);
	
	        foreach(Corner corner in activeCorners)
	        {
		        corner.airHeat = corner.newAirHeat;
                corner.newAirHeat = 0; //CHANGED CYCLE ALTHOUGH IT SHOULD NOT BE A PROBLEM
	        }
	
	        return consumedHeat;
        }

        private static void calculateTemperature(Corner[] corners, Tile[] tiles, double planetRadius)
        {
            for (var i = 0; i < corners.Length; ++i)
	        {
		        var corner = corners[i];
		        var latitudeEffect = Math.Sqrt(1 - Math.Abs(corner.pos.y) / planetRadius);
		        var elevationEffect = 1 - Math.Pow(Math.Max(0, Math.Min(corner.elevation * 0.8, 1)), 2);
		        var normalizedHeat = corner.heat / corner.area;
		        corner.temperature = (latitudeEffect * elevationEffect * 0.7 + normalizedHeat * 0.3) * 5/3 - 2/3;
	        }

	        for (var i = 0; i < tiles.Length; ++i)
	        {
		        var tile = tiles[i];
		        tile.temperature = 0;
		        for (var j = 0; j < tile.corners.Length; ++j)
		        {
			        tile.temperature += tile.corners[j].temperature;
		        }
		        tile.temperature /= tile.corners.Length;
	        }
        }

        private static List<Corner> InitializeAirMoisture(Corner[] corners, double moistureLevel)
        {
            List<Corner> activeCorners = new List<Corner>();
            double airMoisture = 0;
            for (var i = 0; i < corners.Length; ++i)
            {
                var corner = corners[i];
                corner.airMoisture = (corner.elevation > 0) ? 0 : corner.area * moistureLevel * Math.Max(0, Math.Min(0.5 + corner.temperature * 0.5, 1));
                corner.newAirMoisture = 0;
                corner.precipitation = 0;

                corner.precipitationRate = 0.0075 * corner.area / Math.Max(0.1, Math.Min(corner.airCurrentSpeed, 1));
                if(Double.IsNaN(corner.precipitationRate))
                {
                    int b = 9;
                }
                corner.precipitationRate *= 1 + (1 - Math.Max(0, Math.Min(corner.temperature, 1))) * 0.1; //CHANGED MAX TO MIN AS WAS SAID IN COMMENTS
                if (Double.IsNaN(corner.precipitationRate))
                {
                    int b = 9;
                }
                if (corner.elevation > 0)
                {
                    corner.precipitationRate *= 1 + corner.elevation * 0.5;
                    corner.maxPrecipitation = corner.area * (0.25 + Math.Max(0, Math.Min(corner.elevation, 1)) * 0.25);
                }
                else
                {
                    corner.maxPrecipitation = corner.area * 0.25;
                }

                activeCorners.Add(corner);
                airMoisture += corner.airMoisture;
            }
            return activeCorners;
        }

        private static double ProcessAirMoisture(List<Corner> activeCorners)
        {
            double consumedMoisture = 0;
	        var activeCornerCount = activeCorners.Count;
	        for (var i = 0; i < activeCornerCount; ++i)
	        {
		        var corner = activeCorners[i];
		        if (corner.airMoisture == 0) continue;
		        var moistureChange = Math.Max(0, Math.Min(corner.airMoisture, corner.precipitationRate * (1 - corner.precipitation / corner.maxPrecipitation)));
		        corner.precipitation += moistureChange;
                if (Double.IsNaN(corner.precipitation))
		        consumedMoisture += moistureChange;
		        var moistureLoss = corner.area * (corner.precipitation / corner.maxPrecipitation) * 0.02;
		        moistureChange = Math.Min(corner.airMoisture, moistureChange + moistureLoss);
		
		        var remainingCornerAirMoisture = corner.airMoisture - moistureChange;
		        corner.airMoisture = 0;
		
		        for (var j = 0; j < corner.corners.Length; ++j)
		        {
			        var outflow = corner.airCurrentOutflows[j];
			        if (outflow > 0)
			        {
				        corner.corners[j].newAirMoisture += remainingCornerAirMoisture * outflow;
				        activeCorners.Add(corner.corners[j]);
			        }
		        }
	        }

	        activeCorners.RemoveRange(0, activeCornerCount);

            foreach(Corner corner in activeCorners)
            {
                corner.airMoisture = corner.newAirMoisture;
                corner.newAirMoisture = 0;
            }//SAME SHIT WITH CYCLE
	
	        return consumedMoisture;
        }

        private static void CalculateMoisture(Corner[] corners, Tile[] tiles)
        {
            foreach(Corner corner in corners)
            {
                corner.moisture = corner.precipitation / corner.area / 0.5;
                if(Double.IsNaN(corner.moisture))
                {
                    int b = 1;
                }
            }
            foreach(Tile tile in tiles)
            {
                tile.moisture = 0;
                foreach(Corner corner in tile.corners)
                {
                    tile.moisture += corner.moisture;
                }
                tile.moisture /= tile.corners.Length;
            }
        }

        



        private class Whorl
        {
            public Vector center;
            public double strength;
            public double radius;
            public Whorl(Vector center, double strength, double radius)
            {
                this.center = center;
                this.strength = strength;
                this.radius = radius;
            }
        }
    }
}
