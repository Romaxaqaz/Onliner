using Onliner_for_windows_10.View_Model.ProfileViewModels;
using Windows.UI.Xaml.Controls;


namespace Onliner_for_windows_10.Views.Profile
{
    public sealed partial class SearchUserPage : Page
    {
        private SearchUserViewModel viewModel = new SearchUserViewModel();

        public SearchUserPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
