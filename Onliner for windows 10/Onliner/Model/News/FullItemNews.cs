using Onliner.Interface.News;

namespace Onliner.Model.News
{
    public class FullItemNews : IFullNewsItem
    {
        public string NewsID { get; set; }

        public string Author { get; set; }

        public string Category { get; set; }

        public string DataTime { get; set; }

        public string Image { get; set; }

        public string PostItem { get; set; }

        public string TitleNews { get; set; }
    }
}
