using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DisertationFEPrototype.FEModelUpdate.Model.Structure;
namespace Testing
{
    [TestClass]
    public class NodeTests
    {
        [TestMethod]
        public void CrossProduct()
        {
            //Node n1 = new Node(1, 1.0, 1.0, 1.0);
            //Node n2 = new Node(1, 1.0, 1.0, 1.0);
            //Tuple<double, double, double> n1Crossn2 = n1.crossProduct(n2);

        
            //var delta = Math.Abs(res - 12.0);
            //Assert.IsTrue();
        }


        [TestMethod]
        public void DistanceTo()
        {
            Node n1 = new Node(1, 1.0, 1.0, 1.0);
            Node n2 = new Node(1, 1.0, 1.0, 1.0);
            double n1ToN2 = n1.distanceTo(n2);

            var delta = Math.Abs(n1ToN2 - 0.0);
            Assert.IsTrue(delta < 0.01);


            // make up vals here
            //Node n3 = new Node(1, 1.0, 3.0, 1.0);
            //Node n4 = new Node(1, 1.0, 1.0, 1.0);
            //double n3ToN4 = n3.distanceTo(n4);

            //var delta = Math.Abs(n3ToN4 - 0.0);
            //Assert.IsTrue(delta < 0.01);

        }

        [TestMethod]
        public void Clone()
        {
            Node n1 = new Node(1, 1.0, 1.0, 1.0);
            Node n1Clone = (Node)n1.Clone();

           
            Assert.IsTrue(n1Clone.Id == 1);

            Assert.IsTrue(Math.Abs(n1Clone.GetX - 1.0) < 0.01);
            Assert.IsTrue(Math.Abs(n1Clone.GetY - 1.0) < 0.01);
            Assert.IsTrue(Math.Abs(n1Clone.GetZ - 1.0) < 0.01);
            // make up vals here
            //Node n3 = new Node(1, 1.0, 3.0, 1.0);
            //Node n4 = new Node(1, 1.0, 1.0, 1.0);
            //double n3ToN4 = n3.distanceTo(n4);

            //var delta = Math.Abs(n3ToN4 - 0.0);
            //Assert.IsTrue(delta < 0.01);
        }

        [TestMethod]
        public void DotProduct()
        {
            Node n1 = new Node(1, 1.0 , 2.0, 3.0);
            Node n2 = new Node(2, 4.0, -5.0, 6.0);


            double res = n1.dot(n2);
            // calc with tolerance
            var delta = Math.Abs(res - 12.0);
            Assert.IsTrue(delta < 0.01);


            Node n3 = new Node(3, 1.0, 2.0, 3.0);
            var n4 = new Tuple<double, double, double>(4.0, -5.0, 6.0);


            double res2 = n3.dot(n4);

            // calc with tolerance
            var delta2 = Math.Abs(res2 - 12.0);
            Assert.IsTrue(delta2 < 0.01);
  
        }




    }
}
