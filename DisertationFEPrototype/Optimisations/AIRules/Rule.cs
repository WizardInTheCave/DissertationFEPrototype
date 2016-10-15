using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Optimisations.AIRules
{
    class Rule
    {
        List<Edge> edges;
        List<EdgeRelation> relations;

        public Rule(List<Edge> edges, List<EdgeRelation> relations)
        {
            this.edges = edges;
            this.relations = relations;
        }
    }
}
