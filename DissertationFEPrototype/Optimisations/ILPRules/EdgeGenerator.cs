﻿using DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using DissertationFEPrototype.Optimisations;
using Newtonsoft.Json;
using System.IO;
using DissertationFEPrototype.FEModelUpdate.Model;
using DissertationFEPrototype.FEModelUpdate.Model.Structure;

namespace DissertationFEPrototype.Optimisations.ILPRules
{
    /// <summary>
    /// This class is responsible for containing code that identifies edges within the mesh structure, by finding edges it is then
    /// Possible to apply Dolsaks ILP rules. 
    /// </summary>
    class EdgeGenerator
    {

        List<Edge> edges = new List<Edge>();
        public List<Edge> Edges{ get { return this.edges; } }


        public EdgeGenerator(MeshData meshData, string edgeFileLocal)
        {

            // if there is an input file to use then use that, else try and find edges
            try { 
                edges = readEdges(meshData.Nodes.Values.ToList(), edgeFileLocal); 
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                edges = findEdges(meshData);
               
            }
        }

        /// <summary>
        /// Using the nodes alredy present in the model and the JSON file containing edges produce edge objects correctly
        /// referenceing the nodes.
        /// </summary>
        /// <param name="nodes">Node objects currently present in the model</param>
        /// <param name="edgeFileLocal">path to the edges JSON file</param>
        /// <returns></returns>
        private List<Edge> readEdges(List<Node> nodes, string edgeFileLocal)
        {
            DeserializedEdgeGroup eg = null;
            List<Edge> modelEdges = new List<Edge>();
            List<JsonEdge> jsonEdges = new List<JsonEdge>();

            using (StreamReader r = new StreamReader(edgeFileLocal))
            {
                string jsonString = r.ReadToEnd();
                eg = JsonConvert.DeserializeObject<DeserializedEdgeGroup>(jsonString);
            }

            var map = nodes.ToDictionary(x => x.Id, x => x);
            if (eg != null)
            {
                jsonEdges = eg.edges;
            }

            foreach(JsonEdge jEdge in jsonEdges)
            {
                int Id = jEdge.Id;
                Edge.BoundaryType boundaryType = jEdge.getConvertedBoundaryType();
                Edge.EdgeType edgeType = jEdge.getConvertedEdgeType();
                Edge.LoadingType loadingType = jEdge.getConvertedLoadingType();

                List<Node> nodePath = jEdge.nodePath.Select(edgeId => map[edgeId]).ToList();

                Edge newEdge = new Edge(Id, edgeType, boundaryType, loadingType, nodePath);
                modelEdges.Add(newEdge);
            }

            return modelEdges;
        }

        private List<Edge> findEdges(MeshData meshData)
        {
            var edgeIdentifier = new EdgeIdentifier(meshData);
            return edgeIdentifier.Edges;
        }
    }
}
