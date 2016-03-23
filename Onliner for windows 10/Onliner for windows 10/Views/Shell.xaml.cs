using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Services.NavigationService;
using Template10.Controls;

namespace Onliner_for_windows_10.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        private View_Model.ShellViewModel viewModel = new View_Model.ShellViewModel();

        public Shell()
        {
            this.InitializeComponent();
            Instance = this;
            Loaded += Shell_Loaded;    
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            ProfileStackpanel.DataContext = viewModel;
            WeatherStackpanel.DataContext = viewModel;
            CurrentStackpanel.DataContext = viewModel;
            MessageStackpanel.DataContext = viewModel;
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }
    }
}