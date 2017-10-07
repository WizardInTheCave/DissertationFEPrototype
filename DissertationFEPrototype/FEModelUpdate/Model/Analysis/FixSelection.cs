namespace DissertationFEPrototype.Model.Analysis
{
    /// <summary>
    /// Class storing FaceSections, kind of unnecessary but because of LISA representation of model made sense to make this class for
    /// consistency
    /// </summary>
    public class FixSelection
    {
        FaceSelection selection;
        public FaceSelection Selection{ get{ return this.selection; } }

        /// <summary>
        /// This Class represents locations in the model where the structure is fixed, i.e. there is infiniate stiffness
        /// </summary>
        /// <param name="selection">The FaceSelection that we are designating as fixed</param>
        public FixSelection(FaceSelection selection)
        {
            this.selection = selection;
        }
    }
}
