using HtmlAgilityPack;
using Onliner_for_windows_10.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Onliner_for_windows_10.Model;
using Windows.UI.Xaml.Controls;
using Windows.UI;

namespace Onliner_for_windows_10.ParsingHtml
{
    public class ParsingFullNewsPage
    {
        private const string BackGroundColorListItem = "LightGray";
        private string urlPageNews = string.Empty;
        private string loadePage = string.Empty;

        private Request request = new Request();
        private HtmlDocument htmlDoc = new HtmlDocument();
        private List<FullItemNews> listNews = new List<FullItemNews>();
        private List<CommentsItem> listComments = new List<CommentsItem>();
        private List<string> listDataContent = new List<string>();

        public string LoadePage { get { return loadePage; } }

        public ParsingFullNewsPage(string page)
        {
            urlPageNews = page;
            loadePage = GetHtmlPage();
        }
        private string GetHtmlPage()
        {
            request.GetRequestOnliner(urlPageNews);
            string resultGetRequest = request.ResultGetRequsetString;
            return resultGetRequest;
        }

        public async Task<FullItemNews> NewsMainInfo()
        {
            FullItemNews fullNews = new FullItemNews();
            htmlDoc.LoadHtml(loadePage);
            await Task.Run(() =>
            {
                fullNews.Category = htmlDoc.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "b-post-tags-1").LastOrDefault().Descendants("strong").LastOrDefault().Descendants("a").LastOrDefault().InnerText;
                fullNews.DataTime = htmlDoc.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "b-post-tags-1").LastOrDefault().Descendants("time").FirstOrDefault().InnerText;
                fullNews.TitleNews = htmlDoc.DocumentNode.Descendants("h3").Where(div => div.GetAttributeValue("class", string.Empty) == "b-posts-1-item__title").LastOrDefault().Descendants("a").FirstOrDefault().InnerText;
                fullNews.Image = htmlDoc.DocumentNode.Descendants("figure").Where(div => div.GetAttributeValue("class", string.Empty) == "b-posts-1-item__image").LastOrDefault().Descendants("img").FirstOrDefault().Attributes["src"].Value;

                var ListPTag = htmlDoc.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "b-posts-1-item__text").ToList();
                foreach (var item in ListPTag)
                {
                    fullNews.PostItem += item.InnerHtml;
                }
            });
            return fullNews;
        }

        public async Task<List<CommentsItem>> CommentsMainInfo()
        {
            CommentsItem commentsParams;
            htmlDoc.LoadHtml(loadePage);
            int step = 0;
            await Task.Run(() =>
            {
                var commentsList = htmlDoc.DocumentNode.Descendants("li").Where(div => div.GetAttributeValue("class", string.Empty) == "b-comments-1__list-item commentListItem").ToList();
                foreach (var item in commentsList)
                {
                    step++;
                    commentsParams = new CommentsItem();
                    commentsParams.Nickname = item.Descendants("strong").Where(div => div.GetAttributeValue("class", string.Empty) == "author").FirstOrDefault().InnerText;
                    commentsParams.Time = item.Descendants("span").Where(div => div.GetAttributeValue("class", string.Empty) == "date").FirstOrDefault().InnerText;
                    commentsParams.Image = item.Descendants("figure").Where(div => div.GetAttributeValue("class", string.Empty) == "author-image").FirstOrDefault().Descendants("img").FirstOrDefault().Attributes["src"].Value;
                    commentsParams.Data = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "comment-content").LastOrDefault().InnerText.Trim();
                    commentsParams.LikeCount = item.Descendants("span").Where(div => div.GetAttributeValue("class", string.Empty) == "_counter").LastOrDefault().InnerText;
                    if (step % 2 == 0)
                    {
                        commentsParams.ColorItem = BackGroundColorListItem;
                    }
                    listComments.Add(commentsParams);
                }
            });

            return listComments;
        }


    }




}
