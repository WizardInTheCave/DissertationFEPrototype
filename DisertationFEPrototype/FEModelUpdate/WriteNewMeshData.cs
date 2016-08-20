using DisertationFEPrototype.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using DisertationFEPrototype.Model.MeshDataStructure;
using System.IO;

namespace DisertationFEPrototype.ModelUpdate
{
    /// <summary>
    /// write back a lisa XML file after updates to the node positions have been conducted using our algorithm
    /// </summary>
    class WriteNewMeshData
    {
        
        public WriteNewMeshData(MeshData meshData, string outputFilePath)
        {
          

            // this is how we want to do it in an ideal world however because our object model does not map exactly to
            // that of the lisa file it may be easier if we just write out strings
            //try {
            //    System.Xml.Serialization.XmlSerializer writer =
            //        new System.Xml.Serialization.XmlSerializer(typeof(MeshData));
            //    System.IO.FileStream file = System.IO.File.Create(outputFilePath);
            //    writer.Serialize(file, meshData);
            //    file.Close();
            //}
            //catch(Exception ex)
            //{
            //    Debug.WriteLine(ex.InnerException.ToString()); 
            //}

            try
            {
                StreamWriter fw;
                using (fw = new StreamWriter(outputFilePath, true)) {
                    fw.WriteLine("<liml8>");
                    fw.WriteLine("  <analysis type=\"S30\" />");
                    foreach (var node in meshData.Nodes.Values)
                    {
                        fw.WriteLine("  <node nid=\"" + node.Id.ToString() + "\" x=\"" 
                            + node.GetX.ToString() + "\" y=\"" 
                            + node.GetY.ToString() + "\" z=\"" 
                            + node.GetZ.ToString() + "\" />");

                    }
                    fw.WriteLine("  <elset name=\"Default\" color=\"-6710887\">");
                    foreach (var elem in meshData.Elements)
                    {
                        string nodesString = getNodesString(elem.GetNodes);
                        if(elem.Id != null)
                        {
                            fw.WriteLine("    <elem eid=\"" + elem.Id.ToString() + "\" shape=\""
                                + elem.GetShape + "\" nodes=\"" + nodesString + "\" />");
                        }
                    }
                    fw.WriteLine("  </elset >");


                    fw.WriteLine("  <solution>");
                    fw.WriteLine("    <analysis type=\"S30\"/>");
                    fw.WriteLine("    <elset name=\"Default\" color=\"-6710887\"/>");
                    fw.WriteLine("    <table/>");
                    fw.WriteLine("  </solution>");
                    fw.WriteLine("</liml8 >");
                }
            }
            catch (IOException)
            {
                throw;
            }  
        }
        private string getNodesString(List<Node> nodes)
        {
            string nodeString = "";
            foreach(var node in nodes)
            {
                nodeString += node.Id.ToString() + " ";
            }
            return nodeString.TrimEnd(' ');
        }
    }
}
