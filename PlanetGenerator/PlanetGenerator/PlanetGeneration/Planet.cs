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
        public static PolyhedronMesh createPlanet(int subDivisionCount, int plateCount, double oceanicRate, DoubleRandom rng)
        {
            PolyhedronMesh topology = Polyhedron.getDualPolyhedron(Icosahedron.generateSubdividedIcosahedron(subDivisionCount));
            var plates = Plates.createPlates(topology, plateCount, oceanicRate, rng);
            Elevation.GeneratePlanetElevation(topology, plates);
            return topology;
        }
    }
}
