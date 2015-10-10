using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onliner_for_windows_10.Abstract;

namespace Onliner_for_windows_10.Model
{
    public class FullItemNews : IFullNewsItem
    {
        public string Author { get; set; }

        public string Category { get; set; }

        public string DataTime { get; set; }

        public string Image { get; set; }

        public string PostItem { get; set; }

        public string TitleNews { get; set; }
    }
}
