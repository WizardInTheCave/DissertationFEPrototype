
using DissertationFEPrototype.FEModelUpdate.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DissertationFEPrototype.Optimisations.ILPRules
{
    class Edge
    {
        readonly double SAME_DISTANCE_TOLERANCE = 0.1;
        public enum EdgeType
        {
            importantLong, important, importantShort,
            notImportant, circuit, halfCircuit, quaterCircuit, shortForAHole,
            longForAHole, circuitHole, halfCircuitHole, quarterCircuitHole, noneSet
        };

        public enum BoundaryType
        {
            free, fixedOneSide, fixedTwoSides, fixedCompletely, noneSet
        };

        public enum LoadingType {
            notLoaded, oneSideLoaded, twoSidesLoaded, ContinuiousLoading, noneSet
        };

        EdgeType edgeType;
        LoadingType loadType;
        BoundaryType boundaryType;
        int id;
        List<Node> nodePath;
        int elementCount;
        double totalLength;

        public int ID { get { return this.id; } }
        public EdgeType GetEdgeType()
        {
            return edgeType;
        }
        public void SetEdgeType(EdgeType type)
        {
            this.edgeType = type;
        }

        public List<Node> NodePath {
            get { return this.nodePath; }
            set { this.nodePath = value; }
        }

        public BoundaryType GetBoundaryType()
        {
            return this.boundaryType;
        }
        public void SetBoundaryType(BoundaryType type){ this.boundaryType = type; }

        public LoadingType GetLoadType(){ return this.loadType; }
        public double TotalLength{ get{ return this.totalLength; } }

        public void SetLoadType(LoadingType type)
        {
            // Enum.TryParse(value, out this.loadType);
            this.loadType = type; 
        }
        public int ElementCount
        {
            get{
                return this.elementCount;
            }
            set{
                this.elementCount = value;
            }
        }

        /// <summary>
        /// is opposite and also has same length or form according to muggleton and Dolsak paper
        /// </summary>
        /// <param name="edgeB">Edge that we are comparing to the current edge</param>
        /// <returns>true if both are considered the same, false otherwise</returns>
        public bool isSameAs(Edge edgeB)
        {
            return this.isOpposite(edgeB) && (this.isSameLength(edgeB) || this.isSameForm(edgeB));
        }

        /// <summary>
        /// Get the total length along the entire node path, sum of euclidean distances between each pair in the chain.
        /// </summary>
        /// <param name="nodePath">A list of nodes which form the node path</param>
        /// <returns>total length</returns>
        public double computeTotalLength(List<Node> nodePath)
        {

            double totalLength = 0;

            for(int ii = 0; ii < nodePath.Count -1; ii++)
            {
                Node nodeA = nodePath[ii];
                Node nodeB = nodePath[ii + 1];
                totalLength += nodeA.distanceTo(nodeB);
            }
            return totalLength;
        }


        /// <summary>
        /// Check to see if the two edges can be considered opposite from one another
        /// </summary>
        /// <param name="edgeA">The first edge</param>
        /// <param name="edgeB">The second edge</param>
        /// <returns>true if the edges are opposite from one another
        /// false if the edges are not opposite from one another</returns>
        public bool isOpposite(Edge edgeB)
        {
            bool opposite = false;

            List<Node> aPath = this.NodePath;
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
                lengths.Add(nodeA.distanceTo(nodeB));
            }
            double oppositeDistance = lengths[0];
            opposite = lengths.All(len => Math.Abs(len - oppositeDistance) <= SAME_DISTANCE_TOLERANCE);
            return opposite;
        }



        /// <summary>
        /// Determine if two edges are neighbours to one another
        /// </summary>
        /// <param name="edgeA">The first edge</param>
        /// <param name="edgeB">The second edge</param>
        /// <returns>true if are neighbours,
        /// false if not neighbours</returns>
        public bool isNeighbour(Edge edgeB)
        {
            bool neighbour = true;
            return neighbour;
        }

        /// <summary>
        /// Return True if the Edges are the same length, false otherwise
        /// </summary>
        /// <param name="edgeB"></param>
        /// <returns></returns>
        private bool isSameLength(Edge edgeB)
        {
            return Math.Abs(this.TotalLength - edgeB.TotalLength) <= SAME_DISTANCE_TOLERANCE;
        }

        /// <summary>
        /// Return true if the Edges have the same form.
        /// </summary>
        /// <param name="edgeB"></param>
        /// <returns></returns>
        private bool isSameForm(Edge edgeB)
        {
            return this.edgeType == edgeB.edgeType;
        }

        /// <summary>
        /// Constructor for an Edge object, takes an id, its value for the different types and a node path which describes the edge.
        /// </summary>
        /// <param name="id">id of the edge so that it can be refferenced</param>
        /// <param name="type"> specific type of the edge, may have an interface with seperate classes later to implement this</param>
        /// <param name="nodePath">node which define the edge by specifying it's path</param>
        public Edge(int id, EdgeType edgeType, BoundaryType boundaryType, LoadingType loadingType, List<Node> nodePath)
        {

            //Enum.TryParse(edgeType, out this.edgeType);
            //Enum.TryParse(loadType, out this.loadType);
            // List<Quad4Elem> elements,

            this.id = id;
            this.edgeType = edgeType;
            this.boundaryType = boundaryType;
            this.nodePath = nodePath;
            this.loadType = loadingType;
            this.totalLength = computeTotalLength(nodePath);
        }
    }
}
