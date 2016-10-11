using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Math;
using DisertationFEPrototype.Model.MeshDataStructure;

namespace DisertationFEPrototype.MeshQualityMetrics
{
    /// <summary>
    /// methods taken from paper Mesh Optimization Using a Genetic Algorithm  
    /// to Control Mesh Creation Parameters
    /// Jeremy P.Dittmer1, C.Greg Jensen1, Michael Gottschalk3 and Thomas Almy4
    /// </summary>
    class ElementQualityMetrics
    {
        /// <summary>
        /// we want to calculate
        /// </summary>
        /// <param name="elem">a particular element that we are checking the quality of</param>
        /// <returns></returns>
        public static double elementQuality(List<Element> elements)
        {
            List<double> aspectRatios = elements.Select(elem => elem.AspectRatio).ToList();
            List<double> maxCornerAngles = elements.Select(elem => elem.MaxCornerAngle).ToList();
            List<double> maxParallelDev = elements.Select(elem => elem.MaxParallelDev).ToList();

            double meanAspectRatio = aspectRatios.Average();

            double meanMaxCornerAngles = maxCornerAngles.Average();
            double meanMaxParallelDev = maxParallelDev.Average();

            throw new NotImplementedException();
           
        }
        /// <summary>
        /// according to the paper I have taken this method of deviation is used because it is faster
        /// than performing standard deviation.
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="metricAvrg"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        double getDeviationScore(List<double> metric, double metricAvrg, List<Element> elements)
        {
            double deviationScore;
            double thing = 0;
            int n = elements.Count;

            for (int ii = 0; ii < elements.Count; ii++)
            {
                double x = metric[ii];
                double xBar = metricAvrg;
                thing += Math.Abs(x - xBar);
            }
            deviationScore = (1 / n) * thing;
            return deviationScore;
        }
    }
}
