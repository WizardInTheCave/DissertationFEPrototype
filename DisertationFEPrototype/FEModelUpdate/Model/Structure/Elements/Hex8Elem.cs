using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using DisertationFEPrototype.Model.Structure;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    class Hex8Elem : IElement
    {
        Expression expression;

        int? id;

        // id of nodes which form the element
        List<Node> nodes;
        List<Hex8Elem> childElements;
        Hex8Elem parentElement;

        double aspectRatio;
        double maxCornerAngle;
        double maxParallelDev;

        double area;
        // int flatPlaneIndex;

        readonly double LONGEST_EDGE_DEFAULT = 0.0;
        readonly double SHORTEST_EDGE_DEFAULT = 1000000.0;

        double longestEdge = 0.0;
        double shortestEdge = 1000000.0;

    
        public int? Id
        {
            get
            {
                return id;
            }
            set
            {
                this.id = value;
            }
        }

        public double Area { get { return this.area; } }

        public double AspectRatio { get { return this.aspectRatio; } }

        public double MaxCornerAngle { get { return this.maxCornerAngle; } }

        public double MaxParallelDev { get { return this.maxParallelDev; } }

        public List<Node> Nodes { get { return nodes; } }

        public List<IElement> Children
        {
            get
            {
                return childElements.Cast<IElement>().ToList();
            }

            set
            {
                // downcast to hex8 elems
                childElements = value.Select(x => (Hex8Elem)x).ToList();
            }
        }

        public Hex8Elem(int? id, List<Node> nodes)
        {
            this.id = id;
            this.nodes = sortNodes(nodes);
        }

        /// <summary>
        /// Sorts the list of nodes associated with this element into the correct arangement for LISA
        /// </summary>
        /// <returns></returns>
        public List<Node> sortNodes(List<Node> nodes)
        {
            List<Node> sortedNodes = new List<Node>();


            return sortedNodes;
        }

        public List<IElement> createChildElements(Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            List<IElement> childElements = new List<IElement>();

            Hex8Refinement refinement = new Hex8Refinement(nodes);

            return childElements;   
        }

        public List<Node> getDiagonalNodes(Node currentNode)
        {
            throw new NotImplementedException();
        }
    }
}
