﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Optimisations.ILPRules
{
    class JsonEdge
    {
        public int Id { get; set; }
        public string edgeType { get; set; }
        public string loadType { get; set; }
        public string boundaryType { get; set; }
        public List<int> nodePath { get; set; }


        //public enum LoadingType
        //{
        //    notLoaded, oneSideLoaded, twoSidesLoaded, ContinuiousLoading, noneSet
        //};

        public Edge.LoadingType  getConvertedLoadingType()
        {
            Edge.LoadingType returnType = Edge.LoadingType.noneSet;

            switch (this.loadType.ToLower())
            {
                case ("nodloaded"):
                    returnType = Edge.LoadingType.notLoaded;
                    break;

                case ("onesideloaded"):
                    returnType = Edge.LoadingType.oneSideLoaded;
                    break;

                case ("twosidesloaded"):
                    returnType = Edge.LoadingType.twoSidesLoaded;
                    break;

                case ("continuiousloading"):
                    returnType = Edge.LoadingType.ContinuiousLoading;
                    break;
            }
            return returnType;
        }
        public Edge.BoundaryType getConvertedBoundaryType()
        {
            Edge.BoundaryType returnType = Edge.BoundaryType.noneSet;

            switch (boundaryType.ToLower())
            {
                case ("fixedCompletely"):
                    returnType = Edge.BoundaryType.fixedCompletely;
                    break;

                case ("fixedOneSide"):
                    returnType = Edge.BoundaryType.fixedOneSide;
                    break;

                case ("fixedTwoSides"):
                    returnType = Edge.BoundaryType.fixedTwoSides;
                    break;

                case ("free"):
                    returnType = Edge.BoundaryType.free;
                    break;
            }
            return returnType;
        }

        public Edge.EdgeType getConvertedEdgeType()
        {
            Edge.EdgeType returnType = Edge.EdgeType.noneSet;

            switch (this.edgeType.ToLower())
            {
                case ("circuit"):
                    returnType = Edge.EdgeType.circuit;
                    break;

                case ("circuitHole"):
                    returnType = Edge.EdgeType.circuitHole;
                    break;

                case ("halfCircuit"):
                    returnType = Edge.EdgeType.halfCircuit;
                    break;

                case ("halfCircuitHole"):
                    returnType = Edge.EdgeType.halfCircuitHole;
                    break;

                case ("important"):
                    returnType = Edge.EdgeType.important;
                    break;

                case ("importantLong"):
                    returnType = Edge.EdgeType.importantLong;
                    break;

                case ("importantShort"):
                    returnType = Edge.EdgeType.importantShort;
                    break;

                case ("longForAHole"):
                    returnType = Edge.EdgeType.longForAHole;
                    break;

                case ("notImportant"):
                    returnType = Edge.EdgeType.notImportant;
                    break;

                case ("quarterCircuitHole"):
                    returnType = Edge.EdgeType.quarterCircuitHole;
                    break;

                case ("quaterCircuit"):
                    returnType = Edge.EdgeType.quaterCircuit;
                    break;

                case ("shortForAHole"):
                    returnType = Edge.EdgeType.shortForAHole;
                    break;
            }
            return returnType;
        }

    }
}
