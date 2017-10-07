using DissertationFEPrototype.FEModelUpdate.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    /// <summary>
    /// I realised things were starting to get messy with quite a few conditionals
    /// In order to deal with multiple element types, so decided to
    /// switch to using an interface instead in order to better break down the problem 
    /// and allow for more element types to more easily be added in the future.
    /// </summary>
    public interface IElement
    {
        int? getId();
        void setId(int? Id);
        double getArea();
        double getAspectRatio();
        double getMaxCornerAngle();
        double getMaxParallelDev();
        List<IElement> getChildren();
        void setChildren(List<IElement> children);
        List<Node> getNodes();
        void setNodes(List<Node> nodes);
        List<IElement> createChildElements(Dictionary<Tuple<double, double, double>, Node> nodes);
        List<Node> getDiagonalNodes(Node currentNode);

    }
}
