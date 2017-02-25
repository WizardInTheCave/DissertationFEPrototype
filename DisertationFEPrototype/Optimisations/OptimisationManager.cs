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
using DisertationFEPrototype.FEModelUpdate;

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
         
        public OptimisationManager(MeshData meshData, List<NodeAnalysisData> analysisData, int iterationCount, short ILPRefineCount, short stressRefineCount, RuleManager ruleMan)
        {
            this.meshData = meshData;
            this.analysisData = analysisData;
            this.ruleManager = ruleMan;

            this.ILPRefineCount = ILPRefineCount;
            this.stressRefineCount = stressRefineCount; 
        }

        /// <summary>
        /// Main method for refining the mesh, this method has control over how to apply each of the two sub methods in order to get a better hybrid refinement
        /// </summary>
        /// <param name="meshQualityAssessments">Assessments of the mesh quality calculated using the metrics provided in Dittmers paper</param>
        public void refineMesh(List<MeshQualityAssessment> meshQualityAssessments)
        {
            // try a basic mesh refinement by creating more elements first
            List<IElement> elements = meshData.Elements;

            nodes = meshData.Nodes;


            // depending on how heavily we want to perform each type of meshing run that type of meshing
            for (int ii = 0; ii < stressRefineCount; ii++)
            {
                stressGradientDrivenRemesh(elements, analysisData);
            }
            // this.meshData.Elements = getNewElementList(elements);
            // now flatten the tree structure

            for (int jj = 0; jj < ILPRefineCount; jj++)
            {
                ILPEdgeDrivenRefinement();
                
            }
            this.meshData.Elements = getNewElementList(elements);

            //for (int jj = 0; jj < ii; jj++)
            //{




            // we have some derivatives for the straight lines we want to 
            // keep what we are currently doing if the derivative is equally negative, 
            // then apply an even more extreme version of what we were doing

            // otherwise one metric has improved, something is going ok but we did something to upset another variable
            // using an if statement for now but a deligate would ultimately be ideal for doing this.
            // later on need to do both but with some weighting

            //List<double> aspectRatios = meshQualityAssessment.ElemQualMetrics.AspectRatios;
            //List<double> maxCornerAngles = meshQualityAssessment.ElemQualMetrics.MaxCornerAngles;




            //List<Node> all_nodes = getAllNodes(flatElemTree);

            // update the ids on nodes which don't have ids

            // our mesh should now be refined
            var newMeshDataNodes = meshData.Nodes;

            var newmeshData = new MeshData();
            newmeshData.Elements = meshData.Elements;
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
        private void ILPEdgeDrivenRefinement()
        {
           // List<Edge> redefinedEdges = new List<Edge>();

            // the edges which had been assigned an amount for remeshing
            List<Edge> meshingEdges = this.ruleManager.Edges;

            // for every edge discovered remesh the elements along that edge.
            foreach (Edge edge in meshingEdges)
            {
                int elemCount = edge.ElementCount;

                List<Node> nodePath = edge.NodePath;

                // for each node on the path get its elements and mesh those
                List<IElement> allRemeshingElems = nodePath.SelectMany(np => meshData.findElems(np)).ToList();

               
                foreach (IElement elem in allRemeshingElems)
                {
                    List<IElement> children = elem.createChildElements(nodes);
                    // GeneralRefinementMethods.getNewQuadElements(elem, nodes);
                    elem.Children = children;
                        
                }
                    
               


                // edge
                // Edge newEdge = 
                // remesh(allRemeshingElems, ii, edge.ElementCount);
               // redefinedEdges.Add(newEdge);
            }
            // this.ruleManager.Edges = redefinedEdges;
        }


        /// <summary>
        /// for each edge that was detected remesh all the elements along that edge 
        /// and return a new edge where the node path has been updated so that on the next iteration more meshing of the edge
        /// based on the rules can be performed
        /// </summary>
        /// <param name="elements">Elements which are being remeshed, not going to do recursively now because adds unnecessary complexity</param>
        /// <returns>a new Edge for which the node path has been updated. </returns>
       
        // (List<IElement> elements, Edge edge
        //private void remesh(List<IElement> elements, int ii, int elemCount)
        //{
        //    if (ii < elemCount)
        //    {
        //        foreach (IElement elem in elements)
        //        {


        //            List<IElement> refined;
        //            refined = elem.createChildElements(nodes);

        //            elem.Children = refined;

        //            ii++;
        //            // mesh another level
        //            remesh(refined, ii, elemCount);
        //        }
        //    }
        //}
        

    
        /// <summary>
        /// we want to use the data from the previous analyis that we have to refine our mesh specificially in areas with high stress values
        /// </summary>
        /// <param name="elements">All the elements within the model</param>
        /// <param name="analysisData">Analysis data describing the stresses induced upon the model from the pervious iteration of the experiment</param>
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

                // this is the bottom of the tree
                if (elem.Children == null)
                {
                    
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
