using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.Bestrate
{
    public interface IBestrate
    {
        string Amount { get; set; }
        IList<string> Banks { get; set; }
        string Delta { get; set; }
        string Grow { get; set; }
    }
}
