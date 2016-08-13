using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Xml;
using System.IO;
using System.Text;
using System.Linq;

using DisertationFEPrototype.Model.MeshDataStructure;
using DisertationFEPrototype.Model;

namespace DisertationFEPrototype.FEModelUpdate
{
    class ReadMeshData
    {
        MeshData meshData;
        

        public MeshData GetMeshData{
            get{
                return this.meshData;
            }
        }

        /// <summary>
        /// load in our lisa file which contains all the data about our FE Model and extract node data about the model
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns>a MeshData object which represents the model internally so that it can be manipulated</returns>
        public ReadMeshData(string lisaString)
        {
     
            // This text is added only once to the file.
            if (File.Exists(lisaString))
            {
                string xmlString = File.ReadAllText(lisaString);
                List<Node> nodes = readAllNodes(xmlString);
                List<Element> elements = readAllElements(xmlString, nodes);
               
                this.meshData = new MeshData(nodes, elements);
            }
            else
            {
                throw new FileNotFoundException("Could not load the lisa mesh file to rebuild model");
            }

          
        }

        private List<Node> readAllNodes(string xmlString)
        {
            const string nodeTag = "node";
            List<Node> nodes = new List<Node>();
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement() && reader.Name == "elset")
                    {
                        break;
                    }
                    else if (reader.IsStartElement() && reader.Name == nodeTag)
                    {
                        // Get element name and switch on it.
                        Node node = getNodeData(reader);
                        nodes.Add(node);
                    }
                    
                }
            }
            return nodes;
        }

        private List<Element> readAllElements(string xmlString, List<Node> nodes)
        {
            const string elemTag = "elem";

            List<Element> elements = new List<Element>();

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                while (reader.Read())
                {
                    if(reader.IsStartElement() && reader.Name == "faceselection" || reader.Name == "Unnamed(2)")
                    {
                        Console.WriteLine(reader.Name);
                        break;
                    }
                    else if (reader.IsStartElement() && reader.Name == elemTag)
                    {
                        // Get element name and switch on it.
                        Element element = getElementData(reader, nodes);
                        elements.Add(element);

                    }
                }
            }
            return elements;
        }


        private Element getElementData(XmlReader reader, List<Node> nodes)
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

                foreach (Node node in nodes)
                {
                    foreach (int elemNodeId in elemNodeIds)
                    {
                        if (node.GetId == id)
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
        private Node getNodeData(XmlReader reader)
        {
            const string nodeIdAtt = "nid";
            const string xAtt = "x";
            const string yAtt = "y";
            const string zAtt = "z";

            //read in properties from the xml file
            string elementId = reader[nodeIdAtt];
            string xStr = reader[xAtt];
            string yStr = reader[yAtt];
            string zStr = reader[zAtt];

            try
            {
                int id = Convert.ToInt32(elementId);
                double x = Convert.ToDouble(xStr);
                double y = Convert.ToDouble(yStr);
                double z = Convert.ToDouble(zStr);
                Node node = new Node(id, x, y, z);
                return node;
            }
            catch
            {
                throw new Exception("Could not read node data from xml correctly");
            }

        }
    }
}
