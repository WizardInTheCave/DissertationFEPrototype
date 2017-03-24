using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DissertationFEPrototype.MeshQualityMetrics;
using DissertationFEPrototype.Model;
using System.Collections.Generic;
using DissertationFEPrototype.FEModelUpdate.Model;
using DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using DissertationFEPrototype.FEModelUpdate.Model.Structure;

namespace Testing
{
    [TestClass]
    public class QualityMetricTests
    {

        [TestMethod]
        public void AspectRatiosTest()
        {

            //int nodeId, double x, double y, double z,
            //double dispX, double dispY, double dispZ, double dispMag 
            var analysisList = new List<NodeAnalysisData>();

            MeshData meshData = new MeshData();


            Dictionary<Tuple<double, double, double>, Node> allNodes
                = new Dictionary<Tuple<double, double, double>, Node>();

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            List<Node> faceNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 1.0);
            var b = new Node(2, 1.0, 0.0, 1.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            allNodes.Add(new Tuple<double, double, double>(0.0, 0.0, 1.0), a);
            allNodes.Add(new Tuple<double, double, double>(1.0, 0.0, 1.0), b);
            allNodes.Add(new Tuple<double, double, double>(1.0, 1.0, 0.0), d);
            allNodes.Add(new Tuple<double, double, double>(0.0, 1.0, 0.0), c);

            faceNodes.Add(a);
            faceNodes.Add(b);
            faceNodes.Add(c);
            faceNodes.Add(d);

            analysisList.Add(new NodeAnalysisData(1, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0));
            analysisList.Add(new NodeAnalysisData(2, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0));
            analysisList.Add(new NodeAnalysisData(3, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0));
            analysisList.Add(new NodeAnalysisData(4, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0));

            var quadElem = new Quad4Elem(1, faceNodes);

            meshData.Nodes = allNodes;
            meshData.Elements = new List<IElement>() { quadElem };

            var assess = new MeshQualityAssessment(meshData, analysisList);

            assess.assessMesh();

            double aspectRatio = assess.ElemQualMetrics.AspectRatios[0];

            const double EXPECTED = 1.412;

            var delta = Math.Abs(aspectRatio - EXPECTED);
            Assert.IsTrue(delta < 0.01);
        }


        [TestMethod]
        public void MaxCornerAngleTests()
        {
            //int nodeId, double x, double y, double z,
            //double dispX, double dispY, double dispZ, double dispMag 
            var analysisList = new List<NodeAnalysisData>();

            MeshData meshData = new MeshData();


            Dictionary<Tuple<double, double, double>, Node> allNodes
                = new Dictionary<Tuple<double, double, double>, Node>();

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            List<Node> faceNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 1.0);
            var b = new Node(2, 1.0, 0.0, 1.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            allNodes.Add(new Tuple<double, double, double>(0.0, 0.0, 1.0), a);
            allNodes.Add(new Tuple<double, double, double>(1.0, 0.0, 1.0), b);
            allNodes.Add(new Tuple<double, double, double>(1.0, 1.0, 0.0), d);
            allNodes.Add(new Tuple<double, double, double>(0.0, 1.0, 0.0), c);

            faceNodes.Add(a);
            faceNodes.Add(b);
            faceNodes.Add(c);
            faceNodes.Add(d);

            analysisList.Add(new NodeAnalysisData(1, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0));
            analysisList.Add(new NodeAnalysisData(2, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0));
            analysisList.Add(new NodeAnalysisData(3, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0));
            analysisList.Add(new NodeAnalysisData(4, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0));

            var quadElem = new Quad4Elem(1, faceNodes);

            meshData.Nodes = allNodes;
            meshData.Elements = new List<IElement>() { quadElem };

            var assess = new MeshQualityAssessment(meshData, analysisList);

            assess.assessMesh();

            List<double> maxCornerAngles = assess.ElemQualMetrics.MaxCornerAngles;

            const double EXPECTED = 90.0;

            var delta = Math.Abs(maxCornerAngles[0] - EXPECTED);
            Assert.IsTrue(delta < 0.01);
        }


        [TestMethod]
        public void ElemCountScoreTest()
        {

            //int nodeId, double x, double y, double z,
            //double dispX, double dispY, double dispZ, double dispMag 
            var analysisList = new List<NodeAnalysisData>();

            MeshData meshData = new MeshData();
      
            //var a1 = new NodeAnalysisData();
            //var a2 = new NodeAnalysisData();
            //var a3 = new NodeAnalysisData();

            //analysisList.Add(a1);
            //analysisList.Add(a2);
            //analysisList.Add(a3);

            var assess = new MeshQualityAssessment(meshData, analysisList);

            assess.assessMesh();


            var countScore = assess.ElemCountScore;

            const double EXPECTED = 0.0;


            // calc with tolerance
            var delta2 = Math.Abs(countScore - EXPECTED);
            Assert.IsTrue(delta2 < 0.01);

        }


        [TestMethod]
        public void ElemQualityScoreTest()
        {

            //int nodeId, double x, double y, double z,
            //double dispX, double dispY, double dispZ, double dispMag 
            var analysisList = new List<NodeAnalysisData>();

            MeshData meshData = new MeshData();


            Dictionary<Tuple<double, double, double>, Node> allNodes
                = new Dictionary<Tuple<double, double, double>, Node>();

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            List<Node> faceNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 1.0);
            var b = new Node(2, 1.0, 0.0, 1.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            allNodes.Add(new Tuple<double, double, double>(0.0, 0.0, 1.0), a);
            allNodes.Add(new Tuple<double, double, double>(1.0, 0.0, 1.0), b);
            allNodes.Add(new Tuple<double, double, double>(1.0, 1.0, 0.0), d);
            allNodes.Add(new Tuple<double, double, double>(0.0, 1.0, 0.0), c);

            faceNodes.Add(a);
            faceNodes.Add(b);
            faceNodes.Add(c);
            faceNodes.Add(d);

            analysisList.Add(new NodeAnalysisData(1, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0));
            analysisList.Add(new NodeAnalysisData(2, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0));
            analysisList.Add(new NodeAnalysisData(3, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0));
            analysisList.Add(new NodeAnalysisData(4, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0));

            var quadElem = new Quad4Elem(1, faceNodes);

            meshData.Nodes = allNodes;
            meshData.Elements = new List<IElement>() { quadElem };

            var assess = new MeshQualityAssessment(meshData, analysisList);

            assess.assessMesh();

            double elemQualScore = assess.ElemQualityScore;

            const double EXPECTED = 90.0;

            var delta = Math.Abs(elemQualScore - EXPECTED);
          //  Assert.IsTrue(delta < 0.01);
        }


        [TestMethod]
        public void HeuristicQualityImprovementTest()
        {
            var analysisList = new List<NodeAnalysisData>();

            MeshData meshData = new MeshData();

            Dictionary<Tuple<double, double, double>, Node> allNodes
                = new Dictionary<Tuple<double, double, double>, Node>();

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            List<Node> faceNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 1.0);
            var b = new Node(2, 1.0, 0.0, 1.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            allNodes.Add(new Tuple<double, double, double>(0.0, 0.0, 1.0), a);
            allNodes.Add(new Tuple<double, double, double>(1.0, 0.0, 1.0), b);
            allNodes.Add(new Tuple<double, double, double>(1.0, 1.0, 0.0), d);
            allNodes.Add(new Tuple<double, double, double>(0.0, 1.0, 0.0), c);

            faceNodes.Add(a);
            faceNodes.Add(b);
            faceNodes.Add(c);
            faceNodes.Add(d);

            a.NodeOrigin = Node.Origin.Heuristic;
            b.NodeOrigin = Node.Origin.Heuristic;
            c.NodeOrigin = Node.Origin.Heuristic;
            d.NodeOrigin = Node.Origin.Heuristic;

            analysisList.Add(new NodeAnalysisData(1, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 2.0));
            analysisList.Add(new NodeAnalysisData(2, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 2.0));
            analysisList.Add(new NodeAnalysisData(3, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 3.0));
            analysisList.Add(new NodeAnalysisData(4, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 7.0));

            var quadElem = new Quad4Elem(1, faceNodes);
            
            meshData.Nodes = allNodes;
            meshData.Elements = new List<IElement>() { quadElem };

            var assess = new MeshQualityAssessment(meshData, analysisList);

            assess.assessMesh();

            double heuristicQualityScore = assess.HeuristicRefinementIncrease;

            const double EXPECTED = 3.5;

            var delta = Math.Abs(heuristicQualityScore - EXPECTED);
            Assert.IsTrue(delta < 0.01);
        }


        [TestMethod]
        public void StressQualityImprovementTest()
        {
            var analysisList = new List<NodeAnalysisData>();

            MeshData meshData = new MeshData();

            Dictionary<Tuple<double, double, double>, Node> allNodes
                = new Dictionary<Tuple<double, double, double>, Node>();

            // DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements.
            List<Node> faceNodes = new List<Node>();

            var a = new Node(1, 0.0, 0.0, 1.0);
            var b = new Node(2, 1.0, 0.0, 1.0);
            var d = new Node(3, 1.0, 1.0, 0.0);
            var c = new Node(4, 0.0, 1.0, 0.0);

            allNodes.Add(new Tuple<double, double, double>(0.0, 0.0, 1.0), a);
            allNodes.Add(new Tuple<double, double, double>(1.0, 0.0, 1.0), b);
            allNodes.Add(new Tuple<double, double, double>(1.0, 1.0, 0.0), d);
            allNodes.Add(new Tuple<double, double, double>(0.0, 1.0, 0.0), c);

            faceNodes.Add(a);
            faceNodes.Add(b);
            faceNodes.Add(c);
            faceNodes.Add(d);

            a.NodeOrigin = Node.Origin.Stress;
            b.NodeOrigin = Node.Origin.Stress;
            c.NodeOrigin = Node.Origin.Stress;
            d.NodeOrigin = Node.Origin.Stress;

            analysisList.Add(new NodeAnalysisData(1, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 3.0));
            analysisList.Add(new NodeAnalysisData(2, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 2.0));
            analysisList.Add(new NodeAnalysisData(3, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 8.0));
            analysisList.Add(new NodeAnalysisData(4, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 7.0));

            var quadElem = new Quad4Elem(1, faceNodes);

            meshData.Nodes = allNodes;
            meshData.Elements = new List<IElement>() { quadElem };

            var assess = new MeshQualityAssessment(meshData, analysisList);

            assess.assessMesh();

            double heuristicQualityScore = assess.StressRefinementIncrease;

            const double EXPECTED = 5.0;

            var delta = Math.Abs(heuristicQualityScore - EXPECTED);
            Assert.IsTrue(delta < 0.01);
        }
    }
}
