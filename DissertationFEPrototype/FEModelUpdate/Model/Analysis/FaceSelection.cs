using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationFEPrototype.Model.Analysis
{
    public class FaceSelection
    {
        string name;
        List<Face> faces;

        public List<Face> Faces{
            get
            { 
                return faces;
            }
            set
            {
                faces = value;
            }
        }
        public string GetName
        {
            get
            {
                return name;
            }
        }

        public FaceSelection(string name, List<Face> faces)
        {
            this.name = name;
            this.faces = faces;
        }
    }
}
