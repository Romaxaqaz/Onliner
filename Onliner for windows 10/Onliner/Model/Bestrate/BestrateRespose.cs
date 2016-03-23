using System.Collections.Generic;

namespace Onliner.Model.Bestrate
{
    public class BestrateRespose
    {
        public string amount { get; set; }
        public IList<string> banks { get; set; }
        public string delta { get; set; }
        public string grow { get; set; }
    }
}
