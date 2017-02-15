using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.MeshQualityMetrics;

namespace DisertationFEPrototype.MeshQualityMetrics
{
    /// <summary>
    /// Currently these mesh quality metrics are taken from the following paper:
    /// "Mesh Optimization Using a Genetic Algorithm  to Control Mesh Creation Parameters"
    /// </summary>
    class MeshQualityAssessment
    {

        MeshData meshData;
        ElementQualityMetrics elemQualMetrics;
        double elemCountScore;
        double elemQualScore;

        

        double overallQualImprovement;

        // this contains metrics specific to individual elements such as aspect ratio, parallel dev etc.
        public ElementQualityMetrics ElemQualMetrics { get { return this.elemQualMetrics; } }
        public double ElemCountScore { get { return this.elemCountScore; } }
        public double ElemQualityScore { get { return this.elemQualScore; } }

        public double OvarallQualityImprovement {
            set { this.overallQualImprovement = value; }
            get { return this.overallQualImprovement; }
        }


        public MeshQualityAssessment(MeshData meshData)
        {
            this.meshData = meshData;
            this.elemQualMetrics = new ElementQualityMetrics(meshData.Elements);
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
        private double getElemCountScore(int meshElemCount, double meshSurfaceArea, double targetElementSize)
        {
            double elemCountScore = meshElemCount / (meshSurfaceArea / Math.Pow(targetElementSize, 2));
            return elemCountScore;
        }

        public void assessMesh()
        {
            // getSurfaceArea(meshData.Elements);
            // the number of square elements of the desired size that would be required to cover the entire area 
            // (the desired number of elements), 
            // and the entire quotient is the ratio of actual element count to desired element count
            // 
            // for the time being we will take this to mean the smallest element that is produced using h-refinement for the current model

            var areas = meshData.Elements.Select(e => e.Area);

            double meshSurfaceArea = areas.Sum();
            double targetElemSize = meshData.Elements.Min(e => e.Area);

            elemCountScore = getElemCountScore(meshData.Elements.Count, meshSurfaceArea, targetElemSize);

            // this represents the quality of the general element shapes within the mesh
            elemQualScore = elemQualMetrics.getElemQuality();

            // Console.WriteLine("ElemCountScore: " + elemCountScore.ToString() + " Elem Quality: " + elemQual.ToString());
        }
        
    }
}
