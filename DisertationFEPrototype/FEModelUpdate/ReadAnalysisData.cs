using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.FEModel;

namespace DisertationFEPrototype.FEModelUpdate
{

    /// <summary>
    /// read data of the analysis in from the solve csv file
    /// </summary>
    class ReadAnalysisData
    {
        AnalysisData analysisData;

        public AnalysisData GetAnalysisData
        {
            get
            {
                return this.analysisData;
            }
        }
        public ReadAnalysisData()
        {


            //this.analysisData = something;
        }
    }    
}
