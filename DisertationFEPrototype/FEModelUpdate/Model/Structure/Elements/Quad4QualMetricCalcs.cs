using DisertationFEPrototype.Model.Structure;
using DisertationFEPrototype.Model.Structure.Elements;
using DisertationFEPrototype.Optimisations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    class Quad4QualMetricCalcs
    {


        public Quad4QualMetricCalcs(){}

       
        // area3D_Polygon(): computes the area of a 3D planar polygon
        //    Input:  int n = the number of vertices in the polygon
        //            Point[] V = an array of n+2 vertices in a plane
        //                       with V[n]=V[0] and V[n+1]=V[1]
        //            Point N = unit normal vector of the polygon's plane
        //    Return: the (float) area of the polygon



       

        public double computeAspectRatio(double longestEdge, double shortestEdge)
        {
            return GeneralMetricCalcMethods.computeAspectRatio(longestEdge, shortestEdge);
        }

        internal double computeMaxCornerAngle(List<Node> fourPlaneNodes)
        {
            return GeneralMetricCalcMethods.computeMaxCornerAngle(fourPlaneNodes);
        }

        internal double computeMaxparallelDev(Tuple<Node, Node>[] edges)
        {
            return GeneralMetricCalcMethods.computeMaxparallelDev(edges);
        }

        internal double computeLongestEdge(Tuple<Node, Node>[] nodePairings, double LONGEST_EDGE_DEFAULT)
        {
            return GeneralMetricCalcMethods.computeLongestEdge(nodePairings, LONGEST_EDGE_DEFAULT);
        }

        internal double computeShortestEdge(Tuple<Node, Node>[] nodePairings, double SHORTEST_EDGE_DEFAULT)
        {
            return GeneralMetricCalcMethods.computeShortestEdge(nodePairings, SHORTEST_EDGE_DEFAULT);
        }
    }
}
