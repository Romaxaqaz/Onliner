namespace Onliner.Interface.News
{
    public interface IFullNewsItem
    {
        string TitleNews { get; set; }
        string Category { get; set; }
        string DataTime { get; set; }
        string Image { get; set; }
        string PostItem { get; set; }
        string Author { get; set; }
    }

    public interface IImageList
    {
        string Link { get; set; }
    }
    public interface IVideoList
    {
        string Link { get; set; }
    }
}
