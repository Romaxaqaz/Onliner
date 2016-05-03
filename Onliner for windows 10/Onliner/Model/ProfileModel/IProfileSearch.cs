using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.ProfileModel
{
    interface IProfileSearch
    {
        string IdUser { get; set; }
        string UrlImage { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string Status { get; set; }
        string Time { get; set; }
        string CommentsCount { get; set; }
    }
}
