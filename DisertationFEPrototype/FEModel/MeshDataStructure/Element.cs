using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.MeshDataStructure
{
    class Element
    {
        int id;
        string shape;
        // id of nodes which form the element
        List<Node> nodes;

        public List<Node> GetNodes
        {
            get
            {
                return GetNodes;
            }
        }

        public Element(int id, string shape, List<Node> nodes)
        {
            this.id = id;
            this.shape = shape;
            this.nodes = nodes;
        }
    }
}
