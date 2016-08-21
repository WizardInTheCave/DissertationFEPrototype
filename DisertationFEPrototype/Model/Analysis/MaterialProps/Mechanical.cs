using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Model.Analysis.MaterialProps
{
    class Mechanical
    {
        string type;
        long youngsModulus;
        double poissonRatio;

        public Mechanical(string type, long youngsModulus, double poissonRatio)
        {
            this.type = type;
            this.youngsModulus = youngsModulus;
            this.poissonRatio = poissonRatio;
        }
    }
}
