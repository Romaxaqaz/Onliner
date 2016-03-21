using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Interfaces
{
    public interface INewsItems
    {
         string CountViews { get; set; }
         string Themes { get; set; }
         string Title { get; set; }
         byte[] Image { get; set; }
         string Span { get; set; }
         string Description { get; set; }
         string Footer { get; set; }
         string Mediaicongray { get; set; }
         string Popularcount { get; set; }
         string Bmediaicon { get; set; }
    }
}
