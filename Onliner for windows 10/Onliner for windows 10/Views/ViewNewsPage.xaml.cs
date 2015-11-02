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
using Onliner_for_windows_10.UserControls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml.Documents;
using System.Text.RegularExpressions;
using MyToolkit.Multimedia;
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
                if (item.InnerHtml.Contains("<img>"))
                {
                    NewsListData.Children.Add(PostImage(item.Descendants("img").FirstOrDefault().Attributes["src"].Value));
                }
                else if (item.InnerHtml.Contains("iframe"))
                {
                    if (item.InnerHtml.Contains("https://www.youtube.com/embed/"))
                    {
                        string linkVideo = item.Descendants("iframe").FirstOrDefault().Attributes["src"].Value;
                        string path = linkVideo.Replace("https://www.youtube.com/embed/", "");
                        var pathUri = await YouTube.GetVideoUriAsync(path, YouTubeQuality.Quality480P);
                        NewsListData.Children.Add(VideoPost(pathUri));
                    }
                    else
                    {
                        WebView web = new WebView();
                        web.Settings.IsJavaScriptEnabled = true;
                        web.Width = ActualWidth - 10;
                        web.Height = 320;
                        web.NavigateToString(item.InnerHtml);
                        NewsListData.Children.Add(web);
                    }
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

        private void FontSizeSetting_Click(object sender, RoutedEventArgs e)
        {
            TextBlockFontSize tf = new TextBlockFontSize
            {
                FontSize = (int)ContentTextBlock.FontSize
            };

            Binding binding = new Binding
            {
                Source = tf,
                Path = new PropertyPath("FontSize"),
                Mode = BindingMode.TwoWay
            };
            ContentTextBlock.SetBinding(TextBlock.FontSizeProperty, binding);

            Popup popup = new Popup
            {
                Child = tf,
                IsLightDismissEnabled = true
            };

            tf.Loaded += (dialogSender, dialogArgs) =>
            {
                // Получение позиции кнопки относительно экрана
                Button btn = sender as Button;
                Point pt = btn.TransformToVisual(null).TransformPoint(new Point(btn.ActualWidth / 2,
                                                                                btn.ActualHeight / 2));

                popup.HorizontalOffset = pt.X - tf.ActualWidth / 2;

                popup.VerticalOffset = 100;
            };
            popup.IsOpen = true;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ListView list = new ListView();
            list.Width = ActualWidth;
            list.Height = ActualHeight;
            list = CommentsListView;
            Popup popup = new Popup();
            popup.Child = list;
            popup.HorizontalOffset = 0;
            popup.VerticalOffset = 50;
            popup.IsOpen = true;
        }
    }
}
