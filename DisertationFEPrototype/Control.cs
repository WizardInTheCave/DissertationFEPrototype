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

using System.Linq;
using DisertationFEPrototype.Optimisations.ILPRules;

namespace DisertationFEPrototype
{
    class Control
    {


        // make a lock so file IO isn't a problem when running experiments on lots of threads
        //private Object fileIOLock = new Object();

        /// <summary>
        /// main loop which drives iteration until we have a FE model which meets the requirements, simple!
        /// </summary>
        /// <param name="lisaString"></param>
        public Control(string experimentFolder, Tuple<short, short> experimentVals) {

            string experimentFolderLocal = experimentFolder;
           
            bool isNodeOutput = true;

            string lisaFile = "bridgeAdvanced.liml";

            string lisaFileName = "bridgeAdvanced";

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

                short ILPRefinem = experimentVals.Item1;
                short stressRefineCount = experimentVals.Item2;
                // assuming we have different mesh data should get a new set of edges.
                OptimisationManager optimisation = new OptimisationManager(meshData, analysisData, ii, ILPRefinem, stressRefineCount, ruleMan);
                
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
            }

            writeAssessmentSummary(experimentFolder, meshAssessments);
        }

        /// <summary>
        /// Write out a summary of the Assessments for this experiment so we can compare variations in how effective the two methods are
        /// </summary>
        private void writeAssessmentSummary(string experimentFoler, List<MeshQualityAssessment> meshAssessments)
        {

           
                string analysisFile = Path.Combine(experimentFoler, "analysisData.txt");

                StreamWriter file = new StreamWriter(analysisFile);

                int ii = 0;
                foreach (MeshQualityAssessment assessment in meshAssessments)
                {
                file.WriteLine("////////////////////////////Results for iteration " + ii.ToString() + @"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                    file.WriteLine("\nGlobal Metrics");
                    file.WriteLine("ElemCount score: " + assessment.ElemCountScore.ToString());
                    file.WriteLine("Element Quality score: " + assessment.ElemQualityScore.ToString());
                    file.WriteLine("Overall Quality Improvement score: " + assessment.OvarallQualityImprovement.ToString());
                    file.WriteLine("Heuristic Quality Score: " + assessment.HeuristuicQualityScore.ToString());
                    file.WriteLine("");
                    file.WriteLine("ElementMetrics");
                    file.WriteLine("Average Max Angle: " + assessment.ElemQualMetrics.MaxCornerAngles.Average());
                    file.WriteLine("Average Max parallel dev: " + assessment.ElemQualMetrics.MaxParrallelDevs.Average());
                    file.WriteLine("Average AspectRatio: " + assessment.ElemQualMetrics.AspectRatios.Average());
                    file.WriteLine("");
                file.WriteLine("////////////////////////////---------------------\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                ii++;
                }
                file.Close();
        }

        /// <summary>
        /// Tells lisa to run a solve on the lisa file which will produce some output
        /// </summary>
        /// <param name="lisaFile"></param>
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
        /// Some function which determines whether it is cool for us to stop meshing yet.
        /// </summary>
        /// <param name="ii"></param>
        /// <returns></returns>
        private bool evaluationFunction(int ii)
        {
            return ii > 3;
        }
    }
}
