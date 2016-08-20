using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;
using Microsoft.VisualBasic.FileIO;

namespace DisertationFEPrototype.ModelUpdate
{

    /// <summary>
    /// read data of the analysis in from the solve csv file
    /// </summary>
    class ReadAnalysisData
    {
        
        List<AnalysisData> analysisData;
        string solveFile;
      
        public ReadAnalysisData(string solveFile)
        {
            this.solveFile = solveFile;

            //this.analysisData = something;
        }
        public List<AnalysisData> getAnalysisData()
        {
            List<AnalysisData> analysisData = new List<AnalysisData>();

            using (TextFieldParser parser = new TextFieldParser(solveFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                string[] header = parser.ReadFields();

                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields().Skip(1).ToArray();

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

                    analysisData.Add(new AnalysisData(element, localNode, x, y, z, dispX, dispY, dispZ, stressXX, stressYY, stressZZ, stressXY, stressYZ, stressZX));
                }
            }
            return analysisData;
        }
    }    
}
