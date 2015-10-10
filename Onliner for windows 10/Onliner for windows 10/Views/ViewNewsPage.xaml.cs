using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Onliner_for_windows_10.ParsingHtml;
using Onliner_for_windows_10.Model;
using HtmlAgilityPack;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Input;
// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ViewNewsPage : Page
    {
        private ParsingFullNewsPage fullPagePars;
        private FullItemNews fullItem = new FullItemNews();
        private HtmlDocument htmlDoc = new HtmlDocument();
        private TextBlock textBlock = new TextBlock();
        private WebView web = new WebView();
        private Image image = new Image();

        public ViewNewsPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            string loadePage = e.Parameter.ToString();
            fullPagePars = new ParsingFullNewsPage(loadePage);
            fullItem = await fullPagePars.NewsMainInfo();
            MainNewsData.DataContext = fullItem;
            htmlDoc.LoadHtml(fullItem.PostItem);

            var contentNews = htmlDoc.DocumentNode.Descendants("p").ToList();
            foreach (var item in contentNews)
            {
                if (item.InnerHtml.Contains("img"))
                {
                    NewsListData.Children.Add(PostImage(item.Descendants("img").FirstOrDefault().Attributes["src"].Value));
                }
                else if (item.InnerHtml.Contains("iframe"))
                {
                    string linkVideo = item.Descendants("iframe").FirstOrDefault().Attributes["src"].Value;
                    NewsListData.Children.Add(VideoPost(linkVideo));
                }
                else
                {
                    NewsListData.Children.Add(PostItemTextBlock(item.InnerText));
                }
            }
            CommentsListView.ItemsSource = await fullPagePars.CommentsMainInfo();
        }

        private TextBlock PostItemTextBlock(string text)
        {
            textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Margin = new Thickness(0, 0, 0, 10);
            return textBlock;
        }

        private Image PostImage(string uri)
        {
            image = new Image();
            image.Source = new BitmapImage(new Uri(uri));
            return image;
        }

        private WebView VideoPost(string linkVideo)
        {
            web = new WebView();
            web.Width = 300;
            web.Height = 315;
            web.Width = MainGrid.ColumnDefinitions[1].ActualWidth;
            web.NavigateToString("<iframe src = " + linkVideo + " allowfullscreen = \"\" frameborder =\"0\" width = \"100%\" height = \"315\" ></iframe>");
            return web;
        }
    }
}
