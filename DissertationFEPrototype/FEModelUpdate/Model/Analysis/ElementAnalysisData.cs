using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationFEPrototype.Model
{

    /// <summary>
    /// This class contains the data from performing analysis for the previous iteration, used to guide the meshing procedures
    /// And as a record of what has been discovered within the model.
    /// </summary>
    class ElementAnalysisData
    {
        int element;
        int localNode;

        double x;
        double y;
        double z;

        string dispX;
        string dispY;
        string dispZ;

        double stressXX;
        double stressYY;
        double stressZZ;

        double stressXY;
        double stressYZ;
        double stressZX;


        public ElementAnalysisData(int element, int localNode, double x, double y, double z, 
            string dispX, string dispY, string dispZ, 
            double stressXX, double stressYY, double stressZZ,
            double stressXY, double stressYZ, double stressZX)
        {
  
            this.element = element;
            this.localNode = localNode;

            this.x = x;
            this.y = y;
            this.z = z;

            this.dispX = dispX;
            this.dispY = dispY;
            this.dispZ = dispZ;

            this.stressXX = stressXX;
            this.stressYY = stressYY;
            this.stressZZ = stressZZ;

            this.stressXY = stressXY;
            this.stressYZ = stressYZ;
            this.stressZX = stressZX;

        }
    }
}
