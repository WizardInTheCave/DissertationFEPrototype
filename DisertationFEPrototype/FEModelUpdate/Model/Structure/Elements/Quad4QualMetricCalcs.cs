using DisertationFEPrototype.Model.Structure;
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

        /// <summary>
        /// Compute the aspect ratio for an element 
        /// </summary>
        /// <returns></returns>
        public double computeAspectRatio(double longestEdge, double shortestEdge)
        {
            double aspectRatio;
            aspectRatio = longestEdge / shortestEdge;
            return aspectRatio;
        }

        // area3D_Polygon(): computes the area of a 3D planar polygon
        //    Input:  int n = the number of vertices in the polygon
        //            Point[] V = an array of n+2 vertices in a plane
        //                       with V[n]=V[0] and V[n+1]=V[1]
        //            Point N = unit normal vector of the polygon's plane
        //    Return: the (float) area of the polygon

        private double computeEdgeLength(bool[] commonPlanes, Node nodeA, Node nodeB)
        {
            double edgeLength;
            int measuringAxis = commonPlanes.ToList().IndexOf(false);
            switch (measuringAxis)
            {
                case 0:
                    edgeLength = Math.Abs(nodeA.GetX - nodeB.GetX);
                    break;
                case 1:
                    edgeLength = Math.Abs(nodeA.GetY - nodeB.GetY);
                    break;
                case 2:
                    edgeLength = Math.Abs(nodeA.GetZ - nodeB.GetZ);
                    break;
                default:
                    throw new Exception("Quad4Elem:computeEdgeLength there was no differing axis, this can only mean that there are two nodes in the exact same position");
            };
            return edgeLength;
            // work out 
        }


        


        /// <summary>
        /// I want to work out for all the pairs of opposite edges in the node what is the maximum deviation from parrallell
        /// </summary>
        /// <returns>The maximum deviation angle between two opposite edges within the element</returns>
        public double computeMaxparallelDev()
        {

            Tuple<Node, Node>[] edges = getedgePairingsForNode();

            double dev1 = getDevOnEdgePair(edges[0], edges[3]);
            double dev2 = getDevOnEdgePair(edges[1], edges[2]);
            return dev1 > dev2 ? dev1 : dev2;

            // throw new NotImplementedException();
        }

        /// <summary>
        /// Get the angle between a node and two other nodes
        /// </summary>
        /// <param name="angleNode"></param>
        /// <param name="twoCommonNodes"></param>
        /// <returns></returns>
        private double getAngle(Node angleNode, Node[] twoCommonNodes)
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
        /// Take the opposite edges as parameters as this is what we want to calculate the deviation of
        /// </summary>
        /// <param name="edgeA"></param>
        /// <param name="edgeB"></param>
        /// <returns></returns>
        public double getDevOnEdgePair(Tuple<Node, Node> edgeA, Tuple<Node, Node> edgeB)
        {

            // work out 
            var node = edgeA.Item1;

            var twoCommonNodes = new Node[2];
            twoCommonNodes[0] = edgeA.Item2;
            twoCommonNodes[1] = edgeB.Item1;

            double angle1 = getAngle(node, twoCommonNodes);


            var node2 = edgeB.Item1;
            var twoCommonNodes2 = new Node[2];

            twoCommonNodes2[0] = edgeA.Item1;
            twoCommonNodes2[1] = edgeB.Item2;
            double angle2 = getAngle(node2, twoCommonNodes2);

            double combinedAngles = angle1 + angle2;

            // delta from 180 in the pair of angles is the deviation between the two edges
            return Math.Abs(180 - combinedAngles);
        }

        /// <summary>
        /// get a set of tuples which represent each of the edges within the element
        /// </summary>
        /// <returns></returns>
        public Tuple<Node, Node>[] getedgePairingsForNode()
        {

            var edges = new Tuple<Node, Node>[4];

            Node node = this.nodes[0];

            Node[] adjNodes = getNonDiagAdjacentNodes(node, this.nodes);
            edges[0] = new Tuple<Node, Node>(node, adjNodes[0]);
            edges[1] = new Tuple<Node, Node>(node, adjNodes[1]);


          
            
            // in the case of quad4 there is only one diagonal node.
            Node diagonalNode = GeneralRefinementMethods.getDiagonalNode(nodes.ToArray(), node);

            edges[2] = new Tuple<Node, Node>(adjNodes[0], diagonalNode);
            edges[3] = new Tuple<Node, Node>(adjNodes[1], diagonalNode);

            return edges;
        }



        /// <summary>
        /// For a particular node this method can be used to find up to two other nodes that are adjacent to it, thus in the case of quad4 elements
        /// allowing the programmer to ascertain if the element is rectangular or not and if so which side is the longest.
        /// </summary>
        /// <param name="nodeA">the node you want to find the non diagonally adjacent nodes for</param>
        /// <param name="nodes">all the nodes in that particular element</param>
        /// <returns></returns>
        private Node[] getNonDiagAdjacentNodes(Node nodeA, List<Node> nodes)
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
        public double computeMaxCornerAngle()
        {
            // for each element, find the two  others which have at least two planes in common

            double maxAngle = 0;
            foreach (Node nodeA in this.nodes)
            {
                Node[] commonNodes = getNonDiagAdjacentNodes(nodeA, this.nodes);
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
