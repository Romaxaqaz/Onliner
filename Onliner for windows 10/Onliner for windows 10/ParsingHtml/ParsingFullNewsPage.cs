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
        private readonly string TagTypeClass = "class";
        private readonly string NameTagDiv = "div";
        private readonly string NameTagSpan = "span";
        private readonly string NameTagStrong = "strong";
        private readonly string NameTagFigure = "figure";
        private readonly string NameTagImg = "img";
        private readonly string NameTagA = "a";
        private readonly string NameTagLi = "li";
        private readonly string AttributeTagSRC = "src";

        private readonly string BackGroundColorListItem = "#FFF7F7F7";
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
                fullNews.Category = htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-post-tags-1").LastOrDefault().Descendants(NameTagStrong).LastOrDefault().Descendants(NameTagA).LastOrDefault().InnerText;
                fullNews.DataTime = htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-post-tags-1").LastOrDefault().Descendants("time").FirstOrDefault().InnerText;
                fullNews.TitleNews = htmlDoc.DocumentNode.Descendants("h3").Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__title").LastOrDefault().Descendants(NameTagA).FirstOrDefault().InnerText;
                fullNews.Image = htmlDoc.DocumentNode.Descendants(NameTagFigure).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__image").LastOrDefault().Descendants(NameTagImg).FirstOrDefault().Attributes[AttributeTagSRC].Value;

                var ListPTag = htmlDoc.DocumentNode.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-posts-1-item__text").ToList();
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
                var commentsList = htmlDoc.DocumentNode.Descendants(NameTagLi).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "b-comments-1__list-item commentListItem").ToList();
                foreach (var item in commentsList)
                {
                    step++;
                    commentsParams = new CommentsItem();
                    commentsParams.Nickname = item.Descendants(NameTagStrong).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author").FirstOrDefault().InnerText;
                    commentsParams.Time = item.Descendants(NameTagSpan).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "date").FirstOrDefault().InnerText;
                    commentsParams.Image = item.Descendants(NameTagFigure).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "author-image").FirstOrDefault().Descendants(NameTagImg).FirstOrDefault().Attributes[AttributeTagSRC].Value;
                    commentsParams.Data = item.Descendants(NameTagDiv).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "comment-content").LastOrDefault().InnerText.Trim();
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


    }




}
