using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Onliner_for_windows_10.Model;
using Onliner_for_windows_10.ParsingHtml;
using Onliner_for_windows_10.ProfilePage;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;

namespace Onliner_for_windows_10.Views
{
    public sealed partial class NewsPage : Page
    {
        private ParsingNewsSection parsNewsSection;
        private CategoryNews _categoryNews = new CategoryNews();

        public NewsPage()
        {
            this.InitializeComponent();
            this.Loaded += MainOageLoaded;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        private void ShowNews(GridView _myGridView, string path)
        {
            parsNewsSection = new ParsingNewsSection(path);
            _myGridView.ItemsSource = parsNewsSection;
        }

        private async void ChoiceDow(int index)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        ShowNews(TechGridView, "http://tech.onliner.by/");
                        tech.IsChecked = true;
                        break;
                    case 1:
                        ShowNews(PeopleGridView, "http://people.onliner.by/");
                        people.IsChecked = true;
                        break;
                    case 2:
                        ShowNews(AutoGridView, "http://auto.onliner.by/");
                        auto.IsChecked = true;
                        break;
                    case 3:
                        ShowNews(HouseGridView, "http://realt.onliner.by/");
                        house.IsChecked = true;
                        break;
                    case 4:
                        soc.IsChecked = true;
                        break;
                }
            }
            catch (FormatException ex)
            {
                MessageDialog message = new MessageDialog(ex.ToString());
                await message.ShowAsync();
            }
        }

        private void MainOageLoaded(object sender, RoutedEventArgs e)
        {
            Additionalinformation.Instance.NameActivePage = "Новости";
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var rad = sender as RadioButton;
            if (rad != null)
            {
                int index = Convert.ToInt32(rad.Tag);
                FlipNews.SelectedIndex = index;
                FlipNews.SelectionChanged -= FlipNews_SelectionChanged;
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

        private void FlipNews_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlipView flipView = sender as FlipView;
            ChoiceDow(flipView.SelectedIndex);
        }
    }
}
