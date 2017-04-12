using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationFEPrototype.Model.Analysis.MaterialProps
{
    /// <summary>
    /// Class storing the mechanical properties of the material
    /// </summary>
    public class Mechanical
    {
        string type;
        long youngsModulus;
        double poissonRatio;

        public string GetMechanicalType { get { return this.type; } }

        public double GetYoungsModulus { get { return this.youngsModulus; } }

        public double GetPoissonRatio {  get { return this.poissonRatio; } }


        public Mechanical(string type, long youngsModulus, double poissonRatio)
        {
            this.type = type;
            this.youngsModulus = youngsModulus;
            this.poissonRatio = poissonRatio;
        }
    }
}
