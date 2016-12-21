using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Optimisations.ILPRules;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.Model.MeshDataStructure;
using DisertationFEPrototype.FEModelUpdate;

namespace DisertationFEPrototype.Optimisations.ILPRules
{


    /// <summary>
    /// The rules constructed in this section are the rules derived from
    /// work by Bojan Dolsak and Stephen Muggleton in their paper
    /// "The Application of Inductive Logic Programming to Finite Element Mesh Design"
    /// </summary>
    class RuleManager
    {

        readonly double SAME_DISTANCE_TOLERANCE = 0.1;
        List<Edge> edges;


        public List<Edge> GetEdges{
            get{
                return this.edges;
            }
        }

        public RuleManager(MeshData mesh)
        {

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
            
        }


        // rules can be found in Dolsak and muggleton paper "The Application of Inductive Logic Programming to Finite Element Mesh Design"
        // as referenced within the dissertation bibliography
        // rules are coded here in column ordering within the paper

        /// <summary>
        /// for all the edges in the model we want to see if there is a group of edges which meet
        /// this specific rule, if so we want to apply the rule 
        /// </summary>
        /// <param name="edges"></param>
        private void rule1(Edge edgeA, Edge edgeB)
        {
            int involvedEdges = 2;
            bool b1 = edgeA.GetEdgeType() == Edge.EdgeType.importantShort;
            bool b2 = isNeighbour(edgeA, edgeB);
            bool b3 = edgeB.GetBoundaryType() == Edge.BoundaryType.fixedCompletely;
            bool b4 = edgeB.GetLoadType() == Edge.LoadingType.notLoaded;
            if (b1
            && b2
            && b3
            && b4)
            {
                edgeA.ElementCount = involvedEdges;
                edgeB.ElementCount = involvedEdges;
            }
        }
        
        private void rule2(Edge edgeA, Edge edgeB)
        {
            int involvedEdges = 2;
            if (edgeA.GetEdgeType() == Edge.EdgeType.importantShort
            && isNeighbour(edgeA, edgeB)
            && edgeB.GetEdgeType() == Edge.EdgeType.notImportant
            && edgeB.GetLoadType() == Edge.LoadingType.notLoaded)
            {
                edgeA.ElementCount = involvedEdges;
                edgeB.ElementCount = involvedEdges;
            }  
        }
        private void rule3(Edge edgeA, Edge edgeB)
        {
            int involvedEdges = 2;
            if (edgeA.GetEdgeType() == Edge.EdgeType.importantShort
            && edgeA.GetBoundaryType() == Edge.BoundaryType.free
            && isNeighbour(edgeB, edgeA)
            && edgeB.GetLoadType() == Edge.LoadingType.notLoaded)
            {
                edgeA.ElementCount = involvedEdges;
                edgeB.ElementCount = involvedEdges;
            }
        }
        private void rule4(Edge edgeA)
        {
            int involvedEdges = 2;
            if (edgeA.GetBoundaryType() == Edge.BoundaryType.free
               && edgeA.GetLoadType() == Edge.LoadingType.oneSideLoaded
                )
            {
                edgeA.ElementCount = involvedEdges;
            }
        }
        //private void rule5(Edge edgeA, Edge edgeB)
        //{
        //    int involvedEdges = 2;
        //    if (isSame(edgeA, edgeB)
        //       && Ed)
        //    {

        //    }
        //}


        /// <summary>
        /// Determine if two edges are neighbours to one another
        /// </summary>
        /// <param name="edgeA">The first edge</param>
        /// <param name="edgeB">The second edge</param>
        /// <returns>true if are neighbours,
        /// false if not neighbours</returns>
        private bool isNeighbour(Edge edgeA, Edge edgeB)
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
        private bool isOpposite(Edge edgeA, Edge edgeB)
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
        private bool isSame(Edge edgeA, Edge edgeB)
        {
            return isOpposite(edgeA, edgeB) && (hasSameLength(edgeA, edgeB) || hasSameForm(edgeA, edgeB));
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
