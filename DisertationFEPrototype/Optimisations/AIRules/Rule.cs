using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Optimisations.AIRules
{
    class Rule
    {
        Edge keyEdge;
        int elementCountAssignment;
        List<Edge> edges;
        List<EdgeRelation> relations;

        public Rule(Edge keyEdge, int elementCountAssignment, List<Edge> edges, List<EdgeRelation> relations)
        {
            this.keyEdge = keyEdge;
            this.elementCountAssignment = elementCountAssignment;
            this.edges = edges;
            this.relations = relations;
        }
        /// <summary>
        /// based on this rule want to update the number of edges that are associated with each element
        /// </summary>
        public void updateEdgeElemCounts()
        {
            // apply relations
            foreach(EdgeRelation relation in relations)
            {
                switch (relation.Relation)
                {
                    case EdgeRelation.Relations.Neighbour:

                    break;


                    case EdgeRelation.Relations.Opposite:
                        
                    break;


                    case EdgeRelation.Relations.Same:

                    break;
                }
            }

            foreach(Edge edge in this.edges)
            {
                // edge.


            }
        }
    }
}
