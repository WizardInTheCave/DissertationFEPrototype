using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.FEModelUpdate;

namespace DisertationFEPrototype.Model.MeshDataStructure
{
    public class Element
    {
        // was int?
        int? id;
        string shape;
        // id of nodes which form the element
        List<Node> nodes;
        List<Element> childElements;
        Element parentElement;

        double aspectRatio;
        double maxCornerAngle;
        double maxParallelDev;

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


        public string GetShape
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
        public List<Node> GetNodes
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

        private double computeMaxCornerAngle(List<Node> nodes)
        {
            // for each element, find the two  others which have at least two planes in common

            double maxAngle = 0;
            foreach (Node nodeA in nodes)
            {
                Node[] commonNodes = new Node[2];
                int ii = 0;
                bool[] theCommonPlanes = { false, false, false};

                foreach (Node nodeB in nodes)
                {
                    bool[] commonPlanes = GeneralGeomMethods.whichPlanesCommon(nodeA, nodeB);
                    if (GeneralGeomMethods.isCommonAxis(commonPlanes))
                    {
                        commonNodes[ii] = nodeB;
                        theCommonPlanes = commonPlanes;
                        ii++;
                    }
                }
                double angle = getAngle(theCommonPlanes, nodeA, commonNodes);
                if(angle > maxAngle)
                {
                    maxAngle = angle;
                }
            }
            return maxAngle;
        }
        private double getAngle(bool[] theCommonPlanes, Node nodeA, Node[] commonNodes)
        {
            // we now have three nodes, time to work out the internal angle using our two edges
            // bool[] commonPlanes = GeneralGeomMethods.whichPlanesCommon(nodeA, commonNodes[0]);


            double LenA = getEdgeLength(theCommonPlanes, nodeA, commonNodes[0]);
            double LenB = getEdgeLength(theCommonPlanes, nodeA, commonNodes[1]);

            double angle;

            if (LenA > LenB)
            {

            }
            else if (LenB > LenA)
            {
               

            }
            // right angle
            else
            {

            }
            return angle;

        }


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
        }

        
    }
}
