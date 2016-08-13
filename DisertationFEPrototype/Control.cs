using System.Collections.Generic;


using DisertationFEPrototype.ModelUpdate;
using DisertationFEPrototype.Optimisations;
using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype.Model;

namespace DisertationFEPrototype
{
    class Control
    {
        /// <summary>
        /// main loop which drives iteration until we have a FE model which meets the requirements, easy!
        /// </summary>
        /// <param name="lisaString"></param>
        public Control() {

            int ii = 0;

            string lisaString = @"D:\Documents\DissertationWork\basicCube.liml";
            string solveFile = @"D:\Documents\DissertationWork\secondTest.csv";

            List<MeshData> meshLog = new List<MeshData>();

            // only need to do this once to get the inital mesh, after that should try and drive it
            // purely with the solve data

            var meshDataReader = new ReadMeshData(lisaString);
            MeshData meshData = meshDataReader.GetMeshData;

            var model = new FEModel(lisaString, meshData);

            model.solve();

            // read data from the solve
            var analysisDataReader = new ReadAnalysisData(solveFile);
            List<AnalysisData> analysisData = analysisDataReader.getAnalysisData();

            while (evaluationFunction(ii) == false)
            { 

                SomeOptimisation optimisation = new SomeOptimisation(model, analysisData);
                optimisation.runOptimisation();

                MeshData refinedMesh = optimisation.GetUpdatedMesh;
                meshLog.Add(refinedMesh);

                WriteNewMeshData meshWriter = new WriteNewMeshData(refinedMesh);

                model.solve();
                
                // read data from the solve
                analysisData = analysisDataReader.getAnalysisData();

                ii++;
            }

        }
        private bool evaluationFunction(int ii)
        {
            return ii > 3;
        }
    }
}
