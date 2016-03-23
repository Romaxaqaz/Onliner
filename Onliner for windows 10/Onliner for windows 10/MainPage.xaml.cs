using Windows.UI.Xaml.Controls;
using Onliner_for_windows_10.View_Model;

namespace Onliner_for_windows_10
{
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel viewModel = new MainPageViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
