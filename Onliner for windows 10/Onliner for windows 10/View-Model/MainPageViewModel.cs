using System;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.System;
using MyToolkit.Command;
using Onliner.Http;
using static Onliner.Setting.SettingParams;

namespace Onliner_for_windows_10.View_Model
{
    public class MainPageViewModel : ViewModelBase
    {
        private HttpRequest HttpRequestToApi = new HttpRequest();
        private readonly string AutorizationUrl = "https://www.onliner.by/#registration";

        #region Properties
        private bool progressRing;
        public bool ProgressRing
        {
            get
            {
                return this.progressRing;
            }
            set
            {
                Set(ref progressRing, value);
            }
        }

        private bool controlEnable = true;
        public bool ControlEnable
        {
            get
            {
                return this.controlEnable;
            }
            set
            {
                Set(ref controlEnable, value);
            }
        }

        private string login;
        public string Login
        {
            get
            {
                return this.login;
            }
            set
            {
                Set(ref login, value);
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                Set(ref password, value);
            }
        }
        #endregion

        #region Commands
        public RelayCommand AuthorizationLaterCommand { get; private set; }
        public RelayCommand AuthorizationCommand { get; private set; }
        public RelayCommand RegistrationCommand { get; private set; }
        #endregion

        public MainPageViewModel()
        {
            AuthorizationLaterCommand = new RelayCommand(() => AuthorizationLater());
            RegistrationCommand = new RelayCommand(() => Registration());
            AuthorizationCommand = new RelayCommand(async() => await Authorization());
        }

        private async Task Authorization()
        {
            ControlEnable = false;
            ProgressRing = true;
            if (string.IsNullOrEmpty(Login))
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Введите логин");
                await dialog.ShowAsync();
            }
            else if (string.IsNullOrEmpty(Password))
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Введите пароль");
                await dialog.ShowAsync();
            }
            else
            {
                bool Status = await HttpRequestToApi.PostRequestUserApi(Login, Password);
                if (Status)
                {
                    SetParamsSetting(AuthorizationKey, "true");
                    NavigationService.Navigate(typeof(ProfilePage.ProfilePage));
                }
                else
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("Неверный логин или пароль");
                    ControlEnable = true;
                    ProgressRing = false;
                    await dialog.ShowAsync();
                }

            }
        }

        private async void Registration() =>
            await Launcher.LaunchUriAsync(new Uri(AutorizationUrl));

        private void AuthorizationLater()
        {
            SetParamsSetting(AuthorizationKey, "false");
            NavigationService.Navigate(typeof(Views.NewsPage));
        }

    }
}
