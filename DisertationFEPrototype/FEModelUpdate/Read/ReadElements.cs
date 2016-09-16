using DisertationFEPrototype.Model.MeshDataStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DisertationFEPrototype.Model;

namespace DisertationFEPrototype.FEModelUpdate.Read
{
    class ReadElements
    {
        public static List<Element> readAllElements(string xmlString, MeshData meshData)
        {
            const string elemTag = "elem";

            List<Element> elements = new List<Element>();

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                bool inElementsSection = false;
                while (reader.Read())
                {
                    if (reader.IsStartElement() && inElementsSection && reader.Name != elemTag)
                    {
                        Console.WriteLine(reader.Name);
                        break;
                    }
                    else if (reader.IsStartElement() && reader.Name == elemTag)
                    {
                        // Get element name and switch on it.
                        Element element = getElementData(reader, meshData);
                        elements.Add(element);
                        inElementsSection = true;
                    }
                }
            }
            return elements;
        }

        /// <summary>
        /// Go through each element within the file and construct an element object in memory which we can then manipulate
        /// </summary>
        /// <param name="reader">Xml reader object which contains the lisa file data</param>
        /// <param name="nodes">List of all the node objects</param>
        /// <returns>Element object </returns>
        private static Element getElementData(XmlReader reader, MeshData meshData)
        {
            const string elementIdAtt = "eid";
            const string shapeAtt = "shape";
            const string nodesAtt = "nodes";

            //read in properties from the xml file
            string elementId = reader[elementIdAtt];
            string shape = reader[shapeAtt];
            string rawNodes = reader[nodesAtt];

            //try
            //{
                int id = Convert.ToInt32(elementId);
                List<string> splitNodes = new List<string>(rawNodes.Split(' '));
                var elemNodeIds = splitNodes.Select(x => Convert.ToInt32(x));

                // get the nodes which we have been able to load in already
                List<Node> matchedNodes = new List<Node>();

                // iterate through all the stored nodes in the mesh, if we can find the node in the model already
                // then link it up to the element

                // loses order because stored as a dictionary
                foreach (Node node in meshData.Nodes.Values)
                {
                    foreach (int elemNodeId in elemNodeIds)
                    {
                        if (node.Id == elemNodeId)
                        {
                            matchedNodes.Add(node);
                        }
                    }
                }
                matchedNodes = sortMatchedNodes(matchedNodes);

                Element newElement = new Element(id, shape, matchedNodes);
                return newElement;
            //}
            //catch
            //{
            //    throw new Exception("Could not read element data from xml correctly");
            //}
        }
        /// <summary>
        /// this method will sort the match nodes into an order such that when an element is generated
        /// </summary>
        private static List<Node> sortMatchedNodes(List<Node> nodes)
        {
            List<Node> sortMatchedNodes = new List<Node>();

            Node currentNode = nodes[0];
            sortMatchedNodes.Add(currentNode);
             
            // List<Node> remainingNodes = nodes.Skip(1).ToList();

            while (sortMatchedNodes.Count < 4)
            {
                int currentIdx = nodes.IndexOf(currentNode);
                List<int> nums = new List<int>() { 0, 1, 2, 3 };
                nums.Remove(currentIdx);

                var nodeComp1 = nodes[nums[0]];
                var nodeComp2 = nodes[nums[1]];
                var nodeComp3 = nodes[nums[2]];

                if (isCommonAxis(currentNode, nodeComp1) && !sortMatchedNodes.Contains(nodeComp1))
                {
                    //.Clone() as Node
                    sortMatchedNodes.Add(nodeComp1);
                    currentNode = nodeComp1;
                }
                else if (isCommonAxis(currentNode, nodeComp2) && !sortMatchedNodes.Contains(nodeComp2))
                {
                    //.Clone() as Node
                    sortMatchedNodes.Add(nodeComp2);
                    currentNode = nodeComp2;
                }
                else if (isCommonAxis(currentNode, nodeComp3) && !sortMatchedNodes.Contains(nodeComp3))
                {
                    // .Clone() as Node
                    sortMatchedNodes.Add(nodeComp3);
                    currentNode = nodeComp3;
                }
                else
                {
                    break;
                }
            }
            return sortMatchedNodes;

            //while (remainingNodes.Count > 0) { 
            ////for (; ii < remainingNodes.Count; ii++) {

            //    if (isCommonAxis(currentNode, remainingNodes[ii]))
            //    {
            //        Node reaminingNodeClone = remainingNodes[ii].Clone() as Node;
            //        // don't want to re map the same node twice

            //        remainingNodes.RemoveAt(ii);

            //        sortMatchedNodes.Add(reaminingNodeClone);
            //        currentNode = reaminingNodeClone;
            //        // break;
            //    }
            //    ii++;
            //}
            //if (sortMatchedNodes.Count == 4)
            //{
                
            //}
            //else
            //{
            //    throw new Exception("SortMatchedNodes method did not work by producing an output list of the same length as it's input");
            //}
        }
        private static bool isCommonAxis(Node firstNode, Node secondNode)
        {
            bool[] sameVals = GeneralGeomMethods.commonPlaneData(firstNode, secondNode);
            int commonAxisVals = sameVals.Count(b => b == true);
            return commonAxisVals == 2;
        }
    }
}
