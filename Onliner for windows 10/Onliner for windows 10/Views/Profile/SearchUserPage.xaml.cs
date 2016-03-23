using Onliner.Http;
using Onliner.Model.ProfileModel;
using Onliner.ParsingHtml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views.Profile
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SearchUserPage : Page
    {
        private const string UrlApiSearchProfile = "http://forum.onliner.by/memberlist.php?";
        private HttpRequest HttpRequest = new HttpRequest();
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
                    var htmlResult = await HttpRequest.GetRequest(UrlApiSearchProfile, AutoSuggestBox.Text);
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
