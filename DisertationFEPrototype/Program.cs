using System;
using System.Collections.Generic;
using System.IO;

using System.Threading;

namespace DisertationFEPrototype
{
    static class Program
    {
        static object statObjLocker = new object();


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            runAllExperiments();
        }

        /// <summary>
        /// This method runs the system a whole bunch of times with variations on how much each method is used when meshing,
        /// this is so data can be gathered about the effectiveness of each of the different methods
        /// </summary>
        static void runAllExperiments()
        {



            List<Tuple<short, short>> experimentVals = new List<Tuple<short, short>>();

            string modelFile = "bridgeAdvanced.liml";
            string modelAnalysisFileName = "bridgeAdvancedOut.csv";


            // create permutations to try for the different methods.
            for (short ii = 0; ii < 3; ii++)
            {
                for (short jj = 0; jj < 3; jj++)
                {
                    experimentVals.Add(new Tuple<short, short>(ii, jj));
                }
            }

            string topLevelFolder = @"D:\Documents\DissertationWork\models\Experiments\BridgeAdvancedFol";

            Directory.SetCurrentDirectory(topLevelFolder);

            int kk = 0;
            // make a bunch of threads to run the experiments with variation on the two input weightings
            foreach (var experimentVal in experimentVals)
            {

                string experimentFolder = Path.Combine(topLevelFolder, "Experiment" + kk.ToString());

                Directory.CreateDirectory(experimentFolder);

                string sourceBridgeModel = Path.Combine(topLevelFolder, modelFile);
                string modelDestFile = Path.Combine(experimentFolder, modelFile);
                File.Copy(sourceBridgeModel, modelDestFile, true);

                string sourceAnalysisData = Path.Combine(topLevelFolder, modelAnalysisFileName);
                string analysisDestFile = Path.Combine(experimentFolder, modelAnalysisFileName);
                File.Copy(sourceAnalysisData, analysisDestFile, true);

   
                Thread thread = new Thread(() => runExperiment(experimentFolder, experimentVal));
                thread.Name = String.Format("{0}", kk);

                thread.Start();

                kk++;
            }
        }
  
        static void runExperiment(string experimentFolder, Tuple<short, short> experimentVal)
        {
            
            var control = new Control(experimentFolder, experimentVal);
           
        }
    }
}
