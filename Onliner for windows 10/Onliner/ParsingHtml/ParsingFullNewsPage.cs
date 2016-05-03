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

        private string urlPageNews = string.Empty;
        private string loadePage = string.Empty;
        private string newsID = string.Empty;

        private HttpRequest HttpRequest = new HttpRequest();
        private HtmlDocument htmlDoc = new HtmlDocument();
        private List<FullItemNews> listNews = new List<FullItemNews>();
        private ObservableCollection<Comments> listComments = new ObservableCollection<Comments>();
        private List<string> listDataContent = new List<string>();
        private List<CommetsLike> listItem = new List<CommetsLike>();
        private ObservableCollection<ListViewItemSelectorModel> NewsListItem = new ObservableCollection<ListViewItemSelectorModel>();

        public string LoadePage { get { return loadePage; } }
        public string NewsID { get { return newsID; } }

        public ParsingFullNewsPage(string page, string urlPage)
        {
            loadePage = page;
            urlPageNews = urlPage;
        }

        /// <summary>
        /// News information
        /// </summary>
        /// <returns>news item object</returns>
        public async Task<ObservableCollection<ListViewItemSelectorModel>> NewsMainInfo()
        {
            FullItemNews fullNews = new FullItemNews();

            htmlDoc.LoadHtml(LoadePage);

            newsID = htmlDoc.DocumentNode.Descendants(NameTagSpan).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "news_view_count").FirstOrDefault().Attributes["news_id"].Value;

            string likeApiUrl = GetUrlLikeApi(urlPageNews, newsID);
            var likeDataCollection = await HttpRequest.GetTypeRequestOnlinerAsync(likeApiUrl);
            SetCommentsList(likeDataCollection);

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
                if (item.Name.Equals("p"))
                {
                    if (item.InnerHtml.Contains("<img"))
                    {
                        NewsListItem.Add(new ListViewItemSelectorModel("image", ParsingUrlImage(item)));
                    }
                    else if (item.InnerHtml.Contains("iframe"))
                    {
                        if (item.InnerHtml.Contains("https://www.youtube.com/embed/"))
                        {
                            try
                            {
                                var uri = await ParsingYouTubeUrl(item);
                                NewsListItem.Add(new ListViewItemSelectorModel("video", uri.Uri));
                            }
                            catch (YouTubeUriNotFoundException)
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
                if (item.Name.Equals("ul"))
                {
                    NewsListItem.Add(new ListViewItemSelectorModel("story", ParsingLiCollection(item.InnerHtml)));
                }
            }
            NewsListItem.Add(new ListViewItemSelectorModel("story", footerNews));
            return NewsListItem;
        }

        private string ParsingUrlImage(HtmlNode node)
        {
            return node.Descendants("img").FirstOrDefault().Attributes["src"].Value;
        }

        private async Task<YouTubeUri> ParsingYouTubeUrl(HtmlNode node)
        {
            YouTubeUri uri = new YouTubeUri();
            string linkVideo = node.Descendants("iframe").FirstOrDefault().Attributes["src"].Value;
            return await GetYouTubeYriForControl(linkVideo);
        }

        private string ParsingLiCollection(string parsingContent)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(parsingContent);

            var liCollection = htmlDoc.DocumentNode.Descendants("li").ToList();
            string html = string.Empty;
            foreach (var it in liCollection)
            {
                html = html + it.InnerHtml + "<br>";
            }
            return html;
        }

        private void SetCommentsList(string collectionJSon)
        {
            var commentsValue = JsonConvert.DeserializeObject<CommentsClass>(collectionJSon);
            foreach (dynamic item in commentsValue.comments)
            {
                string value = item.Value.ToString();
                CommetsLike commment = new CommetsLike();
                commment.ID = item.Name;
                commment.Item = JsonConvert.DeserializeObject<Item>(value);
                listItem.Add(commment);
            }
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
        public async Task<ObservableCollection<Comments>> CommentsMainInfo()
        {
            Comments commentsParams;
            htmlDoc.LoadHtml(LoadePage);
            await Task.Run(() =>
            {
                var commentsList = htmlDoc.DocumentNode.Descendants(NameTagLi).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-comments-1__list-item commentListItem").ToList();

                foreach (var item in commentsList)
                {
                    commentsParams = new Comments();
                    commentsParams.ID = item.Attributes["data-comment-id"].Value;
                    commentsParams.Nickname = item.Descendants(NameTagStrong).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author").FirstOrDefault().InnerText;
                    commentsParams.UserId = item.Attributes["data-author-id"].Value;
                    commentsParams.Time = item.Descendants(NameTagSpan).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "date").FirstOrDefault().InnerText;
                    commentsParams.Image = "https:" + item.Descendants(NameTagFigure).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author-image").FirstOrDefault().Descendants(NameTagImg).FirstOrDefault().Attributes[AttributeTagSRC].Value;
                    commentsParams.Data = item.InnerHtml;

                    var commentsLikeCount = listItem.FirstOrDefault(x => x.ID == commentsParams.ID);
                    if (commentsLikeCount != null)
                    {
                        commentsParams.Like = commentsLikeCount.Item.like;
                        commentsParams.LikeCount = commentsLikeCount.Item.counter;
                        commentsParams.Best = commentsLikeCount.Item.best;
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

        private string GetUrlLikeApi(string url, string newsId)
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

    public class CommentsClass
    {
        public dynamic comments { get; set; }
    }

    public class Item
    {
        public string counter { get; set; }
        public bool best { get; set; }
        public bool like { get; set; }
    }

    public class CommetsLike
    {
        public string ID { get; set; }
        public Item Item { get; set; }
    }
}
