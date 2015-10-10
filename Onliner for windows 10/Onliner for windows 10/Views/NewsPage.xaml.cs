using HtmlAgilityPack;
using Onliner_for_windows_10.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Onliner_for_windows_10.Model;
using Windows.UI.Xaml.Media.Imaging;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views
{
    public sealed partial class NewsPage : Page
    {
        private List<ItemsNews> myItems = new List<ItemsNews>();
        private HtmlDocument resultat = new HtmlDocument();
        private CategoryNews _categoryNews = new CategoryNews();
        private Request request = new Request();
        private ItemsNews _itemNews;

        public NewsPage()
        {
            this.InitializeComponent();
            this.Loaded += MainOageLoaded;

        }

        private void ShowNews(GridView _myGridView, string path)
        {
            ParsingHtml.ParsingHtml parsHtml = new ParsingHtml.ParsingHtml();
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
                _itemNews.Footer = item.Descendants("span").Where(div => div.GetAttributeValue("class", string.Empty) == "right-side").LastOrDefault().InnerText.Trim();

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
            MainPageProgressRing.IsActive = false;
            _myGridView.ItemsSource = myItems;
        }

        private void ChoiceDow(int index)
        {
            switch (index)
            {
                case 0:
                    ShowNews(TechGridView, "http://tech.onliner.by/");
                    // CategoryNews.ItemsSource = _categoryNews.GetTechCategory();
                    break;
                case 1:
                    ShowNews(PeopleGridView, "http://people.onliner.by/");
                    //CategoryNews.ItemsSource = _categoryNews.GetPeopleCategory();
                    break;
                case 2:
                    ShowNews(AutoGridView, "http://auto.onliner.by/");
                    // CategoryNews.ItemsSource = _categoryNews.GetAutoCategory();
                    break;
                case 3:
                    ShowNews(HouseGridView, "http://realt.onliner.by/");
                    // CategoryNews.ItemsSource = _categoryNews.GetHouseCategory();
                    break;
                case 4:
                    break;

            }
        }

        private void MainOageLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var rad = sender as RadioButton;
            if (rad != null)
            {
                int index = Convert.ToInt32(rad.Tag);
                _flip.SelectedIndex = index;
                ChoiceDow(index);
            }
        }

        private void myGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var grid = sender as GridView;
            var panel = (ItemsWrapGrid)grid.ItemsPanelRoot;

            if (e.NewSize.Width <= 500)
            {
                panel.MaximumRowsOrColumns = 1;
                panel.ItemWidth = e.NewSize.Width;
             
            }
            if (e.NewSize.Width > 500 && e.NewSize.Width <= 640)
            {
                panel.MaximumRowsOrColumns = 2;
                panel.ItemWidth = e.NewSize.Width / 2;
              
            }
            else
            if (e.NewSize.Width > 640 && e.NewSize.Width <= 1024)
            {
                panel.MaximumRowsOrColumns = 3;
                panel.ItemWidth = (e.NewSize.Width) / 3;
            }
            else
            if (e.NewSize.Width > 1024)
            {
                panel.MaximumRowsOrColumns = 4;
                panel.ItemWidth = (e.NewSize.Width) / 4;
                
            }

        }

        private void NewsImage_Loading(FrameworkElement sender, object args)
        {
           
        }

        private void NewsImage_Loaded(object sender, RoutedEventArgs e)
        {
    
        }

        private void NewsImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            if (img.Name == "NewsImage")
            {
                img.Visibility = Visibility.Visible;
                
            }
        }



        private void NewsGridView_SelChange(object sender, SelectionChangedEventArgs e)
        {

         
            ItemsNews feedItem = e.AddedItems[0] as ItemsNews;
            Frame.Navigate(typeof(ViewNewsPage), feedItem.LinkNews.ToString());

        }
    }
}
