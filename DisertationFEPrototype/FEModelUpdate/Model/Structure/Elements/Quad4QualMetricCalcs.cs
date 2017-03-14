using DisertationFEPrototype.Optimisations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    public class Quad4QualMetricCalcs
    {

        Quad4Elem elem;
        public Quad4QualMetricCalcs(Quad4Elem elem)
        {
            this.elem = elem;
        }

       
        // area3D_Polygon(): computes the area of a 3D planar polygon
        //    Input:  int n = the number of vertices in the polygon
        //            Point[] V = an array of n+2 vertices in a plane
        //                       with V[n]=V[0] and V[n+1]=V[1]
        //            Point N = unit normal vector of the polygon's plane
        //    Return: the (float) area of the polygon


        public double computeAspectRatio(double longestEdge, double shortestEdge)
        {
            return elem.computeAspectRatio(longestEdge, shortestEdge);
        }

        internal double computeMaxCornerAngle(List<Node> fourPlaneNodes)
        {
            return elem.computeMaxCornerAngle(fourPlaneNodes);
        }

        internal double computeMaxparallelDev(Tuple<Node, Node>[] edges)
        {
            return elem.computeMaxParallelDev(edges);
        }

        internal double computeLongestEdge(Tuple<Node, Node>[] nodePairings, double LONGEST_EDGE_DEFAULT)
        {
            return elem.computeLongestEdge(nodePairings, LONGEST_EDGE_DEFAULT);
        }

        internal double computeShortestEdge(Tuple<Node, Node>[] nodePairings, double SHORTEST_EDGE_DEFAULT)
        {
            return elem.computeShortestEdge(nodePairings, SHORTEST_EDGE_DEFAULT);
        }
    }
}
