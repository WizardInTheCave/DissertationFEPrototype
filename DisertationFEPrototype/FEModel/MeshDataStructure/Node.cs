using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.FEModel.MeshDataStructure
{
    class Node
    {
        int id;
        double x;
        double y;
        double z;

        public int GetId
        {
            get
            {
                return this.id;
            }

        }

        public Node(int id, double x, double y, double z)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
