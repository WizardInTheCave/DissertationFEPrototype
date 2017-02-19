using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Xml;
using System.IO;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.FEModelUpdate.Read;
using DisertationFEPrototype.Model.Analysis;
using DisertationFEPrototype.Model.Analysis.MaterialProps;
using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;

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

                this.meshData = new MeshData();
                this.meshData.Nodes = ReadNodes.readAllNodes(xmlString);
                this.meshData.Elements= ReadElements.readAllElements(xmlString, meshData);
                this.meshData.Force = readForce(xmlString);
                this.meshData.Material = readMaterial(xmlString);
                this.meshData.FaceSelections = readFaceSelections(xmlString);
                this.meshData.FixSelections = readFixSelections(xmlString);

            }
            else
            {
                throw new FileNotFoundException("Could not load the lisa mesh file to rebuild model");
            }  
        }
        private List<FixSelection> readFixSelections(string xmlString)
        {
            const string fixSelectTag = "fix";
            List<FixSelection> fixSelections = new List<FixSelection>();

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {

                while (reader.ReadToFollowing(fixSelectTag))
                {

                    const string selection = "selection";
                    string selectionName = reader[selection];
                    var allSelections = meshData.FaceSelections.Where(faceSelection => faceSelection.GetName == selectionName);

                    if (allSelections.Count() == 1)
                    {
                        fixSelections.Add(new FixSelection(allSelections.ToArray()[0]));
                    }
                    else
                    {
                        throw new Exception("We shouldn't have more than one face selection object per fixed selection");
                    }
                }
            }
            return fixSelections;
        }

        private List<FaceSelection> readFaceSelections(string xmlString)
        {
            const string faceSelectTag = "faceselection";

            List<FaceSelection> faceSelections = new List<FaceSelection>();

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {

                while (reader.ReadToFollowing(faceSelectTag))
                {


                    const string name = "name";

                    //read in properties from the xml file
                    string selectionName = reader[name];

                    try
                    {
                        List<Face> faceSelectionFaces = new List<Face>();
                        var innerSubtree = reader.ReadSubtree();
                        while (innerSubtree.Read())
                        {
                            var face = innerSubtree["face"];
                            //Console.WriteLine(innerSubtree.NodeType + ": " + innerSubtree.Name);

                            if (innerSubtree.NodeType == XmlNodeType.Element && innerSubtree.Name == "face")
                            {
                                int elemId = Convert.ToInt16(innerSubtree["eid"]);
                                int faceId = Convert.ToInt16(innerSubtree["faceid"]);


                                List<IElement> elements = meshData.Elements.Where(e => e.Id == elemId).ToList();
                                if (elements.Count > 1)
                                {
                                    throw new IOException("ReadLisaData.readFaceSelections: Face seems to have more than one element when reading select elements");
                                }
                                IElement elem = elements[0];

                                faceSelectionFaces.Add(new Face(elem, faceId));
                            }

                        }
                        faceSelections.Add(new FaceSelection(selectionName, faceSelectionFaces));
                    }
                    catch
                    {
                        throw;
                        //throw new Exception("Could not read force data from xml correctly");
                    }
                }
            }
            return faceSelections;
        }
        private Force readForce(string xmlString)
        {
            const string elemTag = "force";

            Force force = null;
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                reader.ReadToFollowing(elemTag);
                   
                const string selection = "selection";

                //read in properties from the xml file
                string theSelection = reader[selection];

                try
                {
                    var innerSubtree = reader.ReadSubtree();

                    innerSubtree.ReadToDescendant("x");
                    double x = innerSubtree.ReadElementContentAsDouble();

                    innerSubtree.ReadToFollowing("y");
                    double y = innerSubtree.ReadElementContentAsDouble();

                    innerSubtree.ReadToFollowing("z");
                    double z = innerSubtree.ReadElementContentAsDouble();

                    force = new Force(theSelection, x, y, z);
                }
                catch
                {
                    Console.WriteLine("LOL WHAT??");
                }
            }
            return force;
        } 
       
        /// <summary>
        /// read material data in from the file and build a material object
        /// </summary>
        /// <param name="xmlString">the xml file as a string</param>
        /// <returns>Material object containing all the material data</returns>
        private Material readMaterial(string xmlString)
        {

            int id = 0;
            string name = "";

            const string elemTag = "mat";

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {

                reader.ReadToFollowing(elemTag);

                id = Convert.ToInt32(reader["mid"]);
                name = reader["name"];

                try
                {
                    var innerSubtree = reader.ReadSubtree();

                    innerSubtree.ReadToFollowing("geometric");
                    string geomType = innerSubtree["type"];
                    double thickness = Convert.ToDouble(innerSubtree["thickness"]);
                    double planestrain = Convert.ToDouble(innerSubtree["planestrain"]);


                    innerSubtree.ReadToFollowing("mechanical");
                    string mechType = innerSubtree["type"];
                    long youngsModulus = Convert.ToInt64(innerSubtree["youngsmodulus"]);
                    double poissonRatio = Convert.ToDouble(innerSubtree["poissonratio"]);


                    var geometric = new Geometric(geomType, thickness, planestrain);
                    var mechanical = new Mechanical(mechType, youngsModulus, poissonRatio);
                    return new Material(id, name, geometric, mechanical);
                    
                }
                catch
                {
                    throw new Exception("Could not read force data from xml correctly");
                }
            }  
        }
    }
}
