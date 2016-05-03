using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner.Model.News
{
    public interface IComments
    {
        string ID { get; set; }
        string Data { get; set; }
        string Image { get; set; }
        string LikeCount { get; set; }
        string Nickname { get; set; }
        string UserId { get; set; }
        string Time { get; set; }
        string ColorItem { get; set; }
        bool Like { get; set; }
        bool Best { get; set; }
    }
}
