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

        private string _urlPageNews = string.Empty;

        private readonly HttpRequest _httpRequest = new HttpRequest();
        private readonly HtmlDocument _htmlDoc = new HtmlDocument();
        private List<FullItemNews> _listNews = new List<FullItemNews>();
        private ObservableCollection<Comments> _listComments = new ObservableCollection<Comments>();
        private List<string> _listDataContent = new List<string>();
        private List<CommetsLike> _listItem = new List<CommetsLike>();
        private ObservableCollection<ListViewItemSelectorModel> _newsListItem = new ObservableCollection<ListViewItemSelectorModel>();

        public string LoadePage { get; set; }
        public string NewsId { get; private set; } = string.Empty;

        public ParsingFullNewsPage(string page, string urlPage)
        {
            LoadePage = page;
            _urlPageNews = urlPage;
        }

        /// <summary>
        /// News information
        /// </summary>
        /// <returns>news item object</returns>
        public async Task<ObservableCollection<ListViewItemSelectorModel>> NewsMainInfo()
        {
            var fullNews = new FullItemNews();

            _htmlDoc.LoadHtml(LoadePage);

            NewsId = _htmlDoc.DocumentNode.Descendants(NameTagSpan).FirstOrDefault(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "news_view_count").Attributes["news_id"].Value;

            var likeApiUrl = GetUrlLikeApi(_urlPageNews, NewsId);
            var likeDataCollection = await _httpRequest.GetTypeRequestOnlinerAsync(likeApiUrl);
            SetCommentsList(likeDataCollection);

            var category = _htmlDoc.DocumentNode.Descendants(NameTagDiv).LastOrDefault(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-post-tags-1").Descendants(NameTagStrong).LastOrDefault().Descendants(NameTagA).LastOrDefault().InnerText;
            var time = _htmlDoc.DocumentNode.Descendants(NameTagDiv).LastOrDefault(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-post-tags-1").Descendants("time").FirstOrDefault().InnerText;
            var title = _htmlDoc.DocumentNode.Descendants("h3").LastOrDefault(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__title").Descendants(NameTagA).FirstOrDefault().InnerText;
            var image = _htmlDoc.DocumentNode.Descendants(NameTagFigure).LastOrDefault(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__image").Descendants(NameTagImg).FirstOrDefault().Attributes[AttributeTagSRC].Value;

            _newsListItem.Add(new ListViewItemSelectorModel("header", category, time));
            _newsListItem.Add(new ListViewItemSelectorModel("title", title));
            _newsListItem.Add(new ListViewItemSelectorModel("picture", image));

            //footer
            var footerNews = _htmlDoc.DocumentNode.Descendants("footer").FirstOrDefault(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-inner-pages-footer-1").InnerHtml;

            // p tag collection
            var listPTag = _htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__text").ToList();
            _htmlDoc.LoadHtml(listPTag[0].InnerHtml);

            foreach (var item in _htmlDoc.DocumentNode.ChildNodes)
            {
                if (item.Name.Equals("p"))
                {
                    if (item.InnerHtml.Contains("<img"))
                    {
                        _newsListItem.Add(new ListViewItemSelectorModel("image", ParsingUrlImage(item)));
                    }
                    else if (item.InnerHtml.Contains("iframe"))
                    {
                        if (item.InnerHtml.Contains("https://www.youtube.com/embed/"))
                        {
                            try
                            {
                                var uri = await ParsingYouTubeUrl(item);
                                _newsListItem.Add(new ListViewItemSelectorModel("video", uri.Uri));
                            }
                            catch (YouTubeUriNotFoundException)
                            {
                                _newsListItem.Add(new ListViewItemSelectorModel("web", item.InnerHtml));
                            }
                        }
                        else
                        {
                            _newsListItem.Add(new ListViewItemSelectorModel("web", item.InnerHtml));
                        }
                    }
                    else
                    {
                        _newsListItem.Add(new ListViewItemSelectorModel("story", item.InnerHtml));
                    }
                }
                if (item.Name.Equals("ul"))
                {
                    _newsListItem.Add(new ListViewItemSelectorModel("story", ParsingLiCollection(item.InnerHtml)));
                }
            }
            _newsListItem.Add(new ListViewItemSelectorModel("story", footerNews));
            return _newsListItem;
        }

        private string ParsingUrlImage(HtmlNode node) =>
            node.Descendants("img").FirstOrDefault().Attributes["src"].Value;

        private async Task<YouTubeUri> ParsingYouTubeUrl(HtmlNode node)
        {
            var uri = new YouTubeUri();
            var linkVideo = node.Descendants("iframe").FirstOrDefault().Attributes["src"].Value;
            return await GetYouTubeYriForControl(linkVideo);
        }

        private string ParsingLiCollection(string parsingContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(parsingContent);

            var liCollection = htmlDoc.DocumentNode.Descendants("li").ToList();
            return liCollection.Aggregate(string.Empty, (current, it) => current + it.InnerHtml + "<br>");
        }

        private void SetCommentsList(string collectionJSon)
        {
            var commentsValue = JsonConvert.DeserializeObject<CommentsClass>(collectionJSon);
            foreach (var item in commentsValue.Comments)
            {
                string value = item.Value.ToString();
                var commment = new CommetsLike
                {
                    ID = item.Name,
                    Item = JsonConvert.DeserializeObject<Item>(value)
                };
                _listItem.Add(commment);
            }
        }

        private async Task<YouTubeUri> GetYouTubeYriForControl(string url)
        {
            var pattern = @"(embed\/[-A-Za-z0-9]+)";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Multiline);
            var clearUrl = regex.Match(url);
            var path = clearUrl.ToString().Replace("embed/", "");
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
            _htmlDoc.LoadHtml(LoadePage);
            await Task.Run(() =>
            {
                var commentsList = _htmlDoc.DocumentNode.Descendants(NameTagLi).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-comments-1__list-item commentListItem").ToList();

                foreach (var item in commentsList)
                {
                    commentsParams = new Comments();
                    commentsParams.ID = item.Attributes["data-comment-id"].Value;
                    commentsParams.Nickname = item.Descendants(NameTagStrong).FirstOrDefault(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author").InnerText;
                    commentsParams.UserId = item.Attributes["data-author-id"].Value;
                    commentsParams.Time = item.Descendants(NameTagSpan).FirstOrDefault(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "date").InnerText;
                    commentsParams.Image = "https:" + item.Descendants(NameTagFigure).FirstOrDefault(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author-image").Descendants(NameTagImg).FirstOrDefault().Attributes[AttributeTagSRC].Value;
                    commentsParams.Data = item.InnerHtml;

                    var @params = commentsParams;
                    var commentsLikeCount = _listItem.FirstOrDefault(x => x.ID == @params.ID);
                    if (commentsLikeCount != null)
                    {
                        commentsParams.Like = commentsLikeCount.Item.Like;
                        commentsParams.LikeCount = commentsLikeCount.Item.Counter;
                        commentsParams.Best = commentsLikeCount.Item.Best;
                    }

                    _listComments.Add(commentsParams);
                }
            });
            return _listComments;
        }

        private HtmlView PostItemTextBlock(string text)
        {
            var textBlock = new HtmlView
            {
                Html = text,
                Margin = new Thickness(10)
            };
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
        [JsonProperty("comments")]
        public dynamic Comments { get; set; }
    }

    public class Item
    {
        [JsonProperty("counter")]
        public string Counter { get; set; }
        [JsonProperty("best")]
        public bool Best { get; set; }
        [JsonProperty("like")]
        public bool Like { get; set; }
    }

    public class CommetsLike
    {
        public string ID { get; set; }
        public Item Item { get; set; }
    }
}
