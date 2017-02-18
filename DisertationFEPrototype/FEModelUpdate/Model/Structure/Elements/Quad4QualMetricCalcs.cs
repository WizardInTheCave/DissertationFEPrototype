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


        

        List<Node> nodes;

        public Quad4QualMetricCalcs(List<Node> nodes)
        {
            this.nodes = nodes;
        }

       
        // area3D_Polygon(): computes the area of a 3D planar polygon
        //    Input:  int n = the number of vertices in the polygon
        //            Point[] V = an array of n+2 vertices in a plane
        //                       with V[n]=V[0] and V[n+1]=V[1]
        //            Point N = unit normal vector of the polygon's plane
        //    Return: the (float) area of the polygon


        internal double computeMaxCornerAngle(List<Node> fourPlaneNodes)
        {
            return GeneralMetricCalcMethods.computeMaxCornerAngle(fourPlaneNodes);
        }

        internal double computeLongestEdge(List<Node> nodes, double sHORTEST_EDGE_DEFAULT)
        {
            throw new NotImplementedException();
        }

        internal double computeShortestEdge(List<Node> nodes, double lONGEST_EDGE_DEFAULT)
        {
            throw new NotImplementedException();
        }

        internal double computeAspectRatio(double longestEdge, double shortestEdge)
        {
            throw new NotImplementedException();
        }
    }
}
