
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model.Analysis.MaterialProps;

namespace DisertationFEPrototype.Model.Analysis
{
    class Material
    {
        int id;
        string name;
        Geometric geometric;
        Mechanical mechanical;

        public Material(int id, string name, Geometric geometric, Mechanical mechanical)
        {
            this.id = id;
            this.name = name;
            this.geometric = geometric;
            this.mechanical = mechanical;
        }
    }
}
