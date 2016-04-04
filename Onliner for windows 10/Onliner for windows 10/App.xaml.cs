using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Common;
using Onliner_for_windows_10.Views;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI;
using static Onliner.Setting.SettingParams;
using System;
using Onliner_for_windows_10.View_Model.Settings;
using Windows.ApplicationModel;

namespace Onliner_for_windows_10
{
    sealed partial class App : BootStrapper
    {
        SettingViewModel setting = new SettingViewModel();

        public App()
        {
            this.InitializeComponent();
        }

        public override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            Window.Current.Content = new Views.Shell(nav);
            setting.GetThemeApp();
            return Task.FromResult<object>(null);
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            var authorization = GetParamsSetting(AuthorizationKey);
            if (authorization == null) authorization = false;

            var boolAuth = Convert.ToBoolean(authorization);
            if (boolAuth)
            {
                NavigationService.Navigate(typeof(NewsPage));
            }
            else
            {
                NavigationService.Navigate(typeof(MainPage));
            }

            await Task.CompletedTask;
        }

        public async override Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            int x = 0;
            await Task.CompletedTask;
        }

    }
}