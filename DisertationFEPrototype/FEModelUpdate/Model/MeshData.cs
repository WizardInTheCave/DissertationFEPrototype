using DisertationFEPrototype.Model.Analysis;
using DisertationFEPrototype.Model.MeshDataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DisertationFEPrototype.Model
{
    /// <summary>
    /// This class is a representation of
    /// </summary>
    class MeshData
    {
        Dictionary<Tuple<double, double, double>, Node> nodes;
        List<Element> elements;
        Force force;
        Material material;
        List<FaceSelection> faceSelections;

        //string lisaFile;

        
        public Dictionary<Tuple<double, double, double>, Node> Nodes
        {
            get
            {
                return this.nodes;
            }
            set
            {
                this.nodes = value;
            }
        }

        public List<Element> Elements
        {
            get
            {
                return this.elements;
            }
            set
            {
                this.elements = value;
            }

        }
        public Force TheForce
        {
            get
            {
                return this.force;
            }
            set
            {
                this.force = value;
            }

        }
        public Material TheMaterial
        {
            get
            {
                return this.material;
            }
            set
            {
                this.material = value;
            }

        }
        public List<FaceSelection> TheFaceSelections
        {
            get
            {
                return this.faceSelections;
            }
            set
            {
                this.faceSelections = value;
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
        public List<Element> findElems(Node node)
        {
            List<Element> nodeElems = new List<Element>();

            // find out what element our start is a part of
            foreach (Element elem in this.elements)
            {
                // this will only select the first element that is found which is not actually what we want
                foreach (Node aNode in elem.Nodes)
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
