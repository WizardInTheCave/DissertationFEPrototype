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

        public List<Edge> GetEdges { get { return this.edges; } }

        public int iterationCount;
        public int lookingFor = 3;
        public List<string> fileLines = new List<string>();

        public RuleManager(MeshData mesh, int iterationCount)
        {

            this.iterationCount = iterationCount;
            EdgeIdentifier edgeIdentifier = new EdgeIdentifier(mesh);
            edges = edgeIdentifier.Edges;

            // build a set of edges out of the mesh data
            // main rule loop to save computation time
            // do comparisons between all of the different edges in the model.
            foreach (Edge edge in edges)
            {
                rule4(edge);
                foreach (Edge otherEdge in edges)
                {
                    if (edge.ID != otherEdge.ID)
                    {
                        // try tro apply each of these rules to the edgeA pair
                        rule1(edge, otherEdge);
                        rule2(edge, otherEdge);
                        rule3(edge, otherEdge);
                    }
                }
            }

            fileLines.Add("PAUSE");
            System.IO.File.WriteAllLines(@"D:\Documents\DissertationWork\models\ruleResults.txt", fileLines.ToArray());

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
            bool b2 = areNeighbours(edgeA, edgeB);
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

            if (iterationCount == lookingFor)
            {
                fileLines.Add("Main edge is " + edgeA.ID + "Rule 1: " + b1 + " " + b2 + " " + b3 + " " + b4);
            }
        }

        private void rule2(Edge edgeA, Edge edgeB)
        {
            const int INVOLVED_EDGES = 2;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.importantShort;
            bool b2 = areNeighbours(edgeA, edgeB);
            bool b3 = edgeB.GetEdgeType() == Edge.EdgeType.notImportant;
            bool b4 = edgeB.GetLoadType() == Edge.LoadingType.notLoaded;

            if (b1 && b2 && b3 && b4)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
                edgeB.ElementCount = INVOLVED_EDGES;
            }
            if (iterationCount == lookingFor)
            {
                fileLines.Add("Main edge is " + edgeA.ID + "Rule 2: " + b1 + " " + b2 + " " + b3 + " " + b4);
            }
        }

        private void rule3(Edge edgeA, Edge edgeB)
        {
            const int INVOLVED_EDGES = 2;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.importantShort;
            bool b2 = edgeA.GetBoundaryType() == Edge.BoundaryType.free;
            bool b3 = areNeighbours(edgeB, edgeA);
            bool b4 = edgeB.GetLoadType() == Edge.LoadingType.notLoaded;

            if (b1 && b2 && b3 && b4)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
                edgeB.ElementCount = INVOLVED_EDGES;
            }
            if (iterationCount == lookingFor)
            {
                fileLines.Add("Main edge is " + edgeA.ID + "Rule 3: " + b1 + " " + b2 + " " + b3 + " " + b4);
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
            if (iterationCount == lookingFor)
            {
                fileLines.Add("Main edge is " + edgeA.ID + "Rule 4: " + b1 + " " + b2);
            }
        }
        private void rule5(Edge edgeA, Edge edgeB)
        {

            const int INVOLVED_EDGES = 2;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.important;
            bool b2 = areOpposite(edgeA, edgeB);
            bool b3 = edgeB.GetEdgeType() == Edge.EdgeType.important;


            if(b1 && b2 && b3)
            {
                edgeA.ElementCount = INVOLVED_EDGES;
            }
        }

        //private void rule6(Edge edgeA, Edge edgeB)
        //{
        //    bool b1 = areSame(edgeA, edgeC);
        //}

        private void rule7(Edge edgeA, Edge edgeB)
        {

            const int INVOLVED_EDGES = 3;

            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.important;
            bool b2 = edgeA.GetLoadType() == Edge.LoadingType.notLoaded;
            bool b3 = areNeighbours(edgeA, edgeB);
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
            //bool b2 = areSame(edgeA, edgeC);

            //if(b1 && b2)
            //{

            //}
        }


        /// <summary>
        /// Determine if two edges are neighbours to one another
        /// </summary>
        /// <param name="edgeA">The first edge</param>
        /// <param name="edgeB">The second edge</param>
        /// <returns>true if are neighbours,
        /// false if not neighbours</returns>
        private bool areNeighbours(Edge edgeA, Edge edgeB)
        {
            bool neighbour = true;
            return neighbour;
        }

        /// <summary>
        /// Check to see if the two edges can be considered opposite from one another
        /// </summary>
        /// <param name="edgeA">The first edge</param>
        /// <param name="edgeB">The second edge</param>
        /// <returns>true if the edges are opposite from one another
        /// false if the edges are not opposite from one another</returns>
        private bool areOpposite(Edge edgeA, Edge edgeB)
        {
            bool opposite = false;

            List<Node> aPath = edgeA.NodePath;
            List<Node> bPath = edgeB.NodePath;

            List<Node> checkingFromPath;
            List<Node> checkingToPath;

            if (aPath.Count <= bPath.Count)
            {
                checkingFromPath = aPath;
                checkingToPath = bPath;
            }
            else
            {
                checkingFromPath = bPath;
                checkingToPath = aPath;
            }
            // check that each node is opposite at least one node in the checking to path
            // the checking from and checking to concept exists for the following scenario
            // @---@---@
            // @---@

            List<double> lengths = new List<double>();
            
            for (int ii = 0; ii < checkingFromPath.Count - 1; ii++)
            {
                Node nodeA = checkingFromPath[ii];
                Node nodeB = checkingToPath[ii + 1];
                lengths.Add(GeneralGeomMethods.distanceBetweenPoints(nodeA, nodeB));
            }
            double oppositeDistance = lengths[0];
            opposite = lengths.All(len => Math.Abs(len - oppositeDistance) <= SAME_DISTANCE_TOLERANCE);
            return opposite;
        }

        private bool hasSameLength(Edge edgeA, Edge edgeB)
        {
           return Math.Abs(edgeA.TotalLength - edgeB.TotalLength) <= SAME_DISTANCE_TOLERANCE;
        }

        private bool hasSameForm(Edge edgeA, Edge edgeB)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// is opposite and also has same length or form according to muggleton and Dolsak paper
        /// </summary>
        /// <param name="edgeA"></param>
        /// <param name="edgeB"></param>
        /// <returns></returns>
        private bool areSame(Edge edgeA, Edge edgeB)
        {
            return areOpposite(edgeA, edgeB) && (hasSameLength(edgeA, edgeB) || hasSameForm(edgeA, edgeB));
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
