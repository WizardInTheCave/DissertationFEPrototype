using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype;
using DisertationFEPrototype.Model.Structure;
using System.Collections;
using System.Linq.Expressions;

using DisertationFEPrototype.Optimisations;
using DisertationFEPrototype.Model.Structure.Elements;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    public class Quad4Elem : IElement
    {
        // was int?
        int? id;
        // enum Shape { Quad4, Tri3};
        // string shape;


        // id of nodes which form the element
        List<Node> nodes;
        List<Quad4Elem> childElements;
        Quad4Elem parentElement;

        double aspectRatio;
        double maxCornerAngle;
        double maxParallelDev;


        readonly double LONGEST_EDGE_DEFAULT = 0.0;
        readonly double SHORTEST_EDGE_DEFAULT = 1000000.0;

        double longestEdge = 0.0;
        double shortestEdge = 1000000.0;

        double area;


        // int flatPlaneIndex;


        Quad4QualMetricCalcs propCalcs;


        // readonly string QUAD4 = "Quad4";
        // readonly string TRI3 = "Tri3";

        public double Area { get { return this.area; } }

        public double AspectRatio { get { return this.aspectRatio; } }

        public double MaxCornerAngle { get { return this.maxCornerAngle; } }

        public double MaxParallelDev { get { return this.maxParallelDev; } }


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
        public List<Node> Nodes { get { return this.nodes; } }


        public Quad4Elem GetParent { get { return this.parentElement; } }


        public List<IElement> Children
        {
            get{
                if (childElements != null)
                {
                    return childElements.Cast<IElement>().ToList();
                }
                else
                {
                    return null;
                }   
            }
            set
            {
                // downcast to quad4 elems
                childElements = value.Select(x => (Quad4Elem)x).ToList();
            }
        }

        public double LongestEdge { get { return longestEdge; } }

        public double ShortestEdge { get { return shortestEdge; } }

       
        /// <summary>
        /// This function basically just acts as a little interface for getting multiple
        /// </summary>
        /// <param name="queryNode">Node we want to find the diagonal pairing of</param>
        /// <returns>List containing the single diagonal node for a quad4 element</returns>
        public List<Node> getDiagonalNodes(Node queryNode){
            Node diagNode = GeneralRefinementMethods.getDiagonalNode(nodes.ToArray(), queryNode);
            return new List<Node>( ) { diagNode };  
        }

    
        /// <summary>
        /// Given an element get a list of sub devided elements the sum of which forms that element
        /// </summary>
        /// <param name="elem">Quad4Elem we want to split into four sub elements using h-refinement</param>
        /// <param name="nodes">Lookup of all nodes in the current model</param>
        /// <returns>List of sub elements to replace the input element</returns>
        public List<IElement> createChildElements(Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            List<IElement> newElements = new List<IElement>();

            var subNodeTup = GeneralRefinementMethods.createMidpointNodes(this.Nodes.ToArray(), nodes);

            List<Node[]> elementEdgeTrios = subNodeTup.Item1;
            List<Node> midpointLineNodes = subNodeTup.Item2;

            // get the new center node which will be a corner for each of the four new elements
            Node centerNode = GeneralRefinementMethods.createCenterNode(midpointLineNodes, nodes);

            /// <summary>
            /// Get four new interior quad4 elements for the current element
            /// </summary>
            /// <param name="elementEdgeTrios"> The current edge nodes which will each </param>
            /// <param name="centreNode">The new node which will be a corner for each of the new sub elements</param>
            /// <returns> The new elements which are the children of the original element </returns>

            foreach (Node[] trio in elementEdgeTrios)
            {
                // add the centre node to get the smaller element
                trio[2] = centerNode;
                List<Node> orderedTrio = trio.ToList();


                //if (trio.Any(x => x == null))
                //{
                //    Console.Write("WHAT???");
                //}

                Quad4Elem elem = new Quad4Elem(null, orderedTrio);
                

                newElements.Add(elem);
            }

            return newElements;
        }



        public Quad4Elem(int? id, List<Node> nodes)
        {
            

            this.id = id;
            this.nodes = GeneralRefinementMethods.sortFourNodes(nodes);
            this.parentElement = null;
            this.childElements = null;

            // make another object responsible for calculating the elements metrics
            propCalcs = new Quad4QualMetricCalcs();

            // all three of these methods use 

            Tuple<Node, Node>[] nodePairings = GeneralMetricCalcMethods.getEdgePairingsForNode(nodes);

            maxCornerAngle = propCalcs.computeMaxCornerAngle(nodes);
            maxParallelDev = propCalcs.computeMaxparallelDev(nodePairings); 
 
            longestEdge = propCalcs.computeLongestEdge(nodePairings, SHORTEST_EDGE_DEFAULT);
            shortestEdge = propCalcs.computeShortestEdge(nodePairings, LONGEST_EDGE_DEFAULT);
            aspectRatio = propCalcs.computeAspectRatio(longestEdge, shortestEdge);

            area = GeneralRefinementMethods.computeFaceArea(nodes, longestEdge, shortestEdge);

            // propCalcs.computeArea(longestEdge, shortestEdge);

        }
    }
}
