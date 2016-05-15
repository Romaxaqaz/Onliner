using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using HtmlAgilityPack;
using Onliner.Model.News;
using Onliner.Http;
using Onliner.Model.OpinionsModel;
using static Onliner.SQLiteDataBase.SqLiteDb;
using Newtonsoft.Json;
using Onliner.Interface.News;

namespace Onliner.ParsingHtml
{
    public class ParsingNewsSection : IEnumerable
    {
        #region Classes
        private ObservableCollection<ItemsNews> _myItems;
        private readonly HtmlDocument _resultat = new HtmlDocument();
        private readonly HttpRequest _httpRequest = new HttpRequest();
        private ItemsNews _itemNews;
        private OpinionModel _opinionModel;
        private readonly HttpClient _client = new HttpClient();
        #endregion

        Dictionary<string, string> _com = new Dictionary<string, string>();

        #region Collections
        private ObservableCollection<OpinionModel> _opinionsItems = new ObservableCollection<OpinionModel>();
        private List<string> _newsListId = new List<string>();

        public ObservableCollection<ItemsNews> OldNewsForUpdate { get; private set; }

        #endregion

        #region Variables
        private string _resultHtmlPage = string.Empty;

        #endregion

        #region Constructor

        public ParsingNewsSection() { }
        #endregion

        #region Methods

        /// <summary>
        /// Parsing news section page
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathDb"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<ItemsNews>> NewsItemList(string path, string pathDb)
        {
            if (!_httpRequest.HasInternet()) return null;
            _myItems = new ObservableCollection<ItemsNews>();

            await Task.Run(async () =>
            {
                OldNewsForUpdate = new ObservableCollection<ItemsNews>();
                _resultHtmlPage = await _httpRequest.GetRequestOnlinerAsync(path);
                _resultat.LoadHtml(_resultHtmlPage);

                var titleList = _resultat.DocumentNode.Descendants().Where
                (x => (x.Name == "article" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains
                ("b-posts-1-item b-content-posts-1-item news_for_copy"))).ToList();

                //db news collections
                var dbNewsList = await GetNeedList(pathDb);

                foreach (var item in titleList)
                {
                    _itemNews = new ItemsNews();
                    _itemNews.NewsId = item.
                             Descendants("span").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "show_news_view_count").Attributes["news_id"].Value;
                    _newsListId.Add(QueryString(_itemNews.NewsId));

                    //data for update
                    _itemNews.LinkNews = item.
                                            Descendants("h3").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "b-posts-1-item__title").Descendants("a").FirstOrDefault().Attributes["href"].Value;
                    _itemNews.CountViews = GetFirstOrDefaultTwoTagInARowHtmlInnerText(item, "a", "span");
                    _itemNews.Footer = Regex.Replace(item.
                                            Descendants("span").LastOrDefault(div => div.GetAttributeValue("class", string.Empty) == "right-side").InnerText.Replace("\n", "").Trim(), @"\s+", " ");

                    var popularCount = (item?.Descendants("span")).FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "popular-count");
                    if (popularCount != null)
                        _itemNews.Popularcount = popularCount.InnerText;

                    var containItem = dbNewsList?.FirstOrDefault(x => x.LinkNews == _itemNews.LinkNews);
                    if (containItem != null)
                    {
                        OldNewsForUpdate.Add(_itemNews);
                        continue;
                    }

                    _itemNews.Themes = GetFirstOrDefaultTwoTagInARowHtmlInnerText(item, "div", "strong");
                    _itemNews.Title = GetFirstOrDefaultThreeTagInARowHtmlInnerText(item, "h3", "a", "span");

                    var clearLink = ClearImageLink(GetFirstOrDefaultTwoTagInARowHtmlInnerHtml(item, "figure", "a"));
                    _itemNews.Image = await _client.GetByteArrayAsync(clearLink);
                    _itemNews.Description = ScrubHtml(item.Descendants("div").LastOrDefault().Descendants("p").First().InnerText.Trim());

                    //photo
                    _itemNews.Bmediaicon = GetFirstOrDefaultTagHtmlInnerText(item, "span", "class", "b-mediaicon");
                    //video
                    _itemNews.Mediaicongray = GetFirstOrDefaultTagHtmlInnerText(item, "span", "class", "b-mediaicon gray");

                    _myItems.Add(_itemNews);
                }
                UpdateViewNews(OldNewsForUpdate, path);
            });
            return _myItems;

        }

        private async void UpdateViewNews(IEnumerable<INewsItems> collections, string pathNews)
        {
            var viewNewsComment = await _httpRequest.NewsViewAll(pathNews, string.Join("", _newsListId));
            var mRes = JsonConvert.DeserializeObject<CounterNewsJsonClass>(viewNewsComment);
            foreach (dynamic numb in mRes.Count)
            {
                var item = collections.FirstOrDefault(x => x.NewsId.Equals(numb.Name.ToString()));
                if (item != null)
                    item.Popularcount = numb.Value.ToString();
            }
        }


        /// <summary>
        /// Parsing html linq one tag.
        /// Example: item.Descendants("tag").Where(div => div.GetAttributeValue("attributeType", string.Empty) == "attributeValue").FirstOrDefault();
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tag"></param>
        /// <param name="attributeType"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        private string GetFirstOrDefaultTagHtmlInnerText(HtmlNode node, string tag, string attributeType, string attributeValue)
        {
            var value = node.Descendants(tag).FirstOrDefault(div => div.GetAttributeValue(attributeType, string.Empty) == attributeValue);
            return value != null ? value.InnerText : string.Empty;
        }

        /// <summary>
        /// Parsing two tag in a row
        /// </summary>
        /// <param name="node"></param>
        /// <param name="firstTag"></param>
        /// <param name="secondTag"></param>
        /// <returns>html string</returns>
        private string GetFirstOrDefaultTwoTagInARowHtmlInnerText(HtmlNode node, string firstTag, string secondTag)
        {
            var value = node.Descendants(firstTag).FirstOrDefault().Descendants(secondTag).FirstOrDefault();
            return value != null ? value.InnerText.Trim() : string.Empty;
        }

        /// <summary>
        /// Parsing two tag in a row
        /// </summary>
        /// <param name="node"></param>
        /// <param name="firstTag"></param>
        /// <param name="secondTag"></param>
        /// <returns>text string</returns>
        private string GetFirstOrDefaultTwoTagInARowHtmlInnerHtml(HtmlNode node, string firstTag, string secondTag)
        {
            var value = node.Descendants(firstTag).FirstOrDefault().Descendants(secondTag).FirstOrDefault();
            return value != null ? value.InnerHtml.Trim() : string.Empty;
        }

        /// <summary>
        /// Parsing trhee tag in a row
        /// </summary>
        /// <param name="node"></param>
        /// <param name="firstTag"></param>
        /// <param name="secondTag"></param>
        /// <param name="thirdTag"></param>
        /// <returns>text string</returns>
        private string GetFirstOrDefaultThreeTagInARowHtmlInnerText(HtmlNode node, string firstTag, string secondTag, string thirdTag)
        {
            var value = node.Descendants(firstTag).FirstOrDefault().Descendants(secondTag).FirstOrDefault().Descendants(thirdTag).FirstOrDefault();
            return value != null ? value.InnerText.Trim() : string.Empty;
        }

        /// <summary>
        /// Gets the collection of data base
        /// </summary>
        /// <param name="pathDb"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<ItemsNews>> GetNeedList(string pathDb)
        {
            IEnumerable<ItemsNews> resultItems = await GetAllNews(pathDb);
            return resultItems;
        }

        /// <summary>
        /// Parsing section opinions
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ObservableCollection<OpinionModel> OpinionList(string path)
        {
            _httpRequest.GetRequestOnliner(path);
            _resultHtmlPage = _httpRequest.ResultGetRequsetString;
            _resultat.LoadHtml(_resultHtmlPage);

            var opinionsItems = _resultat.DocumentNode.Descendants().
            FirstOrDefault(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-opinions-body"))).Descendants("div").Where(y => y.Attributes["class"].Value.Contains("b-opinions-list-item")).ToList();
            var step = 0;
            foreach (var item in opinionsItems)
            {
                if (step == 0 || step % 2 == 0)
                {
                    _opinionModel = new OpinionModel();

                    _opinionModel.LinkNews = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").Descendants("a").FirstOrDefault().Attributes["href"].Value;
                    _opinionModel.Header = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").Descendants("a").FirstOrDefault().Descendants("h2").FirstOrDefault().InnerText;
                    _opinionModel.Body = item.Descendants("p").FirstOrDefault().InnerText.Trim();
                    _opinionModel.ImageUrl = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "person-portrait").Attributes["style"].Value;
                    _opinionModel.PersonAbout = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "person-about").InnerText.Trim();
                    _opinionModel.CommentsCount = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").Descendants("a").LastOrDefault().Descendants("span").FirstOrDefault().InnerText;

                    _opinionsItems.Add(_opinionModel);
                }
                step++;
            }
            return _opinionsItems;
        }

        /// <summary>
        /// list news item
        /// </summary>
        /// <returns>items</returns>
        public IEnumerator GetEnumerator()
        {
            return _myItems.GetEnumerator();
        }

        /// <summary>
        /// Clear the text from tags
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ScrubHtml(string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;|Дальше…", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }

        /// <summary>
        /// Clear the image link
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string ClearImageLink(string html)
        {
            var pattern = @"https?://[\w./]+\/[\w./]+\.(bmp|png|jpg|gif|jpeg)";
            var htmlreg = new Regex("(?<=src=\").*?(?=\")");
            var result = htmlreg.Match(html).ToString();
            if (!string.IsNullOrEmpty(result)) return result;
            var imageUrlPattern = new Regex(pattern);
            return imageUrlPattern.Match(html).ToString();
        }

        public async Task<byte[]> GetBytesFromStream(IRandomAccessStream randomStream)
        {
            var reader = new DataReader(randomStream.GetInputStreamAt(0));
            var bytes = new byte[randomStream.Size];
            await reader.LoadAsync((uint)randomStream.Size);
            reader.ReadBytes(bytes);
            return bytes;
        }

        private string QueryString(string newsId)
        {
            var strings = $"news%5B%5D={newsId}";
            return strings + "&";
        }

        #endregion
    }

    public class CounterNewsJsonClass
    {
        public dynamic Count { get; set; }
    }
}
