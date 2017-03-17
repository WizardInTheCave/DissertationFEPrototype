using System;
using System.Collections.Generic;
using System.Linq;

using DissertationFEPrototype.FEModelUpdate.Model.Structure;

namespace DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    public class Hex8Elem : SquareBasedElem
    {

        //int? id;

        //// id of nodes which form the element
        //List<Node> nodes;
        //List<Hex8Elem> childElements;
        //Hex8Elem parentElement;

        //double aspectRatio;
        //double maxCornerAngle;
        //double maxParallelDev;

        Node[][] faces;
        Hex8QualMetricCalcs propCalcs;
        
        Hex8Refinement hex8Refinement;
       
        public Hex8Elem(int? id, List<Node> nodes)
        {
            
            this.Id = id;

            // sort the nodes when a new element is first made.
            this.nodes = Hex8Refinement.sortNodes(nodes);

            faces = Hex8Refinement.getFacesSplitFromPointCloud(this.nodes);

            propCalcs = new Hex8QualMetricCalcs(this);

            //// all three of these methods use 
            maxCornerAngle = propCalcs.computeMaxCornerAngle(faces);
        
            List<Tuple<Node, Node>[]> nodePairingsfacePairings = faces.Select(x => this.computeEdgePairingsForNode(x.ToList())).ToList();

            maxParallelDev = propCalcs.computeMaxparallelDev(nodePairingsfacePairings);

            // get a single Tuple<Node, Node>[] with the lengths of all the edges in the Hex8 not caring about which face
            // they are associated with.
            Tuple<Node, Node>[] nodePairingsAll = nodePairingsfacePairings
                .Aggregate(new Tuple<Node, Node>[12], (mergedArr, nextArr) => mergedArr.Concat(nextArr).ToArray());

            

            //longestEdge = propCalcs.computeLongestEdge(nodePairingsAll, SHORTEST_EDGE_DEFAULT);
            //shortestEdge = propCalcs.computeShortestEdge(nodePairingsAll, LONGEST_EDGE_DEFAULT);
            aspectRatio = propCalcs.computeAspectRatio(longestEdge, shortestEdge);

            area = propCalcs.computeArea(nodePairingsfacePairings);

        }

        private bool nodeAlreadyExists(Node node, List<Node> checkingList)
        {
            const double TOLERANCE = 0.01;

            foreach(Node checkedNode in checkingList)
            {
                if(
                    Math.Abs(node.GetX - checkedNode.GetX) < TOLERANCE &&
                    Math.Abs(node.GetY - checkedNode.GetY) < TOLERANCE &&
                    Math.Abs(node.GetZ - checkedNode.GetZ) < TOLERANCE)
                {
                    return true;
                }
                // else continue checking
            }
            return false; 
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        private List<Node> faceNodesWithoutDuplicates(Node[][] nodeArray2d)
        {
            List<Node> nodesNoDups = new List<Node>();

            Console.WriteLine("\n\n");
            nodeArray2d.SelectMany(x => x).ToList()
                .ForEach(x => Console.WriteLine("before remove: " + x.GetX + " " + x.GetY + " " + x.GetZ));

            
            Node[] firstArray = nodeArray2d[0];
            nodesNoDups.AddRange(firstArray);

            // loop through the others
            for(int ii=1; ii < nodeArray2d.Length; ii++)
            {
                foreach(Node node in nodeArray2d[ii])
                {
                    // Console.WriteLine(node.GetX + ", " + node.GetY + ", " + node.GetZ);
                    if (!nodeAlreadyExists(node, nodesNoDups))
                    {
                        nodesNoDups.Add(node);
                    }
                }
            }

            return nodesNoDups;
        }

        /// <summary>
        /// Need to be split up Hex8 elements for structures where volume is important
        /// Call this method to create four sub Hex8 elems within the current Hex8 elem
        /// </summary>
        /// <param name="elem">Hex element we want to split into four sub elements using h-refinement</param>
        /// <param name="nodes">Lookup of all nodes in the current model</param>
        /// <returns>A new set of elements which comprise</returns>
        public override List<IElement> createChildElements(Dictionary<Tuple<double, double, double>, Node> nodes)
        {

            Node hexCentre = createHexCentre(faces, nodes);

            Node[][] xPosSubs = getSubSquares(faces[0], nodes);

            Node[][] xNegSubs = getSubSquares(faces[1], nodes);

            //foreach(Node[] sub in xNegSubs)
            //{
            //    sub.ToList().ForEach(x => Console.WriteLine("xNeg: " + x.GetX + " " + x.GetY + " " + x.GetZ));
            //}

            Node[][] yPosSubs = getSubSquares(faces[2], nodes);
            Node[][] yNegSubs = getSubSquares(faces[3], nodes);

            Node[][] zPosSubs = getSubSquares(faces[4], nodes);
            Node[][] zNegSubs = getSubSquares(faces[5], nodes);

            List<Node> xPositive = faceNodesWithoutDuplicates(xPosSubs);
            List<Node> xNegative = faceNodesWithoutDuplicates(xNegSubs);

            List<Node> yPositive = faceNodesWithoutDuplicates(yPosSubs);
            List<Node> yNegative = faceNodesWithoutDuplicates(yNegSubs);

            List<Node> zPositive = faceNodesWithoutDuplicates(zPosSubs);
            List<Node> zNegative = faceNodesWithoutDuplicates(zNegSubs);

            // make four new smaller hexes inside the main hex
            // make subHex in top right hand corner
            Hex8Refinement h8Refine = new Hex8Refinement(hexCentre, xPositive, xNegative, yPositive, yNegative, zPositive, zNegative);

            return h8Refine.SubElems;
          
        }



        /// <summary>
        /// Make a new Hex Quad4Elem, by making a new Quad4Elem object with the right ordering for the input nodes
        /// And the right element type
        /// </summary>
        /// <returns>new hex element for that quadrent of the previous hex</returns>
     


        private Node createHexCentre(Node[][] faces, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            // each of the sub divided sections, now need to sew up
            var xFront = getSubSquares(faces[0], nodes);
            var xBack = getSubSquares(faces[1], nodes);
            var yFront = getSubSquares(faces[2], nodes);
            var yBack = getSubSquares(faces[3], nodes);

            var top = getSubSquares(faces[4], nodes);
            var bottom = getSubSquares(faces[5], nodes);

            Node xFrontCenter = xFront[0][2];
            Node xBackCenter = xBack[0][2];

            Node yFrontCenter = yFront[0][2];
            Node yBackCenter = yBack[0][2];

            Node topCenter = top[0][2];
            Node bottomCentre = bottom[0][2];

        

            double x = (xFrontCenter.GetX + xBackCenter.GetX) / 2;
            double y = (yFrontCenter.GetY + yBackCenter.GetY) / 2;
            double z = (topCenter.GetZ + bottomCentre.GetZ) / 2;

            return createNode(x, y, z, nodes);
        }



        /// <summary>
        /// Get given a set of corner nodes for a Quad4 element create nodes along the edges and a node in the centre
        /// So that the space can be split up into sub elements
        /// </summary>
        /// <returns>array of sub element points</returns>
        private Node[][] getSubSquares(Node[] cornerNodes, Dictionary<Tuple<double, double, double>, Node> allNodes)
        {
 
            var subNodeTup = createMidpointNodes(cornerNodes, allNodes);

            List<Node[]> elementEdgeTrios = subNodeTup.Item1;
            List<Node> midpointLineNodes = subNodeTup.Item2;

            // get the new center node which will be a corner for each of the four new elements
            Node centerNode = createCenterNode(midpointLineNodes, allNodes);

            Node[][] subSquares = new Node[4][];


            int ii = 0;
            foreach (Node[] trio in elementEdgeTrios)
            {
                // add the centre node to get the smaller element
                trio[2] = centerNode;
                subSquares[ii] = trio;
                ii++;
            }

            var flatNodes = subSquares.SelectMany(x => x);

            return subSquares;
        }


        public override List<Node> getDiagonalNodes(Node currentNode)
        {
            throw new NotImplementedException();
        }
    }
}
