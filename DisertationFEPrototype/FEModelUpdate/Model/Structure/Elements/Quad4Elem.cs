using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype;
using DisertationFEPrototype.FEModelUpdate.Model.Structure;
using System.Collections;
using System.Linq.Expressions;

using DisertationFEPrototype.Optimisations;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    public class Quad4Elem : SquareBasedElem
    {

        Quad4QualMetricCalcs propCalcs;
       
        
        /// <summary>
        /// Given an element get a list of sub devided elements the sum of which forms that element
        /// </summary>
        /// <param name="elem">Quad4Elem we want to split into four sub elements using h-refinement</param>
        /// <param name="nodes">Lookup of all nodes in the current model</param>
        /// <returns>List of sub elements to replace the input element</returns>
        public override List<IElement> createChildElements(Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            
            List<IElement> newElements = new List<IElement>();

            var subNodeTup = createMidpointNodes(this.nodes.ToArray(), nodes);

            List<Node[]> elementEdgeTrios = subNodeTup.Item1;
            List<Node> midpointLineNodes = subNodeTup.Item2;

            // get the new center node which will be a corner for each of the four new elements
            Node centerNode = createCenterNode(midpointLineNodes, nodes);

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

                Quad4Elem elem = new Quad4Elem(null, orderedTrio);


                newElements.Add(elem);
            }

            return newElements;
        }
     
        /// <summary>
        /// This function basically just acts as a little interface for getting multiple
        /// </summary>
        /// <param name="queryNode">Node we want to find the diagonal pairing of</param>
        /// <returns>List containing the single diagonal node for a quad4 element</returns>
        public override List<Node> getDiagonalNodes(Node currentNode)
        {
            Node diagNode = this.getDiagonalNode(this.nodes.ToArray(), currentNode);
            return new List<Node>() { diagNode };
        }

        public Quad4Elem(int? id, List<Node> nodes)
        {
                

            this.Id = id;
            this.nodes = sortFace(nodes);
            this.parentElement = null;
            this.childElements = null;

            // make another object responsible for calculating the elements metrics
            propCalcs = new Quad4QualMetricCalcs(this);

            // all three of these methods use 

            Tuple<Node, Node>[] nodePairings = this.computeEdgePairingsForNode(nodes);

            maxCornerAngle = propCalcs.computeMaxCornerAngle(nodes);
            maxParallelDev = propCalcs.computeMaxparallelDev(nodePairings); 
 
            longestEdge = propCalcs.computeLongestEdge(nodePairings, SHORTEST_EDGE_DEFAULT);
            shortestEdge = propCalcs.computeShortestEdge(nodePairings, LONGEST_EDGE_DEFAULT);
            aspectRatio = propCalcs.computeAspectRatio(longestEdge, shortestEdge);

            area = computeFaceArea(nodes, longestEdge, shortestEdge);

            // propCalcs.computeArea(longestEdge, shortestEdge);

        }
    }
}
