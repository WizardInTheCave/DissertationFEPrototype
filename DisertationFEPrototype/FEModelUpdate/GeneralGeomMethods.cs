using DisertationFEPrototype.Model.Structure;
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
        public static double dotProduct(double[] a, Tuple<double, double, double> b)
        {
            return a[0] * b.Item1 + a[1] * b.Item2 + a[2] * b.Item3;
        }


        /// <summary>
        /// Calculate the normal of a plane defined by three points, currently we are just using this to compute the normal of elements within the model
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <param name="c">Point C</param>
        /// <returns>a tuple representing a point in the x, y, z space which is one a distance of one away on the normal of the plane</returns>
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
        /// <summary>
        /// Compute the determinant of a matrix a
        /// </summary>
        /// <param name="a">matrix</param>
        /// <returns></returns>
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
