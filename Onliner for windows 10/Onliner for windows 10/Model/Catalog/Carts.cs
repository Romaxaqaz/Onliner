using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Model.Catalog
{
    public class Carts
    {
        public Total total { get; set; }
    }

    public class Total
    {
        public int quantity { get; set; }
    }
}
