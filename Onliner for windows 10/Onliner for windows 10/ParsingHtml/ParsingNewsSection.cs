using HtmlAgilityPack;
using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.Model;
using Onliner_for_windows_10.SQLiteDataBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using static Onliner_for_windows_10.SQLiteDataBase.SQLiteDB;

namespace Onliner_for_windows_10.ParsingHtml
{
    public class ParsingNewsSection : IEnumerable
    {
        private ObservableCollection<ItemsNews> myItems;
        private HtmlDocument resultat = new HtmlDocument();
        private CategoryNews _categoryNews = new CategoryNews();
        private Request request = new Request();
        private ItemsNews _itemNews;
        private OpinionModel opinionModel;
        private ObservableCollection<OpinionModel> _opinionsItems = new ObservableCollection<OpinionModel>();
        private string ResultHtmlPage = string.Empty;
        private HttpClient client = new HttpClient();
        public ParsingNewsSection() { }

        /// <summary>
        /// parsing list news
        /// </summary>
        /// <param name="path">url page section news</param>
        public ParsingNewsSection(string path)
        {

        }

        public async Task<ObservableCollection<ItemsNews>> NewsItemList(string path, SectionNewsDB section)
        {
            if (!request.HasInternet()) return null;
            myItems = new ObservableCollection<ItemsNews>();
            await Task.Run(async() =>
            {
                HttpClient httpClient = new HttpClient();
                ResultHtmlPage = await request.GetRequestOnlinerAsync(path);
                resultat.LoadHtml(ResultHtmlPage);

                List<HtmlNode> titleList = resultat.DocumentNode.Descendants().Where
                (x => (x.Name == "article" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-posts-1-item b-content-posts-1-item news_for_copy"))).ToList();

                var DBlist = await GetNeedList(section);

                foreach (var item in titleList)
                {
                    _itemNews = new ItemsNews();
                    _itemNews.LinkNews = item.Descendants("h3").Where(div => div.GetAttributeValue("class", string.Empty) == "b-posts-1-item__title").FirstOrDefault().Descendants("a").FirstOrDefault().Attributes["href"].Value;
                    var containItem = DBlist.Where(x => x.LinkNews.Contains(_itemNews.LinkNews)).Any(); if (containItem) continue;
                    _itemNews.CountViews = item.Descendants("a").FirstOrDefault().Descendants("span").FirstOrDefault().InnerText.Trim();
                    _itemNews.Themes = item.Descendants("div").FirstOrDefault().Descendants("strong").FirstOrDefault().InnerText.Trim();
                    _itemNews.Title = item.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().Descendants("span").FirstOrDefault().InnerText.Trim();
                    string clearLink = ClearImageLink(item.Descendants("figure").FirstOrDefault().Descendants("a").FirstOrDefault().InnerHtml.Trim());
                    _itemNews.Image = await client.GetByteArrayAsync(clearLink);
                    _itemNews.Description = ScrubHtml(item.Descendants("div").LastOrDefault().Descendants("p").First().InnerText.Trim());
                    _itemNews.Footer = Regex.Replace(item.Descendants("span").Where(div => div.GetAttributeValue("class", string.Empty) == "right-side").LastOrDefault().InnerText.Replace("\n", "").Trim(), @"\s+", " ");

 
                    //watch
                    if (item.Descendants("span").
                   Where(div => div.GetAttributeValue("class", string.Empty) == "b-mediaicon").
                   FirstOrDefault() != null)
                    {
                        _itemNews.Bmediaicon = item.Descendants("span").
                   Where(div => div.GetAttributeValue("class", string.Empty) == "b-mediaicon").
                   FirstOrDefault().InnerText;
                    }
                    //video
                    if (item.Descendants("span").Where(div => div.GetAttributeValue("class", string.Empty) == "b-mediaicon gray").FirstOrDefault() != null)
                    {
                        _itemNews.Mediaicongray = item.Descendants("span").Where(div => div.GetAttributeValue("class", string.Empty) == "b-mediaicon gray").
                   FirstOrDefault().InnerText;
                    }
                    //comments
                    if (item.Descendants("span").
                    Where(div => div.GetAttributeValue("class", string.Empty) == "popular-count").
                    FirstOrDefault() != null)
                    {
                        _itemNews.Popularcount = item.Descendants("span").Where(div => div.GetAttributeValue("class", string.Empty) == "popular-count").FirstOrDefault().InnerText;
                    }
                    myItems.Add(_itemNews);
                }
            });
            return myItems;

        }

        private async Task<IEnumerable<ItemsNews>> GetNeedList(SectionNewsDB section)
        {
            IEnumerable<ItemsNews> resultItems = new ObservableCollection<ItemsNews>();
            switch (section)
            {
                case SectionNewsDB.Tech:
                    resultItems = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_TECH);
                    break;
                case SectionNewsDB.People:
                    resultItems = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_PEOPLE);
                    break;
                case SectionNewsDB.Auto:
                    resultItems = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_AUTO);
                    break;
                case SectionNewsDB.House:
                    resultItems = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_HOUSE);
                    break;
                case SectionNewsDB.Opinion:
                    resultItems = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_OPINIONS);
                    break;
            }
            return resultItems;
        }

        public ObservableCollection<OpinionModel> OpinionList(string path)
        {
            request.GetRequestOnliner(path);
            ResultHtmlPage = request.ResultGetRequsetString;
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

        private static string ScrubHtml(string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;|Дальше…", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }

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

    }
}
