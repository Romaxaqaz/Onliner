using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using Onliner_for_windows_10.Views;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI;

namespace Onliner_for_windows_10
{
    sealed partial class App : BootStrapper
    {
        public App()
        {
            this.InitializeComponent();
        }

        public override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            Window.Current.Content = new Views.Shell(nav);
            StatusBarCustomiztion();
            return Task.FromResult<object>(null);
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            NavigationService.Navigate(typeof(NewsPage));
            await Task.CompletedTask;
        }

        private void StatusBarCustomiztion()
        {
            //mobile customization
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = Windows.UI.Colors.Yellow;
                statusBar.ForegroundColor = Windows.UI.Colors.Black;
                statusBar.BackgroundOpacity = 1;
            }
            //pc customiztion
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Yellow;
                    titleBar.ButtonForegroundColor = Colors.Black;
                    titleBar.BackgroundColor = Colors.Yellow;
                    titleBar.ForegroundColor = Colors.Black;
                }
            }
        }


    }
}