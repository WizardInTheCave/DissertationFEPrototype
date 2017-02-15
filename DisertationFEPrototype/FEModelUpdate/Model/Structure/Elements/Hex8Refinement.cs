using DisertationFEPrototype.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DisertationFEPrototype.Optimisations;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    class Hex8Refinement
    {
        Node[] inputNodes;
        private Dictionary<Tuple<double, double, double>, Node> allModelNodes;

        List<Hex8Elem> childNodes;

        List<Hex8Elem> ChildNodes { get { return childNodes; } }

        public Hex8Refinement(Dictionary<Tuple<double, double, double>, Node> allModelNodes)
        {
            this.allModelNodes = allModelNodes;
            childNodes = createChildElements(allModelNodes);

        }

        /// <summary>
        /// Need to be split up Hex8 elements for structures where volume is important
        /// </summary>
        /// <param name="elem">Hex element we want to split into four sub elements using h-refinement</param>
        /// <param name="nodes">Lookup of all nodes in the current model</param>
        /// <returns>A new set of elements which comprise</returns>
        public List<Hex8Elem> createChildElements(Dictionary<Tuple<double, double, double>, Node> nodes)
        {
            List<Hex8Elem> newElements = new List<Hex8Elem>();

            Node[][] faces = new Node[6][];


            // This way of finding faces works for most common Hex setups but there are 
            // particular corner cases that will breadk it currently, for example if twisted by 45 degrees
            // which results in more than

            // four faces round size of Hex
            // find the four nodes with greatest x values
            faces[0] = inputNodes.OrderByDescending(n => n.GetX).Take(4).ToArray();

            // find four with smallest x values
            faces[1] = inputNodes.OrderBy(n => n.GetX).Take(4).ToArray();

            // find four with greatest y values
            faces[2] = inputNodes.OrderByDescending(n => n.GetY).Take(4).ToArray();

            // find four with smallest y values
            faces[3] = inputNodes.OrderBy(n => n.GetY).Take(4).ToArray();

            // top of the Hex
            // of all the nodes find the four that have the highest z values
            faces[4] = inputNodes.OrderByDescending(n => n.GetZ).Take(4).ToArray();

            // bottom, find the four with the lowest z values
            faces[5] = inputNodes.OrderBy(n => n.GetZ).Take(4).ToArray();

            Node hexCentre = getHexCentre(faces, nodes);

            Node[][] xPosSubs = getSubSquares(faces[0], nodes);
            Node[][] xNegSubs = getSubSquares(faces[1], nodes);
            Node[][] yPosSubs = getSubSquares(faces[2], nodes);
            Node[][] yNegSubs = getSubSquares(faces[3], nodes);

            Node[][] topSubs = getSubSquares(faces[4], nodes);
            Node[][] bottomSubs = getSubSquares(faces[4], nodes);

            // make four new smaller hexes inside the main hex

            // make subHex in top right hand corner
            makeNewHex(hexCentre, xPosSubs, yPosSubs, topSubs);

            // need to do this for each face on the Hex8

            return newElements;
        }

        /// <summary>
        /// Make a new Hex Quad4Elem, by making a new Quad4Elem object with the right ordering for the input nodes
        /// And the right element type
        /// </summary>
        /// <returns>new hex element for that quadrent of the previous hex</returns>
        private Hex8Elem makeNewHex(Node hexCentre, Node[][] xPosSubs, Node[][] yPosSubs, Node[][] topSubs)
        {

            List<Node> hexNodes = new List<Node>();

            // this ordering is specified by LISA for Hex8 Elements under the create 
            // Quad4Elem window within the GUI application
            hexNodes[0] = yPosSubs[0][2];
            hexNodes[1] = hexCentre;
            hexNodes[2] = xPosSubs[0][2];

            Console.WriteLine("X subsquares: ");
            Console.WriteLine("Sub1 Node1: " + xPosSubs[0][0]);
            Console.WriteLine("Sub1 Node2: " + xPosSubs[0][1]);
            Console.WriteLine("Sub1 Node3: " + xPosSubs[0][2]);
            Console.WriteLine("Sub1 Node4: " + xPosSubs[0][3]);
            Console.WriteLine("");
            Console.WriteLine("Sub2 Node1: " + xPosSubs[1][0]);
            Console.WriteLine("Sub2 Node2: " + xPosSubs[1][1]);
            Console.WriteLine("Sub2 Node3: " + xPosSubs[1][2]);
            Console.WriteLine("Sub2 Node4: " + xPosSubs[1][3]);
            Console.WriteLine("");
            Console.WriteLine("Sub3 Node1: " + xPosSubs[2][0]);
            Console.WriteLine("Sub3 Node2: " + xPosSubs[2][1]);
            Console.WriteLine("Sub3 Node3: " + xPosSubs[2][2]);
            Console.WriteLine("Sub3 Node4: " + xPosSubs[2][3]);
            Console.WriteLine("");
            Console.WriteLine("Sub4 Node1: " + xPosSubs[3][0]);
            Console.WriteLine("Sub4 Node2: " + xPosSubs[3][1]);
            Console.WriteLine("Sub4 Node3: " + xPosSubs[3][2]);
            Console.WriteLine("Sub4 Node4: " + xPosSubs[3][3]);
            Console.WriteLine("");


            Console.WriteLine("Y subsquares: ");
            Console.WriteLine("Sub1 Node1: " + yPosSubs[0][0]);
            Console.WriteLine("Sub1 Node2: " + yPosSubs[0][1]);
            Console.WriteLine("Sub1 Node3: " + yPosSubs[0][2]);
            Console.WriteLine("Sub1 Node4: " + yPosSubs[0][3]);
            Console.WriteLine("");
            Console.WriteLine("Sub2 Node1: " + yPosSubs[1][0]);
            Console.WriteLine("Sub2 Node2: " + yPosSubs[1][1]);
            Console.WriteLine("Sub2 Node3: " + yPosSubs[1][2]);
            Console.WriteLine("Sub2 Node4: " + yPosSubs[1][3]);
            Console.WriteLine("");
            Console.WriteLine("Sub3 Node1: " + yPosSubs[2][0]);
            Console.WriteLine("Sub3 Node2: " + yPosSubs[2][1]);
            Console.WriteLine("Sub3 Node3: " + yPosSubs[2][2]);
            Console.WriteLine("Sub3 Node4: " + yPosSubs[2][3]);
            Console.WriteLine("");
            Console.WriteLine("Sub4 Node1: " + yPosSubs[3][0]);
            Console.WriteLine("Sub4 Node2: " + yPosSubs[3][1]);
            Console.WriteLine("Sub4 Node3: " + yPosSubs[3][2]);
            Console.WriteLine("Sub4 Node4: " + yPosSubs[3][3]);
            Console.WriteLine("");

            Console.WriteLine("Z subsquares: ");
            Console.WriteLine("Sub1 Node1: " + topSubs[0][0]);
            Console.WriteLine("Sub1 Node2: " + topSubs[0][1]);
            Console.WriteLine("Sub1 Node3: " + topSubs[0][2]);
            Console.WriteLine("Sub1 Node4: " + topSubs[0][3]);
            Console.WriteLine("");
            Console.WriteLine("Sub2 Node1: " + topSubs[1][0]);
            Console.WriteLine("Sub2 Node2: " + topSubs[1][1]);
            Console.WriteLine("Sub2 Node3: " + topSubs[1][2]);
            Console.WriteLine("Sub2 Node4: " + topSubs[1][3]);
            Console.WriteLine("");
            Console.WriteLine("Sub3 Node1: " + topSubs[2][0]);
            Console.WriteLine("Sub3 Node2: " + topSubs[2][1]);
            Console.WriteLine("Sub3 Node3: " + topSubs[2][2]);
            Console.WriteLine("Sub3 Node4: " + topSubs[2][3]);
            Console.WriteLine("");
            Console.WriteLine("Sub4 Node1: " + topSubs[3][0]);
            Console.WriteLine("Sub4 Node2: " + topSubs[3][1]);
            Console.WriteLine("Sub4 Node3: " + topSubs[3][2]);
            Console.WriteLine("Sub4 Node4: " + topSubs[3][3]);
            Console.WriteLine("");

            //hexNodees[3] = yPosSubs

            return new Hex8Elem(null, hexNodes);
        }

        private static Node getHexCentre(Node[][] faces, Dictionary<Tuple<double, double, double>, Node> nodes)
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

            int newID = nodes.Count + 1;

            double x = (xFrontCenter.GetX + xBackCenter.GetX) / 2;
            double y = (yFrontCenter.GetY + yBackCenter.GetY) / 2;
            double z = (topCenter.GetZ + bottomCentre.GetZ) / 2;

            Node hexCentre = new Node(newID, x, y, z);
            return hexCentre;
        }



        /// <summary>
        /// Get given a set of corner nodes for a Quad4 element create nodes along the edges and a node in the centre
        /// So that the space can be split up into sub elements
        /// </summary>
        /// <returns>array of sub element points</returns>
        private static Node[][] getSubSquares(Node[] cornerNodes, Dictionary<Tuple<double, double, double>, Node> allNodes)
        {

            var subNodeTup = GeneralRefinementMethods.createMidpointNodes(cornerNodes, allNodes);

            List<Node[]> elementEdgeTrios = subNodeTup.Item1;
            List<Node> midpointLineNodes = subNodeTup.Item2;

            // get the new center node which will be a corner for each of the four new elements
            Node centerNode = GeneralRefinementMethods.createCenterNode(midpointLineNodes, allNodes);

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
            // 

        }
    }
}
