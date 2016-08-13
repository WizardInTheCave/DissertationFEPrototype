using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype.FEModel;
using DisertationFEPrototype.Optimisations;

namespace DisertationFEPrototype
{
    class Control
    {
        public Control(string lisaString) {

            var meshDataReader = new ReadMeshData(lisaString);
            MeshData meshData = meshDataReader.GetMeshData;

            var analysisDataReader = new ReadAnalysisData();
            AnalysisData analysisData = analysisDataReader.GetAnalysisData;

            var model = new FEModel.FEModel(lisaString, meshData, analysisData);

            SomeOptimisation optimisation = new SomeOptimisation(model);
            optimisation.runOptimisation();
            optimisation.GetUpdatedModel;




        }
    }
}
