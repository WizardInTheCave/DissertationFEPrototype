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

            try
            {
                int id = Convert.ToInt32(elementId);
                List<string> splitNodes = new List<string>(rawNodes.Split(' '));
                List<int> elemNodeIds = splitNodes.Select(x => Convert.ToInt32(x)).ToList();

                // get the nodes which we have been able to load in already
                List<Node> matchedNodes = new List<Node>();

                // iterate through all the stored nodes in the mesh, if we can find the node in the model already
                // then link it up to the element
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

                Element newElement = new Element(id, shape, matchedNodes);
                return newElement;
            }
            catch
            {
                throw new Exception("Could not read element data from xml correctly");
            }
        }
    }
}
