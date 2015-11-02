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
using Onliner_for_windows_10.ParsingHtml;
using Windows.UI.Xaml.Media.Imaging;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views
{
    public sealed partial class NewsPage : Page
    {
        private ParsingNewsSection parsNewsSection;

        public NewsPage()
        {
            this.InitializeComponent();
            this.Loaded += MainOageLoaded;
        }

        private void ShowNews(GridView _myGridView, string path)
        {
            parsNewsSection = new ParsingNewsSection(path);
            MainPageProgressRing.IsActive = false;
            _myGridView.ItemsSource = parsNewsSection;
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

        private void NewsGridView_SelChange(object sender, SelectionChangedEventArgs e)
        {
            ItemsNews feedItem = e.AddedItems[0] as ItemsNews;
            Frame.Navigate(typeof(ViewNewsPage), feedItem.LinkNews.ToString());
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //change datatemplate
            TechGridView.ItemTemplate = (DataTemplate)this.Resources["ListViewMobileTrigger"];
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
           ((ProgressRing)((Grid)img.Parent).Children[0]).IsActive = false;
        }
    }
}
