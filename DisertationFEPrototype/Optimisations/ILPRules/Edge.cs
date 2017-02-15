
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

        private double computeTotalLength(List<Node> nodePath)
        {

            double totalLength = 0;

            for(int ii = 0; ii < nodePath.Count -1; ii++)
            {
                Node nodeA = nodePath[ii];
                Node nodeB = nodePath[ii + 1];
                totalLength += GeneralGeomMethods.distanceBetweenPoints(nodeA, nodeB);
            }
            return totalLength;
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
