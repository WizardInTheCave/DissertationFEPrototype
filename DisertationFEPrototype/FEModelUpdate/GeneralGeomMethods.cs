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
        public static double distanceBetweenPoints(Node a, Node b)
        {
            return Math.Sqrt(Math.Pow((a.GetX - b.GetX), 2) + Math.Pow((a.GetY - b.GetY), 2) + Math.Pow((a.GetZ - b.GetZ), 2));
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

        /// <summary>
        /// this method will sort the match nodes into an order such that when an element is generated
        /// </summary>
        public static List<Node> sortMatchedNodes(List<Node> nodes)
        {
            List<Node> sortMatchedNodes = new List<Node>();

            Node currentNode = nodes[0];
            sortMatchedNodes.Add(currentNode);

            // List<Node> remainingNodes = nodes.Skip(1).ToList();

            while (sortMatchedNodes.Count < 4)
            {
                int currentIdx = nodes.IndexOf(currentNode);
                List<int> nums = new List<int>() { 0, 1, 2, 3 };
                nums.Remove(currentIdx);

                var nodeComp1 = nodes[nums[0]];
                var nodeComp2 = nodes[nums[1]];
                var nodeComp3 = nodes[nums[2]];

                if (GeneralGeomMethods.isCommonAxis(currentNode, nodeComp1) && !sortMatchedNodes.Contains(nodeComp1))
                {
                    //.Clone() as Node
                    sortMatchedNodes.Add(nodeComp1);
                    currentNode = nodeComp1;
                }
                else if (GeneralGeomMethods.isCommonAxis(currentNode, nodeComp2) && !sortMatchedNodes.Contains(nodeComp2))
                {
                    //.Clone() as Node
                    sortMatchedNodes.Add(nodeComp2);
                    currentNode = nodeComp2;
                }
                else if (GeneralGeomMethods.isCommonAxis(currentNode, nodeComp3) && !sortMatchedNodes.Contains(nodeComp3))
                {
                    // .Clone() as Node
                    sortMatchedNodes.Add(nodeComp3);
                    currentNode = nodeComp3;
                }
                else
                {
                    break;
                }
            }
            return sortMatchedNodes;
        }

        public static double dotProduct(Node a, Node b)
        {
            return a.GetX * b.GetX + a.GetY * b.GetY + a.GetZ * b.GetZ;
        }
        public static double dotProduct(Node a, Tuple<double, double, double> b)
        {
            return a.GetX * b.Item1 + a.GetY * b.Item2 + a.GetZ * b.Item3;
        }
        public static double dotProduct(double[] a, Tuple<double, double, double> b)
        {
            return a[0] * b.Item1 + a[1] * b.Item2 + a[2] * b.Item3;
        }

        public static Tuple<double, double, double> crossProduct(Node a, Node b)
        {
            double x = a.GetY * b.GetZ - a.GetZ * b.GetY;
            double y = a.GetZ * b.GetX - a.GetX * b.GetZ;
            double z = a.GetX * b.GetY - a.GetY * b.GetX;
            return new Tuple<double, double, double>(x, y, z);
        }
        public static Tuple<double, double, double> unitNormal(Node a, Node b, Node c)
        {

            double[,] array1 = new double[3, 3] { { 1, a.GetY, a.GetZ }, {1, b.GetY, b.GetZ }, {1, c.GetY, c.GetZ} };
            double[,] array2 = new double[3, 3] { { a.GetX, 1, a.GetZ }, { b.GetX, 1, b.GetZ }, { c.GetX, 1, c.GetZ } };
            double[,] array3 = new double[3, 3] { { a.GetX, a.GetY, 1 }, { b.GetX, a.GetY, 1 }, { c.GetX, a.GetY, 1 } };

            double x = matrixDeterminant(array1);
            double y = matrixDeterminant(array2);
            double z = matrixDeterminant(array3);

            double magnitude = Math.Pow((Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2)), 0.5);
            return new Tuple<double, double, double>(x/magnitude, y/magnitude, z/magnitude);

        }
        public static double matrixDeterminant(double[,] a)
        {
            return a[0, 0] * a[1, 1] * a[2, 2] +
                a[0, 1] * a[1, 2] * a[2, 0] +
                a[0, 2] * a[1, 0] * a[2, 1] -
                a[0, 2] * a[1, 1] * a[2, 0] -
                a[0, 1] * a[1, 0] * a[2, 2] -
                a[0, 0] * a[1, 2] * a[2, 1];
         
        }
    }
}
