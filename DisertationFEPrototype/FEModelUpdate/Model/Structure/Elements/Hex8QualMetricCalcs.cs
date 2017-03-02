using DisertationFEPrototype.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DisertationFEPrototype.Optimisations;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    class Hex8QualMetricCalcs
    {

        readonly double LONGEST_EDGE_DEFAULT = 0.0;
        readonly double SHORTEST_EDGE_DEFAULT = 1000000.0;


        Hex8Elem elem;
       
        public Hex8QualMetricCalcs(Hex8Elem elem)
        {
            this.elem = elem;
        }

        internal double computeMaxCornerAngle(Node[][] nodes)
        {
            return nodes.Select(x => elem.computeMaxCornerAngle(x.ToList())).Max();

            // throw new NotImplementedException();
        }

        internal double computeMaxparallelDev(List<Tuple<Node, Node>[]> faceEdgePairings)
        {
            double maxParrallelDev = faceEdgePairings
                .Select(x => elem.computeMaxparallelDev(x)).Max();
            return maxParrallelDev;
        }

        internal double computeAspectRatio(double longestEdge, double shortestEdge)
        {
            return elem.computeAspectRatio(longestEdge, shortestEdge);
        }

        internal double computeLongestEdge(Tuple<Node, Node>[] nodePairings, double LONGEST_EDGE_DEFAULT)
        {
            return elem.computeLongestEdge(nodePairings, LONGEST_EDGE_DEFAULT);
        }

        internal double computeShortestEdge(Tuple<Node, Node>[] nodePairings, double SHORTEST_EDGE_DEFAULT)
        {
            return elem.computeShortestEdge(nodePairings, SHORTEST_EDGE_DEFAULT);
        }

        /// <summary>
        /// Computes the total area for a Hex8Type element, currently doesn't take obstructed faces into account
        /// </summary>
        /// <param name="faces"></param>
        /// <returns></returns>
        internal double computeArea(List<Tuple<Node, Node>[]> lengthsForFace)
        {
            double totalArea = 0;

            foreach(var face in lengthsForFace)
            {
                // get the area for each face then sum these
                double longestEdge = elem.computeLongestEdge(face, LONGEST_EDGE_DEFAULT);
                double shortestEdge = elem.computeShortestEdge(face, SHORTEST_EDGE_DEFAULT);

                double faceArea = longestEdge * shortestEdge;
                totalArea += faceArea;
            }
            return totalArea;
        }
    }
}
