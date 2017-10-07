using DissertationFEPrototype.FEModelUpdate.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DissertationFEPrototype.Optimisations;

namespace DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    /// <summary>
    /// Decided to Make a seperate class for the refinement process for Hex8 since it is a bit more complex 
    /// </summary>
    public class Hex8Refinement
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

        /// <summary>
        /// Method which splits potentially skewed 3d cube into four sub cubes by seperating the 8 points using the x, y and z axis
        /// </summary>
        /// <param name="hexCentre"></param>
        /// <param name="xPositive"></param>
        /// <param name="xNegative"></param>
        /// <param name="yPositive"></param>
        /// <param name="yNegative"></param>
        /// <param name="zPositive"></param>
        /// <param name="zNegative"></param>
        public Hex8Refinement(Node hexCentre, List<Node> xPositive, List<Node> xNegative, List<Node> yPositive,
            List<Node> yNegative, List<Node> zPositive, List<Node> zNegative)
        {
            this.hexCentre = hexCentre;

            Console.WriteLine("Hex centre is: " + this.hexCentre);

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
              
            }

            var ref1 = splitCorner1();
            var ref2 = splitCorner2();
            var ref3 = splitCorner3();
            var ref4 = splitCorner4();
            var ref5 = splitCorner5();
            var ref6 = splitCorner6();
            var ref7 = splitCorner7();
            var ref8 = splitCorner8();



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

        /// <summary>
        /// Hard to explain how these split corner methods work, best thing is to draw a cube with nodes in corners 
        /// and then half way between the corners then go through the code step by step and visualise it
        /// </summary>
        /// <returns></returns>
        private List<Node> splitCorner1()
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

        private List<Node> splitCorner2()
        {

            List<Node> x0y0z0 = new List<Node>();

            var yNegLeastZ = yNegative.OrderBy(x => x.GetZ).Take(6);
            var face1 = yNegLeastZ.OrderBy(x => x.GetX).Take(4);

            // trim the top three nodes
            var xNegLeastZ = xNegative.OrderBy(x => x.GetZ).Take(6).ToList();
            var face2 = xNegLeastZ.OrderBy(x => x.GetY).Take(4).ToList();
            var face2NonOverlapping = face2.Skip(2);

            // This one is the bottom of the cube
            var zNegLeastX = zNegative.OrderBy(x => x.GetX).Take(6).ToList();
            var face3 = zNegLeastX.OrderBy(x => x.GetY).Take(4).ToList();
            var face3NonOverlapping = face3.Skip(2).OrderBy(x => x.GetX).Skip(1);


            // fom the new closest corner Hex8 Element
            x0y0z0.Add(hexCentre);
            x0y0z0.AddRange(face1);
            x0y0z0.AddRange(face2NonOverlapping);
            x0y0z0.AddRange(face3NonOverlapping);

            return x0y0z0;
        }
        private List<Node> splitCorner3()
        {
            List<Node> x1y0z0 = new List<Node>();

            // back face
            var xNegLeastZ = xPositive.OrderBy(x => x.GetZ).Take(6);
            var face1 = xNegLeastZ.OrderBy(x => x.GetY).Take(4);
           
            // right face
            var yNegLeastZ = yNegative.OrderBy(x => x.GetZ).Take(6).ToList();
            var face2 = yNegLeastZ.OrderBy(x => x.GetX).Skip(2).ToList();
            var face2NonOverlapping = face2.Take(2).ToList();

            // This one is the bottom of the cube
            var zNegLeastY = zNegative.OrderBy(x => x.GetY).Take(6).ToList();
            var face3 = zNegLeastY.OrderBy(x => x.GetX).Skip(2).ToList();
            var face3NonOverlapping = face3.Take(2).OrderBy(x => x.GetY).Skip(1);

            x1y0z0.Add(hexCentre);
            x1y0z0.AddRange(face1);
            x1y0z0.AddRange(face2NonOverlapping);
            x1y0z0.AddRange(face3NonOverlapping);

            return x1y0z0;
        }

        private List<Node> splitCorner4()
        {
            List<Node> x1y1z0 = new List<Node>();

            var yPosLeastZ = yPositive.OrderBy(x => x.GetZ).Take(6);
            var face1 = yPosLeastZ.OrderBy(x => x.GetX).Skip(2);

            var xPosLeastZ = xPositive.OrderBy(x => x.GetZ).Take(6).ToList();
            var face2 = xPosLeastZ.OrderBy(x => x.GetY).Skip(2).ToList();
            var face2NonOverlapping = face2.Take(2);

            // This one is the bottom of the cube
            var zNegGreatestY = zNegative.OrderBy(x => x.GetY).Skip(3).ToList();
            var face3 = zNegGreatestY.OrderBy(x => x.GetX).Skip(2).ToList();
            var face3NonOverlapping = face3.Take(2).OrderBy(x => x.GetY).Take(1);

            x1y1z0.Add(hexCentre);
            x1y1z0.AddRange(face1);
            x1y1z0.AddRange(face2NonOverlapping);
            x1y1z0.AddRange(face3NonOverlapping);

            return x1y1z0;
        }

        private List<Node> splitCorner5()
        {
            List<Node> x0y1z1 = new List<Node>();

            var xNegGreatestZ = xNegative.OrderBy(x => x.GetZ).Skip(3);
            var face1 = xNegGreatestZ.OrderBy(x => x.GetY).Skip(2);

            var yPosGreatestZ = yPositive.OrderBy(x => x.GetZ).Skip(3).ToList();
            var face2 = yPosGreatestZ.OrderBy(x => x.GetX).Take(4).ToList();
            var face2NonOverlapping = face2.Skip(2);

            var zPosLeastX = zPositive.OrderBy(x => x.GetX).Take(6).ToList();
            var face3 = zPosLeastX.OrderBy(x => x.GetY).Skip(2).ToList();
            var face3NonOverlapping = face3.Take(2).OrderBy(x => x.GetX).Skip(1);


            x0y1z1.Add(hexCentre);
            x0y1z1.AddRange(face1);
            x0y1z1.AddRange(face2NonOverlapping);
            x0y1z1.AddRange(face3NonOverlapping);

            return x0y1z1;
        }

        private List<Node> splitCorner6()
        {
            List<Node> x0y0z1 = new List<Node>();


            // This is the closest face to us looking on
            var xNegGreatestZ = xNegative.OrderBy(x => x.GetZ).Skip(3);
            var face1 = xNegGreatestZ.OrderBy(x => x.GetY).Take(4);

            // This is the right side face.
            var yNegGreatestZ = yNegative.OrderBy(x => x.GetZ).Skip(3).ToList();
            var face2 = yNegGreatestZ.OrderBy(x => x.GetX).Take(4).ToList();
            var face2NonOverlapping = face2.Skip(2);


            var zPosLeastX = zPositive.OrderBy(x => x.GetX).Take(6).ToList();
            var face3 = zPosLeastX.OrderBy(x => x.GetY).Take(4).ToList();
            var face3NonOverlapping = face3.Skip(2).OrderBy(x => x.GetY).Skip(1);

            x0y0z1.Add(hexCentre);
            x0y0z1.AddRange(face1);
            x0y0z1.AddRange(face2NonOverlapping);
            x0y0z1.AddRange(face3NonOverlapping);

            return x0y0z1;
        }

        private List<Node> splitCorner7()
        {
            List<Node> x1y0z1 = new List<Node>();

            // This is the furthest on cube
            var xPosGreatestZ = xPositive.OrderBy(x => x.GetZ).Skip(3);
            var face1 = xPosGreatestZ.OrderBy(x => x.GetY).Take(4);


            var yNegGreatestX = yNegative.OrderBy(x => x.GetX).Skip(3).ToList();

            var face2 = yNegGreatestX.OrderBy(x => x.GetZ).Skip(2).ToList();

            var face2NonOverlapping = face2.OrderBy(x => x.GetX).Take(2).ToList();

            // top face
            var zPosGreatestX = zPositive.OrderBy(x => x.GetX).Skip(3).ToList();
            var face3 = zPosGreatestX.OrderBy(x => x.GetY).Take(4).ToList();
            var face3NonOverlapping = face3.Skip(2).OrderBy(x => x.GetX).Take(1).ToList();
           


            x1y0z1.Add(hexCentre);
            x1y0z1.AddRange(face1);
            x1y0z1.AddRange(face2NonOverlapping);
            x1y0z1.AddRange(face3NonOverlapping);

            return x1y0z1;
        }

        private List<Node> splitCorner8()
        {
            List<Node> x1y1z1 = new List<Node>();

            // This is the furthest on cube
            var yPosGreatestZ = yPositive.OrderBy(x => x.GetZ).Skip(3);
            var face1 = yPosGreatestZ.OrderBy(x => x.GetX).Skip(2);


            var xPosGreatestZ = xPositive.OrderBy(x => x.GetZ).Skip(3).ToList();
            var face2 = xPosGreatestZ.OrderBy(x => x.GetY).Skip(2).ToList();
            var face2NonOverlapping = face2.Take(2).ToList();

            // top face
            var zPosGreatestx = zPositive.OrderBy(x => x.GetX).Skip(3).ToList();
            var face3 = zPosGreatestx.OrderBy(x => x.GetY).Skip(2).ToList();
            var face3NonOverlapping = face3.Take(2).OrderBy(x => x.GetX).Take(1);

            x1y1z1.Add(hexCentre);
            x1y1z1.AddRange(face1);
            x1y1z1.AddRange(face2NonOverlapping);
            x1y1z1.AddRange(face3NonOverlapping);

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

            return sortedNodes;
        }


    }      
}
