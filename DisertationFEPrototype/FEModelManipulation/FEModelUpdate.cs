using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Xml;
using System.IO;
using System.Text;


using System.Linq;
using DisertationFEPrototype.MeshDataStructure;


namespace DisertationFEPrototype.FEModelManipulation
{
    class FEModelUpdate
    {

        FEModel currentModel;
        /// <summary>
        /// update our FE model based on changes
        /// </summary>
        public void updateFEModel()
        {


        }
 
        /// <summary>
        /// load in our lisa file which contains all the data about our FE Model and extract node data about the model
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns>a FEModel object which represents the model internally so that it can be manipulated</returns>
        private FEModel readModelFile(string xmlString)
        {

            string xmlTest = @"<liml8>
  <analysis type=""S30"" />
  <node nid=""1"" x=""0"" y=""0"" z=""0"" />
  <node nid=""2"" x=""0"" y=""0"" z=""0"" />
  <node nid=""3"" x=""0"" y=""0"" z=""0"" />
  <node nid=""4"" x=""0"" y=""0"" z=""0"" />
  <node nid=""5"" x=""0"" y=""0"" z=""0"" />
  <node nid=""6"" x=""0"" y=""0"" z=""0"" />
  <node nid=""7"" x=""0"" y=""0"" z=""0"" />
  <node nid=""8"" x=""0"" y=""0"" z=""0"" />
  <node nid=""9"" x=""0"" y=""0"" z=""0"" />
  <node nid=""10"" x=""0"" y=""0"" z=""0"" />
  <node nid=""11"" x=""0"" y=""0"" z=""0"" />
  <node nid=""12"" x=""0"" y=""0"" z=""0"" />
  <node nid=""13"" x=""0"" y=""0"" z=""0"" />
  <node nid=""14"" x=""0.694485306739807"" y=""0"" z=""0"" />
  <node nid=""15"" x=""1.69448530673981"" y=""0"" z=""0"" />
  <node nid=""16"" x=""1.69448530673981"" y=""1"" z=""0"" />
  <node nid=""17"" x=""0.694485306739807"" y=""1"" z=""0"" />
  <node nid=""18"" x=""0.694485306739807"" y=""0"" z=""1"" />
  <node nid=""19"" x=""1.69448530673981"" y=""0"" z=""1"" />
  <node nid=""20"" x=""1.69448530673981"" y=""1"" z=""1"" />
  <node nid=""21"" x=""0.694485306739807"" y=""1"" z=""1"" />
  <node nid=""22"" x=""2.69448530673981"" y=""1"" z=""1"" />
  <node nid=""23"" x=""3.69448530673981"" y=""1"" z=""1"" />
  <node nid=""24"" x=""4.69448530673981"" y=""1"" z=""1"" />
  <node nid=""25"" x=""5.69448530673981"" y=""1"" z=""1"" />
  <node nid=""26"" x=""4.69448530673981"" y=""0"" z=""0"" />
  <node nid=""27"" x=""2.69448530673981"" y=""0"" z=""1"" />
  <node nid=""28"" x=""3.69448530673981"" y=""0"" z=""1"" />
  <node nid=""29"" x=""4.69448530673981"" y=""0"" z=""1"" />
  <node nid=""30"" x=""5.69448530673981"" y=""0"" z=""1"" />
  <node nid=""31"" x=""3.69448530673981"" y=""0"" z=""0"" />
  <node nid=""32"" x=""2.69448530673981"" y=""1"" z=""0"" />
  <node nid=""33"" x=""3.69448530673981"" y=""1"" z=""0"" />
  <node nid=""34"" x=""4.69448530673981"" y=""1"" z=""0"" />
  <node nid=""35"" x=""5.69448530673981"" y=""1"" z=""0"" />
  <node nid=""36"" x=""5.69448530673981"" y=""0"" z=""0"" />
  <node nid=""37"" x=""2.69448530673981"" y=""0"" z=""0"" />
  <node nid=""38"" x=""0.69448530673981"" y=""1"" z=""0.5"" />
  <elset name=""Default"" color=""-6710887"">
    <elem eid=""1"" shape=""hex8"" nodes=""14 15 16 17 18 19 20 21 38"" />
    <elem eid=""2"" shape=""hex8"" nodes=""15 16 20 19 37 32 22 27"" />
    <elem eid=""3"" shape=""hex8"" nodes=""37 32 22 27 31 33 23 28"" />
    <elem eid=""4"" shape=""hex8"" nodes=""31 33 23 28 26 34 24 29"" />
    <elem eid=""5"" shape=""hex8"" nodes=""26 34 24 29 36 35 25 30"" />
  </elset>
  <elementselection name=""Unnamed"">
    <element eid=""1"" />
  </elementselection>
  <solution>
    <analysis type=""S30"" />
    <elset name=""Default"" color=""-6710887"" />
    <table />
  </solution>
</liml8>";

            StringBuilder output = new StringBuilder();

            List<Node> nodes = readAllNodes(xmlString);

            List<Element> elements = readAllElements(xmlString, nodes);

            











                // https://msdn.microsoft.com/en-us/library/cc189056(v=vs.95).aspx
                //reader.ReadToFollowing("node");
                //reader.readTo
                //reader.MoveToFirstAttribute();
                //string genre = reader.Value;
                //output.AppendLine("The genre value: " + genre);

            //reader.ReadToFollowing("title");
            //output.AppendLine("Content of the title element: " + reader.ReadElementContentAsString());
        

            FEModel currentModel = new FEModel();
            return currentModel;
        }

        private List<Node> readAllNodes(string xmlString)
        {
            const string nodeTag = "node";
            List<Node> nodes = new List<Node>();
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement() && reader.Name == nodeTag)
                    {
                        // Get element name and switch on it.
                        Node node = getNodeData(reader);
                        nodes.Add(node);
                        break;
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
                    if (reader.IsStartElement() && reader.Name == elemTag)
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

            try { 
                int id = Convert.ToInt32(elementId);
                List<string> splitNodes = new List<string>(nodes.Split(' '));
                List<int> nodeIDs = splitNodes.Select(x => Convert.ToInt32(x)).ToList();

                //var output = nodes.Where(e => !nodeIDs.Any(d => e.EndsWith(d)));
                //var query = nodes.Where(item => item.Jobs.Any(j => longList.Contains(j.ResultElement.Id)));
                //List<Node> matches = nodes.Where(p => p.GetId == nameToExtract);

                Element newElement = new Element(id, shape, matches);
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

        /// <summary>
        /// Writes the model that we have updated with our fancy algorithm back to a file so we can re run the FE
        /// </summary>
        /// <param name="updatedModel">The Modle object which we have updated</param>
        private void writeModelBack(FEModel updatedModel)
        {



        }
        private void executeModel()
        {



        }
    }
}
