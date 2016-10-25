using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Optimisations.AIRules
{
    class EdgeRelation
    {
        Relations relation;
        Edge edgeA;
        Edge edgeB;
        public enum Relations {Neighbour, Opposite, Same}; 

        public Relations Relation{
            get{
                return this.relation;
            }
        }
        public EdgeRelation(Edge A, Edge B, Relations relation)
        {
            this.edgeA = A;
            this.edgeB = B;
            this.relation = relation;

            // Enum.TryParse(relation, out );
        }

     
    }
}
