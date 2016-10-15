using DisertationFEPrototype.Model.MeshDataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Optimisations.AIRules
{
    class Edge
    {

        public enum EdgeType
        {
            importantLong, important, importantShort,
            notImportant, circuit, halfCircuit, quaterCircuit, shortForAHole,
            LongForAHole, CircuitHole, HalfCircuitHole, QuarterCircuitHole, NoneSet
        };

        public enum BoundaryType
        {
            free, fixedOneSide, fixedTwoSides, fixedCompletely, NoneSet
        };

        public enum LoadingType {
            noLoading, oneSideLoaded, twoSidesLoaded, ContinuiousLoading, NoneSet
        };

        EdgeType edgeType;
        LoadingType loadType;
        BoundaryType boundaryType;
        int id;
        List<Node> nodePath;

        public EdgeType GetEdgeType()
        {
            return edgeType;
        }

        public void SetEdgeType(EdgeType type)
        {
            //Enum.TryParse(value, out this.edgeType);
            this.edgeType = type;
        }

        public BoundaryType GetBoundaryType()
        {
            return this.boundaryType;
        }
        public void SetBoundaryType(BoundaryType type)
        {
            this.boundaryType = type;
        }

        public LoadingType GetLoadType()
        {
            return this.loadType;
        }

        public void SetLoadType(LoadingType type)
        {
            // Enum.TryParse(value, out this.loadType);
            this.loadType = type; 
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id of the edge so that it can be refferenced</param>
        /// <param name="type"> specific type of the edge, may have an interface with seperate classes later to implement this</param>
        /// <param name="nodePath">node which define the edge by specifying it's path</param>
        Edge(int id, EdgeType edgeType, BoundaryType boundaryType, LoadingType loadingType,  List<Node> nodePath)
        {
            //Enum.TryParse(edgeType, out this.edgeType);
            //Enum.TryParse(loadType, out this.loadType);
            // List<Element> elements,

            this.id = id;
            this.edgeType = edgeType;
            this.boundaryType = boundaryType;
            this.edgeType = edgeType;

            this.nodePath = nodePath;

        }
    }
}
