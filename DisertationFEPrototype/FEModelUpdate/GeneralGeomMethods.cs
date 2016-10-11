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

        /// <summary>
        /// work out on what planes in 3d space a pair of nodes have in common 
        /// </summary>
        /// <param name="a">first node</param>
        /// <param name="b">second node</param>
        /// <returns>list of bools with pos 0 representing x plane, 1 representing y plane and z representing z plane</returns>
        public static bool[] whichPlanesCommon(Node a, Node b)
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
        public static bool isCommonAxis(Node firstNode, Node secondNode)
        {
            bool[] sameVals = GeneralGeomMethods.whichPlanesCommon(firstNode, secondNode);
            int commonAxisVals = sameVals.Count(b => b == true);
            return commonAxisVals == 2;
        }
        /// <summary>
        /// function override for if we have already computed same vals outside
        /// </summary>
        /// <param name="sameVals"></param>
        /// <returns></returns>
        public static bool isCommonAxis(bool[] sameVals)
        {
            int commonAxisVals = sameVals.Count(b => b == true);
            return commonAxisVals == 2;
        }
    }
}
