using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.MeshQualityMetrics
{
    /// <summary>
    /// Currently these mesh quality metrics are taken from the following paper:
    /// "Mesh Optimization Using a Genetic Algorithm  to Control Mesh Creation Parameters"
    /// </summary>
    class MeshQualityAssessment
    {

        double elementPattern()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshElemCount"></param>
        /// <param name="meshSurfaceArea"></param>
        /// <param name="targetElementSize">
        /// Thus, the denominator of the above equation represents 
        /// the number of square elements of the desired size that would be required to cover 
        /// the entire area (the desired number of elements), 
        /// and the entire quotient is the ratio of actual element count 
        /// to desired element count</param>
        /// <returns></returns>
        double elementCountScore(int meshElemCount, double meshSurfaceArea, double targetElementSize)
        {
            double elemCountScore = meshElemCount / (meshSurfaceArea / Math.Pow(targetElementSize, 2));
            return elemCountScore;

        }
    }
}
