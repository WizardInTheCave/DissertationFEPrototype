using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Optimisations.AIRules
{
    class EdgeRelation
    {
        Relation relation;
        Edge edgeA;
        Edge edgeB;
        public enum Relation {Neighbour, Opposite, Same}; 
        public EdgeRelation(Edge A, Edge B, Relation relation)
        {
            this.edgeA = A;
            this.edgeB = B;
            this.relation = relation;

            // Enum.TryParse(relation, out );
        }

     
    }
}
