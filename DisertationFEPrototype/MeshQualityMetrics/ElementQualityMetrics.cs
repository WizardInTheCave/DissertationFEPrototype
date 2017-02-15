using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;

namespace DisertationFEPrototype.MeshQualityMetrics
{
    /// <summary>
    /// methods taken from paper Mesh Optimization Using a Genetic Algorithm  
    /// to Control Mesh Creation Parameters
    /// Jeremy P.Dittmer1, C.Greg Jensen1, Michael Gottschalk3 and Thomas Almy4
    /// </summary>
    class ElementQualityMetrics
    {
        List<double> aspectRatios;
        List<double> maxCornerAngles;
        List<double> maxParallelDevs;

        public List<double> AspectRatios { get { return this.aspectRatios; } }

        public List<double> MaxCornerAngles { get { return this.maxCornerAngles; } }

        public List<double> MaxParrallelDevs { get  { return this.maxParallelDevs; } }

        public ElementQualityMetrics(List<IElement> elements)
        {
            aspectRatios = elements.Select(elem => elem.AspectRatio).ToList();
            maxCornerAngles = elements.Select(elem => elem.MaxCornerAngle).ToList();
            maxParallelDevs = elements.Select(elem => elem.MaxParallelDev).ToList();
        }

        /// <summary>
        /// we want to calculate
        /// </summary>
        /// <param name="elem">a particular element that we are checking the quality of</param>
        /// <returns></returns>
        public double getElemQuality()
        {

            double qualityMetric = 0;
           

            double meanAspectRatio = aspectRatios.Average();
            double meanMaxCornerAngles = maxCornerAngles.Average();
            double meanMaxParallelDev = maxParallelDevs.Average();

            double aspectDev = getDeviationScore(aspectRatios, meanAspectRatio);
            double maxCornerAngleDev = getDeviationScore(maxCornerAngles, meanMaxCornerAngles);
            double maxParallelDev = getDeviationScore(maxParallelDevs, meanMaxParallelDev);

            var perfectScore = 1;
            var averageScore = 1;

            // use observations to get average for each shape test score

            // these are the six values mentioned in the paper,
            // the first three are averages and the second three are the deviation values
            qualityMetric += weightedScore(meanAspectRatio, perfectScore, averageScore);
            qualityMetric += weightedScore(meanMaxCornerAngles, perfectScore, averageScore);
            qualityMetric += weightedScore(meanMaxParallelDev, perfectScore, averageScore);


            qualityMetric += weightedScore(aspectDev, perfectScore, averageScore);
            qualityMetric += weightedScore(maxCornerAngleDev, perfectScore, averageScore);
            qualityMetric += weightedScore(maxParallelDev, perfectScore, averageScore);

            // smaller values represent better meshes and there should only be positive values
            if (qualityMetric < 0)
            {
                throw new Exception("The element quality metric value should never be 0, there is a bug in the implementation");
            }
            else
            {
                return qualityMetric;
            }
        }
        private static double weightedScore(double score, double perfectScore, double averageScore)
        {
            return (score - perfectScore) / (averageScore - perfectScore); 
        }
        /// <summary>
        /// according to the paper I have taken this method of deviation is used because it is faster
        /// than performing standard deviation.
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="metricAvrg"></param>
        /// <returns></returns>
        private static double getDeviationScore(List<double> metric, double metricAvrg)
        {
            double deviationScore;
            double thing = 0;
            int n = metric.Count;

            for (int ii = 0; ii < metric.Count; ii++)
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
