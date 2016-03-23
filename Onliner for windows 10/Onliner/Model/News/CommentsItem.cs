using Onliner.Interface.News;

namespace Onliner.Model.News
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
