using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Model.Analysis
{
    class Force
    {
        string selection;
        double x;
        double y;
        double z;

        public Force(string selection, double x, double y, double z)
        {
            this.selection = selection;
            this.x = x;
            this.y = y;
            this.z = z;
        }

    }
}
