using System.Collections.Generic;


using DisertationFEPrototype.ModelUpdate;
using DisertationFEPrototype.Optimisations;
using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype.Model;
using System.Diagnostics;
using System.IO;

namespace DisertationFEPrototype
{
    class Control
    {
        /// <summary>
        /// main loop which drives iteration until we have a FE model which meets the requirements, easy!
        /// </summary>
        /// <param name="lisaString"></param>
        public Control() {

            int ii = 0;

            string lisaFile = @"D:\Documents\DissertationWork\simpleElement.liml";  
            string solveFile = @"D:\Documents\DissertationWork\secondTest.csv";

            string lisaFileName = Path.GetFileNameWithoutExtension(lisaFile);
            string lisaFolderPath = Path.GetDirectoryName(lisaFile);

            List<MeshData> meshLog = new List<MeshData>();

            // only need to do this once to get the inital mesh, after that should try and drive it
            // purely with the solve data

            var meshDataReader = new ReadMeshData(lisaFile);
            MeshData meshData = meshDataReader.GetMeshData;

            // read data from the solve
            var analysisDataReader = new ReadAnalysisData(solveFile);
         

            while (evaluationFunction(ii) == false)
            {
                List<AnalysisData> analysisData = analysisDataReader.getAnalysisData();

                // solve(lisaFile);
                // read data from the solve
                analysisData = analysisDataReader.getAnalysisData();

                generalOptimisations optimisation = new generalOptimisations(meshData, analysisData);
                optimisation.doubleNodeCount();

                MeshData refinedMesh = optimisation.GetUpdatedMesh;
                meshLog.Add(refinedMesh);

                string outputFilePath = lisaFolderPath + "\\" + lisaFileName + ii.ToString() + ".liml";
                WriteNewMeshData meshWriter = new WriteNewMeshData(refinedMesh, outputFilePath);

                ii++;
            }

        }
        /// <summary>
        /// tells lisa to run a solve on the lisa file which will produce some output
        /// </summary>
        /// <param name="lisaFile"></param>
        public void solve(string lisaFile)
        {
            string strCmdText = "lisa8" + lisaFile + "solve";
            Process.Start("CMD.exe", strCmdText);
        }

        private bool evaluationFunction(int ii)
        {
            return ii > 3;
        }
    }
}
