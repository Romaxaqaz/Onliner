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
using static Onliner.SQLiteDataBase.SQLiteDB;
using Onliner.SQLiteDataBase;
using System.Net;
using Newtonsoft.Json;

namespace Onliner.ParsingHtml
{
    public class ParsingNewsSection : IEnumerable
    {
        #region Classes
        private ObservableCollection<ItemsNews> myItems;
        private HtmlDocument resultat = new HtmlDocument();
        private CategoryNews _categoryNews = new CategoryNews();
        private HttpRequest HttpRequest = new HttpRequest();
        private ItemsNews _itemNews;
        private OpinionModel opinionModel;
        private HttpClient client = new HttpClient();
        #endregion


        Dictionary<string, string> com = new Dictionary<string, string>();

        #region Collections
        private ObservableCollection<OpinionModel> _opinionsItems = new ObservableCollection<OpinionModel>();
        private ObservableCollection<ItemsNews> bufferNews;
        private List<string> NewsListId = new List<string>();

        public ObservableCollection<ItemsNews> OldNewsForUpdate
        {
            get { return bufferNews; }
        }
        #endregion

        #region Variables
        private string ResultHtmlPage = string.Empty;
        string urlNewsView = string.Empty;
        #endregion

        #region Constructor
        public ParsingNewsSection() { }
        /// <summary>
        /// parsing list news
        /// </summary>
        /// <param name="path">url page section news</param>
        public ParsingNewsSection(string path)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Parsing news section page
        /// </summary>
        /// <param name="path"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<ItemsNews>> NewsItemList(string path, string pathDB)
        {
            if (!HttpRequest.HasInternet()) return null;
            myItems = new ObservableCollection<ItemsNews>();
            await Task.Run(async () =>
            {
                HttpClient httpClient = new HttpClient();
                bufferNews = new ObservableCollection<ItemsNews>();
                ResultHtmlPage = await HttpRequest.GetRequestOnlinerAsync(path);
                resultat.LoadHtml(ResultHtmlPage);

                List<HtmlNode> titleList = resultat.DocumentNode.Descendants().Where
                (x => (x.Name == "article" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains
                ("b-posts-1-item b-content-posts-1-item news_for_copy"))).ToList();

                //db news collections
                var DBlist = await GetNeedList(pathDB);

                foreach (var item in titleList)
                {
                    _itemNews = new ItemsNews();
                    _itemNews.NewsID = item.Descendants("span").
                             Where(div => div.GetAttributeValue("class", string.Empty) == "show_news_view_count").FirstOrDefault().Attributes["news_id"].Value;
                    NewsListId.Add(QueryString(_itemNews.NewsID));


                    //data for update
                    _itemNews.LinkNews = item.Descendants("h3").
                             Where(div => div.GetAttributeValue("class", string.Empty) == "b-posts-1-item__title").FirstOrDefault().Descendants("a").FirstOrDefault().Attributes["href"].Value;
                    _itemNews.CountViews = GetFirstOrDefaultTwoTagInARowHtmlInnerText(item, "a", "span");
                    _itemNews.Footer = Regex.Replace(item.Descendants("span").
                           Where(div => div.GetAttributeValue("class", string.Empty) == "right-side").LastOrDefault().InnerText.Replace("\n", "").Trim(), @"\s+", " ");

                    var m = item?.Descendants("span").Where(div => div.GetAttributeValue("class", string.Empty) == "popular-count").FirstOrDefault();
                    if (m != null)
                        _itemNews.Popularcount = m.InnerText;

                    if (DBlist != null)
                    {
                        var containItem = DBlist.FirstOrDefault(x => x.LinkNews == _itemNews.LinkNews);
                        if (containItem != null)
                        {
                            bufferNews.Add(_itemNews);
                            continue;
                        }
                    }

                    _itemNews.Themes = GetFirstOrDefaultTwoTagInARowHtmlInnerText(item, "div", "strong");
                    _itemNews.Title = GetFirstOrDefaultThreeTagInARowHtmlInnerText(item, "h3", "a", "span");

                    string clearLink = ClearImageLink(GetFirstOrDefaultTwoTagInARowHtmlInnerHtml(item, "figure", "a"));
                    _itemNews.Image = await client.GetByteArrayAsync(clearLink);
                    _itemNews.Description = ScrubHtml(item.Descendants("div").LastOrDefault().Descendants("p").First().InnerText.Trim());

                    //photo
                    _itemNews.Bmediaicon = GetFirstOrDefaultTagHtmlInnerText(item, "span", "class", "b-mediaicon");
                    //video
                    _itemNews.Mediaicongray = GetFirstOrDefaultTagHtmlInnerText(item, "span", "class", "b-mediaicon gray");

                    myItems.Add(_itemNews);
                }


                var viewNewsComment = await HttpRequest.NewsViewAll(path, string.Join("", NewsListId));
                var m_res = JsonConvert.DeserializeObject<CounterNewsJsonClass>(viewNewsComment);
                foreach (dynamic numb in m_res.count)
                { 
                    var item = bufferNews.Where(x => x.NewsID.Equals(numb.Name.ToString())).FirstOrDefault();
                    if (item != null)
                        item.Popularcount = numb.Value.ToString();
                }


                //  await SQLiteDB.UpdateItemDB(bufferNews, section);
            });
            return myItems;

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
            var value = node.Descendants(tag).Where(div => div.GetAttributeValue(attributeType, string.Empty) == attributeValue).FirstOrDefault();
            if (value != null)
                return value.InnerText;
            return string.Empty;
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
            if (value != null)
                return value.InnerText.Trim();
            return string.Empty;
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
            if (value != null)
                return value.InnerHtml.Trim();
            return string.Empty;
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
            if (value != null)
                return value.InnerText.Trim();
            return string.Empty;
        }

        /// <summary>
        /// Gets the collection of data base
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        private async Task<IEnumerable<ItemsNews>> GetNeedList(string pathDB)
        {
            IEnumerable<ItemsNews> resultItems = new ObservableCollection<ItemsNews>();
            resultItems = await SQLiteDB.GetAllNews(pathDB);
            return resultItems;
        }

        /// <summary>
        /// Parsing section opinions
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ObservableCollection<OpinionModel> OpinionList(string path)
        {
            HttpRequest.GetRequestOnliner(path);
            ResultHtmlPage = HttpRequest.ResultGetRequsetString;
            resultat.LoadHtml(ResultHtmlPage);

            List<HtmlNode> opinionsItems = resultat.DocumentNode.Descendants().Where
            (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-opinions-body"))).
            FirstOrDefault().Descendants("div").Where(y => y.Attributes["class"].Value.Contains("b-opinions-list-item")).ToList();
            int step = 0;
            foreach (var item in opinionsItems)
            {
                if (step == 0 || step % 2 == 0)
                {
                    opinionModel = new OpinionModel();

                    opinionModel.LinkNews = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").FirstOrDefault().Descendants("a").FirstOrDefault().Attributes["href"].Value;
                    opinionModel.Header = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").FirstOrDefault().Descendants("a").FirstOrDefault().Descendants("h2").FirstOrDefault().InnerText;
                    opinionModel.Body = item.Descendants("p").FirstOrDefault().InnerText.Trim();
                    opinionModel.ImageUrl = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "person-portrait").FirstOrDefault().Attributes["style"].Value;
                    opinionModel.PersonAbout = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "person-about").FirstOrDefault().InnerText.Trim();
                    opinionModel.CommentsCount = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").FirstOrDefault().Descendants("a").LastOrDefault().Descendants("span").FirstOrDefault().InnerText;

                    _opinionsItems.Add(opinionModel);
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
            foreach (var item in myItems)
            {
                yield return item;
            }

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
            string pattern = @"https?://[\w./]+\/[\w./]+\.(bmp|png|jpg|gif|jpeg)";
            Regex htmlreg = new Regex("(?<=src=\").*?(?=\")");
            string result = htmlreg.Match(html).ToString();
            if (string.IsNullOrEmpty(result))
            {
                Regex imageUrlPattern = new Regex(pattern);
                result = imageUrlPattern.Match(html).ToString();
            }
            return result;
        }

        public async Task<byte[]> GetBytesFromStream(IRandomAccessStream randomStream)
        {
            var reader = new DataReader(randomStream.GetInputStreamAt(0));
            var bytes = new byte[randomStream.Size];
            await reader.LoadAsync((uint)randomStream.Size);
            reader.ReadBytes(bytes);
            return bytes;
        }

        private string QueryString(string newsID)
        {
            var strings = $"news%5B%5D={newsID}";
            return strings + "&";
        }

        #endregion
    }

    public class CounterNewsJsonClass
    {
        public dynamic count { get; set; }
    }
}
