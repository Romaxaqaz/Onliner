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
using MyToolkit.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using MyToolkit.Multimedia;
using Onliner_for_windows_10.Views;

namespace Onliner_for_windows_10.ParsingHtml
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

        private readonly string BackGroundColorListItem = "#FFF7F7F7";
        private string urlPageNews = string.Empty;
        private string loadePage = string.Empty;

        private Request request = new Request();
        private HtmlDocument htmlDoc = new HtmlDocument();
        private List<FullItemNews> listNews = new List<FullItemNews>();
        private List<CommentsItem> listComments = new List<CommentsItem>();
        private List<string> listDataContent = new List<string>();
        private List<UIElement> controlList = new List<UIElement>();

        private int countNews = 0;
        public int CountPTag { get { return countNews; } }

        public string LoadePage { get { return loadePage; } }

        public ParsingFullNewsPage(string page)
        {
            urlPageNews = page;
            loadePage = GetHtmlPage();
        }
        private string GetHtmlPage()
        {
            //
            //get request
            //
            request.GetRequestOnliner(urlPageNews);
            string resultGetRequest = request.ResultGetRequsetString;
            return resultGetRequest;
        }
        //
        //Get collention news data
        //
        public async Task<FullItemNews> NewsMainInfo()
        {
            FullItemNews fullNews = new FullItemNews();
            htmlDoc.LoadHtml(loadePage);
            await Task.Run(() =>
            {
                fullNews.NewsID = htmlDoc.DocumentNode.Descendants(NameTagSpan).Where(div => div.GetAttributeValue(TagTypeClass, string.Empty) == "news_view_count").FirstOrDefault().Attributes["news_id"].Value;
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

        public async Task<List<UIElement>> PostItemDate(string loaderPage, FullItemNews fullItem)
        {

            //<ul><li><a href="http://tech.onliner.by/2015/10/01/top-5-apps-september-2015/">Топ бесплатных приложений в сентябре</a></li>

            htmlDoc.LoadHtml(fullItem.PostItem);
            var contentNews = htmlDoc.DocumentNode.Descendants("p").ToList();
            var contentNewsULtag = htmlDoc.DocumentNode.Descendants("ul").ToList();
            countNews = contentNews.Count;

            foreach (var item in contentNews)
            {
                if (item.InnerHtml.Contains("<img"))
                {
                    controlList.Add(PostImage(item.Descendants("img").FirstOrDefault().Attributes["src"].Value));
                }
                else if (item.InnerHtml.Contains("iframe"))
                {
                    if (item.InnerHtml.Contains("https://www.youtube.com/embed/"))
                    {
                        string linkVideo = item.Descendants("iframe").FirstOrDefault().Attributes["src"].Value;
                        string path = linkVideo.Replace("https://www.youtube.com/embed/", "");
                        var pathUri = await YouTube.GetVideoUriAsync(path, YouTubeQuality.Quality480P);
                        controlList.Add(VideoPost(pathUri));
                    }
                    else
                    {
                        WebView web = new WebView();
                        web.Settings.IsJavaScriptEnabled = true;
                        web.Width = 640;
                        web.Height = 320;
                        web.NavigateToString(item.InnerHtml);
                        controlList.Add(web);
                    }
                }
                else
                {

                    controlList.Add(PostItemTextBlock(item.InnerHtml));
                }
                
            }
            if (contentNewsULtag != null)
            {
                foreach (var itemLi in contentNewsULtag)
                {
                    controlList.Add(PostItemTextBlock(itemLi.InnerHtml));
                }
            }
            return controlList;
        }

        //
        //get comments collention
        //
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

        private HtmlView PostItemTextBlock(string text)
        {
            HtmlView textBlock = new HtmlView();
            textBlock.Html = text;
            textBlock.Margin = new Thickness(10);
            return textBlock;
        }

        private HyperlinkButton HyperlinkBlock(string url)
        {
            HyperlinkButton hyperlinkButton = new HyperlinkButton();
            hyperlinkButton.NavigateUri = new Uri(url);
            hyperlinkButton.Click += HyperlinkButton_Click;
            return hyperlinkButton;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var hyperlinkButton = sender as HyperlinkButton;
            Frame framew = new Frame();
            framew.Navigate(typeof(ViewNewsPage), hyperlinkButton.NavigateUri.ToString());
        }

        private Image PostImage(string uri)
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(uri));
            image.Margin = new Thickness(5);
            return image;
        }

        private MediaElement VideoPost(YouTubeUri linkVideo)
        {
            MediaElement media = new MediaElement();
            media.AutoPlay = false;
            media.AreTransportControlsEnabled = true;
            media.Margin = new Thickness(15);
            media.Source = linkVideo.Uri;
            return media;
        }
    }




}
