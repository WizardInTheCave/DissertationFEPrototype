﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DissertationFEPrototype.FEModelUpdate.Model.Structure;
using DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using System.Collections.Generic;
using System.Linq;


namespace Testing
{
    [TestClass]
    public class SquareElementTests
    {
        [TestMethod]
        public void ComputeAspectRatio()
        {
            List<Node> elemNodes = new List<Node>();

            elemNodes.Add(new Node(1, 0.0, 0.0, 0.0));
            elemNodes.Add(new Node(2, 1.0, 0.0, 0.0));
            elemNodes.Add(new Node(3, 0.0, 1.0, 0.0));
            elemNodes.Add(new Node(4, 1.0, 1.0, 0.0));

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            var quadElem = new Quad4Elem(1, elemNodes);

            var longerEdge = 1.0;
            var shorterEdge = 1.0;

            var aspectRatio = quadElem.computeAspectRatio(longerEdge, shorterEdge);

            var delta = Math.Abs(aspectRatio - 1.0);
            Assert.IsTrue(delta < 0.01);
        }


        [TestMethod]
        public void ComputeDevOnEdgePair()
        {
            List<Node> elemNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 1.0);
            var b = new Node(2, 1.0, 0.0, 1.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            elemNodes.Add(a);
            elemNodes.Add(b);
            elemNodes.Add(c);
            elemNodes.Add(d);

            var quadElem = new Quad4Elem(1, elemNodes);
            
            Tuple<Node, Node> edgeA = new Tuple<Node, Node>(a, b);
            Tuple<Node, Node> edgeB = new Tuple<Node, Node>(c, d);

            var devOnEdgePair = quadElem.computeDevOnEdgePair(edgeA, edgeB);

            var EXPECTED = 0.0;
            var delta = Math.Abs(devOnEdgePair - EXPECTED);
            Assert.IsTrue(delta < 0.01);
        }

        // Test that the element correctly returns the set of edges that it is supposed to
        [TestMethod]
        public void ComputeEdgePairingsForNode()
        {

            List<Node> faceNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 1.0);
            var b = new Node(2, 1.0, 0.0, 1.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            faceNodes.Add(a);
            faceNodes.Add(b);
            faceNodes.Add(c);
            faceNodes.Add(d);

            var quadElem = new Quad4Elem(1, faceNodes);

            var edgePairs = quadElem.computeEdgePairingsForNode(faceNodes);
            

            Assert.IsTrue(edgePairs.Contains(new Tuple<Node, Node>(a, c)));
            Assert.IsTrue(edgePairs.Contains(new Tuple<Node, Node>(c, d)));
            Assert.IsTrue(edgePairs.Contains(new Tuple<Node, Node>(a, b)));
            Assert.IsTrue(edgePairs.Contains(new Tuple<Node, Node>(b, d)));

            // quadElem.computeEdgePairingsForNode(nodes2);

        }

        [TestMethod]
        public void ComputeFaceArea()
        {

            List<Node> faceNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 1.0);
            var b = new Node(2, 1.0, 0.0, 1.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            faceNodes.Add(a);
            faceNodes.Add(b);
            faceNodes.Add(c);
            faceNodes.Add(d);

            var quadElem = new Quad4Elem(1, faceNodes);

            double longestEdge = 2.0;
            double shortestEdge = 2.0;

            double faceArea = quadElem.computeFaceArea(faceNodes, longestEdge, shortestEdge);

            var delta = Math.Abs(faceArea - 4.0);
            Assert.IsTrue(delta < 0.01);
        }


        [TestMethod]
        public void LongestEdge()
        {
            List<Node> elemNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 0.0);
            var b = new Node(2, 1.0, 0.0, 0.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);


            elemNodes.Add(a);
            elemNodes.Add(b);
            elemNodes.Add(c);
            elemNodes.Add(d);

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            Quad4Elem quadElem = new Quad4Elem(1, elemNodes);

            Tuple<Node, Node>[] edges = new Tuple<Node, Node>[4];

            edges[0] = new Tuple<Node, Node>(a, b);
            edges[1] = new Tuple<Node, Node>(a, c);
            edges[2] = new Tuple<Node, Node>(b, d);
            edges[3] = new Tuple<Node, Node>(c, d);

            var longestEdge = quadElem.computeLongestEdge(edges, 0);

            var delta = Math.Abs(longestEdge - 1.0);
            Assert.IsTrue(delta < 0.01);
        }


        [TestMethod]
        public void ShortestEdge()
        {
            List<Node> elemNodes = new List<Node>();


            var a = new Node(1, 0.0, 0.0, 0.0);
            var b = new Node(2, 1.0, 0.0, 0.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            elemNodes.Add(a);
            elemNodes.Add(b);
            elemNodes.Add(c);
            elemNodes.Add(d);

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            Quad4Elem quadElem = new Quad4Elem(1, elemNodes);

            Tuple<Node, Node>[] edges = new Tuple<Node, Node>[4];

            edges[0] = new Tuple<Node, Node>(a, b);
            edges[1] = new Tuple<Node, Node>(a, c);
            edges[2] = new Tuple<Node, Node>(b, d);
            edges[3] = new Tuple<Node, Node>(c, d);

            var shortestEdge = quadElem.computeShortestEdge(edges, 1000000);

            var delta = Math.Abs(shortestEdge - 1.0);
            Assert.IsTrue(delta < 0.01);
        }



        //[TestMethod]
        //public void computeCornerAngleTest()
        //{
        //    List<Node> elemNodes = new List<Node>();

        //    var a = new Node(1, 0.0, 0.0, 0.0);
        //    var b = new Node(2, 1.0, 0.0, 0.0);
        //    var c = new Node(3, 0.0, 1.0, 0.0);
        //    var d = new Node(4, 1.0, 1.0, 0.0);

        //    elemNodes.Add(a);
        //    elemNodes.Add(b);
        //    elemNodes.Add(c);
        //    elemNodes.Add(d);

        //    // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
        //    Quad4Elem quadElem = new Quad4Elem(1, elemNodes);

        //    Tuple<Node, Node>[] edges = new Tuple<Node, Node>[4];

        //    edges[0] = new Tuple<Node, Node>(a, b);
        //    edges[1] = new Tuple<Node, Node>(a, c);
        //    edges[0] = new Tuple<Node, Node>(b, d);
        //    edges[1] = new Tuple<Node, Node>(c, d);

        //    var angle = quadElem.(elemNodes);

        //    var delta = Math.Abs(angle - 90.0);
        //    // Assert.IsTrue(delta < 0.01);
        //}



        [TestMethod]
        public void ComputeMaxCornerAngleTest()
        {
            List<Node> elemNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 0.0);
            var b = new Node(2, 1.0, 0.0, 0.0);
            var c = new Node(3, 0.0, 1.0, 0.0);
            var d = new Node(4, 1.0, 1.0, 0.0);

            elemNodes.Add(a);
            elemNodes.Add(b);
            elemNodes.Add(c);
            elemNodes.Add(d);

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            Quad4Elem quadElem = new Quad4Elem(1, elemNodes);

            Tuple<Node, Node>[] edges = new Tuple<Node, Node>[4];

            edges[0] = new Tuple<Node, Node>(a, b);
            edges[1] = new Tuple<Node, Node>(a, c);
            edges[0] = new Tuple<Node, Node>(b, d);
            edges[1] = new Tuple<Node, Node>(c, d);

            var angle = quadElem.computeMaxCornerAngle(elemNodes);

            var delta = Math.Abs(angle - 90.0);
            // Assert.IsTrue(delta < 0.01);
        }


        [TestMethod]
        public void ComputeMaxParallelDev()
        {
            List<Node> elemNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 0.0);
            var b = new Node(2, 1.0, 0.0, 0.0);
            var c = new Node(3, 0.0, 1.0, 0.0);
            var d = new Node(4, 1.0, 1.0, 0.0);

            elemNodes.Add(a);
            elemNodes.Add(b);
            elemNodes.Add(c);
            elemNodes.Add(d);

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            Quad4Elem quadElem = new Quad4Elem(1, elemNodes);

            Tuple<Node, Node>[] edges = new Tuple<Node, Node>[4];

            edges[0] = new Tuple<Node, Node>(a, b);
            edges[1] = new Tuple<Node, Node>(a, c);
            edges[2] = new Tuple<Node, Node>(b, d);
            edges[3] = new Tuple<Node, Node>(c, d);

            var maxParallelDev = quadElem.computeMaxParallelDev(edges);

            var delta = Math.Abs(maxParallelDev - 0.0);
            Assert.IsTrue(delta < 0.01);
        }



        [TestMethod]
        public void ComputeNonDiagAdjacentNodesTest()
        {
            List<Node> elemNodes = new List<Node>();


            var a = new Node(1, 0.0, 0.0, 1.0);
            var b = new Node(2, 1.0, 0.0, 1.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            elemNodes.Add(a);
            elemNodes.Add(b);
            elemNodes.Add(c);
            elemNodes.Add(d);

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            Quad4Elem quadElem = new Quad4Elem(1, elemNodes);

            Tuple<Node, Node>[] edges = new Tuple<Node, Node>[4];

            edges[0] = new Tuple<Node, Node>(a, b);
            edges[1] = new Tuple<Node, Node>(a, c);
            edges[0] = new Tuple<Node, Node>(b, d);
            edges[1] = new Tuple<Node, Node>(c, d);

            Node[] nonDiagAdjacent = quadElem.computeNonDiagAdjacentNodes(c, elemNodes);



            var correctResult = new List<Node>() { a, d };

            foreach (Node nonDiag in nonDiagAdjacent)
            {
                Assert.IsTrue(correctResult.Contains(nonDiag));
            }
        }
    }
}
