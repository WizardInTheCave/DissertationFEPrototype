using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisertationFEPrototype.Model.Structure;
using DisertationFEPrototYpe.FEModelUpdate.Model.Structure.Elements;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    abstract class SquareBasedElem : IElement
    {

        // was int?
        protected int? Id;

        // id of nodes which form the element
        protected List<Node> nodes;
        protected List<SquareBasedElem> childElements;
        protected SquareBasedElem parentElement;

        protected double aspectRatio;
        protected double maxCornerAngle;
        protected double maxParallelDev;

        protected readonly double LONGEST_EDGE_DEFAULT = 0.0;
        protected readonly double SHORTEST_EDGE_DEFAULT = 1000000.0;

        protected double longestEdge = 0.0;
        protected double shortestEdge = 1000000.0;

        protected double area;

        /// <summary>
        /// for a Quad4 element get Node which is diagonal
        /// </summary>
        /// <param name="nodes">ndoes within the element</param>
        /// <param name="queryNode">Node we want to find the diagonal from</param>
        /// <returns></returns>
        protected Node getDiagonalNode(Node[] nodes, Node queryNode)
        {

            if (nodes.Contains(queryNode))
            {
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
        /// Flatten the points in 3d space to a 2d plane consisting of 2/3 axies, 
        /// basic convex hull algorithm can then be run over the elements in the node to establish the boundary.
        /// </summary>
        /// <param name="nodes">Node in the element</param>
        /// <returns>A list of points in 2d that can be given to the convex hull method</returns>
        protected List<ConvexHullPoint> getPointsIn2d(List<Node> nodes)
        {

            List<ConvexHullPoint> points2d = new List<ConvexHullPoint>();

            var xVals = nodes.Select(x => x.GetX);
            var yVals = nodes.Select(x => x.GetY);
            var zVals = nodes.Select(x => x.GetZ);

            double[] devs = new double[3] { xVals.Max() - xVals.Min(),
             yVals.Max() - yVals.Min(),
                zVals.Max() - zVals.Min()};


            int idx = devs.ToList().IndexOf(devs.Min());

            switch (idx)
            {
                case 0:
                    foreach (Node node in nodes)
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
        protected List<Node> convexHull(List<Node> nodes)
        {

            List<ConvexHullPoint> points2d = getPointsIn2d(nodes);
            List<Node> hull = ConvexHull.ComputeConvexHull(points2d, false).Select(x => x.node3d).ToList();

            return hull;
        }


        /// <summary>
        /// Sort nodes comprising a quad4 element 
        /// so when LISA reads them in the file it can form a valid element.
        /// is static so can be used when splitting individual faces of Hex8 elements
        /// </summary>
        /// <param name="nodes">Some nodes</param>
        /// <returns>The nodes in an order LISA will accept</returns>
        protected List<Node> sortFourNodes(List<Node> nodes)
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


                    // third node should be one which is furtherst from the original
                    if (ii == 1)
                    {
                        Node originNode = sortMatchedNodes[0];
                        // && !sortmatchednodes.contains(nodecomp1)

                        double dist1 = originNode.distanceTo(removableNodes[0]);
                        double dist2 = originNode.distanceTo(removableNodes[1]);

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

                        List<Tuple<double, Node>> vals = removableNodes.Select(x => new Tuple<double, Node>(currentnode.distanceTo(x), x)).ToList();
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
        /// Get four nodes half way along each of the edges when given four elements.
        /// </summary>
        /// <param name="elementNodes">four nodes within a particular element</param>
        /// <returns></returns>
        public Tuple<List<Node[]>, List<Node>> createMidpointNodes(Node[] fourNodes, Dictionary<Tuple<double, double, double>, Node> nodes)
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
                        else {
                            subElemNodes[3] = midEdge;
                        }
                    }
                }

                elementEdgeTrios.Add(subElemNodes);

                if (ii == 3 && subElemNodes[3] == null)
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

        /// <summary>
        /// Get given a set of corner nodes for a Quad4 element create nodes along the edges and a node in the centre
        /// So that the space can be split up into sub elements
        /// </summary>
        /// <returns>array of sub element points</returns>
        public Node[][] getSubSquares(Node[] cornerNodes, Dictionary<Tuple<double, double, double>, Node> allNodes)
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
        protected Node createNode(double x, double y, double z, Dictionary<Tuple<double, double, double>, Node> nodes)
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
        protected Node makeMidEdgeNode(Node node, Node adjacentNode, Dictionary<Tuple<double, double, double>, Node> nodes)
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
        protected Node createCenterNode(List<Node> midpointLineNodes, Dictionary<Tuple<double, double, double>, Node> nodes)
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


   

        abstract public List<IElement> createChildElements(Dictionary<Tuple<double, double, double>, Node> nodes);

        abstract public List<Node> getDiagonalNodes(Node currentNode);

        int? IElement.getId()
        {
            return this.Id;
        }

        void IElement.setId(int? Id)
        {
            this.Id = Id;
        }

        double IElement.getArea()
        {
            return this.area;
        }

        double IElement.getAspectRatio()
        {
            return this.aspectRatio;
        }

        double IElement.getMaxCornerAngle()
        {
            return this.maxCornerAngle;
        }

        double IElement.getMaxParallelDev()
        {
            return this.maxParallelDev;
        }

        List<IElement> IElement.getChildren()
        {
            if(childElements != null) { 
                return childElements.Select(x => (IElement)x).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Down cast generic elements to square elements from generic element interface
        /// </summary>
        /// <param name="children"></param>
        void IElement.setChildren(List<IElement> children)
        {
            this.childElements = children.Select(x => (SquareBasedElem)x).ToList();
        }

        List<Node> IElement.getNodes()
        {
            return this.nodes;
        }

        void IElement.setNodes(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        /// <summary>
        /// Compute the aspect ratio for an element 
        /// </summary>
        /// <returns>The aspec ratio of a Quad4 like surface</returns>
        public double computeAspectRatio(double longerEdge, double shorterEdge)
        {
            double aspectRatio;
            aspectRatio = longerEdge / shorterEdge;
            return aspectRatio;
        }


        public double computeLongestEdge(Tuple<Node, Node>[] nodes, double LONGEST_EDGE_DEFAULT)
        {
            double currentlyLongestEdge = LONGEST_EDGE_DEFAULT;

            double lengthsMax = nodes.Select(x => x.Item1.distanceTo(x.Item2)).Max();
            if (lengthsMax > LONGEST_EDGE_DEFAULT)
            {
                currentlyLongestEdge = lengthsMax;
            }
            return currentlyLongestEdge;
        }

        public double computeShortestEdge(Tuple<Node, Node>[] nodes, double SHORTEST_EDGE_DEFAULT)
        {

            double currentlyShortestEdge = SHORTEST_EDGE_DEFAULT;
            double lengthsMin = nodes.Select(x => x.Item1.distanceTo(x.Item2)).Min();
            if (lengthsMin < SHORTEST_EDGE_DEFAULT)
            {
                currentlyShortestEdge = lengthsMin;
            }
            return currentlyShortestEdge;
        }

        /// <summary>
        /// For the nodes in the element get the cross product for each pair and add to total
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private double[] getTotalCrossProduct(List<Node> nodes)
        {
            double[] total = new double[] { 0, 0, 0 };
            for (int ii = 0; ii < nodes.Count; ii++)
            {
                Node vi1 = nodes[ii];
                Node vi2;

                if (ii == nodes.Count - 1)
                {
                    vi2 = nodes[0];
                }
                else
                {
                    vi2 = nodes[ii + 1];
                }
                Tuple<double, double, double> product = vi1.crossProduct(vi2);
                total[0] += product.Item1;
                total[1] += product.Item2;
                total[2] += product.Item3;
            }
            return total;
        }

        /// <summary>
        /// using the method suggested here currently
        /// http://stackoverflow.com/questions/2350604/get-the-surface-area-of-a-polyhedron-3d-object
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public double computeFaceArea(List<Node> faceNodes, double longestEdge, double shortestEdge)
        {
            // element has no area (not polygon)
            if (faceNodes.Count < 3)
            {
                return 0;
            }

            double b = shortestEdge;
            double h = longestEdge;

            double result = b * h;

            return result;
        }


        /// <summary>
        /// get a set of tuples which represent each of the edges within the element
        /// </summary>
        /// <returns></returns>
        public Tuple<Node, Node>[] getEdgePairingsForNode(List<Node> nodes)
        {

            var edges = new Tuple<Node, Node>[4];

            Node node = nodes[0];

            Node[] adjNodes = getNonDiagAdjacentNodes(node, nodes);
            edges[0] = new Tuple<Node, Node>(node, adjNodes[0]);
            edges[1] = new Tuple<Node, Node>(node, adjNodes[1]);


            // in the case of quad4 there is only one diagonal node.
            Node diagonalNode = getDiagonalNode(nodes.ToArray(), node);

            edges[2] = new Tuple<Node, Node>(adjNodes[0], diagonalNode);
            edges[3] = new Tuple<Node, Node>(adjNodes[1], diagonalNode);

            return edges;
        }

        /// <summary>
        /// Take the opposite edges as parameters as this is what we want to calculate the deviation of
        /// </summary>
        /// <param name="edgeA"></param>
        /// <param name="edgeB"></param>
        /// <returns></returns>
        public double getDevOnEdgePair(Tuple<Node, Node> edgeA, Tuple<Node, Node> edgeB)
        {

            // work out 
            var node = edgeA.Item1;

            var twoCommonNodes = new Node[2];
            twoCommonNodes[0] = edgeA.Item2;
            twoCommonNodes[1] = edgeB.Item1;

            double angle1 = getAngle(node, twoCommonNodes);


            var node2 = edgeB.Item1;
            var twoCommonNodes2 = new Node[2];

            twoCommonNodes2[0] = edgeA.Item1;
            twoCommonNodes2[1] = edgeB.Item2;
            double angle2 = getAngle(node2, twoCommonNodes2);

            double combinedAngles = angle1 + angle2;

            // delta from 180 in the pair of angles is the deviation between the two edges
            return Math.Abs(180 - combinedAngles);
        }


        /// <summary>
        /// I want to work out for all the pairs of opposite edges in the node what is the maximum deviation from parrallell
        /// </summary>
        /// <returns>The maximum deviation angle between two opposite edges within the element</returns>
        public double computeMaxparallelDev(Tuple<Node, Node>[] edges)
        {
            double dev1 = getDevOnEdgePair(edges[0], edges[3]);
            double dev2 = getDevOnEdgePair(edges[1], edges[2]);
            return dev1 > dev2 ? dev1 : dev2;

            // throw new NotImplementedException();
        }
        /// <summary>
        /// Determine if two nodes within a quad4 element are adjactent to one another based on their euclidean distance
        /// </summary>
        /// <param name="firstNode">Node we are want to check if the second node is adjacent to</param>
        /// <param name="secondNode">The second node which may or may not be adjacent</param>
        /// <param name="elem">The element that both nodes are contained within</param>
        /// <returns></returns>
        public bool isLessThanMaxDistance(Node firstNode, Node secondNode, List<Node> allElemNodes)
        {
            List<double> distancesBetweenFirstNodeAndOthers = allElemNodes.Select(elemNode => firstNode.distanceTo(elemNode)).ToList();
            double maxDistanceInElement = distancesBetweenFirstNodeAndOthers.Max();

            double distanceBetweenFirstAndSecond = firstNode.distanceTo(secondNode);

            // is the node we are trying to place next to it less then the max distance, if so then it is adjacent
            return (distanceBetweenFirstAndSecond < maxDistanceInElement);
        }

        /// <summary>
        /// Get the angle between a node and two other nodes
        /// </summary>
        /// <param name="angleNode">The node in the corner of the angle that is being computed</param>
        /// <param name="twoCommonNodes">Two nodes either side of that node within the element that will be used to compute the angle</param>
        /// <returns></returns>
        public double getAngle(Node angleNode, Node[] twoCommonNodes)
        {
            // using method described here: http://stackoverflow.com/questions/19729831/angle-between-3-points-in-3d-space

            double[] v1 = new double[] { twoCommonNodes[0].GetX - angleNode.GetX, twoCommonNodes[0].GetY - angleNode.GetY, twoCommonNodes[0].GetZ - angleNode.GetZ };
            double[] v2 = new double[] { twoCommonNodes[1].GetX - angleNode.GetX, twoCommonNodes[1].GetY - angleNode.GetY, twoCommonNodes[1].GetZ - angleNode.GetZ };

            double v1mag = Math.Sqrt(v1[0] * v1[0] + v1[1] * v1[1] + v1[2] * v1[2]);
            double[] v1norm = { v1[0] / v1mag, v1[1] / v1mag, v1[2] / v1mag };

            double v2mag = Math.Sqrt(v2[0] * v2[0] + v2[1] * v2[1] + v2[2] * v2[2]);
            double[] v2norm = { v2[0] / v2mag, v2[1] / v2mag, v2[2] / v2mag };


            double result = v1norm[0] * v2norm[0] + v1norm[1] * v2norm[1] + v1norm[2] * v2norm[2];
            double angle = Math.Acos(result);
            return angle;
        }

        /// <summary>
        /// For a particular node this method can be used to find up to two other nodes that are adjacent to it, thus in the case of quad4 elements
        /// allowing the programmer to ascertain if the element is rectangular or not and if so which side is the longest.
        /// </summary>
        /// <param name="nodeA">the node you want to find the non diagonally adjacent nodes for</param>
        /// <param name="nodes">all the nodes in that particular element</param>
        /// <returns></returns>
        public Node[] getNonDiagAdjacentNodes(Node nodeA, List<Node> nodes)
        {
            Node[] commonNodes = new Node[2];
            int ii = 0;

            foreach (Node nodeB in nodes)
            {

                if (isLessThanMaxDistance(nodeA, nodeB, nodes) && nodeA != nodeB)
                {
                    commonNodes[ii] = nodeB;
                    ii++;
                }
            }
            return commonNodes;
        }

        /// <summary>
        /// currently assumes that three of the nodes have two axis in common, can't do irregular shapes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public double computeMaxCornerAngle(List<Node> fourPlaneNodes)
        {
            // for each element, find the two  others which have at least two planes in common

            double maxAngle = 0;
            foreach (Node nodeA in fourPlaneNodes)
            {
                Node[] commonNodes = getNonDiagAdjacentNodes(nodeA, fourPlaneNodes);
                // get angle here
                double angle = getAngle(nodeA, commonNodes);
                if (angle > maxAngle)
                {
                    maxAngle = angle;
                }
            }
            return maxAngle;
        }
    }
}
