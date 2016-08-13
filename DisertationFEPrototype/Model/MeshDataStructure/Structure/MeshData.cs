using DisertationFEPrototype.Model.MeshDataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DisertationFEPrototype.Model
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
        public MeshData(List<Node> nodes, List<Element> elements)
        {
            this.nodes = nodes;
            this.elements = elements;
        }
    }
}
