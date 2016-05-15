using System;
using System.Threading.Tasks;
using Windows.System;
using MyToolkit.Command;
using Onliner.Http;
using Onliner.Setting;
using OnlinerApp.Views;
using Template10.Mvvm;

namespace OnlinerApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly string AutorizationUrl = "https://www.onliner.by/#registration";

        private readonly HttpRequest _httpRequestToApi = new HttpRequest();

        #region Constructor
        public MainViewModel()
        {
            AuthorizationLaterCommand = new RelayCommand(AuthorizationLater);
            RegistrationCommand = new RelayCommand(Registration);
            AuthorizationCommand = new RelayCommand(async () => await Authorization());
        }
        #endregion

        #region Properties
        private bool _progressRing;
        public bool ProgressRing
        {
            get
            {
                return _progressRing;
            }
            set
            {
                Set(ref _progressRing, value);
            }
        }

        private bool _controlEnable = true;
        public bool ControlEnable
        {
            get
            {
                return _controlEnable;
            }
            set
            {
                Set(ref _controlEnable, value);
            }
        }

        private string _login;
        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                Set(ref _login, value);
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                Set(ref _password, value);
            }
        }
        #endregion

        #region Commands
        public RelayCommand AuthorizationLaterCommand { get; private set; }
        public RelayCommand AuthorizationCommand { get; private set; }
        public RelayCommand RegistrationCommand { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Authorization
        /// </summary>
        /// <returns></returns>
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
                var status = await _httpRequestToApi.PostRequestUserApi(Login, Password);
                if (status)
                {
                    SettingParams.SetParamsSetting(SettingParams.AuthorizationKey, "true");
                    NavigationService.Navigate(typeof(Views.ProfilePage));
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
            SettingParams.SetParamsSetting(SettingParams.AuthorizationKey, "false");
            NavigationService.Navigate(typeof(NewsPage));
        }
        #endregion
    }
}
