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
            get
            {
                return this.childElements;
            }
            set
            {
                this.childElements = value;
            }
        }


        private double computeAspectRatio(List<Node> nodes)
        {
            double aspectRatio;

            //List<Tuple<Node, Node>> nodePairs = new List<Tuple<Node, Node>>();
            double longestEdge = 0.0;
            double shortestEdge = 10000.0;

            // this is a bit inefficent because going over each edge twice, but most edges we are going to have is 4 so
            // it doesn't matter too much, may write it better later
            foreach (Node nodeA in nodes) {
                foreach (Node nodeB in nodes) {

                    bool[] commonPlanes = GeneralGeomMethods.whichPlanesCommon(nodeA, nodeB);
                    if (GeneralGeomMethods.isCommonAxis(commonPlanes))
                    {
                        double edgeLength = getEdgeLength(commonPlanes, nodeA, nodeB);
                        if (edgeLength > longestEdge)
                        {
                            longestEdge = edgeLength;
                        }
                        else if (edgeLength < shortestEdge)
                        {
                            shortestEdge = edgeLength;
                        }
                    }
                }
            }
            aspectRatio = longestEdge / shortestEdge;
            return aspectRatio;
        }

        /// <summary>
        /// currently assumes that three of the nodes have two axis in common, can't do irregular shapes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private double computeMaxCornerAngle(List<Node> nodes)
        {
            // for each element, find the two  others which have at least two planes in common

            double maxAngle = 0;
            foreach (Node nodeA in nodes)
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
                // get angle here
                double angle = getAngle(nodeA, commonNodes);
                if (angle > maxAngle)
                {
                    maxAngle = angle;
                }
            }
            return maxAngle;
        }

        private double getAngle(Node angleNode, Node[] commonNodes)
        {
            // using method described here: http://stackoverflow.com/questions/19729831/angle-between-3-points-in-3d-space

            double[] v1 = new double[] { commonNodes[0].GetX - angleNode.GetX, commonNodes[0].GetY - angleNode.GetY, commonNodes[0].GetZ - angleNode.GetZ };
            double[] v2 = new double[] { commonNodes[1].GetX - angleNode.GetX, commonNodes[1].GetY - angleNode.GetY, commonNodes[1].GetZ - angleNode.GetZ };
          

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
        private double computeArea(List<Node> nodes)
        {
            // element has no area (not polygon)
            if(nodes.Count < 3)
            {
                return 0;
            }

            double[] total = new double[] { 0, 0, 0 };
            for (int ii = 0; ii < nodes.Count; ii++)
            {
                Node vi1 = nodes[ii];
                Node vi2;

                if(ii == nodes.Count - 1)
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
            var elemNormal = GeneralGeomMethods.unitNormal(nodes[0], nodes[1], nodes[2]);

            double result = GeneralGeomMethods.dotProduct(total, elemNormal);

            // just width * height in this case
            if (double.IsNaN(result))
            {
                // enum Shape { Quad4, Tri3 };
                if(this.shape == "Quad4")
                {
                    double b = 
                    double h =
                    result = b * h;
                }
                else if(this.shape == "Tri3")
                {
                    double b = 
                    double h = 

                    result = (b / 2) * h;
                }
            }
            else
            {
                result = Math.Abs(result / 2);
            }
            return result;


        }
        // area3D_Polygon(): computes the area of a 3D planar polygon
        //    Input:  int n = the number of vertices in the polygon
        //            Point[] V = an array of n+2 vertices in a plane
        //                       with V[n]=V[0] and V[n+1]=V[1]
        //            Point N = unit normal vector of the polygon's plane
        //    Return: the (float) area of the polygon
        




        private double getEdgeLength(bool[] commonPlanes, Node nodeA, Node nodeB)
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
                    throw new Exception("Element:getEdgeLength there was no differing axis, this can only mean that there are two nodes in the exact same position");
            };
            return edgeLength;
            // work out 
        }
       

        public Element(int? id, string shape, List<Node> nodes)
        {
            this.id = id;
            this.shape = shape;
            this.nodes = nodes;
            this.parentElement = null;
            this.childElements = null;
            this.aspectRatio = computeAspectRatio(nodes);
            this.maxCornerAngle = computeMaxCornerAngle(nodes);
            this.area = computeArea(nodes);
        }

        
    }
}
