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
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Windows.UI.Popups;

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
        private string urlPageNews = string.Empty;
        private string loadePage = string.Empty;
        private string newsID = string.Empty;

        private HttpRequest HttpRequest = new HttpRequest();
        private HtmlDocument htmlDoc = new HtmlDocument();
        private List<FullItemNews> listNews = new List<FullItemNews>();
        private ObservableCollection<CommentsItem> listComments = new ObservableCollection<CommentsItem>();
        private List<string> listDataContent = new List<string>();
        private List<UIElement> controlList = new List<UIElement>();

        List<CommetsLike> listItem = new List<CommetsLike>();

        private ObservableCollection<ListViewItemSelectorModel> NewsListItem = new ObservableCollection<ListViewItemSelectorModel>();
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
        public async Task<ObservableCollection<ListViewItemSelectorModel>> NewsMainInfo()
        {
            FullItemNews fullNews = new FullItemNews();
            htmlDoc.LoadHtml(loadePage);



            newsID = htmlDoc.DocumentNode.Descendants(NameTagSpan).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "news_view_count").FirstOrDefault().Attributes["news_id"].Value;

            string urll = UrlLikeApi(urlPageNews, newsID);
            var strings = await HttpRequest.GetTypeRequestOnlinerAsync(urll);


            var m_res = JsonConvert.DeserializeObject<YourJsonClass>(strings);
            foreach (dynamic numb in m_res.comments)
            {
                string h = numb.Value.ToString();
                CommetsLike com = new CommetsLike();
                com.ID = numb.Name;
                com.Item = JsonConvert.DeserializeObject<Item>(h);
                listItem.Add(com);
            }

            string _category = htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-post-tags-1").LastOrDefault().Descendants(NameTagStrong).LastOrDefault().Descendants(NameTagA).LastOrDefault().InnerText;
            string _time = htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-post-tags-1").LastOrDefault().Descendants("time").FirstOrDefault().InnerText;
            string _title = htmlDoc.DocumentNode.Descendants("h3").Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__title").LastOrDefault().Descendants(NameTagA).FirstOrDefault().InnerText;
            string _image = htmlDoc.DocumentNode.Descendants(NameTagFigure).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__image").LastOrDefault().Descendants(NameTagImg).FirstOrDefault().Attributes[AttributeTagSRC].Value;

            NewsListItem.Add(new ListViewItemSelectorModel("header", _category, _time));
            NewsListItem.Add(new ListViewItemSelectorModel("title", _title));
            NewsListItem.Add(new ListViewItemSelectorModel("picture", _image));

            //footer
            var footerNews = htmlDoc.DocumentNode.Descendants("footer").Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-inner-pages-footer-1").FirstOrDefault().InnerHtml;

            // p tag collection
            var ListPTag = htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__text").ToList();
            htmlDoc.LoadHtml(ListPTag[0].InnerHtml);

            foreach (var item in htmlDoc.DocumentNode.ChildNodes)
            {
                if (item.Name == "p")
                {
                    if (item.InnerHtml.Contains("<img"))
                    {
                        string url = item.Descendants("img").FirstOrDefault().Attributes["src"].Value;
                        NewsListItem.Add(new ListViewItemSelectorModel("image", url));
                    }
                    else 
                    if (item.InnerHtml.Contains("iframe"))
                    {
                        if (item.InnerHtml.Contains("https://www.youtube.com/embed/"))
                        {
                            YouTubeUri uri = new YouTubeUri();
                            string linkVideo = item.Descendants("iframe").FirstOrDefault().Attributes["src"].Value;
                            try
                            {
                                uri = await GetYouTubeYriForControl(linkVideo);
                                NewsListItem.Add(new ListViewItemSelectorModel("video", uri.Uri));
                            }
                            catch(YouTubeUriNotFoundException)
                            {
                                NewsListItem.Add(new ListViewItemSelectorModel("web", item.InnerHtml));
                            }
                           
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
                if (item.Name == "ul")
                {
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(item.InnerHtml);

                    var liCollection = htmlDoc.DocumentNode.Descendants("li").ToList();
                    string html = string.Empty;
                    foreach (var it in liCollection)
                    {
                        html = html + it.InnerHtml + "<br>";
                    }
                    NewsListItem.Add(new ListViewItemSelectorModel("story", html));
                }
            }
            NewsListItem.Add(new ListViewItemSelectorModel("story", footerNews));
            return NewsListItem;
        }



        private async Task<YouTubeUri> GetYouTubeYriForControl(string url)
        {
            string pattern = @"(embed\/[-A-Za-z0-9]+)";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Multiline);
            var clearUrl = regex.Match(url);
            string path = clearUrl.ToString().Replace("embed/", "");
            var pathUri = await YouTube.GetVideoUriAsync(path, YouTubeQuality.Quality360P);
            return pathUri;
        }

        /// <summary>
        /// Comments data
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<CommentsItem>> CommentsMainInfo()
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
                    commentsParams.ID = item.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "comment-actions").FirstOrDefault().Descendants("a").FirstOrDefault().Attributes["data-comment-id"].Value;
                    commentsParams.Nickname = item.Descendants(NameTagStrong).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author").FirstOrDefault().InnerText;
                    commentsParams.Time = item.Descendants(NameTagSpan).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "date").FirstOrDefault().InnerText;
                    commentsParams.Image = "https:" + item.Descendants(NameTagFigure).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author-image").FirstOrDefault().Descendants(NameTagImg).FirstOrDefault().Attributes[AttributeTagSRC].Value;
                    commentsParams.Data = item.InnerHtml;
                    var commentsLikeCount = listItem.FirstOrDefault(x => x.ID == commentsParams.ID);
                    if (commentsLikeCount != null)
                        commentsParams.LikeCount = commentsLikeCount.Item.counter;

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

        private string UrlLikeApi(string url, string newsId)
        {
            if (url.Contains("tech.onliner.by"))
            {
                return $"https://tech.onliner.by/sdapi/news.api/tech/posts/{newsId}/likes";
            }
            if (url.Contains("people.onliner.by"))
            {
                return $"https://people.onliner.by/sdapi/news.api/people/posts/{newsId}/likes";
            }
            if (url.Contains("auto.onliner.by"))
            {
                return $"https://auto.onliner.by/sdapi/news.api/auto/posts/{newsId}/likes";
            }
            if (url.Contains("realt.onliner.by"))
            {
                return $"https://realt.onliner.by/sdapi/news.api/realt/posts/{newsId}/likes";
            }
            return string.Empty;
        }

    }

    public class YourJsonClass
    {
        public dynamic comments { get; set; }
    }

    public class Item
    {
        public string counter { get; set; }
        public string best { get; set; }
    }

    public class CommetsLike
    {
        public string ID { get; set; }
        public Item Item { get; set; }
    }
}
