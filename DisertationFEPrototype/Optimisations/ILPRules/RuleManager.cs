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

        Rule r1;
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

            rule1(edges);
            
        }

        

        // for all the edges in the model we want to see if there is a group of edges which meet
        // this specific rule, if so we want to apply the rule 
        private void rule1(List<Edge> edges)
        {
            int involvedEdges = 2;

            // do comparisons between all of the different edges in the model.
            foreach(Edge edge in edges)
            {
                foreach (Edge otherEdge in edges)
                {
                    if (edge.ID != otherEdge.ID)
                    {
                        bool b1 = edge.GetEdgeType() == Edge.EdgeType.importantShort;
                        bool b2 = isNeighbour(edge, otherEdge);
                        bool b3 = otherEdge.GetBoundaryType() == Edge.BoundaryType.fixedCompletely;
                        bool b4 = otherEdge.GetLoadType() == Edge.LoadingType.notLoaded;
                        if (b1
                        && b2
                        && b3
                        && b4)
                        {
                            edge.ElementCount = involvedEdges;
                            otherEdge.ElementCount = involvedEdges;
                        }
                    }
                }
            }
 
            
        }
        private bool isNeighbour(Edge edgeA, Edge edgeB)
        {
            bool neighbour = true;


            return neighbour;
        }
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
