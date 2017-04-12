using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DissertationFEPrototype.Model;
using Microsoft.VisualBasic.FileIO;

namespace DissertationFEPrototype.ModelUpdate
{

    /// <summary>
    /// read data of the analysis in from the solve csv file
    /// </summary>
    class ReadAnalysisData
    {
       
        string solveFile;
        bool isNodeOutput;
      
        /// <summary>
        /// Read data stored in the file produced by the solve operation
        /// between refinement iterations
        /// </summary>
        /// <param name="solveFile"></param>
        public ReadAnalysisData(string solveFile, bool isNodeOutput)
        {
            this.solveFile = solveFile;
            this.isNodeOutput = isNodeOutput;
            //this.analysisData = something;
        }
        public string SolveFile { set  { this.solveFile = value; } }

        public List<NodeAnalysisData> getAnalysisData()
        {

            List<NodeAnalysisData> analysisData;
            using (TextFieldParser parser = new TextFieldParser(solveFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                string[] header = parser.ReadFields();

                if (!isNodeOutput)
                {
                    //need in interface here
                    // analysisData = parseElementAnalysisData(parser);
                    analysisData = null;
                }
                else
                {
                    analysisData = parseNodeAnalysisData(parser);
                }
            }
            return analysisData;
        }


        private List<NodeAnalysisData> parseNodeAnalysisData(TextFieldParser parser)
        {
            List<NodeAnalysisData> analysisData = new List<NodeAnalysisData>();
            while (!parser.EndOfData)
            {
                //Process row
                // .Skip(1)
                string[] fields = parser.ReadFields().ToArray();

                int nodeId = Convert.ToInt32(fields[0]);

                double x = Convert.ToDouble(fields[1]);
                double y = Convert.ToDouble(fields[2]);
                double z = Convert.ToDouble(fields[3]);

                double dispX = Convert.ToDouble(fields[4]);
                double dispY = Convert.ToDouble(fields[5]);
                double dispZ = Convert.ToDouble(fields[6]);

                double sheerUW = Convert.ToDouble(fields[7]);
                double sheerVW = Convert.ToDouble(fields[8]);

                double vonMeisesBottom = Convert.ToDouble(fields[9]);
                double vonMeisesUpper = Convert.ToDouble(fields[10]);

                double principal1Upper = Convert.ToDouble(fields[11]);
                double principal2Upper = Convert.ToDouble(fields[12]);

                double principal1Bottom = Convert.ToDouble(fields[13]);
                double principal2Bottom = Convert.ToDouble(fields[14]);

                double stressUU = Convert.ToDouble(fields[15]);
                double stressVV = Convert.ToDouble(fields[16]);
                double stressUV = Convert.ToDouble(fields[17]);
                double vonMisesMidplane = Convert.ToDouble(fields[18]);
                double principalStress1Midplane = Convert.ToDouble(fields[19]);
                double principalStress2Midplane = Convert.ToDouble(fields[20]);
                
                double dispMag = Convert.ToDouble(fields[21]);

                analysisData.Add(new NodeAnalysisData(nodeId, x, y, z, dispX, dispY, dispZ, 
                    sheerUW, sheerVW, vonMeisesBottom,
                    vonMeisesUpper, principal1Upper, principal2Upper, principal1Bottom, 
                    principal2Bottom, stressUU, stressVV, stressUV, vonMisesMidplane, principalStress1Midplane,
                    principalStress1Midplane, principalStress2Midplane, dispMag));
            }
            return analysisData;
        }

        private List<ElementAnalysisData> parseElementAnalysisData(TextFieldParser parser)
        {
            List<ElementAnalysisData> analysisData = new List<ElementAnalysisData>();
            while (!parser.EndOfData)
            {
                //Process row
                // .Skip(1)
                string[] fields = parser.ReadFields().ToArray();

                // int material = Convert.ToInt32(fields[0]);
                int element = Convert.ToInt32(fields[0]);
                int localNode = Convert.ToInt32(fields[1]);

                double x = Convert.ToDouble(fields[2]);
                double y = Convert.ToDouble(fields[3]);
                double z = Convert.ToDouble(fields[4]);

                string dispX = fields[5];
                string dispY = fields[6];
                string dispZ = fields[7];

                double stressXX = Convert.ToDouble(fields[8]);
                double stressYY = Convert.ToDouble(fields[9]);
                double stressZZ = Convert.ToDouble(fields[10]);

                double stressXY = Convert.ToDouble(fields[11]);
                double stressYZ = Convert.ToDouble(fields[12]);
                double stressZX = Convert.ToDouble(fields[13]);

                analysisData.Add(new ElementAnalysisData(element, localNode, x, y, z, dispX, dispY, dispZ, stressXX, stressYY, stressZZ, stressXY, stressYZ, stressZX));
            }
            return analysisData;
        }
    }    
}
