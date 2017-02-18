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

        List<Node> nodes;

        public Hex8QualMetricCalcs(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        internal double computeMaxCornerAngle()
        {
            throw new NotImplementedException();
        }

        internal double computeMaxparallelDev()
        {
            throw new NotImplementedException();
        }

        internal double computeLongestEdge(double sHORTEST_EDGE_DEFAULT)
        {
            throw new NotImplementedException();
        }

        internal double computeShortestEdge(double lONGEST_EDGE_DEFAULT)
        {
            throw new NotImplementedException();
        }

        internal double computeAspectRatio(double longestEdge, double shortestEdge)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the total area for a Hex8Type element, currently doesn't take obstructed faces into account
        /// </summary>
        /// <param name="faces"></param>
        /// <returns></returns>
        //internal double computeArea(Node[][] faces, double longestEdge, double shortestEdge)
        //{


        //    return faces
        //        .Select(face => GeneralRefinementMethods.computeFaceArea(face.ToList(),)).Sum();
        //}
    }
}
