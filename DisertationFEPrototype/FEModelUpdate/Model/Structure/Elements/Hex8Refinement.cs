using DisertationFEPrototype.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DisertationFEPrototype.Optimisations;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    /// <summary>
    /// Decided to Make a seperate class for the refinement process for Hex8 since it is a bit more complex 
    /// </summary>
    class Hex8Refinement
    {



        //Node[] inputNodes;
        //private Dictionary<Tuple<double, double, double>, Node> allModelNodes;

        //List<Hex8Elem> childNodes;


        public List<IElement> SubElems
        {
            get
            {
                if (newSubElems != null)
                {
                    return newSubElems.Cast<IElement>().ToList();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                // downcast to quad4 elems
                newSubElems = value.Select(x => (Hex8Elem)x).ToList();
            }
        }

       //  public List<IElement> SubElems { get { return newSubElems; } }

        List<Hex8Elem> newSubElems = new List<Hex8Elem>();

        Node hexCentre;
        List<Node> xPositive;
        List<Node> xNegative;

        List<Node> yPositive;
        List<Node> yNegative;

        List<Node> zPositive;
        List<Node> zNegative;


        public Hex8Refinement(Node hexCentre, List<Node> xPositive, List<Node> xNegative, List<Node> yPositive,
            List<Node> yNegative, List<Node> zPositive, List<Node> zNegative)
        {
            this.hexCentre = hexCentre;

            this.xPositive = xPositive;
            this.xNegative = xNegative;

            this.yPositive = yPositive;
            this.yNegative = yNegative;

            this.zPositive = zPositive;
            this.zNegative = zNegative;

            const int MAX_FACE_NODES = 9;

            if (this.xPositive.Count > MAX_FACE_NODES ||
                this.xNegative.Count > MAX_FACE_NODES ||
                this.yPositive.Count > MAX_FACE_NODES ||
                this.yNegative.Count > MAX_FACE_NODES ||
                this.zPositive.Count > MAX_FACE_NODES ||
                this.zNegative.Count > MAX_FACE_NODES
                )
            {
                Console.WriteLine("Wir Haben Problem");
            }

            var ref1 = refineCorner1();
            var ref2 = refineCorner2();
            var ref3 = refineCorner3();
            var ref4 = refineCorner4();
            var ref5 = refineCorner5();
            var ref6 = refineCorner6();
            var ref7 = refineCorner7();
            var ref8 = refineCorner8();

            var hex1 = new Hex8Elem(null, ref1);
            var hex2 = new Hex8Elem(null, ref2);
            var hex3 = new Hex8Elem(null, ref3);
            var hex4 = new Hex8Elem(null, ref4);
            var hex5 = new Hex8Elem(null, ref5);
            var hex6 = new Hex8Elem(null, ref6);
            var hex7 = new Hex8Elem(null, ref7);
            var hex8 = new Hex8Elem(null, ref8);

            // corners are in the order LISA specifies for elements of type Hex8
            newSubElems.Add(hex1);
            newSubElems.Add(hex2);
            newSubElems.Add(hex3);
            newSubElems.Add(hex4);
            newSubElems.Add(hex5);
            newSubElems.Add(hex6);
            newSubElems.Add(hex7);
            newSubElems.Add(hex8);
            // newSubElems.Add(new Hex8Elem(null, sortNodes(x0y0z0)));
        }

        private List<Node> refineCorner1()
        {

            List<Node> x0y1z0 = new List<Node>();

            // closest face
            var xNegLeastZ = xNegative.OrderBy(x => x.GetZ).Take(6).ToList();
            var face1 = xNegLeastZ.OrderBy(x => x.GetY).Skip(2).ToList();

            var yPosLeastZ = yPositive.OrderBy(x => x.GetZ).Take(6).ToList();
            var face2 = yPosLeastZ.OrderBy(x => x.GetX).Take(4).ToList();
            var face2NonOverlapping = face2.Skip(2);

           

            // on the bottom
            var zNegLeastX = zNegative.OrderBy(x => x.GetX).Take(6).ToList();
            var face3 = zNegLeastX.OrderBy(x => x.GetY).Skip(2).ToList();

            var face3NonOverlapping = face3.Take(2).OrderBy(x => x.GetX).Skip(1);

            x0y1z0.Add(hexCentre);
            x0y1z0.AddRange(face1);
            x0y1z0.AddRange(face2NonOverlapping);
            x0y1z0.AddRange(face3NonOverlapping);

            return x0y1z0;
        }

        internal static Node[][] getFacesSplitFromPointCloud(List<Node> nodes)
        {
            Node[][] faces = new Node[6][];


            // This way of finding faces works for most common Hex setups but there are 
            // particular corner cases that will breadk it currently, for example if twisted by 45 degrees
            // which results in more than

            // four faces round size of Hex
            // find the four nodes with greatest x values
            faces[0] = nodes.OrderByDescending(n => n.GetX).Take(4).ToArray();

            // find four with smallest x values
            faces[1] = nodes.OrderBy(n => n.GetX).Take(4).ToArray();

            // find four with greatest y values
            faces[2] = nodes.OrderByDescending(n => n.GetY).Take(4).ToArray();

            // find four with smallest y values
            faces[3] = nodes.OrderBy(n => n.GetY).Take(4).ToArray();

            // top of the Hex
            // of all the nodes find the four that have the highest z values
            faces[4] = nodes.OrderByDescending(n => n.GetZ).Take(4).ToArray();

            // bottom, find the four with the lowest z values
            faces[5] = nodes.OrderBy(n => n.GetZ).Take(4).ToArray();

            return faces;
        }

        private List<Node> refineCorner2()
        {

            List<Node> x0y0z0 = new List<Node>();

            var yNegLeastZ = yNegative.OrderBy(x => x.GetZ).Take(6);
            var face1 = yNegLeastZ.OrderBy(x => x.GetX).Take(4);

            // trim the top three nodes
            var xNegLeastZ = xNegative.OrderBy(x => x.GetZ).Take(6).ToList();
            var face2 = yNegLeastZ.OrderBy(x => x.GetY).Take(4).ToList();
            if (face2[0] == xNegLeastZ[0])
            {
                face2.RemoveAt(0);
            }

            // This one is the bottom of the cube
            var zNegLeastX = xNegative.OrderBy(x => x.GetX).Take(6).ToList();
            var face3 = zNegLeastX.OrderBy(x => x.GetY).Take(4).ToList();
            if (face3[0] == zNegLeastX[0])
            {
                face2.RemoveAt(0);
            }

            // fom the new closest corner Hex8 Element
            x0y0z0.Add(hexCentre);
            x0y0z0.AddRange(face1);
            x0y0z0.AddRange(face2);
            x0y0z0.AddRange(face3);

            return x0y0z0;
        }
        private List<Node> refineCorner3()
        {
            List<Node> x1y0z0 = new List<Node>();


            var yNegLeastZ = xPositive.OrderBy(x => x.GetZ).Take(6);
            var face1 = yNegLeastZ.OrderBy(x => x.GetY).Take(4);

           
            var xNegLeastZ = yNegative.OrderBy(x => x.GetZ).Take(6).ToList();
            var face2 = yNegative.OrderBy(x => x.GetY).Skip(2).ToList();
            if (face2[0] == xNegLeastZ[0])
            {
                face2.RemoveAt(0);
            }

            // This one is the bottom of the cube
            var zNegLeastY = zNegative.OrderBy(x => x.GetY).Take(6).ToList();
            var face3 = zNegLeastY.OrderBy(x => x.GetX).Skip(2).ToList();
            if (face3[0] == zNegLeastY[0])
            {
                face2.RemoveAt(0);
            }

            x1y0z0.Add(hexCentre);
            x1y0z0.AddRange(face1);
            x1y0z0.AddRange(face2);
            x1y0z0.AddRange(face3);

            return x1y0z0;
        }

        private List<Node> refineCorner4()
        {
            List<Node> x1y1z0 = new List<Node>();


            var yPosLeastZ = yPositive.OrderBy(x => x.GetZ).Take(6);
            var face1 = yPosLeastZ.OrderBy(x => x.GetX).Skip(2);


            var xPosLeastZ = xPositive.OrderBy(x => x.GetZ).Take(6).ToList();
            var face2 = xPosLeastZ.OrderBy(x => x.GetY).Skip(2).ToList();
            if (face2[0] == xPosLeastZ[0])
            {
                face2.RemoveAt(0);
            }

            // This one is the bottom of the cube
            var zNegGreatestY = zNegative.OrderBy(x => x.GetY).Skip(3).ToList();
            var face3 = zNegGreatestY.OrderBy(x => x.GetX).Skip(2).ToList();
            if (face3[0] == zNegGreatestY[0])
            {
                face2.RemoveAt(0);
            }

            x1y1z0.Add(hexCentre);
            x1y1z0.AddRange(face1);
            x1y1z0.AddRange(face2);
            x1y1z0.AddRange(face3);

            return x1y1z0;
        }

        private List<Node> refineCorner5()
        {
            List<Node> x0y1z1 = new List<Node>();

            var xNegGreatestZ = xNegative.OrderBy(x => x.GetZ).Skip(3);
            var face1 = xNegGreatestZ.OrderBy(x => x.GetY).Skip(2);


            var yPosGreatestZ = yPositive.OrderBy(x => x.GetZ).Skip(3).ToList();
            var face2 = yPosGreatestZ.OrderBy(x => x.GetX).Take(4).ToList();

            if (face2[0] == yPosGreatestZ[0])
            {
                face2.RemoveAt(0);
            }

            var zPosLeastX = zNegative.OrderBy(x => x.GetX).Take(6).ToList();
            var face3 = zPosLeastX.OrderBy(x => x.GetY).Skip(2).ToList();
            if (face3[0] == zPosLeastX[0])
            {
                face3.RemoveAt(0);
            }

            x0y1z1.Add(hexCentre);
            x0y1z1.AddRange(face1);
            x0y1z1.AddRange(face2);
            x0y1z1.AddRange(face3);

            return x0y1z1;
        }

        private List<Node> refineCorner6()
        {
            List<Node> x0y0z1 = new List<Node>();


            // This is the closest face to us looking on
            var xNegGreatestZ = xNegative.OrderBy(x => x.GetZ).Skip(3);
            var face1 = xNegGreatestZ.OrderBy(x => x.GetY).Take(4);

            // This is the right side face.
            var yNegGreatestZ = yNegative.OrderBy(x => x.GetZ).Skip(3).ToList();
            var face2 = yNegGreatestZ.OrderBy(x => x.GetX).Take(4).ToList();

            if (face2[0] == yNegGreatestZ[0])
            {
                face2.RemoveAt(0);
            }


            var zPosLeastX = zPositive.OrderBy(x => x.GetX).Take(6).ToList();
            var face3 = zPosLeastX.OrderBy(x => x.GetY).Take(4).ToList();
            if (face3[0] == zPosLeastX[0])
            {
                face3.RemoveAt(0);
            }

            x0y0z1.Add(hexCentre);
            x0y0z1.AddRange(face1);
            x0y0z1.AddRange(face2);
            x0y0z1.AddRange(face3);

            return x0y0z1;
        }

        private List<Node> refineCorner7()
        {
            List<Node> x1y0z1 = new List<Node>();

            // This is the furthest on cube
            var xPosGreatestZ = xPositive.OrderBy(x => x.GetZ).Skip(3);
            var face1 = xPosGreatestZ.OrderBy(x => x.GetY).Take(4);

            // top face
            var yPosGreatestZ = zPositive.OrderBy(x => x.GetX).Skip(3).ToList();
            var face2 = yPosGreatestZ.OrderBy(x => x.GetY).Take(4).ToList();
            if (face2[0] == yPosGreatestZ[0])
            {
                face2.RemoveAt(0);
            }

            var zPosLeastX = yNegative.OrderBy(x => x.GetX).Skip(3).ToList();
            var face3 = zPosLeastX.OrderBy(x => x.GetZ).Skip(2).ToList();

            if (face3[0] == zPosLeastX[0])
            {
                face3.RemoveAt(0);
            }

            x1y0z1.Add(hexCentre);
            x1y0z1.AddRange(face1);
            x1y0z1.AddRange(face2);
            x1y0z1.AddRange(face3);

            return x1y0z1;
        }

        private List<Node> refineCorner8()
        {
            List<Node> x1y1z1 = new List<Node>();

            // This is the furthest on cube
            var yPosGreatestZ = yPositive.OrderBy(x => x.GetZ).Skip(3);
            var face1 = yPosGreatestZ.OrderBy(x => x.GetX).Skip(2);

            // top face
            var zPosGreatestx = zPositive.OrderBy(x => x.GetX).Skip(3).ToList();
            var face2 = yPosGreatestZ.OrderBy(x => x.GetY).Skip(2).ToList();
            if (face2[0] == zPosGreatestx[0])
            {
                face2.RemoveAt(0);
            }


            var xPosGreatestZ = xPositive.OrderBy(x => x.GetZ).Skip(3).ToList();
            var face3 = xPosGreatestZ.OrderBy(x => x.GetZ).Skip(2).ToList();

            if (face3[0] == xPosGreatestZ[0])
            {
                face3.RemoveAt(0);
            }

            x1y1z1.Add(hexCentre);
            x1y1z1.AddRange(face1);
            x1y1z1.AddRange(face2);
            x1y1z1.AddRange(face3);

            return x1y1z1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Node[] sortHex8Tier(IEnumerable<Node> nodeTier)
        {
            Node[] nodes = new Node[4];

            var XOrdered = nodeTier.OrderBy(node => node.GetX);

            var lowerXs = XOrdered.Take(2);
            var lxsYOrdering = lowerXs.OrderBy(node => node.GetY).ToArray();

            nodes[0] = (lxsYOrdering[0]);

            var upperXs = XOrdered.Skip(2);
            var uxsYOrdering = upperXs.OrderBy(node => node.GetY).ToArray();
            nodes[1] = (uxsYOrdering[0]);
            nodes[2] = (uxsYOrdering[1]);

            nodes[3] = (lxsYOrdering[1]);


            //var HigherX = XOrdered.Skip(2);

            //var topLowerXSortedY = LowerX.OrderBy(node => node.GetY).ToArray();

            //nodes[0] = (topLowerXSortedY[1]);
            //nodes[1] = (topLowerXSortedY[0]);

            //HigherX.OrderBy(node => node.GetY).ToArray();

            //nodes[2] = (topLowerXSortedY[0]);
            //nodes[3] = (topLowerXSortedY[1]);

            return nodes;
        }
        /// <summary>
        /// Sorts the list of nodes associated with this element into the correct arangement for LISA
        /// </summary>
        /// <returns>The 8 nodes in the Hex8 Element now sorted so that they are accepted by LISA</returns>
        public static List<Node> sortNodes(List<Node> nodes)
        {
            var topBottomOrdered = nodes.OrderBy(node => node.GetZ).ToArray();

            var topFour = topBottomOrdered.Skip(4).ToArray();
            var topTierSorted = sortHex8Tier(topFour).ToArray();

            var bottomFour = topBottomOrdered.Take(4).ToArray();
            var bottomTierSorted = sortHex8Tier(bottomFour).ToArray();

            List<Node> sortedNodes = bottomTierSorted.Concat(topTierSorted).ToList();

            //Node topFourx0y1 = 

            //var sortedBottomFour = GeneralRefinementMethods.sortFourNodes(bottomFour.ToList());
            //var sortedTopFour = GeneralRefinementMethods.sortFourNodes(topFour.ToList());

            return sortedNodes;
        }


    }      
}
