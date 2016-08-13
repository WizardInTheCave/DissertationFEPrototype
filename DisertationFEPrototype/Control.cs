using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using DisertationFEPrototype.FEModelUpdate;

namespace DisertationFEPrototype
{
    class Control
    {
        public Control(string lisaString) {

           var meshData = new ReadMeshData(lisaString);
           MeshDataStructure.MeshData model = meshData.GetCurrentModel;
            


        }
    }
}
