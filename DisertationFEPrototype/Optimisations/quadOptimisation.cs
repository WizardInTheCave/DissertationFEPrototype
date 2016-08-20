using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.Model.MeshDataStructure;

namespace DisertationFEPrototype.Optimisations
{
    /// <summary>
    /// note this method currently only works for finite element, 
    /// </summary>
    static class QuadElementRefinement
    {
        
        ///
        public static List<Element> newElements(Element elem)
        {

            List<Element> newElements = new List<Element>();

            
            List<Node> originalNodes = elem.GetNodes;
            string constantAxis = getConstantAxis(originalNodes);

            var thing = createMidpointNodes(originalNodes);

            List<List<Node>> elementEdgeTrios = thing.Item1;
            List<Node> midpointLineNodes = thing.Item2;

            Node centreNode = createCentreNode(midpointLineNodes);

            return getNewElements(elementEdgeTrios, centreNode, constantAxis);

        }
        private static string getConstantAxis(List<Node> elementNodes)
        {
            // find the constant value
            List<double> xs = elementNodes.Select(x => x.GetX).ToList();
            List<double> ys = elementNodes.Select(x => x.GetY).ToList();
            List<double> zs = elementNodes.Select(x => x.GetZ).ToList();

            string constAxis;
            if (xs.Any(o => o != xs[0]))
            {
                constAxis = "X";
            }
            else if (ys.Any(o => o != ys[0]))
            {
                constAxis = "Y";
            }
            else if (zs.Any(o => o != zs[0]))
            {
                constAxis = "Z";
            }
            else
            {
                throw new Exception("No constant axis found");
            }
            return constAxis;

        }
        /// <summary>
        /// Create the new four interior elements for the current element
        /// </summary>
        /// <returns>the new elements which are the children of the original element</returns>
        private static List<Element> getNewElements(List<List<Node>> elementEdgeTrios, Node centreNode, string constantAxis)
        {
            List<Element> newElements = new List<Element>();

            foreach (List<Node> trio in elementEdgeTrios)
            {
                // add the centre node to get the smaller element
                trio.Add(centreNode);
                newElements.Add(new Element(null, "quad4", trio));
            }
            return newElements;
        }

        /// <summary>
        /// creates the node in the middle of the new set of four elements which we join the others too
        /// </summary>
        /// <param name="midpointLineNodes"></param>
        /// <returns>centeral node object</returns>
        private static Node createCentreNode(List<Node> midpointLineNodes)
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

        
            Node centralNode = new Node(null, xVal, yVal, zVal);
            return centralNode;
        }
        
        private static Tuple<List<List<Node>>, List<Node>> createMidpointNodes(List<Node> elementNodes)
        {
            List<Node> currentMidpointNodes = new List<Node>();
            List<List<Node>> elementEdgeTrios = new List<List<Node>>();

            foreach (Node node in elementNodes)
            {
                List < Node > singleNodeTrio = new List<Node>();
                singleNodeTrio.Add(node);

                foreach (Node adjacentNode in elementNodes)
                {
                    if (adjacentNode != node)
                    {
                        short commonAxisVals = 0;


                        bool[] sameVals = new bool[3] { false, false, false };

                        if (adjacentNode.GetX == node.GetX)
                        {
                            sameVals[0] = true;
                            commonAxisVals++;
                        }
                        if (adjacentNode.GetY == node.GetY)
                        {
                            sameVals[1] = true;
                            commonAxisVals++;
                        }
                        if (adjacentNode.GetZ == node.GetZ)
                        {
                            sameVals[2] = true;
                            commonAxisVals++;
                        }
                        // if two of the nodes share two values in x y or z then they are on a plane together.
                        if (commonAxisVals == 2)
                        {
                            Node midEdge = makeMidEdgeNode(sameVals, node, adjacentNode);

                            Node alreadyRecordedEquiv = tryGetAlreadyRecorded(midEdge, currentMidpointNodes);
                            if (alreadyRecordedEquiv == null)
                            {
                                currentMidpointNodes.Add(midEdge);
                                singleNodeTrio.Add(midEdge);
                            }
                            else
                            {
                                singleNodeTrio.Add(alreadyRecordedEquiv);
                            }
                        }
                    }
                }
                elementEdgeTrios.Add(singleNodeTrio);
            }
            return Tuple.Create(elementEdgeTrios, currentMidpointNodes);
        }
        private static Node tryGetAlreadyRecorded(Node minEdge, List<Node> currentMidPointNodes)
        {
            foreach (Node currentNode in currentMidPointNodes)
            {
                if (minEdge.GetX == currentNode.GetX && minEdge.GetY == currentNode.GetY && minEdge.GetZ == currentNode.GetZ)
                {
                    return currentNode;
                }
            }
            return null;
        }


        /// <summary>
        /// return a node which lies on the midpoint between the two edges
        /// </summary>
        /// <returns></returns>
        private static Node makeMidEdgeNode(bool[] sameVals, Node node, Node adjacentNode)
        {

            // int id = this.meshData.GetNodes.Count + 1;

            // figure if it's the x, y or z coordinate which makes them different
            int index = Array.IndexOf(sameVals, false);

            double newX;
            double newY;
            double newZ;

            switch (index)
            {
                case 0:
                    newX = (node.GetX + adjacentNode.GetX) / 2;
                    newY = node.GetY;
                    newZ = node.GetZ;
                    break;

                case 1:
                    newX = node.GetX;
                    newY = (node.GetY + adjacentNode.GetY) / 2;
                    newZ = node.GetZ;
                    break;
                case 2:
                    newX = node.GetX;
                    newY = node.GetY;
                    newZ = (node.GetZ + adjacentNode.GetZ) / 2;
                    break;
                default:
                    throw new Exception("computeMidEdgeNode: index is incorrect, can't operate in more than 3d space");
            }
            return new Node(null, newX, newY, newZ);

        }
    }
}
