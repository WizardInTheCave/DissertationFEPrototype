using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Model.Analysis.MaterialProps
{
    class Geometric
    {
        string type;
        double thickness;
        double planestrain;

        public string GetGeometricType
        {
            get{
                return this.type;
            }
        }
        public double GetThickness
        {
            get
            {
                return this.thickness;
            }
        }
        public double GetPlaneStrain
        {
            get
            {
                return this.planestrain;
            }
        }

        public Geometric(string type, double thickness, double planestrain)
        {
            this.type = type;
            this.thickness = thickness;
            this.planestrain = planestrain;
        }

    }
}
