using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DissertationFEPrototype.Model;
using DissertationFEPrototype.MeshQualityMetrics;
using DissertationFEPrototype.FEModelUpdate.Model.Structure;
using DissertationFEPrototype.FEModelUpdate.Model;

namespace DissertationFEPrototype.MeshQualityMetrics
{
    /// <summary>
    /// Currently these mesh quality metrics are taken from the following paper:
    /// "Mesh Optimization Using a Genetic Algorithm  to Control Mesh Creation Parameters" by
    /// </summary>
    public class MeshQualityAssessment
    {
        MeshData meshData;

        List<NodeAnalysisData> analysisData;
        ElementQualityMetrics elemQualMetrics;
        double elemCountScore;
        double elemQualScore;

        double overlapScore;
        
        double overallQualImprovement;


        double stressRefinementIncrease;
        double heuristicRefinementIncrease;

        // this contains metrics specific to individual elements such as aspect ratio, parallel dev etc.
        public ElementQualityMetrics ElemQualMetrics { get { return this.elemQualMetrics; } }
        public double ElemCountScore { get { return this.elemCountScore; } }
        public double ElemQualityScore { get { return this.elemQualScore; } }

        public double StressRefinementIncrease { get { return this.stressRefinementIncrease;  } }
        public double HeuristicRefinementIncrease { get { return this.heuristicRefinementIncrease; } }
        // public double HeuristicOverlapScore { get { return this.overlapScore;  } }

        public double OvarallQualityImprovement {
            set { overallQualImprovement = value; }
            get { return overallQualImprovement; }
        }


        public MeshQualityAssessment(MeshData meshData, List<NodeAnalysisData> analysisData)
        {
            this.meshData = meshData;
            this.analysisData = analysisData;
            this.elemQualMetrics = new ElementQualityMetrics(meshData.Elements);
        }
        private double computeStressRefinementImprovement()
        {
            var stressRefinedNodes = this.meshData.Nodes.Values.Where(node => node.NodeOrigin == Node.Origin.Stress);
            var stressOverlap = getRefinedMeshOverlap(stressRefinedNodes, this.analysisData);
            return stressOverlap.Select(x => x.DispMag).Sum() / stressRefinedNodes.Count();

        }

        private double computeHuristicRefinementImprovement()
        {
            var heuristicRefinedNodes = this.meshData.Nodes.Values.Where(node => node.NodeOrigin == Node.Origin.Heuristic);
            var heuristicOverlap = getRefinedMeshOverlap(heuristicRefinedNodes, this.analysisData);
            
            // A simple way to work out how useful the heuristic has been is to
            // sum of all displacement across the section of the mesh, if there was high displacement then
            // meshing here was worthwhile
            return heuristicOverlap.Select(x => x.DispMag).Sum() / heuristicRefinedNodes.Count();
           
        }
        /// <summary>
        /// This method will compute the overlap of area for which meshing was performed due to application of the ILP
        /// Knowledge base and areas under stress within the model.
        /// </summary>
        /// <returns></returns>
        private List<NodeAnalysisData> getRefinedMeshOverlap(IEnumerable<Node> nodeGroup, List<NodeAnalysisData> analysistNodeData)
        {
            // all nodes generated using rule based meshing
           

            // dictionary of nodes with associeated stress
            var analyIds = this.analysisData.ToDictionary(x => x.Id, y => y);

            var intersectingNodes = new List<NodeAnalysisData>();

            foreach (Node heruristicNode in nodeGroup)
            {
                if (analyIds.ContainsKey(heruristicNode.Id))
                {
                    intersectingNodes.Add(analyIds[heruristicNode.Id]);
                }
            }
            return intersectingNodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshElemCount">Element count score metric</param>
        /// <param name="meshSurfaceArea">total surface area of the mesh, 
        /// this shouldn't be altered by remeshing however</param>
        /// <param name="targetElementSize">
        /// Thus, the denominator of the above equation represents 
        /// the number of square elements of the desired size that would be required to cover 
        /// the entire area (the desired number of elements), 
        /// and the entire quotient is the ratio of actual element count 
        /// to desired element count</param>
        /// <returns>An overall element count score metric</returns>
        private double getElemCountScore(int meshElemCount, double meshSurfaceArea, double targetElementSize)
        {
            double elemCountScore = meshElemCount / (meshSurfaceArea / Math.Pow(targetElementSize, 2));
            return elemCountScore;
        }

        /// <summary>
        /// Perform assessment of the mesh
        /// </summary>
        public void assessMesh()
        {
            // getSurfaceArea(meshData.Elements);
            // the number of square elements of the desired size that would be required to cover the entire area 
            // (the desired number of elements), 
            // and the entire quotient is the ratio of actual element count to desired element count
            // 
            // for the time being we will take this to mean the smallest element that is produced using h-refinement for the current model

            var areas = meshData.Elements.Select(e => e.getArea());

            double meshSurfaceArea = areas.Sum();
            double targetElemSize = meshData.Elements.Min(e => e.getArea());

            elemCountScore = getElemCountScore(meshData.Elements.Count, meshSurfaceArea, targetElemSize);

            // this represents the quality of the general element shapes within the mesh
            elemQualScore = elemQualMetrics.getElemQuality();

            this.heuristicRefinementIncrease = computeHuristicRefinementImprovement();
            this.stressRefinementIncrease = computeStressRefinementImprovement();
  
        }
        
    }
}
