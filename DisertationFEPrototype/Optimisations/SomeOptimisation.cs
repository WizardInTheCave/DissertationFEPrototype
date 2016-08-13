using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DisertationFEPrototype.Optimisations
{
    /// <summary>
    /// this class will is the entry point for the optimisation part of the project
    /// </summary>
    class SomeOptimisation
    {
        FEModel.FEModel workingModel;

        public FEModel.FEModel GetUpdatedModel
        {
            get
            {
                return this.workingModel;
            }
        }
        public SomeOptimisation(FEModel.FEModel model)
        {
            this.workingModel = model;

        }

    }
}
