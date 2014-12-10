using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanetGenerator.SphereBuilder;


namespace PlanetGenerator.PlanetGeneration
{
    class Planet
    {
        public static PolyhedronMesh createPlanet(PolyhedronMesh topology, int plateCount, double oceanicRate, double heatLevel, double moistureLevel, double planetRadius, DoubleRandom rng)
        {
            var plates = Plates.createPlates(topology, plateCount, oceanicRate, rng);
            Elevation.GeneratePlanetElevation(topology, plates);
            Weather.generatePlanetWeather(topology, heatLevel, moistureLevel, rng, planetRadius);
            GeneratePlanetBiomes(topology.tiles, planetRadius);
            return topology;
        }

        private static void GeneratePlanetBiomes(Tile[] tiles, double planetRadius)
        {
            for (var i = 0; i < tiles.Length; ++i)
            {
                var tile = tiles[i];
                var elevation = tile.elevation;
                //var latitude = Math.Abs(tile.pos.y / planetRadius);
                var temperature = tile.temperature - 0.3;

                var moisture = tile.moisture;

                if (elevation <= 0)
                {
                    if (temperature > 0)
                    {
                        if (tile.elevation > -0.3) tile.biome = "ocean";
                        else if (tile.elevation > -0.4) tile.biome = "deepOcean";
                        else tile.biome = "veryDeepOcean";
                    }
                    else
                    {
                        tile.biome = "oceanGlacier";
                    }
                }
                else if (elevation < 0.6)
                {
                    if (temperature > 0.75)
                    {
                        if (moisture < 0.25)
                        {
                            tile.biome = "desert";
                        }
                        else
                        {
                            tile.biome = "rainForest";
                        }
                    }
                    else if (temperature > 0.5)
                    {
                        if (moisture < 0.25)
                        {
                            tile.biome = "rocky";
                        }
                        else if (moisture < 0.50)
                        {
                            tile.biome = "plains";
                        }
                        else
                        {
                            tile.biome = "swamp";
                        }
                    }
                    else if (temperature > 0)
                    {
                        if (moisture < 0.25)
                        {
                            tile.biome = "plains";
                        }
                        else if (moisture < 0.50)
                        {
                            tile.biome = "grassland";
                        }
                        else
                        {
                            tile.biome = "deciduousForest";
                        }
                    }
                    else
                    {
                        if (moisture < 0.25)
                        {
                            tile.biome = "landGlacier";
                        }
                        else
                        {
                            tile.biome = "landGlacier";
                        }
                    }
                }
                else if (elevation < 0.8)
                {
                    if (temperature > 0)
                    {
                        if (moisture < 0.25)
                        {
                            tile.biome = "tundra";
                        }
                        else
                        {
                            tile.biome = "coniferForest";
                        }
                    }
                    else
                    {
                        tile.biome = "tundra";
                    }
                }
                else
                {
                    if (temperature > 0 || moisture < 0.25)
                    {
                        tile.biome = "mountain";
                    }
                    else
                    {
                        tile.biome = "snowyMountain";
                    }
                }
            }
        }
    }
}
