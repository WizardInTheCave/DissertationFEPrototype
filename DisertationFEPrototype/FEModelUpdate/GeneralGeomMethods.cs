using DisertationFEPrototype.Model.MeshDataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.FEModelUpdate
{
    /// <summary>
    /// this class contains any geometry based methods which are used within different sections of the codebase
    /// </summary>
    class GeneralGeomMethods
    {

        public static bool[] commonPlaneData(Node a, Node b)
        {

            bool[] sameVals = new bool[3] { false, false, false };

            if (a.GetX == b.GetX)
            {
                sameVals[0] = true;

            }
            if (a.GetY == b.GetY)
            {
                sameVals[1] = true;

            }
            if (a.GetZ == b.GetZ)
            {
                sameVals[2] = true;
            }

            return sameVals;
        }
    }
}
