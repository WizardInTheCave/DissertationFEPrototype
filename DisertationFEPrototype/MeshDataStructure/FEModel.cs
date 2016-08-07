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
    class FEModel
    {
        List<Node> modelNodes;
        List<Element> modelElements;

        public List<Node> GetModelNodes
        {
            get{
                return modelNodes;
            }
        }
        public FEModel()
        {

        }

        /// <summary>
        /// For every pair of nodes add an extra node between the two of them
        /// </summary>
        /// <returns></returns>
        public void createMidpointNodes(List<Element> elements)
        {
            foreach(Element element in elements){
                List<Node> elementNodes = element.GetNodes;

            }
        }
        private void createNodeMidpoints(List<Node> nodes)
        {


        }
    }
}
