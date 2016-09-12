using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Model.Analysis
{
    class Face
    {
        int elementId;
        int faceId;
        public Face(int elementId, int faceId)
        {
            this.elementId = elementId;
            this.faceId = faceId;
        }
    }
}
