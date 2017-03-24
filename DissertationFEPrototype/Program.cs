using System;
using System.Collections.Generic;
using System.IO;

using System.Threading;

namespace DissertationFEPrototype
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// Read in possible command line arguments specifying key inoput folders and files so the system can execute
        /// </summary>
        // [STAThread]
        static void Main(string[] args)
        {

            string topLevelFolder = @"D:\Documents\DissertationWork\models\FinalDissoExperiments\Experiments\BridgeAdvanced";
            string modelFile = "bridgeAdvanced.liml";
            string edgeDefinitionFile = "modelEdges.json";
            string modelAnalysisFileName = "bridgeAdvancedOut.csv";
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

            experimentVals.Add(new Tuple<short, short>(2, 0));

            Directory.SetCurrentDirectory(topLevelFolder);

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

                Thread thread = new Thread(() => runExperiment(experimentFolder, experimentVal));
                thread.Name = String.Format("{0}", kk);

                thread.Start();

                kk++;
            }
        }
    
        /// <summary>
        /// Run an Individual experiment by creating a new Control object.
        /// </summary>
        /// <param name="experimentFolder">Folder that iterations for this threads particular experiment will run within</param>
        /// <param name="experimentVal">values to use for this the particular experiment running on this thread</param>
        static void runExperiment(string experimentFolder, Tuple<short, short> experimentVal)
        {   
            var control = new Control(experimentFolder, experimentVal);  
        }
    }
}
