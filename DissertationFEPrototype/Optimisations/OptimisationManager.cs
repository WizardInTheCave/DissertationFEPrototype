using System;
using System.Collections.Generic;
using System.Linq;
using DissertationFEPrototype.Model;
using DissertationFEPrototype.Model.Analysis;
using DissertationFEPrototype.Optimisations.ILPRules;
using DissertationFEPrototype.MeshQualityMetrics;
using DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using DissertationFEPrototype.FEModelUpdate.Model.Structure;
using DissertationFEPrototype.FEModelUpdate.Model;

namespace DissertationFEPrototype.Optimisations
{

    /// <summary>
    /// this class will is the entry point for the optimisation part of the project
    /// </summary>
    class OptimisationManager
    {
        MeshData meshData;


        List<NodeAnalysisData> analysisData;
        RuleManager ruleManager;

        Dictionary<Tuple<double, double, double>, Node> allNodes;

        bool firstIteration = true;

        short ILPRefineCount = 1;
        short stressRefineCount = 1;

        int flatStructElemId = 1;


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

            allNodes = meshData.Nodes;

            // depending on how heavily we want to perform each type of meshing run that type of meshing
            for (int ii = 0; ii < stressRefineCount; ii++)
            {
                stressGradientDrivenRemesh(elements, analysisData);
                this.meshData.Elements = getNewElementList(elements);
                flatStructElemId = 1;
            }

            for (int jj = 0; jj < ILPRefineCount; jj++)
            {
                ILPEdgeDrivenRefinement();
                this.meshData.Elements = getNewElementList(elements);
                flatStructElemId = 1;
            }


            // now flatten the tree structure
            var newMeshDataNodes = meshData.Nodes;

            var newmeshData = new MeshData();
            newmeshData.Elements = meshData.Elements;
            newmeshData.Nodes = newMeshDataNodes;
            newmeshData.Forces = meshData.Forces;
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
          
            // the edges which had been assigned an amount for remeshing
            List<Edge> meshingEdges = this.ruleManager.Edges;

            // for every edge discovered remesh the elements along that edge.
            foreach (Edge edge in meshingEdges)
            {
                int elemCount = edge.ElementCount;

                List<Node> nodePath = edge.NodePath;

                // for each node on the path get its elements and mesh those
                List<IElement> allRemeshingElems = nodePath.SelectMany(np => meshData.findElems(np)).ToList();

                List<Node> newNodePath = new List<Node>();

                List<Node> allPathNodes = new List<Node>();
                List<IElement> allRefinedElems = new List<IElement>();

                // for each element in all the elements we are remeshing remesh it then remesh the child elements again until ii is
                foreach (IElement elem in allRemeshingElems)
                {
                    List<IElement> refined;

                    // get the allNodes for the element which sit directly on the edge
                    var pathNodes = edge.NodePath.Intersect(elem.getNodes()).ToArray();

                    refined = elem.createChildElements(allNodes);

                    foreach (IElement child in refined)
                    {
                        child.getNodes().ForEach(node => node.NodeOrigin = Node.Origin.Heuristic);
                    }

                    elem.setChildren(refined);
                    allRefinedElems.AddRange(refined);

                }
                relinkNodes(edge, allRefinedElems); 
            }
        }


        /// <summary>
        /// Reconnect the allNodes so that the method can be applied multiple times for subsequent iterations.
        /// </summary>
        /// <param name="edgeToUpdate">edge from previous iteration that needs to be updated to include the new allNodes just created</param>
        /// <param name="pathNodes">Nodes that currently form the edge</param>
        /// <param name="refined">Elements just created through the refinement process which need to now be included in the new path</param>
        private void relinkNodes(Edge edgeToUpdate, List<IElement> refined)
        {

            List<Node> newNodePath = new List<Node>();

            // assuming each element only has two allNodes on the path currently.

            // We want to add the new refined node that is closest to the other allNodes already in the path
            // var distances = new List<double>();

            var theNewNodes = refined.SelectMany(x => x.getNodes());
            HashSet<Node> refinedNodes = new HashSet<Node>(theNewNodes);

            Node[] pathNodes = edgeToUpdate.NodePath.ToArray();
            Node currentNode;

            // loop through each of the gaps between two allNodes
            for (int ii = 0; ii + 1 < pathNodes.Length; ii++)
            {
                var firstNode = pathNodes[ii];
                var secondNode = pathNodes[ii + 1];

                currentNode = firstNode;

                // start with two very high initial values
                List<double> distancesToSecond = new List<double>() {100000.0, 10000.0};

                // while the currentNode keeps getting closer to the second node and the search doesn't veer off
                while(distancesToSecond[distancesToSecond.Count - 1]  < distancesToSecond[distancesToSecond.Count - 2])
                {

                    // Get the distance between the current and all other allNodes
                    List<Tuple<Node, double>> comparisonsAgainstCurrent = refinedNodes
                    .Select(x => new Tuple<Node, double>(x, x.distanceTo(currentNode))).ToList();


                    List<Tuple<Node, double>> comparisonsAgainstSecond = refinedNodes
                       .Select(x => new Tuple<Node, double>(x, x.distanceTo(secondNode))).ToList();


                    // get the four closest to the first node, which of these is closes to the second
                    var fourClosestToCurrent = new List<Tuple<Node, double>>(comparisonsAgainstCurrent.OrderBy(x => x.Item2).Take(4));


                    List<Tuple<Node, double>> intersections = new List<Tuple<Node, double>>();

                    foreach(var closeToCurr in fourClosestToCurrent)
                    {
                        foreach(var compAgainstSec in comparisonsAgainstSecond)
                        {
                            if(closeToCurr.Item1.Id == closeToCurr.Item1.Id)
                            {
                                intersections.Add(closeToCurr);
                            }
                        }
                    }

                    var closestRoundCurrentToSecond = intersections.OrderBy(x => x.Item2).ToArray()[0];

                    currentNode = closestRoundCurrentToSecond.Item1;

                    // get rid of the node we have just added to the updated edge
                    refinedNodes.Remove(currentNode);
                    distancesToSecond.Add(currentNode.distanceTo(secondNode));
                }
            }

            newNodePath.Add(pathNodes[pathNodes.Length - 1]);

            edgeToUpdate.NodePath = newNodePath;
        }

        /// <summary>
        /// We want to use the data from the previous analyis that we have to refine our mesh specificially in areas with high stress values
        /// </summary>
        /// <param name="elements">All the elements within the model</param>
        /// <param name="analysisData">Analysis data describing the stresses induced upon the model from the pervious iteration of the experiment</param>
        private void stressGradientDrivenRemesh(List<IElement> elements, List<NodeAnalysisData> analysisData)
        {

            double remeshThreshold = determineRemeshThreshold(analysisData);

            // ruleRefinement();
            foreach (var elem in elements)
            {
                List<Node> elemNodes = elem.getNodes();
                List<int> elemNodesIds = elemNodes.Select(node => node.Id).ToList();

                // get the analysis data objecs for this particular element
                List<NodeAnalysisData> nodeAnalysisData = analysisData.Where(d => elemNodesIds.Contains(d.Id)).ToList();

                // get the average displacement for the element based on the nodal displacements
                double avgDispMag = nodeAnalysisData.Select(nad => nad.DispMag).Average();


                if (avgDispMag > remeshThreshold)
                {
                    Console.WriteLine("Remeshed elements: " + elem.getId().ToString());
                    List<IElement> children = elem.createChildElements(allNodes);

                    foreach (IElement child in children)
                    {
                        child.getNodes().ForEach(node => node.NodeOrigin = Node.Origin.Stress);
                    }

                    // GeneralRefinementMethods.getNewQuadElements(elem, nodes);
                    elem.setChildren(children);
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

            threshold = Percentile(allDisps.ToArray(), 0.94);            

            return threshold;
        }

        /// <summary>
        /// taken from here, needed to compare heursitics against stress refinement http://stackoverflow.com/questions/8137391/percentile-calculation
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="excelPercentile"></param>
        /// <returns></returns>
        public double Percentile(double[] sequence, double excelPercentile)
        {
            Array.Sort(sequence);
            int N = sequence.Length;
            double n = (N - 1) * excelPercentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1d) return sequence[0];
            else if (n == N) return sequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }

       
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
                if (elem.getChildren() == null)
                {
                    
                    elem.setId(flatStructElemId);
                    flatElemTree.Add(elem);

                    //updateFaceSelection(elem, new List<Quad4Elem>() { elem });

                    flatStructElemId++;
                }
                // not at the bottom of the tree yet so continue to recurse until we hit leaf elements
                else
                {
                    // this element we have recursed through is in a face selection, 
                    // therefore all it's children should be under that same face selection
                    List<IElement> subElems = getNewElementList(elem.getChildren());
                    updateFaceSelection(elem, subElems);

                    flatElemTree.AddRange(subElems);

                }
            }
            // ids need to start at 1
            //foreach(var flatElem in flatElemTree)
            //{
            //    int? newId = flatElem.getId() - flatElemTree[0].getId() + 1;
            //    flatElem.setId(newId);
            //}
         
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
                foreach(Node node in elem.getNodes())
                {
                    modelNodes.Add(node);
                }
            }
            return modelNodes;
        }
    }
}
