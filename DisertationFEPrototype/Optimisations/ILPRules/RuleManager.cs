using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Optimisations.ILPRules;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype.Model.Structure;

namespace DisertationFEPrototype.Optimisations.ILPRules
{

    /// <summary>
    /// The rules constructed in this section are the rules derived from
    /// work by Bojan Dolsak and Stephen Muggleton in their paper
    /// "The Application of Inductive Logic Programming to Finite Quad4Elem Mesh Design"
    /// </summary>
    class RuleManager
    {
        readonly double SAME_DISTANCE_TOLERANCE = 0.1;
        List<Edge> edges;

        public List<Edge> Edges {

            get{
                return this.edges;
            }
            set
            {
                this.edges = value;
            }
        }

        public int iterationCount;
        //  public int lookingFor = 3;
        // public List<string> fileLines = new List<string>();

        // int iterationCount
        public RuleManager(MeshData mesh, string localEdgesFile)
        {

            // we shouldn't identify new edges for each iteration, we should just use edgess that we have already identified but extend them
            // this.iterationCount = iterationCount;
            EdgeGenerator edgeIdentifier = new EdgeGenerator(mesh, localEdgesFile);
            edges = edgeIdentifier.Edges;

            // build a set of edges out of the mesh data
            // main rule loop to save computation time
            // do comparisons between all of the different edges in the model.
            foreach (Edge edge in edges)
            {


                rule4(edge);
                rule9(edge);
                rule10(edge);
                rule11(edge);
                foreach (Edge otherEdge in edges)
                {
                    if (edge.ID != otherEdge.ID)
                    {
                        // try tro apply each of these rules to the edgeA pair
                        rule1(edge, otherEdge);
                        rule2(edge, otherEdge);
                        rule3(edge, otherEdge);
                        rule5(edge, otherEdge);
                        rule7(edge, otherEdge);
                    }
                }
            }

            // fileLines.Add("PAUSE");
            // System.IO.File.WriteAllLines(@"D:\Documents\DissertationWork\models\ruleResults.txt", fileLines.ToArray());

        }


        // rules can be found in Dolsak and muggleton paper "The Application of Inductive Logic Programming to Finite Quad4Elem Mesh Design"
        // as referenced within the dissertation bibliography
        // rules are coded here in column ordering within the paper

        /// <summary>
        /// for all the edges in the model we want to see if there is a group of edges which meet
        /// this specific rule, if so we want to apply the rule 
        /// </summary>
        /// <param name="edges"></param>
        private void rule1(Edge edgeA, Edge edgeB)
        {
            const int INVOLVED_EDGES = 2;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.importantShort;
            bool b2 = edgeA.isNeighbour(edgeB);
            bool b3 = edgeB.GetBoundaryType() == Edge.BoundaryType.fixedCompletely;
            bool b4 = edgeB.GetLoadType() == Edge.LoadingType.notLoaded;
            if (b1
            && b2
            && b3
            && b4)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
                edgeB.ElementCount = INVOLVED_EDGES;
            }
        }

        private void rule2(Edge edgeA, Edge edgeB)
        {
            const int INVOLVED_EDGES = 2;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.importantShort;
            bool b2 = edgeA.isNeighbour(edgeB);
            bool b3 = edgeB.GetEdgeType() == Edge.EdgeType.notImportant;
            bool b4 = edgeB.GetLoadType() == Edge.LoadingType.notLoaded;

            if (b1 && b2 && b3 && b4)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
                edgeB.ElementCount = INVOLVED_EDGES;
            }
        }

        private void rule3(Edge edgeA, Edge edgeB)
        {
            const int INVOLVED_EDGES = 2;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.importantShort;
            bool b2 = edgeA.GetBoundaryType() == Edge.BoundaryType.free;
            bool b3 = edgeA.isNeighbour(edgeB);
            bool b4 = edgeB.GetLoadType() == Edge.LoadingType.notLoaded;

            if (b1 && b2 && b3 && b4)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
                edgeB.ElementCount = INVOLVED_EDGES;
            }
        }
        private void rule4(Edge edgeA)
        {
            const int INVOLVED_EDGES = 2;

            bool b1 = edgeA.GetBoundaryType() == Edge.BoundaryType.free;
            bool b2 = edgeA.GetLoadType() == Edge.LoadingType.oneSideLoaded;

            if (b1 && b2)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
            }
        }
        private void rule5(Edge edgeA, Edge edgeB)
        {

            const int INVOLVED_EDGES = 2;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.important;
            bool b2 = edgeA.isOpposite(edgeB);
            bool b3 = edgeB.GetEdgeType() == Edge.EdgeType.important;


            if(b1 && b2 && b3)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
            }
        }
        private void rule7(Edge edgeA, Edge edgeB)
        {

            const int INVOLVED_EDGES = 3;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.important;
            bool b2 = edgeA.GetLoadType() == Edge.LoadingType.notLoaded;
            bool b3 = edgeA.isNeighbour(edgeB);
            bool b4 = edgeB.GetEdgeType() == Edge.EdgeType.importantShort;

            if(b1 && b2 && b3 && b4)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
            }
        }

        private void rule8()
        {

        }
        
        private void rule9(Edge edgeA)
        {

            const int INVOLVED_EDGES = 7;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.important;
            bool b2 = edgeA.GetLoadType() == Edge.LoadingType.oneSideLoaded;


            if(b1 && b2)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
            }
        }

        private void rule10(Edge edgeA)
        {
            const int INVOLVED_EDGES = 8;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.circuit;

            if (b1)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
            }
        }

        private void rule11(Edge edgeA)
        {

            const int INVOLVED_EDGES = 9;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.importantLong;
            bool b2 = edgeA.GetBoundaryType() == Edge.BoundaryType.fixedOneSide;
            bool b3 = edgeA.GetLoadType() == Edge.LoadingType.notLoaded;

            if(b1 && b2 && b3)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
            }

        }


        private void rule12(Edge edgeA, Edge edgeB)
        {
            //bool b1 = edgeA.GetLoadType() == Edge.LoadingType.notLoaded;
            //bool b2 = isSameAs(edgeA, edgeC);

            //if(b1 && b2)
            //{

            //}
        }
       
        private void resetEdgeTypes(List<Edge> edges)
        {
            foreach (Edge edge in edges)
            {
                edge.SetEdgeType(Edge.EdgeType.noneSet);
                edge.SetBoundaryType(Edge.BoundaryType.noneSet);
                edge.SetLoadType(Edge.LoadingType.noneSet);
            }
        }

    }
}
