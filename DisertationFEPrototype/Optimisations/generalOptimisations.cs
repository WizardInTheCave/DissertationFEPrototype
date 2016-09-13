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
        List<AnalysisData> analysisData;

        public MeshData GetUpdatedMesh
        {
            get
            {
                return this.meshData;
            }
        }
        public generalOptimisations(MeshData meshData, List<AnalysisData> analysisData)
        {
            this.meshData = meshData;
            this.analysisData = analysisData;
        }

        public void doubleNodeCount()
        {
            // try a basic mesh refinement by creating more elements first
            List<Element> elements = this.meshData.Elements;

            var nodes = this.meshData.Nodes;

            List<Element> dividedElements = new List<Element>();


            foreach (var elem in elements)
            {    
                List<Element> children = QuadElementRefinement.newElements(elem, nodes);
                elem.Children = children;
            }


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
