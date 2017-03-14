using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DisertationFEPrototype;

namespace Testing
{
    [TestClass]
    public class GeneralGeomTests
    {
        [TestMethod]
        public void DotProduct()
        {

            var vec1 = new double[3] { 1, 2, 3 };
            var vec2 = new Tuple<double, double, double>(4.0, -5.0, 6);
            double res = DisertationFEPrototype.FEModelUpdate.GeneralGeomMethods.dotProduct(vec1, vec2);
            // calc with tolerance
            var delta = Math.Abs(res - 12.0);
            Assert.IsTrue(delta < 0.01);

            //var vec1 = new double[3] { 1, 2, 3 };
            //var vec2 = new Tuple<double, double, double>(4.0, -5.0, 6);
            //double res = DisertationFEPrototype.FEModelUpdate.GeneralGeomMethods.dotProduct(vec1, vec2);
            //// calc with tolerance
            //var delta = Math.Abs(res - 12.0);

            //Assert.IsTrue(delta < 0.01);
        }

        /// <summary>
        /// Test for checking matrix determinant method, 
        /// Wolfam Alpha used to help with generating matricies 
        /// and calculating particular results
        /// </summary>
        [TestMethod]
        public void MatrixDeterminent()
        {

            var matrix = new double[3, 3] { { 9, 3, 5 }, { -6, -9, 7 }, { -1, -8, 1 } };
            double res = DisertationFEPrototype.FEModelUpdate.GeneralGeomMethods.matrixDeterminant(matrix);


            var delta = Math.Abs(res - 615.0);
            Assert.IsTrue(delta < 0.01);
        }

        public void UnitNormal()
        {
            // need to make nodes public in order to test this properly

            //    double res = DisertationFEPrototype.FEModelUpdate.GeneralGeomMethods.unitNormal()

            //    var delta = Math.Abs(res );
            //    Assert.IsTrue(delta < 0.01);
        }
    }
}
