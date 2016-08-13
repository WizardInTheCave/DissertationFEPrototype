using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DisertationFEPrototype.Model.MeshDataStructure;

namespace DisertationFEPrototype.Model
{
    class FEModel
    {
        MeshData meshData; 
        string lisaFile;
        private string lisaString;

        public FEModel(string lisaFile, MeshData meshData, AnalysisData analyData)
        {
            this.lisaFile = lisaFile;
            this.meshData = meshData;
        }

        public FEModel(string lisaString, MeshData meshData)
        {
            this.lisaString = lisaString;
            this.meshData = meshData;
        }

        /// <summary>
        /// tells lisa to run a solve on the lisa file which will produce some output
        /// </summary>
        /// <param name="lisaFile"></param>
        public void solve()
        {
            string strCmdText = "lisa8" + this.lisaFile + "solve";
            Process.Start("CMD.exe", strCmdText);
        }
        /// <summary>
        /// For every pair of nodes add an extra node between the two of them
        /// </summary>
        /// <returns></returns>
        public void createMidpointNodes(List<Element> elements)
        {
            foreach (Element element in elements)
            {
                List<Node> elementNodes = element.GetNodes;

            }
        }
    }
}
