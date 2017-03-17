using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationFEPrototype.Model
{

    public class NodeAnalysisData
    {
        int id;

        double x;
        double y;
        double z;

        double dispX;
        double dispY;
        double dispZ;

        double dispMag;

        public int Id { get { return this.id; } }

        public double DispMag { get { return this.dispMag; } }


        public NodeAnalysisData(int nodeId, double x, double y, double z, 
            double dispX, double dispY, double dispZ, double dispMag)
        {
            this.id = nodeId;

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
