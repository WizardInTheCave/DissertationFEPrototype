using DisertationFEPrototype.Model.Structure;
using DisertationFEPrototype.Model.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    class GeneralMetricCalcMethods
    {

        /// <summary>
        /// Compute the aspect ratio for an element 
        /// </summary>
        /// <returns>The aspec ratio of a Quad4 like surface</returns>
        public static double computeAspectRatio(double longerEdge, double shorterEdge)
        {
            double aspectRatio;
            aspectRatio = longerEdge / shorterEdge;
            return aspectRatio;
        }


        public static double computeLongestEdge(Tuple<Node, Node>[] nodes, double LONGEST_EDGE_DEFAULT)
        {
            double currentlyLongestEdge = LONGEST_EDGE_DEFAULT;

            double lengthsMax = nodes.Select(x => GeneralGeomMethods.distanceBetweenPoints(x.Item1, x.Item2)).Max();
            if (lengthsMax > LONGEST_EDGE_DEFAULT)
            {
                currentlyLongestEdge = lengthsMax;
            }
            return currentlyLongestEdge;
        }

        public static double computeShortestEdge(Tuple<Node, Node>[] nodes, double SHORTEST_EDGE_DEFAULT)
        {

            double currentlyShortestEdge = SHORTEST_EDGE_DEFAULT;
            double lengthsMin = nodes.Select(x => GeneralGeomMethods.distanceBetweenPoints(x.Item1, x.Item2)).Min();
            if (lengthsMin < SHORTEST_EDGE_DEFAULT)
            {
                currentlyShortestEdge = lengthsMin;
            }
            return currentlyShortestEdge;
        }



        /// <summary>
        /// For the nodes in the element get the cross product for each pair and add to total
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static double[] getTotalCrossProduct(List<Node> nodes)
        {
            double[] total = new double[] { 0, 0, 0 };
            for (int ii = 0; ii < nodes.Count; ii++)
            {
                Node vi1 = nodes[ii];
                Node vi2;

                if (ii == nodes.Count - 1)
                {
                    vi2 = nodes[0];
                }
                else
                {
                    vi2 = nodes[ii + 1];
                }
                Tuple<double, double, double> product = GeneralGeomMethods.crossProduct(vi1, vi2);
                total[0] += product.Item1;
                total[1] += product.Item2;
                total[2] += product.Item3;
            }
            return total;
        }

        /// <summary>
        /// using the method suggested here currently
        /// http://stackoverflow.com/questions/2350604/get-the-surface-area-of-a-polyhedron-3d-object
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static double computeFaceArea(List<Node> faceNodes, double longestEdge, double shortestEdge)
        {
            // element has no area (not polygon)
            if (faceNodes.Count < 3)
            {
                return 0;
            }

            double[] total = getTotalCrossProduct(faceNodes);
            var elemNormal = GeneralGeomMethods.unitNormal(faceNodes[0], faceNodes[1], faceNodes[2]);

            double result = GeneralGeomMethods.dotProduct(total, elemNormal);

            // just width * height in this case
            if (double.IsNaN(result))
            {
                double b = shortestEdge;
                double h = longestEdge;

                result = b * h;

            }
            else
            {
                result = Math.Abs(result / 2);
            }
            return result;
        }

        /// <summary>
        /// get a set of tuples which represent each of the edges within the element
        /// </summary>
        /// <returns></returns>
        public static Tuple<Node, Node>[] getEdgePairingsForNode(List<Node> nodes)
        {

            var edges = new Tuple<Node, Node>[4];

            Node node = nodes[0];

            Node[] adjNodes = GeneralMetricCalcMethods.getNonDiagAdjacentNodes(node, nodes);
            edges[0] = new Tuple<Node, Node>(node, adjNodes[0]);
            edges[1] = new Tuple<Node, Node>(node, adjNodes[1]);


            // in the case of quad4 there is only one diagonal node.
            Node diagonalNode = GeneralRefinementMethods.getDiagonalNode(nodes.ToArray(), node);

            edges[2] = new Tuple<Node, Node>(adjNodes[0], diagonalNode);
            edges[3] = new Tuple<Node, Node>(adjNodes[1], diagonalNode);

            return edges;
        }

        /// <summary>
        /// Take the opposite edges as parameters as this is what we want to calculate the deviation of
        /// </summary>
        /// <param name="edgeA"></param>
        /// <param name="edgeB"></param>
        /// <returns></returns>
        public static double getDevOnEdgePair(Tuple<Node, Node> edgeA, Tuple<Node, Node> edgeB)
        {

            // work out 
            var node = edgeA.Item1;

            var twoCommonNodes = new Node[2];
            twoCommonNodes[0] = edgeA.Item2;
            twoCommonNodes[1] = edgeB.Item1;

            double angle1 = GeneralMetricCalcMethods.getAngle(node, twoCommonNodes);


            var node2 = edgeB.Item1;
            var twoCommonNodes2 = new Node[2];

            twoCommonNodes2[0] = edgeA.Item1;
            twoCommonNodes2[1] = edgeB.Item2;
            double angle2 = GeneralMetricCalcMethods.getAngle(node2, twoCommonNodes2);

            double combinedAngles = angle1 + angle2;

            // delta from 180 in the pair of angles is the deviation between the two edges
            return Math.Abs(180 - combinedAngles);
        }


        /// <summary>
        /// I want to work out for all the pairs of opposite edges in the node what is the maximum deviation from parrallell
        /// </summary>
        /// <returns>The maximum deviation angle between two opposite edges within the element</returns>
        public static double computeMaxparallelDev(Tuple<Node, Node>[] edges)
        {
            double dev1 = getDevOnEdgePair(edges[0], edges[3]);
            double dev2 = getDevOnEdgePair(edges[1], edges[2]);
            return dev1 > dev2 ? dev1 : dev2;

            // throw new NotImplementedException();
        }


        /// <summary>
        /// Get the angle between a node and two other nodes
        /// </summary>
        /// <param name="angleNode">The node in the corner of the angle that is being computed</param>
        /// <param name="twoCommonNodes">Two nodes either side of that node within the element that will be used to compute the angle</param>
        /// <returns></returns>
        public static double getAngle(Node angleNode, Node[] twoCommonNodes)
        {
            // using method described here: http://stackoverflow.com/questions/19729831/angle-between-3-points-in-3d-space

            double[] v1 = new double[] { twoCommonNodes[0].GetX - angleNode.GetX, twoCommonNodes[0].GetY - angleNode.GetY, twoCommonNodes[0].GetZ - angleNode.GetZ };
            double[] v2 = new double[] { twoCommonNodes[1].GetX - angleNode.GetX, twoCommonNodes[1].GetY - angleNode.GetY, twoCommonNodes[1].GetZ - angleNode.GetZ };

            double v1mag = Math.Sqrt(v1[0] * v1[0] + v1[1] * v1[1] + v1[2] * v1[2]);
            double[] v1norm = { v1[0] / v1mag, v1[1] / v1mag, v1[2] / v1mag };

            double v2mag = Math.Sqrt(v2[0] * v2[0] + v2[1] * v2[1] + v2[2] * v2[2]);
            double[] v2norm = { v2[0] / v2mag, v2[1] / v2mag, v2[2] / v2mag };


            double result = v1norm[0] * v2norm[0] + v1norm[1] * v2norm[1] + v1norm[2] * v2norm[2];
            double angle = Math.Acos(result);
            return angle;
        }

        /// <summary>
        /// For a particular node this method can be used to find up to two other nodes that are adjacent to it, thus in the case of quad4 elements
        /// allowing the programmer to ascertain if the element is rectangular or not and if so which side is the longest.
        /// </summary>
        /// <param name="nodeA">the node you want to find the non diagonally adjacent nodes for</param>
        /// <param name="nodes">all the nodes in that particular element</param>
        /// <returns></returns>
        public static Node[] getNonDiagAdjacentNodes(Node nodeA, List<Node> nodes)
        {
            Node[] commonNodes = new Node[2];
            int ii = 0;

            foreach (Node nodeB in nodes)
            {
           
                if (GeneralRefinementMethods.isAdjacentQuad4(nodeA, nodeB, nodes) && nodeA != nodeB)
                {
                    commonNodes[ii] = nodeB;
                    ii++;
                }
            }
            return commonNodes;
        }

        /// <summary>
        /// currently assumes that three of the nodes have two axis in common, can't do irregular shapes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static double computeMaxCornerAngle(List<Node> fourPlaneNodes)
        {
            // for each element, find the two  others which have at least two planes in common

            double maxAngle = 0;
            foreach (Node nodeA in fourPlaneNodes)
            {
                Node[] commonNodes = getNonDiagAdjacentNodes(nodeA, fourPlaneNodes);
                // get angle here
                double angle = getAngle(nodeA, commonNodes);
                if (angle > maxAngle)
                {
                    maxAngle = angle;
                }
            }
            return maxAngle;
        }
    }
}
