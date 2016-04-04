using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Services.NavigationService;
using Template10.Controls;
using Onliner_for_windows_10.View_Model;

namespace Onliner_for_windows_10.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
       

        public Shell()
        {
            this.InitializeComponent();
            Instance = this;
            Loaded += Shell_Loaded;    
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            var ai = ShellViewModel.Instance;
            ProfileStackpanel.DataContext = ai;
            WeatherStackpanel.DataContext = ai;
            CurrentStackpanel.DataContext = ai;
            MessageStackpanel.DataContext = ai;
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