using DisertationFEPrototype.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;
using DisertationFEPrototype.Model.Analysis;
using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using DisertationFEPrototype.FEModelUpdate.Model.Structure;
using DisertationFEPrototype.FEModelUpdate.Model;

namespace DisertationFEPrototype.ModelUpdate
{
    /// <summary>
    /// write back a lisa XML file after updates to the node positions have been conducted using our algorithm
    /// </summary>
    class WriteNewMeshData
    {
        public WriteNewMeshData(MeshData meshData, string newLisaModelPath, string analysisOutputPath)
        {
            try
            {
                StreamWriter fw;
                using (fw = new StreamWriter(newLisaModelPath, true)) {
                    fw.WriteLine("<liml8>");
                    fw.WriteLine("  <analysis type=\"S30\" />");

                    writeNodes(fw, meshData.Nodes.Values.ToList());
                    writeElements(fw, meshData.Elements, meshData.Material.GetName);
                    meshData.FixSelections.ForEach(fix => fw.WriteLine("  <fix selection=\"" + fix.Selection.GetName + "\" />"));
                    writeForces(fw, meshData.Forces);
                    writeMaterials(fw, meshData.Material);
                    writeFaceSections(fw, meshData.FaceSelections);

                    fw.WriteLine("  <solution>");
                    fw.WriteLine("    <analysis type=\"S30\"/>");
                    fw.WriteLine("    <elset name=\"Default\" color=\"-6710887\"/>");
                    fw.WriteLine("    <table>");
                    writeTableSetup(fw, analysisOutputPath);
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

            fw.WriteLine("      <fieldvalue>displmag</fieldvalue>");

            // elem values results only in data from the element analysis being outputted, 
            //we are interested in displacement which is associeated with Nodes
            //fw.WriteLine("      <elementvalues />");
            fw.WriteLine("      <coordinates />");

            fw.WriteLine("      <saveonsolve filename=\"" + outputFileName + "\" />");
        }

        private void writeForces(StreamWriter fw, List<Force> forces)
        {
            foreach(Force f in forces)
            {
                fw.WriteLine("  <force selection=\"" + f.Selection + "\">");
                fw.WriteLine("    <x>" + f.X + "</x>");
                fw.WriteLine("    <y>" + f.Y + "</y>");
                fw.WriteLine("    <z>" + f.Z + "</z>");
                fw.WriteLine("  </force>");
            }
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
        private void writeElements(StreamWriter fw, List<IElement> elements, string materialName)
        {
            fw.WriteLine("  <elset name=\"Default\" color=\"-6710887\" material=\"" + materialName + "\" >");
            foreach (var elem in elements)
            {
                string nodesString = getNodesString(elem.getNodes());
                if (elem.getId() != null)
                {
                    fw.WriteLine("    <elem eid=\"" + elem.getId().ToString() + "\" shape=\""
                        + getElemString(elem) + "\" nodes=\"" + nodesString + "\" />");
                }
            }
            fw.WriteLine("  </elset >");
        }

        private string getElemString(IElement elem)
        {
            //Type elemType = elem.GetType(); 

            const string QUAD4_SHAPE = "quad4";
            const string HEX8_SHAPE = "hex8";

            string shape;

            if (elem is Quad4Elem)
            {
                shape = QUAD4_SHAPE;
            }
            else if(elem is Hex8Elem)
            {
                shape = HEX8_SHAPE;
            }
            else
            {
                throw new Exception("IElement is not of a class that can be handed back to LISA currently");
            }
            return shape;

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
                    fw.WriteLine("    <face eid=\"" + face.Element.getId() + "\" faceid=\"" + face.GetId + "\" />");
                }
                fw.WriteLine("  </faceselection>");
            }
        }

        private string getNodesString(List<Node> nodes)
        {
            var nodeIds = nodes.Select(x => x.Id);

            if (nodes.Count < 4)
            {
                Console.WriteLine("What???");
            }

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
