using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Optimisations.AIRules;
using DisertationFEPrototype.Model;
using DisertationFEPrototype.Model.MeshDataStructure;
using DisertationFEPrototype.FEModelUpdate;

namespace DisertationFEPrototype.Optimisations.AIRules
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

        public RuleManager(MeshData mesh)
        {
            // build a set of edges out of the mesh data
            List<Edge> edges = getEdges(mesh);

           

            rule1(edges);
            
        }

        /// <summary>
        /// using the mesh model build a set of edges that we can apply rules to and then map back into the mesh data domain
        /// based on what I can tell from the paper "Mesh generation expert system for engineering analyses with FEM", (the earlier paper by Bojan Dolsak)
        /// an edge is defined as anywhere on the model where the outside of the structure meets the inside of the structure that isn't a plane
        /// </summary>
        /// <param name="mesh">MeshData object</param>
        /// <returns>edges built out of the mesh data</returns>
        private List<Edge> getEdges(MeshData mesh)
        {
            List<Edge> modelEdges = new List<Edge>();

            int id;
            Edge.EdgeType edgeType;
            Edge.BoundaryType boundaryType;
            Edge.LoadingType loadingType;
            List<Node> nodePath;

            List<Node> edgeNodes = findEdgeStart(meshData);


            modelEdges.Add(new Edge(id, edgeType, boundaryType, loadingType, nodePath));

            throw new NotImplementedException();
        }

        /// <summary>
        /// given all the nodes in the model form a list of nodes which are a part of an edge
        /// </summary>
        /// <param name="nodes">all the nodes in the model, can be indexed via coordinates</param>
        /// <returns></returns>
        private List<Node> findEdgeStart(MeshData meshData)
        {
            // edge

            foreach (Element elem in meshData.Elements)
            {

                int plane = elem.FlatPlaneIndex;
                // now we know what plane the element is on we want to check if there is a node above xor below
                // and a node to the right xor left 
               
                foreach (Node node in elem.Nodes)
                {
                    List<Node> higherY = new List<Node>();
                    List<Node> lowerY = new List<Node>();

                    List<Node> higherX = new List<Node>();
                    List<Node> lowerX = new List<Node>();

                    List<Node> higherZ = new List<Node>();
                    List<Node> lowerZ = new List<Node>();

                    foreach (Node node2 in meshData.Nodes.Values)
                    {
                        // found node up or down of current position
                        if (node.Id != node2.Id && node.GetX == node2.GetX && node.GetZ == node2.GetZ)
                        {
                            if(node.GetY < node2.GetY)
                            {
                                higherY.Add(node2);
                            }
                            else if (node.GetY > node2.GetY)
                            {
                                lowerY.Add(node2);
                            }
                        }

                        else if (node.Id != node2.Id && node.GetY == node2.GetY && node.GetZ == node2.GetZ)
                        {
                            if (node.GetX < node2.GetX)
                            {
                                higherX.Add(node2);
                            }
                            else if (node.GetX > node2.GetX)
                            {
                                lowerY.Add(node2);
                            }
                        }

                        else if (node.Id != node2.Id && node.GetX == node2.GetX && node.GetY == node2.GetY)
                        {
                            if (node.GetZ < node2.GetZ)
                            {
                                higherZ.Add(node2);
                            }
                            else if (node.GetZ> node2.GetZ)
                            {
                                lowerZ.Add(node2);
                            }
                        }

                        if (
                            (higherX.Count == 0 && higherY.Count == 0)
                            || (lowerZ.Count == 0 && higherY.Count == 0)
                            ||
                            ||
                            ||
                            ||
                            
                            )

                    }
                }
            }
        }

        // for all the edges in the model we want to see if there is a group of edges which meet
        // this specific rule, if so we want to apply the rule 
        private void rule1(List<Edge> edges)
        {
            int involvedEdges = 2;

            foreach(Edge edge in edges)
            {
                foreach (Edge otherEdge in edges)
                {
                    if (edge.ID != otherEdge.ID)
                    {
                        if (edge.GetEdgeType() == Edge.EdgeType.importantShort
                        && isNeighbour(edge, otherEdge)
                        && otherEdge.GetBoundaryType() == Edge.BoundaryType.fixedCompletely
                        && otherEdge.GetLoadType() == Edge.LoadingType.notLoaded)
                        {
                            edge.ElementCount = involvedEdges;
                            otherEdge.ElementCount = involvedEdges;
                        }
                    }
                }
            }
            //resetEdgeTypes(edges);
            
            //List<Edge> ruleEdges = edges.ToList();
            ////resetEdgeTypes(ruleEdges);

            
            //ruleEdges[0].SetEdgeType(Edge.EdgeType.importantShort);
            //var edgeRel = new AIRules.EdgeRelation(edges[0], edges[2], EdgeRelation.Relations.Neighbour);
            //ruleEdges[1].SetBoundaryType(Edge.BoundaryType.fixedCompletely);
            //ruleEdges[1].SetLoadType(Edge.LoadingType.notLoaded);
            //return new AIRules.Rule(ruleEdges[0], 2, ruleEdges, new List<EdgeRelation>() { edgeRel });
            
        }
        private bool isNeighbour(Edge edgeA, Edge edgeB)
        {
            bool neighbour = false;


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
                edge.SetEdgeType(Edge.EdgeType.NoneSet);
                edge.SetBoundaryType(Edge.BoundaryType.NoneSet);
                edge.SetLoadType(Edge.LoadingType.NoneSet);
            }
        }

    }
}
