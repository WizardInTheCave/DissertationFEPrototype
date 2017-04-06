using DissertationFEPrototype;
using DissertationFEPrototype.FEModelUpdate.Model;
using DissertationFEPrototype.FEModelUpdate.Model.Structure;
using DissertationFEPrototype.MeshQualityMetrics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DisertationFEPrototype.FEModelUpdate
{


    /// <summary>
    /// Write out a summary of the Assessments for this experiment so we can compare variations in how effective the two methods are
    /// </summary>
    public class TableWriter
    {


        // private ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();

        
        private Tuple<List<int>, List<int>> getHeuristicCounts(List<MeshData> meshes)
        {

            List<int> hurCount = new List<int>();
            List<int> stressCount = new List<int>();

            int jj = 0;
            foreach (var mesh in meshes)
            {
                var elems = mesh.Elements;

                hurCount.Add(0);
                stressCount.Add(0);

                foreach (var elem in elems)
                {
                    Node[] nodes = elem.getNodes().ToArray();

                    var nOri = nodes.Select(n => n.NodeOrigin);

                    int hur = nOri.Where(x => x == Node.Origin.Heuristic).Count();
                    int stress = nOri.Where(x => x == Node.Origin.Stress).Count();
                    if (hur > stress)
                    {
                        hurCount[jj]++;
                    }
                    else
                    {
                        stressCount[jj]++;
                    }
                }
                jj++;
            }
            return Tuple.Create(hurCount, stressCount);
        }


        /// <summary>
        /// All other threads needs to write their respective experiment data to as a set of rows
        /// </summary>
        private void collectExperimentData(IntWrapper threadEditCount, List<List<string>> columns, Tuple<short, short> experimentVals, List<MeshQualityAssessment> meshAssessments, List<MeshData> meshes, List<double> times)
        {


            int threadId = threadEditCount.Value;

            // AppDomain.GetCurrentThreadId();
            // Thread.CurrentThread.ManagedThreadId;

            // will write the lines out in the end
            // List<List<string>> columns = new List<List<string>>();

            // set up the header

            // add the first row just once
            if (threadEditCount.Value == 0)
            {
                List<string> col = new List<string>(new string[77]);
                for (int mm = 0; mm < 70; mm++)
                {
                    col[mm] = ",";
                }     
                columns.Add(col);
            }
           

            const string colDelim = ", ";

            string expRef = experimentVals.Item1.ToString() + " " + experimentVals.Item2.ToString() + colDelim;

            var expRow = columns[0];

            expRow[threadId + 0] = expRef;
            expRow[threadId + 7] = expRef;
            expRow[threadId + 14] = expRef;
            expRow[threadId + 21] = expRef;
            expRow[threadId + 27] = expRef;
            expRow[threadId + 34] = expRef;
            //lines[threadId + 27] = meshAssessments[kk].ElemQualityScore.ToString();
            //lines[threadId + 34] = meshAssessments[kk].ElemCountScore.ToString();
            expRow[threadId + 42] = expRef;
            expRow[threadId + 49] = expRef;
            expRow[threadId + 56] = expRef;
            //expRow[threadId + 63] = expRef;
            //expRow[threadId + 70] = expRef;

            // headings already in the file need to be retained
            


            int a = meshAssessments.Count;
            int b = times.Count();
            int c = meshes.Count;

            var elemCounts = getHeuristicCounts(meshes);
            var hurCount = elemCounts.Item1;
            var stressCount = elemCounts.Item2;
         

            int max = new int[] { a, b, c }.Min();

            // for each row in the data for every experiment
            for (int kk = 0; kk < max; kk++)
            {
                List<string> col;
                if (threadEditCount.Value == 0)
                {
                    col = new List<string>(new string[77]);
                    for (int mm = 0; mm < 70; mm++)
                    {
                        col[mm] = ",";
                    }
                    columns.Add(col);
                }
                else
                {
                    // col already exists
                    col = columns[kk + 1];
                }
                col[threadId + 0] = times[kk].ToString() + colDelim;
                col[threadId + 7] = meshes[kk].Elements.Count.ToString() + colDelim;
                col[threadId + 14] = stressCount[kk].ToString() + colDelim;
                col[threadId + 21] = hurCount[kk].ToString() + colDelim;
                col[threadId + 27] = meshAssessments[kk].ElemQualMetrics.MaxCornerAngles.Average().ToString() + colDelim;
                col[threadId + 34] = meshAssessments[kk].OvarallQualityImprovement.ToString() + colDelim;
                //lines[threadId + 27] = meshAssessments[kk].ElemQualityScore.ToString();
                //lines[threadId + 34] = meshAssessments[kk].ElemCountScore.ToString();
                col[threadId + 42] = meshAssessments[kk].StressRefinementIncrease.ToString() + colDelim;
                col[threadId + 49] = meshAssessments[kk].HeuristicRefinementIncrease.ToString() + colDelim;
                col[threadId + 56] = meshAssessments[kk].ElemQualMetrics.MaxParrallelDevs.Average().ToString() + colDelim;
                // col[threadId + 63];
                // col[threadId + 70];
                // columns.Add(col);
            }
        }

            //int ii = 0;
            //foreach (double time in times)
            //{
            //    file.WriteLine(time.ToString());
            //    ii++;
            //}

            //jj = 0;
            //foreach (var mesh in meshes)
            //{

            //    file.WriteLine(mesh.Elements.Count);
            //    jj++;
            //}
            //foreach (int hur in hurCount)
            //{
            //    file.WriteLine(hur.ToString());
            //}

            //foreach (int stress in stressCount)
            //{
            //    file.WriteLine(stress.ToString());
            //}
            //foreach (MeshQualityAssessment assessment in meshAssessments)
            //{
            //    file.WriteLine(assessment.ElemQualityScore);
            //}
            //foreach (MeshQualityAssessment assessment in meshAssessments)
            //{
            //    file.WriteLine(assessment.ElemCountScore);
            //}
            //foreach (MeshQualityAssessment assessment in meshAssessments)
            //{
            //    file.WriteLine(assessment.ElemQualMetrics.MaxCornerAngles.Average());
            //}
            //foreach (MeshQualityAssessment assessment in meshAssessments)
            //{
            //    file.WriteLine(assessment.ElemQualMetrics.MaxParrallelDevs.Average());
            //}
            //foreach (MeshQualityAssessment assessment in meshAssessments)
            //{
            //    file.WriteLine(assessment.OvarallQualityImprovement);
            //}
            //foreach (MeshQualityAssessment assessment in meshAssessments)
            //{
            //    file.WriteLine(assessment.StressRefinementIncrease);
            //}
            //foreach (MeshQualityAssessment assessment in meshAssessments)
            //{
            //    file.WriteLine(assessment.HeuristicRefinementIncrease);
            //}


        // file.WriteLine("TimesForRuns");
        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine("ElemCount");

        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine();

        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine("StressElemCount");

        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine("Elem Qual Score");

        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine("Element Count Score");

        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine("Average Max Angle");

        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine("Average Max Parallel Devs");

        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine("Average OverallQualScore");

        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine("StressImprove");

        //file.WriteLine("");
        //file.WriteLine("");
        //file.WriteLine("HeuristicImprove");


        public void WriteData(IntWrapper threadEditCount, List<List<string>> resultCols, Tuple<short, short> experimentVals, string experimentFolder, List<MeshQualityAssessment> meshAssessments, List<MeshData> meshes, List<double> times)
        {
            // lock_.EnterWriteLock();
            //try
            //{

            // TextWriter sw = new StreamWriter(analysisFile);
            // File.AppendAllText(analysisFile, newData);

            // should update the data in all the columns across the threads.
            lock (threadEditCount)
            {
                collectExperimentData(threadEditCount, resultCols, experimentVals, meshAssessments, meshes, times);
                threadEditCount.Value++;
            }

            // }
            //finally
            //{
            //    lock_.ExitWriteLock();
            //}
        }
    } // eo class FileWriter   
}
