using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype;

namespace DisertationFEPrototype.Model.MeshDataStructure
{
    public class Element
    {
        // was int?
        int? id;
        // enum Shape { Quad4, Tri3};
        string shape;
        // id of nodes which form the element
        List<Node> nodes;
        List<Element> childElements;
        Element parentElement;

        double aspectRatio;
        double maxCornerAngle;
        double maxParallelDev;
        double area;
        int flatPlaneIndex;

        readonly double LONGEST_EDGE_DEFAULT = 0.0;
        readonly double SHORTEST_EDGE_DEFAULT = 1000000.0;

        double longestEdge = 0.0;
        double shortestEdge = 1000000.0;


        readonly string QUAD4 = "Quad4";
        readonly string TRI3 = "Tri3";

        public double Area
        {
            get
            {
                return this.area;
            }
        }

        public double AspectRatio
        {
            get
            {
                return this.aspectRatio;
            }
        }

        public double MaxCornerAngle
        {
            get
            {
                return this.maxCornerAngle;
            }
        }
        public double MaxParallelDev
        {
            get
            {
                return this.maxParallelDev;
            }
        }


        public string Shape
        {
            get
            {
                return this.shape;
            }
        }
        public int? Id
        {
            get
            {
                return id;
            }
            set
            {
                this.id = value;
            }
        }
        public List<Node> Nodes
        {
            get
            {
                return this.nodes;
            }
        }
        public Element GetParent
        {
            get
            {
                return this.parentElement;
            }

        }
        public List<Element> Children
        {
            get{
                return this.childElements;
            }
            set
            {
                this.childElements = value;
            }
        }
        public double LongestEdge
        {
            get
            {
                return this.longestEdge;
            }
        }
        public double ShortestEdge
        {
            get
            {
                return this.shortestEdge;               
            }
        }
        public int FlatPlaneIndex
        {
            get
            {
                if(this.flatPlaneIndex != -1)
                {
                    return this.flatPlaneIndex;
                }
                else
                {
                    throw new Exception("Sorry but this element is not situated precisely on an x,y,z plane");
                }
            }
        }

        private int computeCommonPlane()
        {
            // to be considered an edge node one of the nodes four partners should be 
            // positioned on a different plane to the other three??? (one x,y or z value which 3 out of 4 have in
            // common

            bool[] commonPlanesA = GeneralGeomMethods.whichPlanesCommon(this.nodes[0], this.nodes[1]);
            bool[] commonPlanesB = GeneralGeomMethods.whichPlanesCommon(this.nodes[1], this.nodes[2]);
            bool[] commonPlanesC = GeneralGeomMethods.whichPlanesCommon(this.nodes[2], this.nodes[3]);

            for (int ii = 0; ii < this.Nodes.Count; ii++)
            {
                bool a = commonPlanesA[ii];
                bool b = commonPlanesB[ii];
                bool c = commonPlanesC[ii];

                if (a == b == c == true)
                {
                    return ii;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// Compute the aspect ratio for an element 
        /// </summary>
        /// <returns></returns>
        private double computeAspectRatio()
        {
            double aspectRatio;
            aspectRatio = LongestEdge / ShortestEdge;
            return aspectRatio;
        }

        /// <summary>
        /// Compute the longest edge for an element
        /// </summary>
        /// <returns></returns>
        private double computeLongestEdge()
        {
            double longestEdge = SHORTEST_EDGE_DEFAULT;

            // this is a bit inefficent because going over each edge twice, but most edges we are going to have is 4 so
            // it doesn't matter too much, may write it better later
            foreach (Node nodeA in this.nodes)
            {
                foreach (Node nodeB in this.nodes)
                {
                    //bool[] commonPlanes = GeneralGeomMethods.whichPlanesCommon(nodeA, nodeB);
                    //if (GeneralGeomMethods.isCommonAxis(commonPlanes))

                    double edgeLength = GeneralGeomMethods.distanceBetweenPoints(nodeA, nodeB);
                    if (edgeLength > longestEdge)
                    {
                        longestEdge = edgeLength;
                    }
                      
                }
            }
            return longestEdge;
        }

        private double computeShortestEdge()
        {
            double shortestEdge = LONGEST_EDGE_DEFAULT;

            // this is a bit inefficent because going over each edge twice, but most edges we are going to have is 4 so
            // it doesn't matter too much, may write it better later
            foreach (Node nodeA in this.nodes)
            {
                foreach (Node nodeB in this.nodes)
                {
                    double edgeLength = GeneralGeomMethods.distanceBetweenPoints(nodeA, nodeB);
                    if (edgeLength < shortestEdge)
                    {
                        shortestEdge = edgeLength;
                    } 
                }
            }
            return shortestEdge;
        }

        public Node getDiagonalNode(Node queryNode){
            if (nodes.Contains(queryNode))
            {
                List<Node> sortedNodes = GeneralGeomMethods.sortMatchedNodes(this.nodes);
                int diagIndex = (sortedNodes.IndexOf(queryNode) + 2) % 4;
                Node diagNode = sortedNodes[diagIndex];
                return diagNode;
            }
            else
            {
                throw new Exception("canot compute the diagonal node when the node you have provided is not a part of this element");
            }

        }

        /// <summary>
        /// currently assumes that three of the nodes have two axis in common, can't do irregular shapes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private double computeMaxCornerAngle()
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

        /// <summary>
        /// For a particular node this method can be used to find up to two other nodes that are adjacent to it, thus in the case of quad4 elements
        /// allowing the programmer to ascertain if the element is rectangular or not and if so which side is the longest.
        /// </summary>
        /// <param name="nodeA">the node you want to find the non diagonally adjacent nodes for</param>
        /// <param name="nodes">all the nodes in that particular element</param>
        /// <returns></returns>
        private static Node[] getNonDiagAdjacentNodes(Node nodeA, List<Node> nodes)
        {
            Node[] commonNodes = new Node[2];
            int ii = 0;
            // bool[] theCommonPlanes = { false, false, false };

            foreach (Node nodeB in nodes)
            {
                bool[] commonPlanes = GeneralGeomMethods.whichPlanesCommon(nodeA, nodeB);
                if (GeneralGeomMethods.isCommonAxis(commonPlanes))
                {
                    commonNodes[ii] = nodeB;
                    // theCommonPlanes = commonPlanes;
                    ii++;
                }
            }
            return commonNodes;
        }

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
        /// using the method suggested here currently
        /// http://stackoverflow.com/questions/2350604/get-the-surface-area-of-a-polyhedron-3d-object
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private double computeArea()
        {
            // element has no area (not polygon)
            if (this.nodes.Count < 3)
            {
                return 0;
            }

            double[] total = getTotalCrossProduct(this.nodes);
            var elemNormal = GeneralGeomMethods.unitNormal(this.nodes[0], this.nodes[1], this.nodes[2]);

            double result = GeneralGeomMethods.dotProduct(total, elemNormal);

            // just width * height in this case
            if (double.IsNaN(result))
            {
                double b = ShortestEdge;
                double h = LongestEdge;
                // enum Shape { Quad4, Tri3 };

                if (this.shape == QUAD4)
                {
                    result = b * h;
                }
                else if (this.shape == TRI3)
                {
                    result = (b * h) / 2;
                }
            }
            else
            {
                result = Math.Abs(result / 2);
            }
            return result;
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
                    throw new Exception("Element:computeEdgeLength there was no differing axis, this can only mean that there are two nodes in the exact same position");
            };
            return edgeLength;
            // work out 
        }
        //private double computeEdgeLength(Node nodeA, Node nodeB)
        //{
        //    double edgeLength;
        //    bool[] commonPlanes = GeneralGeomMethods.whichPlanesCommon(nodeA, nodeB);
        //    int measuringAxis = commonPlanes.ToList().IndexOf(false);
        //    switch (measuringAxis)
        //    {
        //        case 0:
        //            edgeLength = Math.Abs(nodeA.GetX - nodeB.GetX);
        //            break;
        //        case 1:
        //            edgeLength = Math.Abs(nodeA.GetY - nodeB.GetY);
        //            break;
        //        case 2:
        //            edgeLength = Math.Abs(nodeA.GetZ - nodeB.GetZ);
        //            break;
        //        default:
        //            throw new Exception("Element:computeEdgeLength there was no differing axis, this can only mean that there are two nodes in the exact same position");
        //    };
        //    return edgeLength;
        //}


        public Element(int? id, string shape, List<Node> nodes)
        {
            this.id = id;
            this.shape = shape;
            this.nodes = nodes;
            this.parentElement = null;
            this.childElements = null;

            // all three of these methods use 
            this.aspectRatio = computeAspectRatio();
            this.maxCornerAngle = computeMaxCornerAngle();
            this.area = computeArea();
            this.longestEdge = computeLongestEdge();
            this.shortestEdge = computeShortestEdge();
            this.flatPlaneIndex = computeCommonPlane();
        }
    }
}
