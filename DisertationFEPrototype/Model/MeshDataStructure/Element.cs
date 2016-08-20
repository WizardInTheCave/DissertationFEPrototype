using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Model.MeshDataStructure
{
    public class Element
    {
        int? id;
        string shape;
        // id of nodes which form the element
        List<Node> nodes;
        List<Element> childElements;
        Element parentElement;

        public string GetShape
        {
            get
            {
                return this.shape;
            }
        }
        public int? Id{
            get
            {
                return id;
            }
            set{
                this.id = value;
            }
        }
        public List<Node> GetNodes
        {
            get
            {
                return this.nodes;
            }
        }
        public Element GetParent
        {
            get
            {
                return this.parentElement;
            }

        }
        public List<Element> Children
        {
            get
            {
                return this.childElements;
            }
            set
            {
                this.childElements = value;
            }
        }
       

        public Element(int? id, string shape, List<Node> nodes)
        {
            this.id = id;
            this.shape = shape;
            this.nodes = nodes;
            this.parentElement = null;
            this.childElements = null;
        }
    }
}
