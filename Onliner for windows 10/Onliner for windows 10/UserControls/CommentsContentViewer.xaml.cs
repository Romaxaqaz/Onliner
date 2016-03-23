using HtmlAgilityPack;
using MyToolkit.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Onliner.Http;

namespace Onliner_for_windows_10.UserControls
{
    public sealed partial class CommentsContentViewer : UserControl
    {


        public static DependencyProperty HtmlContentProperty { private set; get; }
        public static new DependencyProperty FontSizeProperty { private set; get; }
        public static new DependencyProperty BorderThicknessProperty { private set; get; }

        public static string Page { get; set; }

        public class CommentsContent
        {
            public string Tag { get; set; }
            public string Content { get; set; }

            public CommentsContent(string tag, string content)
            {
                this.Tag = tag;
                this.Content = content;
            }
        }

        private HttpRequest HttpRequest = new HttpRequest();
        private HtmlDocument htmlDoc = new HtmlDocument();
        private ObservableCollection<CommentsContent> liststring = new ObservableCollection<CommentsContent>();
        private SolidColorBrush grayColor = new SolidColorBrush(Color.FromArgb(255, 190, 190, 190));

        private int countStep = 0;
        private int countBlock = 0;

        public CommentsContentViewer()
        {
            this.InitializeComponent();
        }

        static CommentsContentViewer()
        {
            FontSizeProperty = DependencyProperty.Register("FontSize",
                typeof(int),
                typeof(CommentsContentViewer),
                new PropertyMetadata(TextBlock.FontSizeProperty, null));

            BorderThicknessProperty = DependencyProperty.Register("BorderThicknessProperty",
                typeof(double),
                typeof(CommentsContentViewer),
                new PropertyMetadata(Grid.BorderThicknessProperty, null));

            HtmlContentProperty = DependencyProperty.Register("HtmlContentProperty",
               typeof(string),
               typeof(CommentsContentViewer),
               new PropertyMetadata(default(string), null));
        }

        public new int FontSize
        {
            set { SetValue(FontSizeProperty, value); }
            get { return (int)GetValue(FontSizeProperty); }
        }

        public new double BorderThickness
        {
            set { SetValue(BorderThicknessProperty, value); }
            get { return (double)GetValue(BorderThicknessProperty); }
        }

        public string HtmlContent
        {
            set
            {
                SetValue(HtmlContentProperty, value);
                SetGridContent(value);
            }
            get { return (string)GetValue(HtmlContentProperty); }
        }

        public static readonly DependencyProperty HtmlStringProperty =
        DependencyProperty.RegisterAttached("HtmlString", typeof(string), typeof(CommentsContentViewer), new PropertyMetadata("", OnHtmlStringChanged));

        public static string GetHtmlString(DependencyObject obj) { return (string)obj.GetValue(HtmlStringProperty); }
        public static void SetHtmlString(DependencyObject obj, string value) { obj.SetValue(HtmlStringProperty, value); }

        private static void OnHtmlStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = e.NewValue;
            CommentsContentViewer c = d as CommentsContentViewer;
            c.HtmlContent = s.ToString();
        }

        private void SetGridContent(string html)
        {
            if (string.IsNullOrEmpty(html)) return;
            htmlDoc.LoadHtml(html);

            StackPanel gridC = new StackPanel();

            List<HtmlNodeCollection> userList = htmlDoc.DocumentNode.Descendants().Where
               (x => (x.Name == "div" && x.Attributes["class"] != null &&
                x.Attributes["class"].Value.Contains("comment-content"))).
                Select(x => x.ChildNodes).ToList();

            foreach (var item in userList[0])
            {
                if (item.Name.Equals("blockquote"))
                {
                    liststring.Add(new CommentsContent("blockquote", item.InnerHtml));
                }
                else if (item.Name.Equals("p"))
                {
                    liststring.Add(new CommentsContent("p", item.InnerHtml));
                }
            }

            foreach (var item in liststring)
            {
                if (item.Tag.Equals("blockquote"))
                {
                    htmlDoc.LoadHtml(item.Content);
                    List<HtmlNode> countBlo = htmlDoc.DocumentNode.Descendants("blockquote").ToList();
                    countBlock = countBlo.Count + 1;
                    var bordres = ReturnBorderComplete(item.Content);
                    gridC.Children.Add(bordres);
                    countStep = 0;
                }
                else if (item.Tag.Equals("p"))
                {
                    HtmlView htmlViewr = new HtmlView();
                    htmlViewr.Html = item.Content;
                    htmlViewr.FontSize = 14;
                    htmlViewr.Foreground = (Brush)(Application.Current.Resources["ForegroundCustomOtherBlackBrush"]);
                    gridC.Children.Add(htmlViewr);
                }

            }
            liststring.Clear();
            ST.Children.Add(gridC);
        }

        private Border ReturnBorderComplete(string s)
        {
            Border mainBorder = new Border();
            StackPanel stackPanel = new StackPanel();

            if (countStep < countBlock)
            {
                countStep++;
                htmlDoc.LoadHtml(s);


                #region Border Create
                mainBorder.BorderThickness = new Thickness(1);
                mainBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 190, 190, 190));
                mainBorder.Margin = new Thickness(5, 5, 2, 2);
                #endregion

                #region stackPanel Create
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                #endregion

                if (countStep == countBlock)
                {
                    var htmlVewer = GetHtmlControl(htmlDoc.DocumentNode.Descendants("div").LastOrDefault().InnerHtml);
                    stackPanel.Children.Add(htmlVewer);
                }
                else
                {
                    var htmlVewer = GetHtmlControl(htmlDoc.DocumentNode.Descendants("div").FirstOrDefault().Descendants("cite").FirstOrDefault().InnerHtml);
                    stackPanel.Children.Add(htmlVewer);

                    string sss = string.Empty;

                    foreach (var node in htmlDoc.DocumentNode.Descendants().Where(x => x.Name == "p" || x.Name == "br"))
                    {
                        sss += node.NextSibling.InnerHtml;
                    }
                    var htmlVewerPost = GetHtmlControl(sss);
                    stackPanel.Children.Add(htmlVewerPost);

                    var buff = ReturnBorderComplete(htmlDoc.DocumentNode.Descendants("div").FirstOrDefault().Descendants("blockquote").FirstOrDefault().InnerHtml);
                    stackPanel.Children.Add(buff);
                }


            }
            mainBorder.Child = stackPanel;
            stackPanel = null;
            return mainBorder;
        }

        private HtmlView GetHtmlControl(string content)
        {
            HtmlView htmlview = new HtmlView();
            htmlview.Html = content.Replace("cite", "b").Replace("</b>", "</b><br>");
            htmlview.FontSize = 14;
            htmlview.Foreground = grayColor;
            return htmlview;
        }

    }
}
