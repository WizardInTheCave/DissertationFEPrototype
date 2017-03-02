
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.FEModelUpdate;
using DisertationFEPrototype.Model.Structure;

namespace DisertationFEPrototype.Optimisations.ILPRules
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
            //Enum.TryParse(value, out this.edgeType);
            this.edgeType = type;
        }
        public List<Node> NodePath { get { return this.nodePath; } }

        public BoundaryType GetBoundaryType()
        {
            return this.boundaryType;
        }
        public void SetBoundaryType(BoundaryType type){ this.boundaryType = type; }

        public LoadingType GetLoadType(){ return this.loadType; }
        public double TotalLength{ get{ return this.totalLength; } }

        public List<Node> GetNodePath() { return this.nodePath; }

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
        /// <param name="edgeA"></param>
        /// <param name="edgeB"></param>
        /// <returns></returns>
        public bool isSameAs(Edge edgeB)
        {
            return this.isOpposite(edgeB) && (this.isSameLength(edgeB) || this.isSameForm(edgeB));
        }

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


        private bool isSameLength(Edge edgeB)
        {
            return Math.Abs(this.TotalLength - edgeB.TotalLength) <= SAME_DISTANCE_TOLERANCE;
        }

        private bool isSameForm(Edge edgeB)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// 
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
