// using DisertationFEPrototype.Model.MeshDataStructure;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DisertationFEPrototype.Model;

using DisertationFEPrototype.FEModelUpdate.Model.Structure;
using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using DisertationFEPrototype.Model.Structure;

namespace DisertationFEPrototype.FEModelUpdate.Read
{
    class ReadElements
    {

        //static Dictionary<string, Quad4Elem.Shape> shapeLookup = new Dictionary<string, Quad4Elem.Shape>()
        //{
        //    {"quad4", Quad4Elem.Shape.Quad4 },
        //    {"hex8", Quad4Elem.Shape.Hex8 }
        //};

        public static List<IElement> readAllElements(string xmlString, MeshData meshData)
        {
            const string elemTag = "elem";

            List<IElement> elements = new List<IElement>();

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
                        IElement element = getElementData(reader, meshData);
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
        /// <returns>Quad4Elem object </returns>
        private static IElement getElementData(XmlReader reader, MeshData meshData)
        {
            const string QUAD4_SHAPE = "quad4";
            const string HEX8_SHAPE = "hex8";

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

                // parse node ids as a string delimited by spaces to a list of ints
                List<int> elemNodeIds = rawNodes.Split(' ').Select(x => Convert.ToInt32(x)).ToList();

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
               
                // try {

                IElement newElement;

                if(shape == HEX8_SHAPE)
                {
                    newElement = new Hex8Elem(id, matchedNodes);
                }

                else if(shape == QUAD4_SHAPE)
                {
                    newElement = new Quad4Elem(id, matchedNodes);
                }
                else
                {
                    throw new Exception("IElement type created not handled by this program");
                }
                  
                return newElement;
                // }
                //catch(Exception e)
                //{
                //    List<Node> myNodes = meshData.Nodes.Values.ToList();
                //    myNodes.ForEach(node => Console.WriteLine("node ID: " + node.Id.ToString()));
                //    throw e;
                //}
                

                
            //}
            //catch
            //{
            //    throw new Exception("Could not read element data from xml correctly");
            //}
        }
       
        
    }
}
