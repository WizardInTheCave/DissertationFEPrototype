using DisertationFEPrototype.Model.Analysis;
using DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements;

using DisertationFEPrototype.FEModelUpdate.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DisertationFEPrototype.FEModelUpdate.Model
{
    /// <summary>
    /// This class is a representation of
    /// </summary>
    public class MeshData
    {
        Dictionary<Tuple<double, double, double>, Node> nodes;
        List<IElement> elements;
        List<Force> forces;
        Material material;
        List<FaceSelection> faceSelections;
        List<FixSelection> fixSelections;

        //string lisaFile;

        
        public Dictionary<Tuple<double, double, double>, Node> Nodes
        {
            get
            {
                return nodes;
            }
            set
            {
                nodes = value;
            }
        }

        public List<IElement> Elements
        {
            get
            {
                return elements;
            }
            set
            {
                elements = value;
            }

        }
        public List<Force> Forces
        {
            get
            {
                return forces;
            }
            set
            {
                forces = value;
            }

        }
        public Material Material
        {
            get
            {
                return material;
            }
            set
            {
                material = value;
            }

        }
        public List<FaceSelection> FaceSelections
        {
            get
            {
                return faceSelections;
            }
            set
            {
                faceSelections = value;
            }
        }
        public List<FixSelection> FixSelections
        {
            get
            {
                return fixSelections;
            }
            set
            {
                fixSelections = value;
            }
        }
        public MeshData()
        {
            //this.nodes = nodes;
            //this.elements = elements;
        }

        /// <summary>
        /// Given a node finds all the elements in the mesh data structure that are constructed using that node.
        /// For elements of type quad4 there shouldn't be more than four elements for each node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public List<IElement> findElems(Node node)
        {
            List<IElement> nodeElems = new List<IElement>();

            // find out what element our start is a part of
            foreach (IElement elem in elements)
            {
                // this will only select the first element that is found which is not actually what we want
                foreach (Node aNode in elem.getNodes())
                {
                    if (aNode == node)
                    {
                        nodeElems.Add(elem);
                    }
                }
            }
            return nodeElems;
        }

    }
}
