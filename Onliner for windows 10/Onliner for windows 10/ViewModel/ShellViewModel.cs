using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using Onliner.Http;
using static Onliner.Setting.SettingParams;

namespace Onliner_for_windows_10.View_Model
{
    public sealed class ShellViewModel : ViewModelBase
    {
        public static ShellViewModel Instance { set; get; } = new ShellViewModel();

        private readonly HttpRequest _httpRequest = new HttpRequest();

        #region Constructor
        private ShellViewModel()
        {
            SetAutoLoadNews();
            Parallel.Invoke(
                    async () => await GetWeatherNow(),
                    async () => await GetCurrent(),
                    async () => await GetMessage()
                    );
        }
        #endregion

        #region Methods

        /// <summary>
        /// Set automatically load new after run app
        /// </summary>
        private void SetAutoLoadNews()
        {
            var boolValue = true;
            var resultKeyValue = GetParamsSetting(AutoLoadNewsAtStartUpAppKey);
            if (resultKeyValue != null)
                boolValue = Convert.ToBoolean(GetParamsSetting(AutoLoadNewsAtStartUpAppKey));

            SetParamsSetting(AutoLoadNewsAtStartUpAppKey, boolValue.ToString());
            TechSectionNewsFirstLoad = boolValue;
            PeopleSectionNewsFirstLoad = boolValue;
            HomeSectionNewsFirstLoad = boolValue;
            AutoSectionNewsFirstLoad = boolValue;
        }

        private bool GetBoolAutoLoadNews()
        {
            var boolValue = GetParamsSetting(AutoLoadNewsAtStartUpAppKey) ?? true;
            return Convert.ToBoolean(boolValue);
        }

        /// <summary>
        /// Get weather
        /// </summary>
        /// <returns></returns>
        private async Task GetWeatherNow()
        {
            var weatherJSon = await _httpRequest.Weather();
            Weather = weatherJSon == null ? (string)GetParamsSetting(LastWeatherKey) : weatherJSon.now.temperature;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get current
        /// </summary>
        /// <returns></returns>
        private async Task GetCurrent()
        {
            var bestrateRespose = await _httpRequest.Bestrate();
            Current = bestrateRespose == null ? (string)GetParamsSetting(LastCurrentKey) : bestrateRespose.Amount;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get message
        /// </summary>
        /// <returns></returns>
        private async Task GetMessage()
        {
            try
            {
                var msg = await _httpRequest.MessageUnread();
                Message = msg ?? (string)GetParamsSetting(LastMessageKey);
            }
            catch (ArgumentException)
            {
                Message = "";
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get shop count
        /// </summary>
        private void GetCartCount()
        {
            // Shop = await HttpRequest.ShopCount("543687");
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //await GetWeatherNow();
            //await GetCurrent();
            //await GetMessage();
            await Task.CompletedTask;
        }
        #endregion

        #region Properties
        private string _shop = "0";
        public string Shop
        {
            get { return _shop; }
            set { Set(ref _shop, value); }
        }

        private string _weather = "0";
        public string Weather
        {
            get { return _weather; }
            set
            {
                Set(ref _weather, value);
                SetParamsSetting(LastWeatherKey, value);
            }
        }

        private string _message = "0";
        public string Message
        {
            get { return _message; }
            set
            {
                Set(ref _message, value);
                SetParamsSetting(LastMessageKey, value);
            }
        }

        private string _current = "0";
        public string Current
        {
            get { return _current; }
            set
            {
                Set(ref _current, value);
                SetParamsSetting(LastCurrentKey, value);
            }
        }

        private string _status = "offline";
        public string Status
        {
            get { return _status; }
            set
            {
                if (value == null) value = "offline";
                Set(ref _status, value);
            }
        }

        private string _avatarUrl;
        public string AvatarUrl
        {
            get
            {
                var avatar = GetParamsSetting(AvatarKey) ?? "/ImageCollection/default_avatar.png";
                return avatar.ToString();
            }
            set
            {
                if (value == null) value = "/ImageCollection/default_avatar.png";
                SetParamsSetting(AvatarKey, value);
                Set(ref _avatarUrl, value);
            }
        }

        private string _login;
        public string Login
        {
            get
            {
                var profileName = GetParamsSetting(NickNameKey) ?? "Войти";
                return profileName.ToString();
            }
            set
            {
                if (value == null) value = "Войти";
                SetParamsSetting(NickNameKey, value);
                Set(ref _login, value);
            }
        }

        public bool TechSectionNewsFirstLoad { get; set; } = true;

        public bool PeopleSectionNewsFirstLoad { get; set; } = true;

        public bool HomeSectionNewsFirstLoad { get; set; } = true;

        public bool AutoSectionNewsFirstLoad { get; set; } = true;

        #endregion
    }
}
