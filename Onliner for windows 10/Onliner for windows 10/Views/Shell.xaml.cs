using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.ProfilePage;
using Onliner_for_windows_10.Model.Message;
// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views
{
    public sealed partial class Shell : Page
    {
        private Request request = new Request();

        public Shell()
        {
            this.InitializeComponent();
            MessageStackPanel.DataContext = Additionalinformation.Instance;
            SplitViewPanePanel.DataContext = Additionalinformation.Instance;
            NavigationStackpanel.DataContext = Additionalinformation.Instance;
            Loaded += Shell_Loaded;
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            request.MessageUnread();
        }

        private void HomeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(MainPage))
            {
                frame.Navigate(typeof(MainPage));
            }
        }
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.SplitView.IsPaneOpen)
            {
                this.SplitView.IsPaneOpen = true;
            }
            else
            {
                this.SplitView.IsPaneOpen = false;
            }
        }
        private void NewsRadioButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(NewsPage))
            {
                this.SplitView.IsPaneOpen = false;
                frame.Navigate(typeof(NewsPage));
            }
        }
        private void CatalogRadioButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(CatalogPage))
            {
                this.SplitView.IsPaneOpen = false;
                frame.Navigate(typeof(CatalogPage));
            }
        }
        private void BaraholkaRadoButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(WeatherPage))
            {
                this.SplitView.IsPaneOpen = false;
                frame.Navigate(typeof(WeatherPage));
            }
        }
        private void ForumRadiadoButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(MainPage))
            {
                this.SplitView.IsPaneOpen = false;
                frame.Navigate(typeof(ViewNewsPage));
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            if (frame?.CanGoBack == true)
            {
                frame.GoBack();
            }
        }
        private void SettingRadioButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(MainPage))
            {
                frame.Navigate(typeof(MainPage));
            }
        }
        private static readonly string[] carList = {
                "Saturn", "Isuzu", "Toyota", "Nissan", "Ford", "Chevy", "Honda", "Hummer", "Tata", "Mahindra" };
        private void mySearchBox_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
            Windows.ApplicationModel.Search.SearchSuggestionCollection suggestionCollection = args.Request.SearchSuggestionCollection;
            foreach (string suggestion in carList)
            {
                if (suggestion.StartsWith(args.QueryText, StringComparison.CurrentCultureIgnoreCase))
                {
                    suggestionCollection.AppendQuerySuggestion(suggestion);
                }
            }
        }

        private void mySearchBox_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }

        private void Message_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(MessagePage))
            {
                frame.Navigate(typeof(MessagePage));
            }
        }

        private void WeatherButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(WeatherPage))
            {
                this.SplitView.IsPaneOpen = false;
                frame.Navigate(typeof(WeatherPage));
            }
        }

        private void MoneyButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(testpage))
            {
                this.SplitView.IsPaneOpen = false;
                frame.Navigate(typeof(testpage));
            }
        }

        private void Message_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void MessagePage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(ProfilePage.ProfilePage))
            {
                this.SplitView.IsPaneOpen = false;
                frame.Navigate(typeof(ProfilePage.ProfilePage));
            }
        }
    }
}
