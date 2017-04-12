using System.Collections.Generic;


using DissertationFEPrototype.ModelUpdate;
using DissertationFEPrototype.Optimisations;
using DissertationFEPrototype.FEModelUpdate;
using DissertationFEPrototype.Model;
using System.Diagnostics;
using System.IO;
using DissertationFEPrototype.MeshQualityMetrics;
using System;

using System.Linq;
using DissertationFEPrototype.Optimisations.ILPRules;
using DissertationFEPrototype.FEModelUpdate.Model;

using DisertationFEPrototype.FEModelUpdate;

namespace DissertationFEPrototype
{
    class Control
    {

        /// <summary>
        /// Main loop which drives iteration until we have a FE model which meets the requirements, simple!
        /// </summary>
        /// <param name="lisaString"></param>
        public Control(IntWrapper threadEditCount, string experimentFolder, Tuple<short, short> experimentVals, List<List<string>> resultCols) {

            List<double> times = new List<double>();
            List<MeshData> meshes = new List<MeshData>();


            Stopwatch stopwatch = Stopwatch.StartNew();

            string experimentFolderLocal = experimentFolder;
           
            bool isNodeOutput = true;

            //string lisaFile = "bridgeAdvanced.liml";
            //string lisaFileName = "bridgeAdvanced";

            //string lisaFile = "paperMill.liml";
            //string lisaFileName = "paperMill";

            
            string lisaFileName = "cylinderCrossSection";
            string lisaFile = lisaFileName + ".liml";

            string outputCSVPath = Path.Combine(experimentFolderLocal, lisaFileName + "Out.csv");
            string lisaFilePath = Path.Combine(experimentFolderLocal, lisaFile);
            

            // only need to do this once to get the inital mesh, after that should try and drive it
            // purely with the solve data
            
            var meshDataReader = new ReadMeshData(lisaFilePath);

            MeshData meshData = meshDataReader.GetMeshData;

            // read data from the solve
            var analysisDataReader = new ReadAnalysisData(outputCSVPath, isNodeOutput);


            int ii = 0;
            List<NodeAnalysisData> analysisData;

            MeshQualityAssessment meshQualityAssessment = null;
            List<MeshQualityAssessment> meshAssessments = new List<MeshQualityAssessment>();

            string localEdgesFile = Path.Combine(experimentFolderLocal, "modelEdges.json");
            RuleManager ruleMan = new RuleManager(meshData, localEdgesFile);

            while (evaluationFunction(ii) == false)
            {
               
                solve(lisaFile, experimentFolderLocal);

                // read data from the solve
                analysisData = analysisDataReader.getAnalysisData();

                short ILPRefineCount = experimentVals.Item1;
                short stressRefineCount = experimentVals.Item2;
                // assuming we have different mesh data should get a new set of edges.
                RefinementManager optimisation = new RefinementManager(meshData, analysisData, ii, ILPRefineCount, stressRefineCount, ruleMan);
                
                // hand quality assessment down to the refinement method so we can apply apply either rule based
                // or traditional meshing further
                optimisation.refineMesh(meshAssessments);
                MeshData refinedMesh = optimisation.GetUpdatedMesh;

                meshQualityAssessment = new MeshQualityAssessment(refinedMesh, analysisData);
                meshQualityAssessment.assessMesh();
                meshAssessments.Add(meshQualityAssessment);

                // update the lisa file we are now working on (we next want to solve the updated file)
                lisaFile = lisaFileName + ii.ToString() + ".liml";
                lisaFilePath = Path.Combine(experimentFolderLocal, lisaFile);
                // update the location to the next analysis folder which will be read for the following iteration.

                
                outputCSVPath = Path.Combine(experimentFolderLocal, lisaFileName + "Out" + ii.ToString() + ".csv");
                WriteNewMeshData meshWriter = new WriteNewMeshData(refinedMesh, lisaFilePath, outputCSVPath);
                analysisDataReader.SolveFile = outputCSVPath;
               

                // update the model for the next iteration
                meshData = refinedMesh;
                ii++;

                times.Add(stopwatch.Elapsed.TotalSeconds);
                meshes.Add(meshData);
            }

            stopwatch.Stop();

            var fw = new TableWriter();
            fw.WriteData(threadEditCount, resultCols, experimentVals, experimentFolder, meshAssessments, meshes, times.ToList());
        }

     
        /// <summary>
        /// Tells lisa to run a solve on the lisa file which will produce some output
        /// </summary>
        /// <param name="lisaFile">name of the LISA file that the updated model has just been written to and needs solving</param>
        private void solve(string lisaFile, string experimentFolderLocal)
        {
            string executeString = Path.GetFileName(experimentFolderLocal) + "\\" +  lisaFile + " solve";
            using (Process lisaProcess = Process.Start("lisa8", executeString))
            {
                lisaProcess.StartInfo.RedirectStandardOutput = true;
                lisaProcess.WaitForExit();
            }
            
        }

        /// <summary>
        /// Some function which determines whether it is good to stop meshing yet.
        /// Since this will depend on the specifics of what the software is used for
        /// currently just hardcoding values here for testing purposes.
        /// </summary>
        /// <param name="ii"></param>
        /// <returns></returns>
        private bool evaluationFunction(int ii)
        {
            return ii > 8;
        }
    }
}
