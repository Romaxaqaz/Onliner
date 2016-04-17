using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Common;
using System;
using Onliner_for_windows_10.View_Model.Settings;
using Windows.ApplicationModel;
using Template10.Controls;
using static Onliner.Setting.SettingParams;
using Onliner_for_windows_10.Views;

namespace Onliner_for_windows_10
{
    sealed partial class App : BootStrapper
    {
        private SettingViewModel setting = new SettingViewModel();

        public App()
        {
            this.InitializeComponent();
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {

            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
                // create modal root
                setting.GetThemeApp();
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                };
            }
            await Task.CompletedTask;
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
            await Task.CompletedTask;
        }

    }
}