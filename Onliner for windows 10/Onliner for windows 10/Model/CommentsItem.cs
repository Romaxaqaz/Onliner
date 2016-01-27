using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onliner_for_windows_10.Abstract;
using Windows.UI;

namespace Onliner_for_windows_10.Model
{
    public class CommentsItem : ICommentsItem
    {
        public string Data { get;  set; }

        public string Image { get; set; }

        public string LikeCount { get; set; }

        public string Nickname { get; set; }

        public string Time { get; set; }

        public string ColorItem { get; set; }
    }
}
