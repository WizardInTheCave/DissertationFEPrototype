using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.Model.MeshDataStructure;
using DisertationFEPrototype.Model.Analysis;

namespace DisertationFEPrototype.Optimisations
{

    /// <summary>
    /// this class will is the entry point for the optimisation part of the project
    /// </summary>
    class generalOptimisations
    {
        MeshData meshData;
        List<NodeAnalysisData> analysisData;

        public MeshData GetUpdatedMesh
        {
            get
            {
                return this.meshData;
            }
        }
        public generalOptimisations(MeshData meshData, List<NodeAnalysisData> analysisData)
        {
            this.meshData = meshData;
            this.analysisData = analysisData;
        }

        public void doubleNodeCount()
        {
            // try a basic mesh refinement by creating more elements first
            List<Element> elements = this.meshData.Elements;

            Dictionary<Tuple<double, double, double>, Node> nodes = this.meshData.Nodes;

            var elem = elements.Select(x => x.Id);

            //if (elem.Contains(32))
            //{
            //    Console.WriteLine("WHAT???");
            //}

            basicAnalysisDrivenMesh(nodes, elements, analysisData);

            // now flatten the tree structure
            List<Element> flatElemTree = getNewElementList(elements);
            //List<Node> all_nodes = getAllNodes(flatElemTree);

            // update the ids on nodes which don't have ids

            // our mesh should now be refined
            var newMeshDataNodes = meshData.Nodes;

            var newmeshData = new MeshData();
            newmeshData.Elements = flatElemTree;
            newmeshData.Nodes = newMeshDataNodes;
            newmeshData.TheForce = meshData.TheForce;
            newmeshData.TheMaterial = meshData.TheMaterial;
            newmeshData.TheFaceSelections = meshData.TheFaceSelections;
            this.meshData = newmeshData;

        }

        /// <summary>
        /// we want to use the data from the previous analyis that we have to refine our mesh specificially in areas with high stress values
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="analysisData"></param>
        private void basicAnalysisDrivenMesh(Dictionary<Tuple<double, double, double>, Node> nodes, List<Element> elements, List<NodeAnalysisData> analysisData)
        {

            double remeshThreshold = determineRemeshThreshold(analysisData);

            foreach (var elem in elements)
            {
                List<Node> elemNodes = elem.GetNodes;
                List<int> elemNodesIds = elemNodes.Select(node => node.Id).ToList();

                // get the analysis data objecs for this particular element
                List<NodeAnalysisData> nodeAnalysisData = analysisData.Where(d => elemNodesIds.Contains(d.NodeId)).ToList();

                // get the average displacement for the element based on the nodal displacements
                double avgDispMag = nodeAnalysisData.Select(nad => nad.DispMag).Average();

                if (avgDispMag > remeshThreshold)
                {
                    Console.WriteLine("Remeshed elements: " + elem.Id.ToString());
                    List<Element> children = QuadElementRefinement.newElements(elem, nodes);
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
        private List<Element> getNewElementList(List<Element> rootElements)
        {
            List<Element> flatElemTree = new List<Element>();

            List<Face> myFaces = new List<Face>();

            foreach (Element elem in rootElements)
            {
                

                // this is the bottom of the tree
                if (elem.Children == null)
                {
                    if (elem.Id == 12)
                    {
                        Console.WriteLine("current id: " + elem.Id + "new id: " + flatStructElemId);
                    }
                    
                    elem.Id = flatStructElemId;
                    flatElemTree.Add(elem);

                    //updateFaceSelection(elem, new List<Element>() { elem });

                    flatStructElemId++;
                }
                // not at the bottom of the tree yet so continue to recurse until we hit leaf elements
                else
                {
                    // this element we have recursed through is in a face selection, 
                    // therefore all it's children should be under that same face selection
                    var subElems = getNewElementList(elem.Children);
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
        private void updateFaceSelection(Element elem, List<Element> subElems)
        {
            foreach (FaceSelection faceSelection in meshData.TheFaceSelections)
            {
                // if the current element is an element in a face selection then we want to
                // getnerate a list of it's sub faces containing it's sub elements and store these within the face selection.
                List<Element> faceSelectElements = faceSelection.Faces.Select(f => f.Element).ToList();
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
        private List<Node> getAllNodes(List<Element> flatElemTree)
        {
            List<Node> modelNodes = new List<Node>();

            foreach(Element elem in flatElemTree)
            {
                foreach(Node node in elem.GetNodes)
                {
                    modelNodes.Add(node);
                }
            }
            return modelNodes;
        }
    }
}
