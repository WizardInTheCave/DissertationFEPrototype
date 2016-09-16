using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Model
{

    class NodeAnalysisData
    {
        int node;

        double x;
        double y;
        double z;

        double dispX;
        double dispY;
        double dispZ;

        double dispMag;

        public int NodeId
        {
            get
            {
                return this.node;
            }
        }

        public double DispMag
        {
            get
            {
                return this.dispMag;
            }
        }


        public NodeAnalysisData(int node, double x, double y, double z, 
            double dispX, double dispY, double dispZ, double dispMag)
        {
            this.node = node;

            this.x = x;
            this.y = y;
            this.z = z;

            this.dispX = dispX;
            this.dispY = dispY;
            this.dispZ = dispZ;

            this.dispMag = dispMag;
        }
    }
}
