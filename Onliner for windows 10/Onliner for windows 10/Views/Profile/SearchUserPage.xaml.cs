using OnlinerApp.View_Model.ProfileViewModels;
using Windows.UI.Xaml.Controls;


namespace OnlinerApp.Views.Profile
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
