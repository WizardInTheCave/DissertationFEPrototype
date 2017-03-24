using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using System.Collections.Generic;

namespace Testing
{
    [TestClass]
    public class ConvexHullTests
    {
        [TestMethod]
        public void ConvexHullTest()
        {
            var a = new List<ConvexHullPoint>();
            var convexHullOrderedPoints = ConvexHull.ComputeConvexHull(a);

            Assert.IsTrue(convexHullOrderedPoints == new List<ConvexHullPoint>());
        }
    }
}
