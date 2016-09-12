using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Model.Analysis
{
    class FaceSelection
    {
        string name;
        List<Face> faces;

        public FaceSelection(string name, List<Face> faces)
        {
            this.name = name;
            this.faces = faces;
        }
    }
}
