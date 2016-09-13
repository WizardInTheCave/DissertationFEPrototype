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
        
        /// <summary>
        /// given an element get a list of sub devided elements the sum of which forms that element
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static List<Element> newElements(Element elem, Dictionary<Tuple<double, double, double>,Node> nodes)
        {

            List<Element> newElements = new List<Element>();

            
            List<Node> originalNodes = elem.GetNodes;
            string constantAxis = getConstantAxis(originalNodes);

            var subNodeTup = createMidpointNodes(originalNodes, nodes);

            List<Node[]> elementEdgeTrios = subNodeTup.Item1;
            List<Node> midpointLineNodes = subNodeTup.Item2;

            Node centerNode = createCenterNode(midpointLineNodes, nodes);
           
           
            // list of all the new elements with their four nodes
            return getNewElements(elementEdgeTrios, centerNode, constantAxis);

        }

        
        /// <summary>
        /// check if node already exists within the model, if yes then use the node object already in the model, else add
        /// the node to the model
        /// </summary>
        /// <param name="node">the node we want to check the presence of in the model</param>
        /// 
        /// <returns>the safe node reference</returns>
        private static Node createNode(double x, double y, double z, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            var key = new Tuple<double, double, double>(x, y, z);

            // kind of expensive but fine for the time being, we just want to know what to use as the starting value for assigning node ids
            // nodes.Values.ToArray().Max(x => x.Id);

            Node node;
            if (nodes.ContainsKey(key)) {
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
        private static List<Element> getNewElements(List<Node[]> elementEdgeTrios, Node centreNode, string constantAxis)
        {
            List<Element> newElements = new List<Element>();

            foreach (Node[] trio in elementEdgeTrios)
            {
                // problem here is that the matched nodes need to be in such an order that they go around in squares,
                // not an order where nodes are linked across the center of the element

                // add the centre node to get the smaller element
                trio[2] = centreNode;
                var orderedTrio = trio.ToList();
                newElements.Add(new Element(null, "quad4", orderedTrio));
            }
            return newElements;
        }

        /// <summary>
        /// creates the node in the middle of the new set of four elements which we join the others too
        /// </summary>
        /// <param name="midpointLineNodes"></param>
        /// <returns>centeral node object</returns>
        private static Node createCenterNode(List<Node> midpointLineNodes, Dictionary<Tuple<double, double, double>, Node> nodes)
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

            
           
            Node centralNode = createNode(xVal, yVal, zVal, nodes);


            return centralNode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementNodes">four nodes within a particular element</param>
        /// <returns></returns>
        private static Tuple<List<Node[]>, List<Node>> createMidpointNodes(List<Node> elementNodes, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            List<Node[]> elementEdgeTrios = new List<Node[]>();
            List<Node> midEdgeNodes = new List<Node>();

            foreach (Node node in elementNodes)
            {
                Node[] singleNodeTrio = new Node[4];
                // add the corner node to the trio
                singleNodeTrio[0] = node;
                
                //singleNodeTrio.Add(node);

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
                            Node midEdge = makeMidEdgeNode(sameVals, node, adjacentNode, nodes);
                            midEdgeNodes.Add(midEdge);

                            // this is important so that the mesh forms correct square elements
                            if(singleNodeTrio[1] == null)
                            {
                                singleNodeTrio[1] = midEdge;
                            }
                            else{
                                singleNodeTrio[3] = midEdge;
                            }      
                        }
                    }
                }
                elementEdgeTrios.Add(singleNodeTrio);
            }
            return Tuple.Create(elementEdgeTrios, midEdgeNodes);
        }

        /// <summary>
        /// return a node which lies on the midpoint between the two edges
        /// </summary>
        /// <returns></returns>
        private static Node makeMidEdgeNode(bool[] sameVals, Node node, Node adjacentNode, Dictionary<Tuple<double, double, double>, Node> nodes)
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
            return createNode(newX, newY, newZ, nodes); 
        }
    }
}
