using DisertationFEPrototype.Model.MeshDataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Model.Analysis
{
    /// <summary>
    /// Element Id is the id which relates the finite element to this particular face.
    /// FaceId represents the kind of shape the face is, so far I know that
    /// a face id of 5 corresponds to a tri 3 wareas a face id of 6 corresponds to a quad 4
    /// </summary>
    class Face
    {
        Element element;
        int faceId;

        public int GetId
        {
            get
            {
                return this.faceId;
            }
        }
        public Element Element{
            get{
                return this.element;
            }
            set
            {
                this.element = value;
            }
        }

        public Face(Element element, int faceId)
        {
            this.element = element;
            this.faceId = faceId;
        }
    }
}
