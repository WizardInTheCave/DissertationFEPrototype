using DisertationFEPrototype.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    /// <summary>
    /// I realised things were starting to get messy with quite a few conditionals
    /// In order to deal with multiple element types, so decided to
    /// switch to using an interface instead in order to better break down the problem 
    /// and allow for more element types to more easily be added in the future.
    /// </summary>
    public interface IElement
    {

        int? Id { get; set; }

        double Area { get; }

        double AspectRatio { get; }

        double MaxCornerAngle { get; }

        double MaxParallelDev { get; }

        List<IElement> Children { get; set;  }

        List<Node> Nodes { get; }

        List<IElement> createChildElements(Dictionary<Tuple<double, double, double>, Node> nodes);

        List<Node> getDiagonalNodes(Node currentNode);
    }
}
