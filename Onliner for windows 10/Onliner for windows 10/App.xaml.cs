using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using Onliner_for_windows_10.Views;

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
            return Task.FromResult<object>(null);
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            NavigationService.Navigate(typeof(NewsPage));
            await Task.CompletedTask;
        }
    }
}