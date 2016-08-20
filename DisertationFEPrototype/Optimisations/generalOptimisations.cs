using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.Model.MeshDataStructure;


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

            foreach (var elem in elements)
            {
                
                List<Element> children = QuadElementRefinement.newElements(elem);
                elem.Children = children;
            }
            // now flatten the tree structure
            List<Element> flatElemTree = getNewElementList(elements);
            List<Node> all_nodes = getAllNodes(flatElemTree);

            // our mesh should now be refined
            this.meshData = new MeshData();

            this.meshData.Elements = flatElemTree;
            this.meshData.Nodes = all_nodes;
        }

        int flatStructElemId = 0;
        /// <summary>
        /// Use this method to flatten the element tree to produce a single list of leaf nodes by recursing through the quad tree 
        /// </summary>
        /// <returns>all elements in model in a 1d list</returns>
        private List<Element> getNewElementList(List<Element> rootElements)
        {
            List<Element> flatElemTree = new List<Element>();

            foreach (Element elem in rootElements)
            {
                // this is the bottom of the tree
                if(elem.Children == null)
                {
                    elem.SetId = flatStructElemId;
                    flatElemTree.Add(elem);
                    flatStructElemId++;
                }
                // not at the bottom of the tree yet so continue to recurse until we hit leaf elements
                else
                {
                    flatElemTree.AddRange(getNewElementList(elem.Children));
                }
            }
            return flatElemTree;
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
