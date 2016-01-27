using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Model.Catalog
{
    public class CategoryListViewModel
    {
        public string CategoryName { get; set; }
        public List<ElectronicSection> ElectronicSection { get; set; }
    }
}
