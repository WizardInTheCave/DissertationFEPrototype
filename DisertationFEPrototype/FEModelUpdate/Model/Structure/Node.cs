using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Model.MeshDataStructure
{
    public class Node : ICloneable
    {
        int id;
        double x;
        double y;
        double z;

        public object Clone()
        {
            return new Node(this.id, this.x, this.y, this.z);
        }
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public double GetX
        {
            get
            {
                return this.x;
            }
        }

        public double GetY
        {
            get
            {
                return this.y;
            }
        }

        public double GetZ
        {
            get
            {
                return this.z;
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
