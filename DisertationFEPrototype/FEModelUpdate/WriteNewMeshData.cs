using DisertationFEPrototype.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using DisertationFEPrototype.Model.MeshDataStructure;
using System.IO;
using DisertationFEPrototype.Model.Analysis;

namespace DisertationFEPrototype.ModelUpdate
{
    /// <summary>
    /// write back a lisa XML file after updates to the node positions have been conducted using our algorithm
    /// </summary>
    class WriteNewMeshData
    {
        
        public WriteNewMeshData(MeshData meshData, string lisaPath, string outputFileName)
        {

            // Directory.SetCurrentDirectory(lisaFolderPath);
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
                using (fw = new StreamWriter(lisaPath, true)) {
                    fw.WriteLine("<liml8>");
                    fw.WriteLine("  <analysis type=\"S30\" />");

                    writeNodes(fw, meshData.Nodes.Values.ToList());
                    writeElements(fw, meshData.Elements, meshData.TheMaterial.GetName);
                    fw.WriteLine("  <fix selection=\"Unnamed\" />");
                    writeForce(fw, meshData.TheForce);
                    writeMaterials(fw, meshData.TheMaterial);
                    writeFaceSections(fw, meshData.TheFaceSelections);

                    fw.WriteLine("  <solution>");
                    fw.WriteLine("    <analysis type=\"S30\"/>");
                    fw.WriteLine("    <elset name=\"Default\" color=\"-6710887\"/>");
                    fw.WriteLine("    <table>");
                    writeTableSetup(fw, outputFileName);
                    fw.WriteLine("    </table>");
                    fw.WriteLine("  </solution>");
                    fw.WriteLine("</liml8 >");
                }
            }
            catch (IOException)
            {
                throw;
            }  
        }
        /// <summary>
        /// It's important that we write the table fields that we expect to the
        /// lisa file so that when we perform a solve operation an output containing
        /// displacement/stress values is created by lisa
        /// </summary>
        /// <param name="fw"></param>
        /// <param name="outputFilePath"></param>
        private void writeTableSetup(StreamWriter fw, string outputFileName)
        {
            fw.WriteLine("      <component>Default</component>");

            fw.WriteLine("      <namedselection>Unnamed</namedselection>");
            fw.WriteLine("      <namedselection>Unnamed(2)</namedselection>");

            fw.WriteLine("      <fieldvalue>displx</fieldvalue>");
            fw.WriteLine("      <fieldvalue>disply</fieldvalue>");
            fw.WriteLine("      <fieldvalue>displz</fieldvalue>");

            // at the moment I am just interested in displacement
            //fw.WriteLine("      <fieldvalue>rotx</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>roty</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>rotz</fieldvalue>");

            //fw.WriteLine("      <fieldvalue>momentu</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>momentv</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>momentuv</fieldvalue>");

            //fw.WriteLine("      <fieldvalue>shearuw</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>shearvw</fieldvalue>");

            //fw.WriteLine("      <fieldvalue>vonmisesb</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>vonmisesu</fieldvalue>");

            //fw.WriteLine("      <fieldvalue>principal1u</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>principal2u</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>principal1b</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>principal2b</fieldvalue>");

            //fw.WriteLine("      <fieldvalue>stressuu</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>stressvv</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>stressuv</fieldvalue>");

            //fw.WriteLine("      <fieldvalue>vonmisesm</fieldvalue>");

            //fw.WriteLine("      <fieldvalue>principal1m</fieldvalue>");
            //fw.WriteLine("      <fieldvalue>principal2m</fieldvalue>");
            fw.WriteLine("      <fieldvalue>displmag</fieldvalue>");

            // elem values results only in data from the element analysis being outputted, 
            //we are interested in displacement which is associeated with Nodes

            //fw.WriteLine("      <elementvalues />");
            fw.WriteLine("      <coordinates />");

            fw.WriteLine("      <saveonsolve filename=\"" + outputFileName + "\" />");
        }

        private void writeForce(StreamWriter fw, Force theForce)
        {
            fw.WriteLine("  <force selection=\"" + theForce.Selection + "\">");
            fw.WriteLine("    <x>" + theForce.X + "</x>");
            fw.WriteLine("    <y>" + theForce.Y + "</y>");
            fw.WriteLine("    <z>" + theForce.Z + "</z>");
            fw.WriteLine("  </force>");
        }

        private void writeNodes(StreamWriter fw, List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                fw.WriteLine("  <node nid=\"" + node.Id.ToString() + "\" x=\""
                + node.GetX.ToString() + "\" y=\""
                + node.GetY.ToString() + "\" z=\""
                + node.GetZ.ToString() + "\" />");
            }
        }
        private void writeElements(StreamWriter fw, List<Element> elements, string materialName)
        {
            fw.WriteLine("  <elset name=\"Default\" color=\"-6710887\" material=\"" + materialName + "\" >");
            foreach (var elem in elements)
            {
                string nodesString = getNodesString(elem.GetNodes);
                if (elem.Id != null)
                {
                    fw.WriteLine("    <elem eid=\"" + elem.Id.ToString() + "\" shape=\""
                        + elem.GetShape + "\" nodes=\"" + nodesString + "\" />");
                }
            }
            fw.WriteLine("  </elset >");
        }
        private void writeMaterials(StreamWriter fw, Material theMaterial)
        {
            //< mat mid = "1" name = "Material" >
            //< geometric type = "Plate" thickness = "3" planestrain = "0" />
            //< mechanical type = "Isotropic" youngsmodulus = "200000000000" poissonratio = "0.3" />
            //</ mat >
            fw.WriteLine("  <mat mid = \"" + theMaterial.GetId.ToString() + "\" name=\"Material\" >");

            fw.WriteLine("    <geometric type=\"" + theMaterial.Geometric.GetGeometricType + "\" thickness=\"" + 
                theMaterial.Geometric.GetThickness + "\" planestrain = \"" + 
                theMaterial.Geometric.GetPlaneStrain + "\" />");

            fw.WriteLine("    <mechanical type=\"" + theMaterial.Mechanical.GetMechanicalType + "\" youngsmodulus=\""
                + theMaterial.Mechanical.GetYoungsModulus + "\" poissonratio=\""
                + theMaterial.Mechanical.GetPoissonRatio + "\" />");

            fw.WriteLine("  </mat>");

        }
        private void writeFaceSections(StreamWriter fw, List<FaceSelection> theFaceSelections)
        {
            foreach (FaceSelection faceSelection in theFaceSelections)
            {
                fw.WriteLine("  <faceselection name=\"" + faceSelection.GetName +  "\">");
                foreach(Face face in faceSelection.Faces)
                {
                    fw.WriteLine("    <face eid=\"" + face.Element.Id + "\" faceid=\"" + face.GetId + "\" />");
                }
                fw.WriteLine("  </faceselection>");
            }
        }

        private string getNodesString(List<Node> nodes)
        {
            var nodeIds = nodes.Select(x => x.Id);


            if (nodeIds.Contains(13) && nodeIds.Contains(12) && nodeIds.Contains(14) && nodeIds.Contains(15))
            {
                Console.WriteLine("Node order: " + nodes[0].Id + " " + nodes[1].Id + " " + nodes[2].Id + " " + nodes[3].Id);
            }
            string nodeString = "";
            foreach(var node in nodes)
            {
                nodeString += node.Id.ToString() + " ";
            }
            return nodeString.TrimEnd(' ');
        }
    }
}
