namespace DissertationFEPrototype.Model
{

    public class NodeAnalysisData
    {

        private int nodeId;
        private double x;
        private double y;
        private double z;
        private double dispX;
        private double dispY;
        private double dispZ;
        private double dispMag;
        private double shearUW;
        private double sheerVW;
        private double vonMeisesBottom;
        private double vonMeisesUpper;
        private double principal1Upper;
        private double principal2Upper;
        private double principal1Bottom;
        private double principal2Bottom;
        private double stressUUMidplane;
        private double stressVVMidplane;
        private double stressUVMidplane;
        private double vonMisesMidplane;
        private double principalStress1Midplane1;
        private double principalStress1Midplane2;
        private double principalStress2Midplane;

        public int Id { get { return this.nodeId; } }


        public double ShearUW {
                get { return this.shearUW; }
        }

        public double StressUVMidplace
        {
            get { return this.stressUVMidplane; }
        }
        public double DispMag { get { return this.dispMag; } }


        public NodeAnalysisData(int nodeId, double x, double y, double z, double dispX, double dispY,
            double dispZ, double sheerUW, double sheerVW, double vonMeisesBottom, double vonMeisesUpper, 
            double principal1Upper, double principal2Upper, double principal1Bottom, double principal2Bottom,
            double stressUUMidplane, double stressVVMidplane, double stressUVMidplane, double vonMisesMidplane, double principalStress1Midplane1,
            double principalStress1Midplane2, double principalStress2Midplane, double dispMag)
        {
            this.nodeId = nodeId;
            this.x = x;
            this.y = y;
            this.z = z;
            this.dispX = dispX;
            this.dispY = dispY;
            this.dispZ = dispZ;
            this.shearUW = sheerUW;
            this.sheerVW = sheerVW;
            this.vonMeisesBottom = vonMeisesBottom;
            this.vonMeisesUpper = vonMeisesUpper;
            this.principal1Upper = principal1Upper;
            this.principal2Upper = principal2Upper;
            this.principal1Bottom = principal1Bottom;
            this.principal2Bottom = principal2Bottom;
            this.stressUUMidplane = stressUUMidplane;
            this.stressVVMidplane = stressVVMidplane;
            this.stressUVMidplane = stressUVMidplane;
            this.vonMisesMidplane = vonMisesMidplane;
            this.principalStress1Midplane1 = principalStress1Midplane1;
            this.principalStress1Midplane2 = principalStress1Midplane2;
            this.principalStress2Midplane = principalStress2Midplane;
            this.dispMag = dispMag;
        }
    }
}
