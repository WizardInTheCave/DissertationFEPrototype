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
        public MeshData()
        {
            //this.nodes = nodes;
            //this.elements = elements;
        }
    }
}
