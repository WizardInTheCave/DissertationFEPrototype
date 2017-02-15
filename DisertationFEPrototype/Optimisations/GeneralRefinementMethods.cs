using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.FEModelUpdate;

using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using DisertationFEPrototype.Model.Structure;

namespace DisertationFEPrototype.Optimisations
{
    /// <summary>
    /// note this method currently only works for finite element, 
    /// </summary>
    static class GeneralRefinementMethods
    {

        /// <summary>
        /// Get given a set of corner nodes for a Quad4 element create nodes along the edges and a node in the centre
        /// So that the space can be split up into sub elements
        /// </summary>
        /// <returns>array of sub element points</returns>
        public static Node[][] getSubSquares(Node[] cornerNodes, Dictionary<Tuple<double, double, double>, Node> allNodes)
        {

            var subNodeTup = createMidpointNodes(cornerNodes, allNodes);

            List<Node[]> elementEdgeTrios = subNodeTup.Item1;
            List<Node> midpointLineNodes = subNodeTup.Item2;

            // get the new center node which will be a corner for each of the four new elements
            Node centerNode = createCenterNode(midpointLineNodes, allNodes);

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

        /// <summary>
        /// check if node already exists within the model, if yes then use the node object already in the model, else add
        /// the node to the model
        /// </summary>
        /// <param name="node">the node we want to check the presence of in the model</param>
        /// <returns>the safe node reference</returns>
        private static Node createNode(double x, double y, double z, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            var key = new Tuple<double, double, double>(x, y, z);

            // kind of expensive but fine for the time being, we just want to know what to use as the starting value for assigning node ids
            // nodes.Values.ToArray().Max(x => x.Id);

            Node node;
            if (nodes.ContainsKey(key))
            {
                node = nodes[key];
            }
            else
            {
                // set the centre node to the node that already exists, otherwise keep the one we just made
                int maxNodeCount = nodes.Values.ToArray().Select(a => a.Id).Max();
                node = new Node(maxNodeCount + 1, x, y, z);
                nodes[key] = node;
            }
            return node;
        }

        /// <summary>
        /// Make a new node half way along the edge of an element side
        /// </summary>
        /// <param name="node"></param>
        /// <param name="adjacentNode"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static Node makeMidEdgeNode(Node node, Node adjacentNode, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            double newX = (node.GetX + adjacentNode.GetX) / 2;
            double newY = (node.GetY + adjacentNode.GetY) / 2;
            double newZ = (node.GetZ + adjacentNode.GetZ) / 2;

            return createNode(newX, newY, newZ, nodes);
        }

        /// <summary>
        /// creates the node in the middle of the new set of four elements which we join the others too
        /// </summary>
        /// <param name="midpointLineNodes"></param>
        /// <returns>centeral node object</returns>
        public static Node createCenterNode(List<Node> midpointLineNodes, Dictionary<Tuple<double, double, double>, Node> nodes)
        {

            double maxX = midpointLineNodes.Max(node => node.GetX);
            double minX = midpointLineNodes.Min(node => node.GetX);
            double xVal = (maxX + minX) / 2;

            double maxY = midpointLineNodes.Max(node => node.GetY);
            double minY = midpointLineNodes.Min(node => node.GetY);
            double yVal = (maxY + minY) / 2;

            double maxZ = midpointLineNodes.Max(node => node.GetZ);
            double minZ = midpointLineNodes.Min(node => node.GetZ);
            double zVal = (maxZ + minZ) / 2;

            return createNode(xVal, yVal, zVal, nodes);
        }


        /// <summary>
        /// Determine if two nodes within a quad4 element are adjactent to one another based on their euclidean distance
        /// </summary>
        /// <param name="firstNode">Node we are want to check if the second node is adjacent to</param>
        /// <param name="secondNode">The second node which may or may not be adjacent</param>
        /// <param name="elem">The element that both nodes are contained within</param>
        /// <returns></returns>
        public static bool isAdjacentQuad4(Node firstNode, Node secondNode, List<Node> allElemNodes)
        {
            List<double> distancesBetweenFirstNodeAndOthers = allElemNodes.Select(elemNode => GeneralGeomMethods.distanceBetweenPoints(firstNode, elemNode)).ToList();
            double maxDistanceInElement = distancesBetweenFirstNodeAndOthers.Max();

            double distanceBetweenFirstAndSecond = GeneralGeomMethods.distanceBetweenPoints(firstNode, secondNode);
  

            //if (distancesBetweenFirstNodeAndOthers[1] == 0 || distancesBetweenFirstNodeAndOthers[2] == 0)
            //{
            //    //Console.WriteLine("FirstSecDist: " + distanceBetweenFirstAndSecond + " MaxDist: " + maxDistanceInElement + "\n");
            //    Console.WriteLine(distancesBetweenFirstNodeAndOthers[0] + " " + distancesBetweenFirstNodeAndOthers[1]
            //        + " " + distancesBetweenFirstNodeAndOthers[2] + "\n");
            //}


            // is the node we are trying to place next to it less then the max distance, if so then it is adjacent
            return distanceBetweenFirstAndSecond < maxDistanceInElement;
        }



        /// <summary>
        /// Sort nodes comprising a quad4 element 
        /// so when LISA reads them in the file it can form a valid element.
        /// is static so can be used when splitting individual faces of Hex8 elements
        /// </summary>
        /// <param name="nodes">Some nodes</param>
        /// <returns>The nodes in an order LISA will accept</returns>
        public static List<Node> sortFourNodes(List<Node> nodes)
        {
            List<Node> sortMatchedNodes = new List<Node>();

            if (nodes.Count < 4)
            {
                Console.WriteLine("What???");
            }

            // add the first node in the current list
            Node currentNode = nodes[0];
            sortMatchedNodes.Add(currentNode);

            // List<Node> remainingNodes = nodes.Skip(1).ToList();

            const int NODES_IN_QUAD_FOUR = 4;

            while (sortMatchedNodes.Count < NODES_IN_QUAD_FOUR)
            {
                // remove the node we just added to the sorted list
                int currentIdx = nodes.IndexOf(currentNode);
                List<int> nums = new List<int>() { 0, 1, 2, 3 };
                nums.Remove(currentIdx);

                Node nodeComp1 = nodes[nums[0]];
                Node nodeComp2 = nodes[nums[1]];
                Node nodeComp3 = nodes[nums[2]];

                List<double> smallDist1 = new List<double>(); 
                List<Node> nearNode1 = new List<Node>(); 

                if (isAdjacentQuad4(currentNode, nodeComp1, nodes) && !sortMatchedNodes.Contains(nodeComp1))
                {
                    smallDist1.Add(GeneralGeomMethods.distanceBetweenPoints(currentNode, nodeComp1));
                    nearNode1.Add(nodeComp1);
                }
                if (isAdjacentQuad4(currentNode, nodeComp2, nodes) && !sortMatchedNodes.Contains(nodeComp2))
                {
                    smallDist1.Add(GeneralGeomMethods.distanceBetweenPoints(currentNode, nodeComp2));
                    nearNode1.Add(nodeComp2);
                }
                if (isAdjacentQuad4(currentNode, nodeComp3, nodes) && !sortMatchedNodes.Contains(nodeComp3))
                {
                    smallDist1.Add(GeneralGeomMethods.distanceBetweenPoints(currentNode, nodeComp3));
                    nearNode1.Add(nodeComp3); 
                }

                // This bit of code here deals with issues where the diagonal isn't actually the longest distance from the currentNode, 
                // such a situation can occur when an element is quite skewed
                int idx = smallDist1.IndexOf(smallDist1.Min());
                currentNode = nearNode1[idx];
                sortMatchedNodes.Add(currentNode);

            }
            //if (sortMatchedNodes.Count < 4)
            //{
            //    Console.WriteLine("What???"); 
            //}
            return sortMatchedNodes;
        }

        /// <summary>
        /// for a Quad4 element get Node which is diagonal
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="queryNode"></param>
        /// <returns></returns>
        public static Node getDiagonalNode(Node[] nodes, Node queryNode)
        {


            if (nodes.Length < 4)
            {
                Console.WriteLine("What???");
            }
            

            if (nodes.Contains(queryNode)) {
                var fourNodes = nodes.ToList();
                List<Node> sortedNodes = sortFourNodes(fourNodes);


                if (sortedNodes.Count < 4)
                {
                    Console.WriteLine("What???");
                }

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
        /// Get four nodes half way along each of the edges when given four elements.
        /// </summary>
        /// <param name="elementNodes">four nodes within a particular element</param>
        /// <returns></returns>
        public static Tuple<List<Node[]>, List<Node>> createMidpointNodes(Node[] fourNodes, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            List<Node[]> elementEdgeTrios = new List<Node[]>();
            List<Node> midEdgeNodes = new List<Node>();



            foreach (Node node in fourNodes)
            {
                // this array r
                Node[] subElemNodes = new Node[4];

                // add the corner node to the trio
                subElemNodes[0] = node;

                //singleNodeTrio.Add(node);
                Node diag = getDiagonalNode(fourNodes, node);

                foreach (Node adjacentNode in fourNodes)
                {    
                    if (adjacentNode != node && diag != adjacentNode)
                    {
                        Node midEdge = makeMidEdgeNode(node, adjacentNode, nodes);
                        midEdgeNodes.Add(midEdge);

                        // this is important so that the mesh forms correct square elements
                        if(subElemNodes[1] == null)
                        {
                            subElemNodes[1] = midEdge;
                        }
                        else{
                            subElemNodes[3] = midEdge;
                        }
                    }
                }
                elementEdgeTrios.Add(subElemNodes);
            }
            return Tuple.Create(elementEdgeTrios, midEdgeNodes);
        }
    }
}
