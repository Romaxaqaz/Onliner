using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Onliner.Http;
using Onliner.Model.News;
using Onliner.Model.DataTemplateSelector;
using HtmlAgilityPack;
using MyToolkit.Multimedia;
using MyToolkit.Controls;

namespace Onliner.ParsingHtml
{
    public class ParsingFullNewsPage
    {
        #region TagName
        private readonly string TagTypeClass = "class";
        private readonly string NameTagDiv = "div";
        private readonly string NameTagSpan = "span";
        private readonly string NameTagStrong = "strong";
        private readonly string NameTagFigure = "figure";
        private readonly string NameTagImg = "img";
        private readonly string NameTagA = "a";
        private readonly string NameTagLi = "li";
        private readonly string AttributeTagSRC = "src";
        #endregion
        /// <summary>
        /// <value> Background color listview item</value>
        /// </summary>
        private readonly string BackGroundColorListItem = "#FFF7F7F7";
        private string urlPageNews = string.Empty;
        private string loadePage = string.Empty;
        private string newsID = string.Empty;
        
        private HttpRequest HttpRequest = new HttpRequest();
        private HtmlDocument htmlDoc = new HtmlDocument();
        private List<FullItemNews> listNews = new List<FullItemNews>();
        private List<CommentsItem> listComments = new List<CommentsItem>();
        private List<string> listDataContent = new List<string>();
        private List<UIElement> controlList = new List<UIElement>();

        private List<ListViewItemSelectorModel> NewsListItem = new List<ListViewItemSelectorModel>();
        ListViewItemSelectorModel listViewMode = new ListViewItemSelectorModel();
        private int countNews = 0;
        public int CountPTag { get { return countNews; } }

        public string LoadePage { get { return loadePage; } }
        public string NewsID { get { return newsID; } }

        public ParsingFullNewsPage(string page)
        {
            urlPageNews = page;
            loadePage = GetHtmlPage();
        }

        /// <summary>
        /// Get html page to string
        /// </summary>
        /// <returns>string page</returns>
        private string GetHtmlPage()
        {
            HttpRequest.GetRequestOnliner(urlPageNews);
            return HttpRequest.ResultGetRequsetString;
        }

        /// <summary>
        /// News information
        /// </summary>
        /// <returns>news item object</returns>
        public async Task<List<ListViewItemSelectorModel>> NewsMainInfo()
        {
            FullItemNews fullNews = new FullItemNews();
            htmlDoc.LoadHtml(loadePage);

            newsID = htmlDoc.DocumentNode.Descendants(NameTagSpan).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "news_view_count").FirstOrDefault().Attributes["news_id"].Value;
            string _category = htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-post-tags-1").LastOrDefault().Descendants(NameTagStrong).LastOrDefault().Descendants(NameTagA).LastOrDefault().InnerText;
            string _time = htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-post-tags-1").LastOrDefault().Descendants("time").FirstOrDefault().InnerText;
            string _title = htmlDoc.DocumentNode.Descendants("h3").Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__title").LastOrDefault().Descendants(NameTagA).FirstOrDefault().InnerText;
            string _image = htmlDoc.DocumentNode.Descendants(NameTagFigure).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__image").LastOrDefault().Descendants(NameTagImg).FirstOrDefault().Attributes[AttributeTagSRC].Value;

            NewsListItem.Add(new ListViewItemSelectorModel("header", _category, _time));
            NewsListItem.Add(new ListViewItemSelectorModel("title", _title));
            NewsListItem.Add(new ListViewItemSelectorModel("picture", _image));

            var ListPTag = htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__text").ToList();
            htmlDoc.LoadHtml(ListPTag[0].InnerHtml);
            var contentNews = htmlDoc.DocumentNode.Descendants("p").ToList();
            foreach (var item in contentNews)
            {
                if (item.InnerHtml.Contains("<img"))
                {
                    string url = item.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                    NewsListItem.Add(new ListViewItemSelectorModel("image", url));
                }
                else if (item.InnerHtml.Contains("iframe"))
                {
                    if (item.InnerHtml.Contains("https://www.youtube.com/embed/"))
                    {
                        string linkVideo = item.Descendants("iframe").FirstOrDefault().Attributes["src"].Value;
                        string path = linkVideo.Replace("https://www.youtube.com/embed/", "");
                        var pathUri = await YouTube.GetVideoUriAsync(path, YouTubeQuality.Quality480P);
                        NewsListItem.Add(new ListViewItemSelectorModel("video", pathUri.Uri));
                    }
                    else
                    {
                        NewsListItem.Add(new ListViewItemSelectorModel("web", item.InnerHtml));
                    }
                }
                else
                {
                    NewsListItem.Add(new ListViewItemSelectorModel("story", item.InnerHtml));
                }
            }
            return NewsListItem;
        }




        /// <summary>
        /// Comments data
        /// </summary>
        /// <returns></returns>
        public async Task<List<CommentsItem>> CommentsMainInfo()
        {
            CommentsItem commentsParams;
            htmlDoc.LoadHtml(loadePage);
            int step = 0;
            await Task.Run(() =>
            {
                var commentsList = htmlDoc.DocumentNode.Descendants(NameTagLi).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-comments-1__list-item commentListItem").ToList();
                foreach (var item in commentsList)
                {
                    step++;
                    commentsParams = new CommentsItem();
                    commentsParams.Nickname = item.Descendants(NameTagStrong).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author").FirstOrDefault().InnerText;
                    commentsParams.Time = item.Descendants(NameTagSpan).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "date").FirstOrDefault().InnerText;
                    commentsParams.Image = "https:" + item.Descendants(NameTagFigure).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author-image").FirstOrDefault().Descendants(NameTagImg).FirstOrDefault().Attributes[AttributeTagSRC].Value;
                    commentsParams.Data = item.InnerHtml;
                    commentsParams.LikeCount = item.Descendants(NameTagSpan).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "_counter").LastOrDefault().InnerText;
                    if (step % 2 == 0)
                    {
                        commentsParams.ColorItem = BackGroundColorListItem;
                    }
                    listComments.Add(commentsParams);
                }
            });
            return listComments;
        }

        private HtmlView PostItemTextBlock(string text)
        {
            HtmlView textBlock = new HtmlView();
            textBlock.Html = text;
            textBlock.Margin = new Thickness(10);
            return textBlock;
        }
    }
}
