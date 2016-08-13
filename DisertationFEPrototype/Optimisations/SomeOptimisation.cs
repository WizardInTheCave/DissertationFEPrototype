using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;

namespace DisertationFEPrototype.Optimisations
{
    /// <summary>
    /// this class will is the entry point for the optimisation part of the project
    /// </summary>
    class SomeOptimisation
    {
        FEModel workingModel;
        List<AnalysisData> analysisData;
        MeshData updatedMesh;

        public MeshData GetUpdatedMesh
        {
            get
            {
                return this.updatedMesh;
            }
        }
        public SomeOptimisation(FEModel model, List<AnalysisData> analysisData)
        {
            this.workingModel = model;
            this.analysisData = analysisData;
        }

        internal void runOptimisation()
        {



            //updatedMesh = new MeshData();
            updatedMesh = null;
            throw new NotImplementedException();
        }
    }
}
