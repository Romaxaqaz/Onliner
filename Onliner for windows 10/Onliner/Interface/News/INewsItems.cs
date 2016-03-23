
namespace Onliner.Interface.News
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
