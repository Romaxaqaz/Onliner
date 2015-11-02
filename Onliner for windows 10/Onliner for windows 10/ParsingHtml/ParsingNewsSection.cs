using HtmlAgilityPack;
using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.ParsingHtml
{
    public class ParsingNewsSection : IEnumerable
    {
        private List<ItemsNews> myItems = new List<ItemsNews>();
        private HtmlDocument resultat = new HtmlDocument();
        private CategoryNews _categoryNews = new CategoryNews();
        private Request request = new Request();
        private ItemsNews _itemNews;

        public ParsingNewsSection(string path)
        {
            ParsingHtml parsHtml = new ParsingHtml();
            request.GetRequestOnliner(path);
            string resultGetRequest = request.ResultGetRequsetString;
            resultat.LoadHtml(resultGetRequest);

            List<HtmlNode> titleList = resultat.DocumentNode.Descendants().Where
            (x => (x.Name == "article" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-posts-1-item b-content-posts-1-item news_for_copy"))).ToList();

            myItems.Clear();
            foreach (var item in titleList)
            {
                _itemNews = new ItemsNews();
                _itemNews.CountViews = item.Descendants("a").FirstOrDefault().Descendants("span").FirstOrDefault().InnerText.Trim();
                _itemNews.Themes = item.Descendants("div").FirstOrDefault().Descendants("strong").FirstOrDefault().InnerText.Trim();
                _itemNews.Title = item.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().Descendants("span").FirstOrDefault().InnerText.Trim();
                _itemNews.LinkNews = item.Descendants("h3").Where(div => div.GetAttributeValue("class", string.Empty) == "b-posts-1-item__title").FirstOrDefault().Descendants("a").FirstOrDefault().Attributes["href"].Value;
                _itemNews.Image = item.Descendants("figure").FirstOrDefault().Descendants("a").FirstOrDefault().InnerHtml.Trim();
                _itemNews.Description = item.Descendants("div").LastOrDefault().Descendants("p").LastOrDefault().InnerText.Trim();
                _itemNews.Footer = item.Descendants("span").Where(div => div.GetAttributeValue("class", string.Empty) == "right-side").LastOrDefault().InnerText.Replace("\n", "").Trim();

                //watch
                if (item.Descendants("span").
               Where(div => div.GetAttributeValue("class", string.Empty) == "b-mediaicon").
               FirstOrDefault() != null)
                {
                    _itemNews.Bmediaicon = item.Descendants("span").
               Where(div => div.GetAttributeValue("class", string.Empty) == "b-mediaicon").
               FirstOrDefault().InnerText;
                }
                ///video
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
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var item in myItems)
            {
                yield return item;
            }
            
        }
    }
}
