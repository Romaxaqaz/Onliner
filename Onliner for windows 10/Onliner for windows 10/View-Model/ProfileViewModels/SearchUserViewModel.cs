using MyToolkit.Command;
using Onliner.Http;
using Onliner.Model.ProfileModel;
using Onliner.ParsingHtml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace Onliner_for_windows_10.View_Model.ProfileViewModels
{
    public class SearchUserViewModel : ViewModelBase
    {
        private const string UrlApiSearchProfile = "http://forum.onliner.by/memberlist.php?";
        private HttpRequest HttpRequest = new HttpRequest();
        private ParsingSearchUserResult searchResult = new ParsingSearchUserResult();

        private ObservableCollection<ProfileSearchModel> usersCollection = new ObservableCollection<ProfileSearchModel>();
        public ObservableCollection<ProfileSearchModel> UsersCollection
        {
            get { return usersCollection; }
            set { Set(ref usersCollection, value); }
        }

        public SearchUserViewModel()
        {
            SelectedItemCommand = new RelayCommand<object>((obj) => SelectedItem(obj));
            SearchCommand = new RelayCommand(async () => await SearchUsers());
        }

        private void SelectedItem(object obj)
        {
            ProfileSearchModel item = obj as ProfileSearchModel;
            NavigationService.Navigate(typeof(ProfilePage.ProfilePage), item.IdUser);
        }

        private async Task SearchUsers()
        {
            await LoadUsersResult();
        }

        private async Task LoadUsersResult()
        {
            var htmlResult = await HttpRequest.GetRequest(UrlApiSearchProfile, SearchBoxContent);
            UsersCollection = searchResult.GetResultList(htmlResult);
        }


        private string searchBoxContent;

        public string SearchBoxContent
        {
            get { return searchBoxContent; }
            set { Set(ref searchBoxContent, value); }
        }



        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand<object> SelectedItemCommand { get; private set; }
    }
}
