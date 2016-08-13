using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.MeshDataStructure
{
    /// <summary>
    /// This class is a representation of
    /// </summary>
    class MeshData
    {
        List<Node> nodes;
        List<Element> elements;
        string lisaFile;

        public List<Node> GetNodes
        {
            get
            {
                return this.nodes;
            }
        }

        public List<Element> GetElements
        {
            get
            {
                return this.elements;
            }
        }
        public MeshData(string lisaFile, Node)
        {
            this.lisaFile = lisaFile;

        }
        /// <summary>
        /// For every pair of nodes add an extra node between the two of them
        /// </summary>
        /// <returns></returns>
        public void createMidpointNodes(List<Element> elements)
        {
            foreach (Element element in elements)
            {
                List<Node> elementNodes = element.GetNodes;

            }
        }
        private void createNodeMidpoints(List<Node> nodes)
        {


        }
    }
}
