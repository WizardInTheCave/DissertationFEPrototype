using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationFEPrototype.Model.Analysis
{
    public class Force
    {
        string selection;
        double x;
        double y;
        double z;

        public string Selection { get {  return this.selection; }}
        public double X { get  {  return this.x;  } }

        public double Y { get  {  return this.y;  }}

        public double Z { get { return this.z;  }}

        public Force(string selection, double x, double y, double z)
        {
            this.selection = selection;
            this.x = x;
            this.y = y;
            this.z = z;
        }

    }
}
