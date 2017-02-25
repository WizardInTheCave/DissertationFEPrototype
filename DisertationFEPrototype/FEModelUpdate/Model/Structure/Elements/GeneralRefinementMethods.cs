using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DisertationFEPrototype.FEModelUpdate;

using System.Diagnostics;
using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;
using DisertationFEPrototype.Model.Structure;
using DisertationFEPrototYpe.FEModelUpdate.Model.Structure.Elements;

namespace DisertationFEPrototype.Model.Structure.Elements
{
    /// <summary>
    /// note this method currently only works for finite element, 
    /// </summary>
    static class GeneralRefinementMethods
    {

        /// <summary>
        /// Get given a set of corner nodes for a Quad4 element create nodes along the edges and a node in the centre
        /// So that the space can be split up into sub elements
        /// </summary>
        /// <returns>array of sub element points</returns>
        public static Node[][] getSubSquares(Node[] cornerNodes, Dictionary<Tuple<double, double, double>, Node> allNodes)
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
            return subSquares;
           
        }

        /// <summary>
        /// check if node already exists within the model, if yes then use the node object already in the model, else add
        /// the node to the model
        /// </summary>
        /// <param name="node">the node we want to check the presence of in the model</param>
        /// <returns>the safe node reference</returns>
        public static Node createNode(double x, double y, double z, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            var key = new Tuple<double, double, double>(x, y, z);

            // kind of expensive but fine for the time being, we just want to know what to use as the starting value for assigning node ids
            // nodes.Values.ToArray().Max(x => x.Id);

            Node node;
            if (nodes.ContainsKey(key))
            {
                node = nodes[key];
            }
            else
            {
                // set the centre node to the node that already exists, otherwise keep the one we just made
                int maxNodeCount = nodes.Values.ToArray().Select(a => a.Id).Max();
                node = new Node(maxNodeCount + 1, x, y, z);
                nodes[key] = node;
            }
            return node;
        }

     

        /// <summary>
        /// Make a new node half way along the edge of an element side
        /// </summary>
        /// <param name="node"></param>
        /// <param name="adjacentNode"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static Node makeMidEdgeNode(Node node, Node adjacentNode, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            double newX = (node.GetX + adjacentNode.GetX) / 2;
            double newY = (node.GetY + adjacentNode.GetY) / 2;
            double newZ = (node.GetZ + adjacentNode.GetZ) / 2;

            return createNode(newX, newY, newZ, nodes);
        }

        /// <summary>
        /// Creates the node in the middle of the new set of four elements which we join the others too
        /// </summary>
        /// <param name="midpointLineNodes"></param>
        /// <returns>centeral node object</returns>
        public static Node createCenterNode(List<Node> midpointLineNodes, Dictionary<Tuple<double, double, double>, Node> nodes)
        {

            double maxX = midpointLineNodes.Max(node => node.GetX);
            double minX = midpointLineNodes.Min(node => node.GetX);
            double xVal = (maxX + minX) / 2;

            double maxY = midpointLineNodes.Max(node => node.GetY);
            double minY = midpointLineNodes.Min(node => node.GetY);
            double yVal = (maxY + minY) / 2;

            double maxZ = midpointLineNodes.Max(node => node.GetZ);
            double minZ = midpointLineNodes.Min(node => node.GetZ);
            double zVal = (maxZ + minZ) / 2;

            return createNode(xVal, yVal, zVal, nodes);
        }


        /// <summary>
        /// Determine if two nodes within a quad4 element are adjactent to one another based on their euclidean distance
        /// </summary>
        /// <param name="firstNode">Node we are want to check if the second node is adjacent to</param>
        /// <param name="secondNode">The second node which may or may not be adjacent</param>
        /// <param name="elem">The element that both nodes are contained within</param>
        /// <returns></returns>
        public static bool isLessThanMaxDistance(Node firstNode, Node secondNode, List<Node> allElemNodes)
        {
            List<double> distancesBetweenFirstNodeAndOthers = allElemNodes.Select(elemNode => GeneralGeomMethods.distanceBetweenPoints(firstNode, elemNode)).ToList();
            double maxDistanceInElement = distancesBetweenFirstNodeAndOthers.Max();

            double distanceBetweenFirstAndSecond = GeneralGeomMethods.distanceBetweenPoints(firstNode, secondNode);
  

            //if (distancesBetweenFirstNodeAndOthers[1] == 0 || distancesBetweenFirstNodeAndOthers[2] == 0)
            //{
            //    //Console.WriteLine("FirstSecDist: " + distanceBetweenFirstAndSecond + " MaxDist: " + maxDistanceInElement + "\n");
            //    Console.WriteLine(distancesBetweenFirstNodeAndOthers[0] + " " + distancesBetweenFirstNodeAndOthers[1]
            //        + " " + distancesBetweenFirstNodeAndOthers[2] + "\n");
            //}


            // is the node we are trying to place next to it less then the max distance, if so then it is adjacent
            return (distanceBetweenFirstAndSecond < maxDistanceInElement);
        }
        
       

        internal static double computeFaceArea(List<Node> nodes, double longestEdge, double shortestEdge)
        {
            // throw new NotImplementedException();
            return longestEdge * shortestEdge;
        }

        ///// <summary>
        ///// Compute the longest edge for an element
        ///// </summary>
        ///// <returns></returns>
        //public static double computeLongestEdge(List<Node> nodes, double shortestEdgeDefault)
        //{
        //    double longestEdge = shortestEdgeDefault;

        //    // this is a bit inefficent because going over each edge twice, but most edges we are going to have is 4 so
        //    // it doesn't matter too much, may write it better later
        //    foreach (Node nodeA in nodes)
        //    {
        //        foreach (Node nodeB in nodes)
        //        {
        //            //bool[] commonPlanes = GeneralGeomMethods.whichPlanesCommon(nodeA, nodeB);
        //            //if (GeneralGeomMethods.isAdjacent(commonPlanes))

        //            double edgeLength = GeneralGeomMethods.distanceBetweenPoints(nodeA, nodeB);
        //            if (edgeLength > longestEdge)
        //            {
        //                longestEdge = edgeLength;
        //            }

        //        }
        //    }
        //    return longestEdge;
        //}
        //public static double computeShortestEdge(List<Node> nodes, double longestEdgeDefault)
        //{
        //    double shortestEdge = longestEdgeDefault;

        //    // this is a bit inefficent because going over each edge twice, but most edges we are going to have is 4 so
        //    // it doesn't matter too much, may write it better later
        //    foreach (Node nodeA in nodes)
        //    {
        //        foreach (Node nodeB in nodes)
        //        {
        //            double edgeLength = GeneralGeomMethods.distanceBetweenPoints(nodeA, nodeB);
        //            if (edgeLength < shortestEdge)
        //            {
        //                shortestEdge = edgeLength;
        //            }
        //        }
        //    }
        //    return shortestEdge;
        //}

       

        private static List<ConvexHullPoint> getPointsIn2d(List<Node> nodes)
        {
            
            List<ConvexHullPoint> points2d = new List<ConvexHullPoint>();

            var xVals = nodes.Select(x => x.GetX);
            var yVals = nodes.Select(x => x.GetY);
            var zVals = nodes.Select(x => x.GetZ);

            double[] devs = new double[3] { xVals.Max() - xVals.Min(),
             yVals.Max() - yVals.Min(),
                zVals.Max() - zVals.Min()};

            //if(devs[0] == devs[1] || devs[0] == devs[2])
            //{
            //    devs[0] += 1;
            //}
            //if (devs[1] == devs[2])
            //{
            //    devs[1] += 1;
            //}

            int idx = devs.ToList().IndexOf(devs.Min());

            switch (idx)
            {
                case 0:
                    foreach(Node node in nodes)
                    {
                        points2d.Add(new ConvexHullPoint(node, node.GetY, node.GetZ));
                    }
                    break;
                case 1:
                    foreach (Node node in nodes)
                    {

                        points2d.Add(new ConvexHullPoint(node, node.GetX, node.GetZ));
                    }
                    break;

                case 2:
                    foreach (Node node in nodes)
                    {
                        points2d.Add(new ConvexHullPoint(node, node.GetX, node.GetY));
                    }
                    break;
            }
            return points2d;
            
        }


        /// <summary>
        /// After doing some research on convex hull algorithms for helping to sort the points in 3d space, have decided to try using graham scan on
        /// a 2d representation of the element
        /// 
        /// I derived this code from that in the wikipedia article for the algorithm see: https://en.wikipedia.org/wiki/Graham_scan
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static List<Node> convexHull(List<Node> nodes)
        {

            List<ConvexHullPoint> points2d = getPointsIn2d(nodes);
            List<Node> hull = ConvexHull.ComputeConvexHull(points2d, true).Select(x => x.node3d).ToList();

            
            return hull;
            //var n = nodes.Count;
            //// + 1
            //Tuple<Node, double, double>[] points = new Tuple<Node, double, double>[n];

            //points[1] = points2d.OrderBy(node => node.Item3).ToArray()[0];

            //// points 0 is the final point
            //points[0] = points2d[n-1];

            //int m = 1;

            //for (int ii = 2; ii < n;)
            //{
            //    while (ccw(points[m - 1], points[m], points[ii]) <= 0)
            //    {
            //        if (m > 1)
            //        {
            //            m--;
            //            continue;
            //        }
            //        else if (ii == n -1)
            //        {
            //            break;
            //        }
            //        else
            //        {
            //            ii++;
            //        }
            //    }
            //}
            //return points.Select(point => point.Item1).ToList();
        }

        /// <summary>
        /// Sort nodes comprising a quad4 element 
        /// so when LISA reads them in the file it can form a valid element.
        /// is static so can be used when splitting individual faces of Hex8 elements
        /// </summary>
        /// <param name="nodes">Some nodes</param>
        /// <returns>The nodes in an order LISA will accept</returns>
        public static List<Node> sortFourNodes(List<Node> nodes)
        {

            List<Node> sortMatchedNodes = new List<Node>();

            List<Node> removableNodes = new List<Node>(nodes);

            var convexHullVals = convexHull(nodes);


            
            // try my method
            if (convexHullVals.Count < 4)
            {
                Node currentnode = removableNodes[0];
                sortMatchedNodes.Add(currentnode);

                removableNodes.Remove(currentnode);

                const int nodes_in_quad_four = 4;

                int ii = 0;
                while (sortMatchedNodes.Count < nodes_in_quad_four)
                {

                    List<double> smalldist1 = new List<double>();
                    List<Node> nearnode1 = new List<Node>();


                    // third node should be one which is furtherst from the original
                    if (ii == 1)
                    {
                        Node originnode = sortMatchedNodes[0];
                        // && !sortmatchednodes.contains(nodecomp1)

                        double dist1 = GeneralGeomMethods.distanceBetweenPoints(originnode, removableNodes[0]);
                        double dist2 = GeneralGeomMethods.distanceBetweenPoints(originnode, removableNodes[1]);

                        if (dist1 > dist2)
                        {
                            currentnode = removableNodes[0];
                            sortMatchedNodes.Add(currentnode);
                            removableNodes.Remove(currentnode);
                        }
                        else
                        {
                            currentnode = removableNodes[1];
                            sortMatchedNodes.Add(currentnode);
                            removableNodes.Remove(currentnode);
                        }
                    }
                    else
                    {
                        // select the shortest one that hasn't already been found

                        List<Tuple<double, Node>> vals = removableNodes.Select(x => new Tuple<double, Node>(GeneralGeomMethods.distanceBetweenPoints(currentnode, x), x)).ToList();
                        Tuple<double, Node> minval = vals.OrderBy(x => x.Item1).ToArray()[0];
                        currentnode = minval.Item2;
                        sortMatchedNodes.Add(currentnode);
                        removableNodes.Remove(currentnode);
                    }

                    ii++;
                }
            }
            else
            {
                sortMatchedNodes = convexHullVals;
            }

            return sortMatchedNodes;

            //// add the first node in the current list
        }

        /// <summary>
        /// for a Quad4 element get Node which is diagonal
        /// </summary>
        /// <param name="nodes">ndoes within the element</param>
        /// <param name="queryNode">Node we want to find the diagonal from</param>
        /// <returns></returns>
        public static Node getDiagonalNode(Node[] nodes, Node queryNode)
        {

            if (nodes.Contains(queryNode)) {
                var fourNodes = nodes.ToList();
                List<Node> sortedNodes = sortFourNodes(fourNodes);

                // bottomLeft, bottomRight, topRight, topLeft

                int diagIndex = (sortedNodes.IndexOf(queryNode) + 2) % 4;
                Node diagNode = sortedNodes[diagIndex];
                return diagNode;
            }
            else
            {
                throw new Exception("canot compute the diagonal node when the node you have provided is not a part of this element");
            }

        }

        /// <summary>
        /// Get four nodes half way along each of the edges when given four elements.
        /// </summary>
        /// <param name="elementNodes">four nodes within a particular element</param>
        /// <returns></returns>
        public static Tuple<List<Node[]>, List<Node>> createMidpointNodes(Node[] fourNodes, Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            List<Node[]> elementEdgeTrios = new List<Node[]>();
            List<Node> midEdgeNodes = new List<Node>();

            int ii = 0;
            foreach (Node node in fourNodes)
            {
                // this array r
                Node[] subElemNodes = new Node[4];

                // add the corner node to the trio
                subElemNodes[0] = node;

                //singleNodeTrio.Add(node);
                Node diag = getDiagonalNode(fourNodes, node);
                

                foreach (Node possibleAdjacentNode in fourNodes)
                {

                    //if (subElemNodes.Count(null) > 1 && ii == 3)
                    //{
                    //    Console.Write("WHAT???");
                    //}

                    // if it is one of the nodes on a connecting edge when thinking about it as a square
                    if (possibleAdjacentNode != node && diag != possibleAdjacentNode)
                    {
                        Node midEdge = makeMidEdgeNode(node, possibleAdjacentNode, nodes);
                        midEdgeNodes.Add(midEdge);

                        // this is important so that the mesh forms correct square elements
                        if (subElemNodes[1] == null)
                        {
                            subElemNodes[1] = midEdge;
                        }
                        else{
                            subElemNodes[3] = midEdge;
                        }
                    } 
                }  
                //if (subElemNodes.Any(x => x == null))
                //{
                //    Console.Write("WHAT???");
                //}
                elementEdgeTrios.Add(subElemNodes);

                if(ii == 3 && subElemNodes[3] == null)
                {
                    throw new Exception("Do not have all node values");
                }
                ii++;
            }
            
            if (!elementEdgeTrios.Any(x => x == null))
            {
                return Tuple.Create(elementEdgeTrios, midEdgeNodes);
            }
            else
            {
                throw new Exception("Do not have all node values");
            }
        }
    }
}
