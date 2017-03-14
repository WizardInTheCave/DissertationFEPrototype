using DisertationFEPrototype.FEModelUpdate.Model;
using DisertationFEPrototype.FEModelUpdate.Model.Structure;
using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Optimisations.ILPRules
{
    /// <summary>
    /// This class contains some code I wrote with the intended purpose of identifying edges of interest within the model.
    /// Dolsak Et al do not do this however and provide some indication of the different edges they know are of interest to the program based on
    /// real life experiance with similar problems. Hence for my examples I have resored to reading in this data from a file rather than trying
    /// To cleverly identifiy the edges in the model algorithmicly
    /// 
    /// What this module currently tries to do essentially is start at corner nodes within the model, then traverse across a path of nodes until another
    /// corner node is found thus generating an edge, the edge is then classified into one of the categories described by dolsak in his paper 
    /// "Application of Inductive logic programming to finite element mesh design" so that the rules he specifies can be run over the different kinds of edge.
    /// 
    /// To be honest for a robust implementation of this alone which can be shown to work well would constitude a dissertation project in its own right
    /// </summary>
    class EdgeIdentifier
    {

        List<Node> internalEdgeNodes;
        List<Node> externalEdgeNodes;

        List<Node> externalCornerNodes;
        List<Node> internalCornerNodes;

        int totalEdgeCount = 1;

        // all the edges we have been able to find automatically in the model by applying some logic 
        // about what is and isn't an edge
        List<Edge> foundEdges;

        /// <summary>
        /// Using the mesh model build a set of edges that we can apply rules to and then map back into the mesh data domain
        /// based on what I can tell from the paper "Mesh generation expert system for engineering analyses with FEM", (the earlier paper by Bojan Dolsak)
        /// an edge is loosely defined as anywhere on the model where the outside of the structure meets the inside of the structure that isn't a plane, 
        /// that being said not all edges are of interest as some are distant to the regions of the model under high stress.
        /// </summary>
        /// <param name="mesh">MeshData object</param>
        /// <returns>edges built out of the mesh data</returns>
        public EdgeIdentifier(MeshData mesh)
        {

            this.internalCornerNodes = new List<Node>();
            this.internalEdgeNodes = new List<Node>();

            this.externalCornerNodes = new List<Node>();
            this.externalEdgeNodes = new List<Node>();

            // populate the above lists so that the code for identifing edges within the structure has some information is can search through
            findEdgeNodes(mesh);

            // get a list of edges identified within the model that each have a set of properties
            foundEdges = buildEdges(mesh);
        }

        public List<Edge> Edges { get { return this.foundEdges; } }

        private List<Edge> buildEdges(MeshData mesh)
        {
            List<Edge> modelEdges = new List<Edge>();

            // take a corner node, lookup elemet it is in, see if there are any elements that contain a node that is in the general
            // edge nodes category


            // get an edge for each corner node
            int ii = 0;
            foreach (Node corner in externalCornerNodes)
            {
                List<Node> externalCornerNodesReduced = externalCornerNodes.Select(x => x).ToList();
                externalCornerNodesReduced.RemoveAt(ii);
                List<Node> otherEdgeNodes = externalEdgeNodes.Select(edgeNode => edgeNode).ToList();
                otherEdgeNodes.AddRange(externalCornerNodesReduced);
                // need to do something here to check if the same edge has been found twice.
                modelEdges.Add(findEdge(corner, otherEdgeNodes, modelEdges, mesh));
                ii++;
            }

            // get an edge for each corner node
            ii = 0;
            foreach (Node corner in internalCornerNodes)
            {
                List<Node> internalCornerNodesReduced = internalCornerNodes.Select(x => x).ToList();
                internalCornerNodesReduced.RemoveAt(ii);
                List<Node> otherEdgeNodes = internalEdgeNodes.Select(edgeNode => edgeNode).ToList();
                otherEdgeNodes.AddRange(internalCornerNodesReduced);
                // need to do something here to check if the same edge has been found twice.
                modelEdges.Add(findEdge(corner, otherEdgeNodes, modelEdges, mesh));
                ii++;
            }
            return modelEdges;
        }

        private Edge findEdge(Node cornerNode, List<Node> otherEdgeNodes, List<Edge> modelEdges, MeshData mesh)
        {
            // build the edge
            List<Node> ourEdge = findNodeChain(cornerNode, otherEdgeNodes, mesh);

            Edge.EdgeType et = determineEdgeType(ourEdge, modelEdges);
            Edge.BoundaryType bt = determineBoundaryType(ourEdge, modelEdges);
            Edge.LoadingType lt = determineLoadingType(ourEdge, modelEdges, mesh);

            // get element for starting node
            var newEdge = new Edge(totalEdgeCount, et, bt, lt, ourEdge);
            totalEdgeCount++;
            return newEdge;
        }
        /// <summary>
        /// Try to establish the type of edge this new edge might be
        /// </summary>
        /// <param name="ourEdge">The potential new edge as a path of nodes across the structure</param>
        /// <param name="currentEdges"></param>
        /// <returns></returns>
        private Edge.EdgeType determineEdgeType(List<Node> ourEdge, List<Edge> currentEdges)
        {
            Edge.EdgeType et;

            double chainAvg = 0;
            if (currentEdges.Count > 0)
            {
                chainAvg = currentEdges.Select(edge => edge.GetNodePath().Count).Average();
            }

            if (ourEdge.Count > chainAvg)
            {
                et = Edge.EdgeType.importantLong;
            }
            else
            {
                et = Edge.EdgeType.importantShort;
            }
            return et;
        }
        private Edge.BoundaryType determineBoundaryType(List<Node> ourEdge, List<Edge> currentEdges)
        {
            Edge.BoundaryType bt;
            bt = Edge.BoundaryType.fixedCompletely;
            return bt;

        }
        private Edge.LoadingType determineLoadingType(List<Node> ourEdge, List<Edge> currentEdges, MeshData mesh)
        {
            Edge.LoadingType lt;

            List<List<IElement>> elementsGrouped = ourEdge.Select(edgeNode => mesh.findElems(edgeNode)).ToList();
            List<IElement> allNodeElems = elementsGrouped.SelectMany(elems => elems).ToList();

            // cross reference the elements in the model which have a force applied to them against 
            // elements which lie on an edge then intersect the two lists to get the elements which lie on the edge
            // and have an applied force
            List<IElement> edgeElemsWithAppliedForce =
                mesh.FaceSelections
                .Where(selection => selection.GetName == mesh.Force.Selection)
                .SelectMany(select => select.Faces)
                .Select(face => face.Element).Intersect(allNodeElems)
                .ToList();


            short numOfLoadedSides = checkNumberOfLoadedSides(elementsGrouped, edgeElemsWithAppliedForce);

            if (numOfLoadedSides == 0)
            {
                lt = Edge.LoadingType.notLoaded;
            }
            else if (numOfLoadedSides == 1)
            {
                lt = Edge.LoadingType.oneSideLoaded;
            }
            else if (numOfLoadedSides == 2)
            {
                lt = Edge.LoadingType.twoSidesLoaded;
            }
            else
            {
                lt = Edge.LoadingType.notLoaded;
            }
            return lt;
        }

        /// <summary>
        /// Using the elements along the edge which have a force applied we want to determine how many sides of the edge are loaded
        /// </summary>
        /// <param name="allNodeElems"></param>
        /// <param name="edgeElemsWithAppliedForce"></param>
        /// <returns></returns>
        private short checkNumberOfLoadedSides(List<List<IElement>> allNodeElems, List<IElement> edgeElemsWithAppliedForce)
        {
            short numOfSides = -1;
            if (edgeElemsWithAppliedForce.Count == 0)
            {
                numOfSides = 0;
            }
            else
            {
                List<int> elemsAroundNodeWithForce = allNodeElems.Select(elems => elems.Intersect(edgeElemsWithAppliedForce).Count()).ToList();
                int max = elemsAroundNodeWithForce.Max();

                if (max > 2)
                {
                    numOfSides = 2;
                }
                else if (max < 2)
                {
                    numOfSides = 1;
                }
                //else if(max == 2 && elemsAreDiagonal(edgeElemsWithAppliedForce))
                //{
                //     numOfSides = 2;
                //}
            }
            return numOfSides;
        }
        /// <summary>
        /// need to conclude if the elements are diagonal from eachother in which case even with two force elements the edge is loaded on both sides
        /// </summary>
        /// <returns>true if two elements are diagonal to one another and false if not</returns>
        //private bool elemsAreDiagonal(List<Quad4Elem> edgeElemsWithAppliedForce)
        //{
        //}

        /// <summary>
        /// Given a node which we are going to use as the starting point for an edge we need to find other nodes which have been 
        /// considered as part of an edge which can be connected to that initial node and so on.
        /// </summary>
        /// <param name="node">stat</param>
        /// <param name="associatedNodes"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        private List<Node> findNodeChain(Node currentNode, List<Node> otherEdgeNodes, MeshData mesh)
        {
            //List<Node> ourEdge, List<Node> nodesToSelectFrom, List<Quad4Elem> edgeNodeElems)

            List<Node> ourEdge = new List<Node>();
            ourEdge.Add(currentNode);

            // elements associated with the current node
            List<List<IElement>> otherEdgeNodeElements = otherEdgeNodes.Select(x => mesh.findElems(x)).ToList();

            // while we can still move to another edge node.
            while (true)
            {
                var currentNodeElems = mesh.findElems(currentNode);
                Node nextNode = getNextNode(currentNode, currentNodeElems, otherEdgeNodes, otherEdgeNodeElements, ourEdge);

                // Console.WriteLine("something");

                if (nextNode != null)
                {
                    ourEdge.Add(nextNode);
                    currentNode = nextNode;
                }
                else
                {
                    break;
                }
            }
            // ourEdge.ForEach(x => Console.WriteLine("this " + x.GetX + " " + x.GetY + " " + x.GetZ + "\n"));
            // Console.WriteLine("=============================================================================");
            // Console.WriteLine("something");
            return ourEdge;

        }
        /// <summary>
        /// Get the next node in a potential edge
        /// </summary>
        /// <param name="currentNode">the node we are currently searching for more nodes from</param>
        /// <param name="currentNodeElems">elements around the current node we are searching from</param>
        /// <param name="otherEdgeNodes">all edge nodes within the graph</param>
        /// <param name="otherEdgeNodeElements">elements around all edge nodes within the graph</param>
        /// <param name="ourEdge">the current edge</param>
        /// <returns></returns>
        private Node getNextNode(Node currentNode, List<IElement> currentNodeElems, List<Node> otherEdgeNodes, List<List<IElement>> otherEdgeNodeElements, List<Node> ourEdge)
        {
            int ii = 0;
            foreach (List<IElement> elementSet in otherEdgeNodeElements)
            {
                List<IElement> intersects = elementSet.Intersect(currentNodeElems).ToList();


                // the two nodes can be linked
                if (intersects.Count() > 0)
                {


                    List<Node> diags = intersects[0].getDiagonalNodes(currentNode);

                    // Node nodeCopy = new Node(otherEdgeNodes[ii]);
                    // otherEdgeNodeElements.RemoveAt(ii);
                    // otherEdgeNodes.RemoveAt(ii);

                    if (otherEdgeNodes[ii] != diags[0] && !ourEdge.Contains(otherEdgeNodes[ii]))
                    {
                        return otherEdgeNodes[ii];
                    }
                }
                ii++;
            }

            // if we get to here then the edge has ended
            return null;
        }


        /// <summary>
        /// given all the nodes in the model form a list of nodes which are a part of an edge
        /// </summary>
        /// <param name="nodes">all the nodes in the model, can be indexed via coordinates</param>
        /// <returns></returns>
        private void findEdgeNodes(MeshData meshData)
        {
            // edge
            //foreach (Quad4Elem elem in meshData.Elements)
            //{

            //    int plane = elem.FlatPlaneIndex;
            //    // now we know what plane the element is on we want to check if there is a node above xor below
            //    // and a node to the right xor left 

            foreach (Node node in meshData.Nodes.Values)
            {

                List<Node> XYZ = new List<Node>();

                List<Node> mXYZ = new List<Node>();
                List<Node> XmYZ = new List<Node>();
                List<Node> XYmZ = new List<Node>();

                List<Node> mXmYZ = new List<Node>();
                List<Node> XmYmZ = new List<Node>();
                List<Node> mXYmZ = new List<Node>();

                List<Node> mXmYmZ = new List<Node>();


                List<Node>[] sectors = new List<Node>[8] { XYZ, mXYZ, XmYZ, XYmZ, mXmYZ, XmYmZ, mXYmZ, mXmYmZ };


                const double xTol = 0.25;
                const double yTol = 0.25;
                const double zTol = 0.25;

                // here we go through all the nodes, check that the two nodes are not the same, 
                // if not are two of the planes the same then chech if the remaining plane is higher or lower
                // and add to a list representing all positions greater or less than that position on that axis
                foreach (Node node2 in meshData.Nodes.Values)
                {
                    if (node.Id != node2.Id)
                    {

                        double xDelta = Math.Abs(node.GetX - node2.GetX);
                        double yDelta = Math.Abs(node.GetY - node2.GetY);
                        double zDelta = Math.Abs(node.GetZ - node2.GetZ);

                        if (xDelta < xTol && yDelta < yTol && zDelta < zTol)
                        {

                            if (node.GetX < node2.GetX && node.GetY < node2.GetY && node.GetZ < node2.GetZ)
                            {
                                XYZ.Add(node2);
                            }
                            else if (node.GetX > node2.GetX && node.GetY < node2.GetY && node.GetZ < node2.GetZ)
                            {
                                mXYZ.Add(node2);
                            }
                            else if (node.GetX < node2.GetX && node.GetY > node2.GetY && node.GetZ < node2.GetZ)
                            {
                                XmYZ.Add(node2);
                            }
                            else if (node.GetX < node2.GetX && node.GetY < node2.GetY && node.GetZ > node2.GetZ)
                            {
                                XYmZ.Add(node2);
                            }

                            else if (node.GetX > node2.GetX && node.GetY > node2.GetY && node.GetZ < node2.GetZ)
                            {
                                mXmYZ.Add(node2);
                            }
                            else if (node.GetX < node2.GetX && node.GetY > node2.GetY && node.GetZ > node2.GetZ)
                            {
                                XmYmZ.Add(node2);
                            }
                            else if (node.GetX > node2.GetX && node.GetY < node2.GetY && node.GetZ > node2.GetZ)
                            {
                                mXYmZ.Add(node2);
                            }
                            else if (node.GetX > node2.GetX && node.GetY > node2.GetY && node.GetZ > node2.GetZ)
                            {
                                mXmYmZ.Add(node2);
                            }
                        }
                    }
                }

                // perhaps split the edges up here into a tree data structure?
                int emptyRegions = sectors.Where(sector => sector.Count == 0).ToList().Count;

                if (emptyRegions != 4 || emptyRegions != 0)
                {

                    // node is on a plane or in the center of the model and therefore not counted as an edge.
                    // now just need to work out the type of edge.  
                    // it helps to work out why the empty regions 
                    if (emptyRegions == 1)
                    {
                        // corner internal
                        // if(internalCornerNodes.)
                        internalCornerNodes.Add(node);
                    }
                    else if (emptyRegions == 7)
                    {
                        // corner external
                        externalCornerNodes.Add(node);
                    }
                    if (emptyRegions == 6)
                    {
                        // external edge
                        externalEdgeNodes.Add(node);
                    }
                    else if (emptyRegions == 2)
                    {
                        // internal edge
                        internalEdgeNodes.Add(node);
                    }
                }
            }
        }
    }
}
