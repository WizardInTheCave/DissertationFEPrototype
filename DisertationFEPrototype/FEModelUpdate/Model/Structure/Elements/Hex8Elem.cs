using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using DisertationFEPrototype.Model.Structure;
using DisertationFEPrototype.Optimisations;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    class Hex8Elem : IElement
    {
        Expression expression;

        int? id;

        // id of nodes which form the element
        List<Node> nodes;
        List<Hex8Elem> childElements;
        Hex8Elem parentElement;

        double aspectRatio;
        double maxCornerAngle;
        double maxParallelDev;

        Node[][] faces;

        double area;
        // int flatPlaneIndex;

        readonly double LONGEST_EDGE_DEFAULT = 0.0;
        readonly double SHORTEST_EDGE_DEFAULT = 1000000.0;

        double longestEdge = 0.0;
        double shortestEdge = 1000000.0;

        Hex8Refinement hex8Refinement;
        Hex8QualMetricCalcs propCalcs;

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

        public double Area { get { return this.area; } }

        public double AspectRatio { get { return this.aspectRatio; } }

        public double MaxCornerAngle { get { return this.maxCornerAngle; } }

        public double MaxParallelDev { get { return this.maxParallelDev; } }

        public List<Node> Nodes { get { return nodes; } }

        public List<IElement> Children
        {
            get
            {
                if (childElements != null)
                {
                    return childElements.Cast<IElement>().ToList();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                // downcast to hex8 elems
                childElements = value.Select(x => (Hex8Elem)x).ToList();
            }
        }

        public Hex8Elem(int? id, List<Node> nodes)
        {
            
            this.id = id;

            
            this.nodes = Hex8Refinement.sortNodes(nodes);

            faces = Hex8Refinement.getFacesSplitFromPointCloud(this.nodes);

            //propCalcs = new Hex8QualMetricCalcs(nodes);

            //// all three of these methods use 

            //maxCornerAngle = propCalcs.computeMaxCornerAngle();
            //maxParallelDev = propCalcs.computeMaxparallelDev();

            
            //longestEdge = GeneralRefinementMethods.computeLongestEdge(this.nodes, SHORTEST_EDGE_DEFAULT);
            //shortestEdge = GeneralRefinementMethods.computeShortestEdge(this.nodes, LONGEST_EDGE_DEFAULT);
            //aspectRatio = propCalcs.computeAspectRatio(longestEdge, shortestEdge);
            //area = propCalcs.computeArea(faces, longestEdge, shortestEdge);

        }

        private bool nodeAlreadyExists(Node node, List<Node> checkingList)
        {
            const double TOLERANCE = 0.01;

            foreach(Node checkedNode in checkingList)
            {
                if(
                    Math.Abs(node.GetX - checkedNode.GetX) < TOLERANCE &&
                    Math.Abs(node.GetY - checkedNode.GetY) < TOLERANCE &&
                    Math.Abs(node.GetZ - checkedNode.GetZ) < TOLERANCE)
                {
                    return true;
                }
                // else continue checking
            }
            return false; 
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        private List<Node> faceNodesWithoutDuplicates(Node[][] nodeArray2d)
        {
            List<Node> nodesNoDups = new List<Node>();

            Node[] firstArray = nodeArray2d[0];
            nodesNoDups.AddRange(firstArray);

            //if(nodesNoDups.Count < 4)
            //{
            //    Console.WriteLine("WHAT??");
            //}
            // loop through the others
            for(int ii=1; ii < nodeArray2d.Length; ii++)
            {
                foreach(Node node in nodeArray2d[ii])
                {
                    Console.WriteLine(node.GetX + ", " + node.GetY + ", " + node.GetZ);
                    if (!nodeAlreadyExists(node, nodesNoDups))
                    {
                        nodesNoDups.Add(node);
                    }
                }
            }

            // nodesNoDups.ForEach( x => Console.WriteLine(x.GetX + ", " + x.GetY + ", " + x.GetZ));
            Console.WriteLine("\n");

            return nodesNoDups;
        }
      
        /// <summary>
        /// Need to be split up Hex8 elements for structures where volume is important
        /// Call this method to create four sub Hex8 elems within the current Hex8 elem
        /// </summary>
        /// <param name="elem">Hex element we want to split into four sub elements using h-refinement</param>
        /// <param name="nodes">Lookup of all nodes in the current model</param>
        /// <returns>A new set of elements which comprise</returns>
        public List<IElement> createChildElements(Dictionary<Tuple<double, double, double>, Node> nodes)
        {

            Node hexCentre = getHexCentre(faces, nodes);

            Node[][] xPosSubs = getSubSquares(faces[0], nodes);
            Node[][] xNegSubs = getSubSquares(faces[1], nodes);
            Node[][] yPosSubs = getSubSquares(faces[2], nodes);
            Node[][] yNegSubs = getSubSquares(faces[3], nodes);

            Node[][] zPosSubs = getSubSquares(faces[4], nodes);
            Node[][] ZNegSubs = getSubSquares(faces[5], nodes);

            List<Node> xPositive = faceNodesWithoutDuplicates(xPosSubs);
            List<Node> xNegative = faceNodesWithoutDuplicates(xNegSubs);

            List<Node> yPositive = faceNodesWithoutDuplicates(yPosSubs);
            List<Node> yNegative = faceNodesWithoutDuplicates(yNegSubs);

            List<Node> zPositive = faceNodesWithoutDuplicates(zPosSubs);
            List<Node> zNegative = faceNodesWithoutDuplicates(ZNegSubs);

            // make four new smaller hexes inside the main hex
            // make subHex in top right hand corner
            Hex8Refinement h8Refine = new Hex8Refinement(hexCentre, xPositive, xNegative, yPositive, yNegative, zPositive, zNegative);

            return h8Refine.SubElems;
          
        }

        private void secondCorner()
        {

        }



        /// <summary>
        /// Make a new Hex Quad4Elem, by making a new Quad4Elem object with the right ordering for the input nodes
        /// And the right element type
        /// </summary>
        /// <returns>new hex element for that quadrent of the previous hex</returns>
       


            //Console.WriteLine("X subsquares: ");
            //Console.WriteLine("Sub1 Node1: " + xPosSubs[0][0]);
            //Console.WriteLine("Sub1 Node2: " + xPosSubs[0][1]);
            //Console.WriteLine("Sub1 Node3: " + xPosSubs[0][2]);
            //Console.WriteLine("Sub1 Node4: " + xPosSubs[0][3]);
            //Console.WriteLine("");
            //Console.WriteLine("Sub2 Node1: " + xPosSubs[1][0]);
            //Console.WriteLine("Sub2 Node2: " + xPosSubs[1][1]);
            //Console.WriteLine("Sub2 Node3: " + xPosSubs[1][2]);
            //Console.WriteLine("Sub2 Node4: " + xPosSubs[1][3]);
            //Console.WriteLine("");
            //Console.WriteLine("Sub3 Node1: " + xPosSubs[2][0]);
            //Console.WriteLine("Sub3 Node2: " + xPosSubs[2][1]);
            //Console.WriteLine("Sub3 Node3: " + xPosSubs[2][2]);
            //Console.WriteLine("Sub3 Node4: " + xPosSubs[2][3]);
            //Console.WriteLine("");
            //Console.WriteLine("Sub4 Node1: " + xPosSubs[3][0]);
            //Console.WriteLine("Sub4 Node2: " + xPosSubs[3][1]);
            //Console.WriteLine("Sub4 Node3: " + xPosSubs[3][2]);
            //Console.WriteLine("Sub4 Node4: " + xPosSubs[3][3]);
            //Console.WriteLine("");


            //Console.WriteLine("Y subsquares: ");
            //Console.WriteLine("Sub1 Node1: " + yPosSubs[0][0]);
            //Console.WriteLine("Sub1 Node2: " + yPosSubs[0][1]);
            //Console.WriteLine("Sub1 Node3: " + yPosSubs[0][2]);
            //Console.WriteLine("Sub1 Node4: " + yPosSubs[0][3]);
            //Console.WriteLine("");
            //Console.WriteLine("Sub2 Node1: " + yPosSubs[1][0]);
            //Console.WriteLine("Sub2 Node2: " + yPosSubs[1][1]);
            //Console.WriteLine("Sub2 Node3: " + yPosSubs[1][2]);
            //Console.WriteLine("Sub2 Node4: " + yPosSubs[1][3]);
            //Console.WriteLine("");
            //Console.WriteLine("Sub3 Node1: " + yPosSubs[2][0]);
            //Console.WriteLine("Sub3 Node2: " + yPosSubs[2][1]);
            //Console.WriteLine("Sub3 Node3: " + yPosSubs[2][2]);
            //Console.WriteLine("Sub3 Node4: " + yPosSubs[2][3]);
            //Console.WriteLine("");
            //Console.WriteLine("Sub4 Node1: " + yPosSubs[3][0]);
            //Console.WriteLine("Sub4 Node2: " + yPosSubs[3][1]);
            //Console.WriteLine("Sub4 Node3: " + yPosSubs[3][2]);
            //Console.WriteLine("Sub4 Node4: " + yPosSubs[3][3]);
            //Console.WriteLine("");

            //Console.WriteLine("Z subsquares: ");
            //Console.WriteLine("Sub1 Node1: " + topSubs[0][0]);
            //Console.WriteLine("Sub1 Node2: " + topSubs[0][1]);
            //Console.WriteLine("Sub1 Node3: " + topSubs[0][2]);
            //Console.WriteLine("Sub1 Node4: " + topSubs[0][3]);
            //Console.WriteLine("");
            //Console.WriteLine("Sub2 Node1: " + topSubs[1][0]);
            //Console.WriteLine("Sub2 Node2: " + topSubs[1][1]);
            //Console.WriteLine("Sub2 Node3: " + topSubs[1][2]);
            //Console.WriteLine("Sub2 Node4: " + topSubs[1][3]);
            //Console.WriteLine("");
            //Console.WriteLine("Sub3 Node1: " + topSubs[2][0]);
            //Console.WriteLine("Sub3 Node2: " + topSubs[2][1]);
            //Console.WriteLine("Sub3 Node3: " + topSubs[2][2]);
            //Console.WriteLine("Sub3 Node4: " + topSubs[2][3]);
            //Console.WriteLine("");
            //Console.WriteLine("Sub4 Node1: " + topSubs[3][0]);
            //Console.WriteLine("Sub4 Node2: " + topSubs[3][1]);
            //Console.WriteLine("Sub4 Node3: " + topSubs[3][2]);
            //Console.WriteLine("Sub4 Node4: " + topSubs[3][3]);
            //Console.WriteLine("");

            //hexNodees[3] = yPosSubs


        private static Node getHexCentre(Node[][] faces, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            // each of the sub divided sections, now need to sew up
            var xFront = getSubSquares(faces[0], nodes);
            var xBack = getSubSquares(faces[1], nodes);
            var yFront = getSubSquares(faces[2], nodes);
            var yBack = getSubSquares(faces[3], nodes);

            var top = getSubSquares(faces[4], nodes);
            var bottom = getSubSquares(faces[5], nodes);

            Node xFrontCenter = xFront[0][2];
            Node xBackCenter = xBack[0][2];

            Node yFrontCenter = yFront[0][2];
            Node yBackCenter = yBack[0][2];

            Node topCenter = top[0][2];
            Node bottomCentre = bottom[0][2];

            int newID = nodes.Count + 1;

            double x = (xFrontCenter.GetX + xBackCenter.GetX) / 2;
            double y = (yFrontCenter.GetY + yBackCenter.GetY) / 2;
            double z = (topCenter.GetZ + bottomCentre.GetZ) / 2;

            Node hexCentre = new Node(newID, x, y, z);
            return hexCentre;
        }



        /// <summary>
        /// Get given a set of corner nodes for a Quad4 element create nodes along the edges and a node in the centre
        /// So that the space can be split up into sub elements
        /// </summary>
        /// <returns>array of sub element points</returns>
        private static Node[][] getSubSquares(Node[] cornerNodes, Dictionary<Tuple<double, double, double>, Node> allNodes)
        {

            var subNodeTup = GeneralRefinementMethods.createMidpointNodes(cornerNodes, allNodes);

            List<Node[]> elementEdgeTrios = subNodeTup.Item1;
            List<Node> midpointLineNodes = subNodeTup.Item2;

            // get the new center node which will be a corner for each of the four new elements
            Node centerNode = GeneralRefinementMethods.createCenterNode(midpointLineNodes, allNodes);

            Node[][] subSquares = new Node[4][];


            int ii = 0;
            foreach (Node[] trio in elementEdgeTrios)
            {
                // add the centre node to get the smaller element
                trio[2] = centerNode;
                subSquares[ii] = trio;
                ii++;
            }
            return subSquares;
        }


        public List<Node> getDiagonalNodes(Node currentNode)
        {
            throw new NotImplementedException();
        }
    }
}
