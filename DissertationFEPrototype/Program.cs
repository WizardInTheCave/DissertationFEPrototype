using System;
using System.Collections.Generic;
using System.IO;

using System.Threading;

namespace DissertationFEPrototype
{
    static class Program
    {

        // static string folderName = "PaperMillQuads";
        // static string folderName = "BridgeAdvanced";

        static string folderName = "Cylinder";

        /// <summary>
        /// The main entry point for the application.
        /// Read in possible command line arguments specifying key inoput folders and files so the system can execute
        /// </summary>
        // [STAThread]
        static void Main(string[] args)
        {
            //string topLevelFolder = @"D:\Documents\DissertationWork\models\FinalDissoExperiments\Experiments\BridgeAdvanced";
            //string modelFile = "bridgeAdvanced.liml";
            //string edgeDefinitionFile = "modelEdges.json";
            //string modelAnalysisFileName = "bridgeAdvancedOut.csv";

            //string topLevelFolder = @"D:\Documents\DissertationWork\models\FinalDissoExperiments\Experiments\" + folderName;
            //string modelFile = "paperMill.liml";
            //string edgeDefinitionFile = "modelEdges.json";
            //string modelAnalysisFileName = "paperMillOut.csv";

            string topLevelFolder = @"D:\Documents\DissertationWork\models\FinalDissoExperiments\Experiments\Cylinder";
            string modelFile = "CylinderCrossSection.liml";
            string edgeDefinitionFile = "modelEdges.json";
            string modelAnalysisFileName = "CylinderCrossSectionOut.csv";


            int k = 3;

            if (args.Length > 0)
            {
                topLevelFolder = args[0];
            }
            else if (args.Length > 1)
            {
                modelFile = args[1];
            }
            else if (args.Length > 2)
            {
                edgeDefinitionFile = args[2];
            }
            else if (args.Length > 3)
            {
                edgeDefinitionFile = args[3];
            }
            else if (args.Length > 4)
            {
                int.TryParse(args[3], out k);
            }

            runAllExperiments(topLevelFolder, modelFile, edgeDefinitionFile, modelAnalysisFileName, k);
        }

        /// <summary>
        /// This method runs the system a whole bunch of times with variations on how much each method is used when meshing,
        /// this is so data can be gathered about the effectiveness of each of the different methods
        /// </summary>
        static void runAllExperiments(string topLevelFolder, string modelFile, string edgeDefinitionFile, string modelAnalysisFileName, int k)
        {

            List<Tuple<short, short>> experimentVals = new List<Tuple<short, short>>();

            // create combinations to try for the different methods.    
            //for (short ii = 0; ii < k; ii++){
            //    for (short jj = 0; jj < k; jj++)
            //    {
            //        experimentVals.Add(new Tuple<short, short>(ii, jj));
            //    }
            // }

            //experimentVals.Add(new Tuple<short, short>(2, 2));
            //experimentVals.Add(new Tuple<short, short>(2, 3));

            //experimentVals.Add(new Tuple<short, short>(1, 0));
            //experimentVals.Add(new Tuple<short, short>(0, 1));

            experimentVals.Add(new Tuple<short, short>(2, 3));
            experimentVals.Add(new Tuple<short, short>(3, 2));
            experimentVals.Add(new Tuple<short, short>(2, 4));
            experimentVals.Add(new Tuple<short, short>(4, 2));

            //experimentVals.Add(new Tuple<short, short>(5, 2));

            Directory.SetCurrentDirectory(topLevelFolder);


            List<List<string>> resultCols = new List<List<string>>();
            IntWrapper threadEditCount = new IntWrapper(0);


            Thread[] threads = new Thread[experimentVals.Count];

            int kk = 0;
            // make a bunch of threads to run the experiments with variation on the two input weightings
            foreach (var experimentVal in experimentVals)
            {

                string experimentFolder = Path.Combine(topLevelFolder, "Experiment-" + experimentVal.Item1 + "-" + experimentVal.Item2);

                Directory.CreateDirectory(experimentFolder);

                string sourceBridgeModel = Path.Combine(topLevelFolder, modelFile);
                string modelDestFile = Path.Combine(experimentFolder, modelFile);
                File.Copy(sourceBridgeModel, modelDestFile, true);

                string sourceAnalysisData = Path.Combine(topLevelFolder, modelAnalysisFileName);
                string analysisDestFile = Path.Combine(experimentFolder, modelAnalysisFileName);
                File.Copy(sourceAnalysisData, analysisDestFile, true);

                string edgeDefFile = Path.Combine(topLevelFolder, edgeDefinitionFile);
                string edgeDefLocal = Path.Combine(experimentFolder, edgeDefinitionFile);
                File.Copy(edgeDefFile, edgeDefLocal, true);

                Thread thread = new Thread(() => runExperiment(threadEditCount, experimentFolder, experimentVal, resultCols));
                threads[kk] = thread;
                thread.Name = String.Format("{0}", kk);

                thread.Start();

                kk++;
            }

            for (int i = 0; i < threads.Length; i++)    
            {
                threads[i].Join();
            }

            
            string analysisFile = Path.Combine(@"D:\Documents\DissertationWork\models\FinalDissoExperiments\Experiments\" + folderName, "analysisData.csv");
            StreamWriter file = new StreamWriter(analysisFile);

            const string fieldDelim = ", , , , , , , ";


            //+"Element Count Score" + fieldDelim
            //  + "Average Max Angle" + fieldDelim

            // Write the headers for the different types of information saved to the output file.
            file.WriteLine("TimesForRuns" + fieldDelim
               + "ElemCount" + fieldDelim
               + "StressElemCount" + fieldDelim
               + "HeuristicElemCount" + fieldDelim
               + "Average Max Angle" + fieldDelim
               + "Average OverallQualScore" + fieldDelim
               + "StressImprove" + fieldDelim
               + "HeuristicImprove" + fieldDelim
               + "Average Max Parallel Devs" + fieldDelim
               
              
               );

            // write to the file at the end.
            foreach (var column in resultCols)
            {
                file.WriteLine(string.Join("", column));
            }
            file.Close();
        }
 
        /// <summary>
        /// Run an Individual experiment by creating a new Control object.
        /// </summary>
        /// <param name="experimentFolder">Folder that iterations for this threads particular experiment will run within</param>
        /// <param name="experimentVal">values to use for this the particular experiment running on this thread</param>
        static void runExperiment(IntWrapper threadEditCount, string experimentFolder, Tuple<short, short> experimentVal, List<List<string>> resultCols)
        {   
            var control = new Control(threadEditCount, experimentFolder, experimentVal, resultCols);  
        }
    }
}
