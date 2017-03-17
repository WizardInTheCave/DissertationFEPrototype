using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationFEPrototype.Model.Analysis
{
    public class FixSelection
    {
        FaceSelection selection;
        public FaceSelection Selection{
            get{
                return this.selection;
            }
        }

        /// <summary>
        /// This Class represents locations in the model where the structure is fixed, i.e. there is infiniate stiffness
        /// </summary>
        /// <param name="selection">The FaceSelection that we are designating as fixed</param>
        public FixSelection(FaceSelection selection)
        {
            this.selection = selection;
        }
    }
}
