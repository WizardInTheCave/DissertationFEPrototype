using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.Optimisations.ILPRules
{
    class DeserializedEdgeGroup
    {
        [JsonProperty("Edges")]
        public List<JsonEdge> edges { get; set; }
        public int count { get; set; }
    }
}
