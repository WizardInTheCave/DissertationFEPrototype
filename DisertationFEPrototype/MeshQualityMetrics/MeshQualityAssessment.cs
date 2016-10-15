using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.MeshQualityMetrics;
using DisertationFEPrototype.Model.MeshDataStructure;

namespace DisertationFEPrototype.MeshQualityMetrics
{
    /// <summary>
    /// Currently these mesh quality metrics are taken from the following paper:
    /// "Mesh Optimization Using a Genetic Algorithm  to Control Mesh Creation Parameters"
    /// </summary>
    class MeshQualityAssessment
    {

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
        private static double getElemCountScore(int meshElemCount, double meshSurfaceArea, double targetElementSize)
        {
            double elemCountScore = meshElemCount / (meshSurfaceArea / Math.Pow(targetElementSize, 2));
            return elemCountScore;

        }

        public static void assessMesh(MeshData meshData)
        {

            var areas =  meshData.Elements.Select(e => e.Area);
            foreach(Element elem in meshData.Elements)
            {
                if (Double.IsNaN(elem.Area))
                {
                    Console.WriteLine(elem.Nodes[0].Id.ToString() + " " + elem.Nodes[1].Id.ToString() + " " + elem.Nodes[2].Id.ToString() + " " + elem.Nodes[3].Id.ToString());
                }
            }
            double meshSurfaceArea = areas.Sum();
            // getSurfaceArea(meshData.Elements);


            // the number of square elements of the desired size that would be required to cover the entire area 
            // (the desired number of elements), 
            // and the entire quotient is the ratio of actual element count to desired element count
            // 
            // for the time being we will take this to mean the smallest element that is produced using h-refinement for the current model

            double targetElemSize = meshData.Elements.Min(e => e.Area);

            double elemCountScore = getElemCountScore(meshData.Elements.Count, meshSurfaceArea, targetElemSize);
            double elemQual = MeshQualityMetrics.ElementQualityMetrics.getElemQuality(meshData.Elements);
            Console.WriteLine("ElemCountScore: " + elemCountScore.ToString() + " Elem Quality: " + elemQual.ToString());
            

        }

        //private static double getSurfaceArea(List<Element> elements)
        //{
        //    foreach(Element in elements)
        //    {

        //    }

        //    throw new NotImplementedException();
        //}

        /// <summary>
     
        /// </summary>
        /// <returns></returns>
        private static double getTargElemSize()
        {
            throw new NotImplementedException();
        }

        
    }
}
