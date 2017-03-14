using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DisertationFEPrototype.MeshQualityMetrics;
using DisertationFEPrototype.Model;
using System.Collections.Generic;
using DisertationFEPrototype.FEModelUpdate.Model;

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

            //var a1 = new NodeAnalysisData();
            //var a2 = new NodeAnalysisData();
            //var a3 = new NodeAnalysisData();

            //analysisList.Add(a1);
            //analysisList.Add(a2);
            //analysisList.Add(a3);

            var assess = new MeshQualityAssessment(meshData, analysisList);

            assess.assessMesh();

            List<double> countScore = assess.ElemQualMetrics.AspectRatios;

            const double EXPECTED = 0.0;

            // calc with tolerance
            // var delta2 = Math.Abs(countScore - EXPECTED);
            // Assert.IsTrue(delta2 < 0.01);
        }


        [TestMethod]
        public void MaxCornerAngleTests()
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

            List<double> countScore = assess.ElemQualMetrics.MaxCornerAngles;

            const double EXPECTED = 0.0;

            // calc with tolerance
            // var delta2 = Math.Abs(countScore - EXPECTED);
            // Assert.IsTrue(delta2 < 0.01);
        }


        [TestMethod]
        public void MaxParallelDevTests()
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

            List<double> countScore = assess.ElemQualMetrics.MaxParrallelDevs;

            const double EXPECTED = 0.0;

            // calc with tolerance
            // var delta2 = Math.Abs(countScore - EXPECTED);
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

            //var a1 = new NodeAnalysisData();
            //var a2 = new NodeAnalysisData();
            //var a3 = new NodeAnalysisData();

            //analysisList.Add(a1);
            //analysisList.Add(a2);
            //analysisList.Add(a3);

            var assess = new MeshQualityAssessment(meshData, analysisList);

            assess.assessMesh();
            
            var countScore = assess.ElemQualityScore;

            const double EXPECTED = 0.0;


            // calc with tolerance
            var delta2 = Math.Abs(countScore - EXPECTED);
            Assert.IsTrue(delta2 < 0.01);
        }

        [TestMethod]
        public void HeuristicQualityScoreTest()
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

            var countScore = assess.HeuristuicQualityScore;

            const double EXPECTED = 0.0;

            // calc with tolerance
            var delta2 = Math.Abs(countScore - EXPECTED);
            Assert.IsTrue(delta2 < 0.01);
        }
    }
}
