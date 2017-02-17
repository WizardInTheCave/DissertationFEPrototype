using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.Model.Analysis;
using DisertationFEPrototype.Optimisations.ILPRules;
using DisertationFEPrototype.MeshQualityMetrics;
using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using DisertationFEPrototype.Model.Structure;

namespace DisertationFEPrototype.Optimisations
{

    /// <summary>
    /// this class will is the entry point for the optimisation part of the project
    /// </summary>
    class OptimisationManager
    {
        MeshData meshData;


        List<NodeAnalysisData> analysisData;
        RuleManager ruleManager;

        Dictionary<Tuple<double, double, double>, Node> nodes;

        bool firstIteration = true;

        short ILPRefineCount = 1;
        short stressRefineCount = 1;

        public MeshData GetUpdatedMesh
        {
            get
            {
                return this.meshData;
            }
        }
         
        public OptimisationManager(MeshData meshData, List<NodeAnalysisData> analysisData, int iterationCount)
        {
            this.meshData = meshData;
            this.analysisData = analysisData;
            this.ruleManager = new RuleManager(meshData, iterationCount);
        }

        /// <summary>
        /// get the derivatives
        /// </summary>
        private List<double> getDerivatives(double[] values)
        {
            List<double> devs = new List<double>();
            for(int ii = 0; ii < values.Count() - 2; ii++)
            {
                var item1 = values[ii];
                var item2 = values[ii + 1];
                devs.Add(item2 - item1);
            }   

            return devs;

            //var countScoreLast = elemCountScores[elemCountScores.Count() - 1];
            //var countScorePenultimate = elemCountScores[elemCountScores.Count() - 2];
            //var countScoreRateOfChange = Math.Abs(countScoreLast - countScorePenultimate);
            //bool countImprove = countScoreLast < countScorePenultimate;

        }

        /// <summary>
        /// Compute how much the current weighting is improving the results for a particular metric.
        /// </summary>
        /// <param name="metricVals">The values recorded for each iteration of the refinement process for a particular metric</param>
        /// <returns>Rate of change of improvement</returns>
        private double getImprovementForMethodOnMetric(double[] metricVals)
        {
            // We want to keep having a negative derivatives
            List<double> elemCountScoresDevs = getDerivatives(metricVals);

            // if these are negative then there is improvement in the optimisation between each iteration of LISA
            // each derivative represents the rate of improvement for that iteration
            double penultimateDev = elemCountScoresDevs[elemCountScoresDevs.Count - 2];
            double finalDev = elemCountScoresDevs[elemCountScoresDevs.Count - 1];

            // we want to see if the rate of change of improvement is increasing or decreasing to do this we take the second derivative essentially
            double secondDev = finalDev - penultimateDev;
            return secondDev;
        }

        /// <summary>
        /// Take a the derivatives for both the linear functions representing the metric changes over the iterations.
        /// this provides an indication of the improvement overall for the iteration.
        /// </summary>
        /// <param name="meshQualityAssessments"></param>
        /// <returns>score combining both the metrics, negative value means the last iteration improved the quality of the mesh
        /// taking into account both the time in which to execute it and it's quality for producing accurate results</returns>
        //private double getOverallImprovementScore(List<MeshQualityAssessment> meshQualityAssessments)
        //{
        //    var elemCountScores = meshQualityAssessments.Select(x => x.ElemCountScore).ToArray();

        //    // this metric basically represents whether the improvement has made improved the mesh from a time to solve standpoint
        //    double elemCountImprovement = getImprovementForMethodOnMetric(elemCountScores);
        //    var elemQualScores = meshQualityAssessments.Select(x => x.ElemQualityScore).ToArray();

        //    // this metric tells us if the quality of the mesh has improved which will allow for more accurate prediction
        //    double elemQualImprovement = getImprovementForMethodOnMetric(elemQualScores);

        //    double combinedImprovementValue = elemCountImprovement + elemQualImprovement;

        //    // conclude the overall global rate of improvement considering the two metrics, for this improvement we want to append the global calculated
        //    meshQualityAssessments[meshQualityAssessments.Count - 1].OvarallQualityImprovement = combinedImprovementValue;
        //    var improvementsForEachIteration = meshQualityAssessments.Select(x => x.OvarallQualityImprovement).ToArray();

        //    // if this is negative then things are improving 
        //    double overallImprovementScore = getImprovementForMethodOnMetric(improvementsForEachIteration);

        //    return overallImprovementScore;
        //}

        /// <summary>
        /// Main method for refining the mesh, this method has control over how to apply each of the two sub methods in order to get a better hybrid refinement
        /// </summary>
        /// <param name="meshQualityAssessments">Assessments of the mesh quality calculated using the metrics provided in Dittmers paper</param>
        public void refineMesh(List<MeshQualityAssessment> meshQualityAssessments)
        {
            // try a basic mesh refinement by creating more elements first
            List<IElement> elements = meshData.Elements;


            foreach (IElement elem in elements)
            {
                if (elem.Nodes.Count < 4)
                {
                    Console.WriteLine("What???");
                }
            }



            nodes = meshData.Nodes;

            // var elem = elements.Select(x => x.Id);

            // if these are going down then carry on applying the same strategy that we are currently
            // otherwise change it

            // vary element count score to get an element quality score 
            // can't assume the two values are directly related though

            //var funElemCountScores = MathNet.Numerics.Interpolation.LinearSpline.Interpolate(, elemCountScores);
            //var funElemQualScores = MathNet.Numerics.Interpolation.LinearSpline.Interpolate(, elemQualScores);
            //linearInterpolation.Differentiate()


            // if this is negative then things are improving 

            ////double overallImprovementScore = getOverallImprovementScore(meshQualityAssessments);
            ////// revert to previous method which produced a good score
            ////if(overallImprovementScore > 0)
            ////{
            ////    // change the weightings
            ////    // meshQualityAssessments.
            ////}
            ////else
            ////{

            ////}



            // depending on how heavily we want to perform each type of meshing run that type of meshing
            ////for(int ii = 0; ii < stressRefineCount; ii++)
            ////{
            stressGradientDrivenRemesh(elements, analysisData);
            // ILPEdgeDrivenRefinement(0);
            ////}
            //for (int ii = 0; ii < ILPRefineCount; ii++)
            //{
            // ILPEdgeDrivenRefinement(0);
            //}

            //foreach (IElement elem in elements)
            //{
            //    if (elem.Nodes.Count < 4)
            //    {
            //        Console.WriteLine("What???");
            //    }
            //}



            // we have some derivatives for the straight lines we want to 
            // keep what we are currently doing if the derivative is equally negative, 
            // then apply an even more extreme version of what we were doing

            // otherwise one metric has improved, something is going ok but we did something to upset another variable
            // using an if statement for now but a deligate would ultimately be ideal for doing this.
            // later on need to do both but with some weighting

            //List<double> aspectRatios = meshQualityAssessment.ElemQualMetrics.AspectRatios;
            //List<double> maxCornerAngles = meshQualityAssessment.ElemQualMetrics.MaxCornerAngles;






            // now flatten the tree structure
            List<IElement> flatElemTree = getNewElementList(elements);

            //foreach (IElement elem in flatElemTree)
            //{
            //    if (elem.Nodes.Count < 4)
            //    {
            //        Console.WriteLine("What???");
            //    }
            //}
            //List<Node> all_nodes = getAllNodes(flatElemTree);

            // update the ids on nodes which don't have ids

            // our mesh should now be refined
            var newMeshDataNodes = meshData.Nodes;

            var newmeshData = new MeshData();
            newmeshData.Elements = flatElemTree;
            newmeshData.Nodes = newMeshDataNodes;
            newmeshData.Force = meshData.Force;
            newmeshData.Material = meshData.Material;
            newmeshData.FaceSelections = meshData.FaceSelections;
            newmeshData.FixSelections = meshData.FixSelections;
            meshData = newmeshData;

        }

    
        /// <summary>
        /// This method will use information derived from running the ILP rules that determine how much meshing should occur around each edge.
        /// Data for the edges has already been computed in the EdgeIdentification module.
        /// </summary>
        private void ILPEdgeDrivenRefinement(int ii)
        {
            // the edges which had been assigned an amount for remeshing
            List<Edge> meshingEdges = this.ruleManager.GetEdges;

            foreach (Edge edge in meshingEdges)
            {
                int elemCount = edge.ElementCount;

                if(ii == 2 && elemCount > 0)
                {
                    Console.WriteLine("DADA!!!");
                }
                List<Node> nodePath = edge.NodePath;
                // for each node on the path get its elements and mesh those
                List<IElement> allRemeshingElems = nodePath.SelectMany(np => meshData.findElems(np)).ToList();

                foreach (IElement elem in allRemeshingElems)
                {
                    if (elem.Nodes.Count < 4)
                    {
                        Console.WriteLine("What???");
                    }
                }



                remesh(allRemeshingElems, 0, edge.ElementCount);
            }
        }

       /// <summary>
       /// for each edge that was detected remesh a specified amount of times recursively
       /// </summary>
       /// <param name="elements"></param>
       /// <param name="ii"></param>
       /// <param name="elemCount"></param>
        private void remesh(List<IElement> elements, int ii, int elemCount)
        {
            if (ii < elemCount) {
                foreach (IElement elem in elements)
                {

                    
                    //if (elem.Nodes.Count < 4)
                    //{
                    //    Console.WriteLine("What???");
                    //}
                    
                    List<IElement> refined;

                    refined = elem.createChildElements(nodes);
                    

                    elem.Children = refined;

                    ii++;
                    // mesh another level
                    remesh(refined, ii, elemCount);
                }
            }
        }

    
        /// <summary>
        /// we want to use the data from the previous analyis that we have to refine our mesh specificially in areas with high stress values
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="analysisData"></param>
        private void stressGradientDrivenRemesh(List<IElement> elements, List<NodeAnalysisData> analysisData)
        {

            double remeshThreshold = determineRemeshThreshold(analysisData);

            // ruleRefinement();
            foreach (var elem in elements)
            {
                List<Node> elemNodes = elem.Nodes;
                List<int> elemNodesIds = elemNodes.Select(node => node.Id).ToList();

                // get the analysis data objecs for this particular element
                List<NodeAnalysisData> nodeAnalysisData = analysisData.Where(d => elemNodesIds.Contains(d.NodeId)).ToList();

                // get the average displacement for the element based on the nodal displacements
                double avgDispMag = nodeAnalysisData.Select(nad => nad.DispMag).Average();


                if (avgDispMag > remeshThreshold)
                {
                    Console.WriteLine("Remeshed elements: " + elem.Id.ToString());
                    List<IElement> children = elem.createChildElements(nodes);
                        // GeneralRefinementMethods.getNewQuadElements(elem, nodes);
                    elem.Children = children;
                }
            }
            Console.WriteLine("done all");
        }
        
      

        /// <summary>
        /// some function which using the nodal displacements for the whole model determines what is considered a high displacement,
        /// this can then be used to determine whether to do a h-refinement of a particular element
        /// </summary>
        /// <param name="analysisData"></param>
        /// <returns>threshold value</returns>
        private double determineRemeshThreshold(List<NodeAnalysisData> analysisData)
        {
            double threshold = 0;

            List<double> allDisps = analysisData.Select(ad => ad.DispMag).ToList();

            threshold = allDisps.Average();

            return threshold;
        }

        int flatStructElemId = 1;
        /// <summary>
        /// Use this method to flatten the element tree to produce a single list of leaf nodes by recursing through the quad tree 
        /// </summary>
        /// <returns>all elements in model in a 1d list</returns>
        // List<FaceSelection> faceSelections
        private List<IElement> getNewElementList(List<IElement> rootElements)
        {
            List<IElement> flatElemTree = new List<IElement>();

            List<Face> myFaces = new List<Face>();

            foreach (IElement elem in rootElements)
            {
                if (elem.Nodes.Count < 4)
                {
                    Console.WriteLine("What???");
                }

                // this is the bottom of the tree
                if (elem.Children == null)
                {
                    if (elem.Id == 12)
                    {
                        Console.WriteLine("current id: " + elem.Id + "new id: " + flatStructElemId);
                    }
                    
                    elem.Id = flatStructElemId;
                    flatElemTree.Add(elem);

                    //updateFaceSelection(elem, new List<Quad4Elem>() { elem });

                    flatStructElemId++;
                }
                // not at the bottom of the tree yet so continue to recurse until we hit leaf elements
                else
                {
                    // this element we have recursed through is in a face selection, 
                    // therefore all it's children should be under that same face selection
                    List<IElement> subElems = getNewElementList(elem.Children);
                    updateFaceSelection(elem, subElems);

                    flatElemTree.AddRange(subElems);

                }
            }

            foreach(IElement elem in flatElemTree)
            {
                if (elem.Nodes.Count < 4)
                {
                    Console.WriteLine("What???");
                }
            
            }
            // this.meshData.TheFaceSelections
            return flatElemTree;
        }

        /// <summary>
        /// updates the face selection objects in the mesh data model
        /// </summary>
        private void updateFaceSelection(IElement elem, List<IElement> subElems)
        {
            foreach (FaceSelection faceSelection in meshData.FaceSelections)
            {
                // if the current element is an element in a face selection then we want to
                // getnerate a list of it's sub faces containing it's sub elements and store these within the face selection.
                List<IElement> faceSelectElements = faceSelection.Faces.Select(f => f.Element).ToList();
                if (faceSelectElements.Contains(elem))
                {
                    // get a list of new faces for the face selection to update the constraint
                    // this has to be all of the loads or constraints
                    List<Face> newConstraintFaces = new List<Face>();
                    subElems.ForEach(sub => newConstraintFaces.Add(new Face(sub, 6)));

                    Face[] tempFaces = faceSelection.Faces.Where(f => f.Element == elem).ToArray();
                    if (tempFaces.Length > 1)
                    {
                        throw new Exception("There should never be two faces here!");
                    }
                    Face removeFace = tempFaces[0];
                    faceSelection.Faces.Remove(removeFace);

                    faceSelection.Faces.AddRange(newConstraintFaces);
                }
            }

        }
        /// <summary>
        /// for a set of elements extract all the nodes that make them up in order to repopulate the model file
        /// </summary>
        /// <returns>allNodes in model is a 1d List</returns>
        private List<Node> getAllNodes(List<IElement> flatElemTree)
        {
            List<Node> modelNodes = new List<Node>();

            foreach(IElement elem in flatElemTree)
            {
                foreach(Node node in elem.Nodes)
                {
                    modelNodes.Add(node);
                }
            }
            return modelNodes;
        }
    }
}
