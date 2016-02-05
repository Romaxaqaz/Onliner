using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.Model.ProfileModel;
using Onliner_for_windows_10.ParsingHtml;
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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views.Profile
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SearchUserPage : Page
    {
        private const string UrlApiSearchProfile = "http://forum.onliner.by/memberlist.php?";
        private Request request = new Request();
        private ParsingSearchUserResult searchResult = new ParsingSearchUserResult();

        public SearchUserPage()
        {
            this.InitializeComponent();
        }

        private async void AutoSuggestBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (AutoSuggestBox.Text != string.Empty)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    var htmlResult = await request.GetRequest(UrlApiSearchProfile, AutoSuggestBox.Text);
                    var resultList = searchResult.GetResultList(htmlResult);
                    ListviewUserResult.ItemsSource = resultList;
                }
            }
        }

        private void ListviewUserResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProfileSearchModel Item = e.AddedItems[0] as ProfileSearchModel;
            Frame.Navigate(typeof(ProfilePage.ProfilePage), Item.IdUser);
        }
    }
}
