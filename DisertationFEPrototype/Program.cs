using System;
using System.Collections.Generic;
using System.IO;
namespace DisertationFEPrototype
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            runExperiments();
        }

        /// <summary>
        /// This method runs the system a whole bunch of times with variations on how much each method is used when meshing,
        /// this is so data can be gathered about the effectiveness of each of the different methods
        /// </summary>
        static void runExperiments()
        {

            List<Tuple<short, short>> experimentVals = new List<Tuple<short, short>>();

            string modelFileName = "bridgeAdvanced.liml";

            // create permutations to try for the different methods.
            for (short ii = 0; ii < 3; ii++)
            {
                for (short jj = 0; jj < 3; jj++)
                {
                    experimentVals.Add(new Tuple<short, short>(ii, jj));
                }
            }

            string topLevelFolder = @"D:\Documents\DissertationWork\models\Experiments\BridgeAdvancedFol";
            
            int kk = 0;
            foreach (var experimentVal in experimentVals)
            {

                string experimentFolder = Path.Combine(topLevelFolder, "Experiment" + kk.ToString());

                Directory.CreateDirectory(experimentFolder);

                string sourceBridgeModel = Path.Combine(topLevelFolder, modelFileName);
                string modelDestFile = Path.Combine(experimentFolder, modelFileName);
                File.Copy(sourceBridgeModel, modelDestFile, true);

                string sourceAnalysisData = Path.Combine(topLevelFolder, modelFileName);
                string analysisDestFile = Path.Combine(experimentFolder, modelFileName);
                File.Copy(sourceAnalysisData, analysisDestFile, true);

                var control = new Control(experimentFolder, experimentVal);
                kk++;
            }

        }
    }
}
