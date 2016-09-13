using System.Collections.Generic;


using DisertationFEPrototype.ModelUpdate;
using DisertationFEPrototype.Optimisations;
using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype.Model;
using System.Diagnostics;
using System.IO;
using System.Threading;

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

            //string lisaFile = @"D:\Documents\DissertationWork\models\newBlockTestSquareNodes.liml";  
            string lisaFile = "newBlockTestSquareNodes.liml";
            string solveFile = @"D:\Documents\DissertationWork\secondTest.csv";

            string lisaFileName = Path.GetFileNameWithoutExtension(lisaFile);
            string lisaFolderPath = @"D:\Documents\DissertationWork\models\";

            List<MeshData> meshLog = new List<MeshData>();

            // only need to do this once to get the inital mesh, after that should try and drive it
            // purely with the solve data

            Directory.SetCurrentDirectory(lisaFolderPath);

            var meshDataReader = new ReadMeshData(lisaFile);
            MeshData meshData = meshDataReader.GetMeshData;

            // read data from the solve
            var analysisDataReader = new ReadAnalysisData(solveFile);
         

            while (evaluationFunction(ii) == false)
            {
                List<AnalysisData> analysisData = analysisDataReader.getAnalysisData();

                //if (ii == 0)
                //{
                solve(lisaFile, lisaFolderPath);
                //}

                // read data from the solve
                analysisData = analysisDataReader.getAnalysisData();

                generalOptimisations optimisation = new generalOptimisations(meshData, analysisData);
                optimisation.doubleNodeCount();

                MeshData refinedMesh = optimisation.GetUpdatedMesh;
                meshLog.Add(refinedMesh);

                // update the lisa file we are now working on (we next want to solve the updated file)
                lisaFile = lisaFolderPath + lisaFileName + ii.ToString() + ".liml";
                WriteNewMeshData meshWriter = new WriteNewMeshData(refinedMesh, lisaFile, lisaFolderPath);
                Thread.Sleep(1000);

                // update the model for the next iteration
                meshData = refinedMesh;
                ii++;
            }

        }
        /// <summary>
        /// tells lisa to run a solve on the lisa file which will produce some output
        /// </summary>
        /// <param name="lisaFile"></param>
        public void solve(string lisaFile, string lisaFolderPath)
        {

            //ProcessStartInfo startInfo = new ProcessStartInfo("lisa8");
            //startInfo.WindowStyle = ProcessWindowStyle.Minimized;

            //Process.Start(startInfo);

            //startInfo.Arguments = ;

            //Process.Start(startInfo);

            using (Process exeProcess = Process.Start("lisa8", lisaFile + " solve"))
            {
                
                exeProcess.WaitForExit();
            }
        }

        private bool evaluationFunction(int ii)
        {
            return ii > 3;
        }
    }
}
