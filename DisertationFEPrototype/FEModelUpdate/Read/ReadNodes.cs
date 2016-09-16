using DisertationFEPrototype.Model.MeshDataStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DisertationFEPrototype.FEModelUpdate.Read
{
    class ReadNodes
    {
        public static Dictionary<Tuple<double, double, double>, Node> readAllNodes(string xmlString)
        {
            const string nodeTag = "node";
            var nodes = new Dictionary<Tuple<double, double, double>, Node>();

            // List<Node> nodes = new List<Node>();
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
                        //nodes.Add(node);
                        nodes[new Tuple<double, double, double>(node.GetX, node.GetY, node.GetZ)] = node;
                    }
                }
            }
            return nodes;
        }

        private static Node getNodeData(XmlReader reader)
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

                // allow fast lookup of a node in the database with just x, y, z choords, useful for checking node overlaps

                return node;
            }
            catch
            {
                throw new Exception("Could not read node data from xml correctly");
            }
        }
    }
}
