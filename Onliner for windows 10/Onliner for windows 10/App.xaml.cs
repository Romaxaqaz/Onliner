using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Common;
using System;
using Windows.ApplicationModel;
using OnlinerApp.ViewModel.Settings;
using Template10.Controls;
using static Onliner.Setting.SettingParams;
using OnlinerApp.Views;
using NewsPage = OnlinerApp.Views.NewsPage;

namespace OnlinerApp
{
    public sealed partial class App : BootStrapper
    {
        private readonly SettingViewModel _setting = new SettingViewModel();

        public App()
        {
            this.InitializeComponent();
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (!(Window.Current.Content is ModalDialog))
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
                // create modal root
                _setting.GetThemeApp();
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
            var authorization = GetParamsSetting(AuthorizationKey) ?? false;
            var boolAuth = Convert.ToBoolean(authorization);
            NavigationService.Navigate(boolAuth ? typeof(NewsPage) : typeof(MainPage));
            await Task.CompletedTask;
        }

        public override async Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            await Task.CompletedTask;
        }
    }
}