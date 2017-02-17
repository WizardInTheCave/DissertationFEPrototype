using System.Collections.Generic;


using DisertationFEPrototype.ModelUpdate;
using DisertationFEPrototype.Optimisations;
using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype.Model;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DisertationFEPrototype.MeshQualityMetrics;
using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using System;

namespace DisertationFEPrototype
{
    class Control
    {
        /// <summary>
        /// main loop which drives iteration until we have a FE model which meets the requirements, simple!
        /// </summary>
        /// <param name="lisaString"></param>
        public Control() {

            //string lisaFile = @"D:\Documents\DissertationWork\models\newBlockTestSquareNodes.liml";  
            string lisaFile = "SingleCube.liml";
            //string solveFile = @"D:\Documents\DissertationWork\secondTest.csv";
            bool isNodeOutput = true;

            string lisaFileName = Path.GetFileNameWithoutExtension(lisaFile);
            string outputCSVname = lisaFileName + "Out.csv";

            string lisaFolderPath = @"D:\Documents\DissertationWork\models\";

            // only need to do this once to get the inital mesh, after that should try and drive it
            // purely with the solve data
            Directory.SetCurrentDirectory(lisaFolderPath);

            var meshDataReader = new ReadMeshData(lisaFile);
            MeshData meshData = meshDataReader.GetMeshData;

            // read data from the solve
            var analysisDataReader = new ReadAnalysisData(outputCSVname, isNodeOutput);


            int ii = 0;
            List<NodeAnalysisData> analysisData;

            // MeshQualityAssessment meshQualityAssessment = null;
            List<MeshQualityAssessment> meshAssessments = new List<MeshQualityAssessment>();

            while (evaluationFunction(ii) == false)
            {    
                solve(lisaFile);

                // read data from the solve
                analysisData = analysisDataReader.getAnalysisData();


               
                // assuming we have different mesh data should get a new set of edges.
                OptimisationManager optimisation = new OptimisationManager(meshData, analysisData, ii);
                
                // hand quality assessment down to the refinement method so we can apply apply either rule based
                // or traditional meshing further
                optimisation.refineMesh(meshAssessments);
                MeshData refinedMesh = optimisation.GetUpdatedMesh;

               // meshQualityAssessment = new MeshQualityAssessment(refinedMesh);
               // meshQualityAssessment.assessMesh();
               // meshAssessments.Add(meshQualityAssessment);


                // update the lisa file we are now working on (we next want to solve the updated file)
                lisaFile = lisaFileName + ii.ToString() + ".liml";
                outputCSVname = lisaFileName + "Out" + ii.ToString() + ".csv";
                WriteNewMeshData meshWriter = new WriteNewMeshData(refinedMesh, lisaFile, outputCSVname);
                analysisDataReader.SolveFile = outputCSVname;


                // update the model for the next iteration
                meshData = refinedMesh;
                ii++;
            }

        }
        /// <summary>
        /// Tells lisa to run a solve on the lisa file which will produce some output
        /// </summary>
        /// <param name="lisaFile"></param>
        public void solve(string lisaFile)
        {
            using (Process lisaProcess = Process.Start("lisa8", lisaFile + " solve"))
            {
                lisaProcess.StartInfo.RedirectStandardOutput = true;
                lisaProcess.WaitForExit();
            }
        }

        /// <summary>
        /// Some function which determines whether it is cool for us to stop meshing yet.
        /// </summary>
        /// <param name="ii"></param>
        /// <returns></returns>
        private bool evaluationFunction(int ii)
        {
            return ii > 1;
        }
    }
}
