using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Optimisations.AIRules;

namespace DisertationFEPrototype.Optimisations.AIRules
{

    
    /// <summary>
    /// The rules constructed in this section are the rules derived from
    /// work by Bojan Dolsak and Stephen Muggleton in their paper
    /// "The Application of Inductive Logic Programming to Finite Element Mesh Design"
    /// </summary>
    class RuleManager
    {
        // mesh(a13, 1) - this means on 
        // edge a13 there is one finite element
        // 

        Rule r1;
        public Rule Rule1
        {
            get
            {
                return r1;
            }
        }

        public RuleManager(List<Edge> edges)
        {
            this.r1 = rule1(edges);
            
        } 
        private Rule rule1(List<Edge> edges)
        {
            //resetEdgeTypes(edges);
            
            var ruleEdges = edges.ToList();
            resetEdgeTypes(ruleEdges);
            ruleEdges[0].SetEdgeType(Edge.EdgeType.importantShort);
            var edgeRel = new AIRules.EdgeRelation(edges[0], edges[2], EdgeRelation.Relation.Neighbour);
            ruleEdges[1].SetBoundaryType(Edge.BoundaryType.fixedCompletely);
            ruleEdges[1].SetLoadType(Edge.LoadingType.noLoading);
            return new AIRules.Rule(ruleEdges, new List<EdgeRelation>() { edgeRel });
        }


        private void resetEdgeTypes(List<Edge> edges)
        {
            foreach (Edge edge in edges)
            {
                edge.SetEdgeType(Edge.EdgeType.NoneSet);
                edge.SetBoundaryType(Edge.BoundaryType.NoneSet);
                edge.SetLoadType(Edge.LoadingType.NoneSet);
            }
        }

    }
}
