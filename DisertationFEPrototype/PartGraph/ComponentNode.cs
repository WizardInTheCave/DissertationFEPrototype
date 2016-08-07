using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.PartGraph
{
    /// <summary>
    /// Represents a component within our whole engine model
    /// </summary>
    class ComponentNode
    {
        private string partFile { get; set; }

        private string name { get; set; }

        private List<ComponentNode> connectingNodes { get; set; }

            

    }
}
