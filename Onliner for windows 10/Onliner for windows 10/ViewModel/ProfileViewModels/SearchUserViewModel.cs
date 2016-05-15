using MyToolkit.Command;
using Onliner.Http;
using Onliner.Model.ProfileModel;
using Onliner.ParsingHtml;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace OnlinerApp.View_Model.ProfileViewModels
{
    public class SearchUserViewModel : ViewModelBase
    {
        private const string UrlApiSearchProfile = "http://forum.onliner.by/memberlist.php?";
        private readonly HttpRequest _httpRequest = new HttpRequest();
        private readonly ParsingSearchUserResult _searchResult = new ParsingSearchUserResult();

        #region Collections
        private ObservableCollection<ProfileSearchModel> _usersCollection = new ObservableCollection<ProfileSearchModel>();
        public ObservableCollection<ProfileSearchModel> UsersCollection
        {
            get { return _usersCollection; }
            set { Set(ref _usersCollection, value); }
        }
        #endregion

        #region Constructor
        public SearchUserViewModel()
        {
            SelectedItemCommand = new RelayCommand<object>(SelectedItem);
            SearchCommand = new RelayCommand(async () => await SearchUsers());
        }
        #endregion

        #region Methods
        private void SelectedItem(object obj)
        {
            var item = obj as ProfileSearchModel;
            if (item != null) NavigationService.Navigate(typeof(Views.ProfilePage), item.IdUser);
        }

        private async Task SearchUsers()
        {
            await LoadUsersResult();
        }

        private async Task LoadUsersResult()
        {
            var htmlResult = await _httpRequest.GetRequest(UrlApiSearchProfile, SearchBoxContent);
            UsersCollection = _searchResult.GetResultList(htmlResult);
        }
        #endregion

        #region Properties
        private string _searchBoxContent;
        public string SearchBoxContent
        {
            get { return _searchBoxContent; }
            set { Set(ref _searchBoxContent, value); }
        }
        #endregion

        #region Commands
        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand<object> SelectedItemCommand { get; private set; }
        #endregion
    }
}
