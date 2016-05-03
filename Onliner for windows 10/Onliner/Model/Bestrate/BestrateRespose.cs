using Newtonsoft.Json;
using System.Collections.Generic;

namespace Onliner.Model.Bestrate
{
    public class BestrateRespose : IBestrate
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("banks")]
        public IList<string> Banks { get; set; }
        [JsonProperty("delta")]
        public string Delta { get; set; }
        [JsonProperty("grow")]
        public string Grow { get; set; }
    }
}
