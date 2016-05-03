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
        private static ShellViewModel _instance = new ShellViewModel();

        public static ShellViewModel Instance
        {
            set { _instance = value; }
            get { return _instance; }
        }
        private HttpRequest HttpRequest = new HttpRequest();

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
            bool boolValue = true;
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
            var boolValue = GetParamsSetting(AutoLoadNewsAtStartUpAppKey);
            if (boolValue == null) boolValue = true;
            return Convert.ToBoolean(boolValue);
        }

        /// <summary>
        /// Get weather
        /// </summary>
        /// <returns></returns>
        private async Task GetWeatherNow()
        {
            var weather = await HttpRequest.Weather();
            Weather = weather == null ? (string)GetParamsSetting(LastWeatherKey) : weather.now.temperature;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get current
        /// </summary>
        /// <returns></returns>
        private async Task GetCurrent()
        {
            var current = await HttpRequest.Bestrate();
            Current = current == null ? (string)GetParamsSetting(LastCurrentKey) : current.Amount;
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
                var msg = await HttpRequest.MessageUnread();
                Message = msg == null ? (string)GetParamsSetting(LastMessageKey) : msg;
            }catch(ArgumentException)
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
        private string shop = "0";
        public string Shop
        {
            get { return shop; }
            set { Set(ref shop, value); }
        }

        private string weather = "0";
        public string Weather
        {
            get { return weather; }
            set
            {
                Set(ref weather, value);
                SetParamsSetting(LastWeatherKey, value);
            }
        }

        private string message = "0";
        public string Message
        {
            get { return message; }
            set
            {
                Set(ref message, value);
                SetParamsSetting(LastMessageKey, value);
            }
        }

        private string current = "0";
        public string Current
        {
            get { return current; }
            set
            {
                Set(ref current, value);
                SetParamsSetting(LastCurrentKey, value);
            }
        }

        private string status = "offline";
        public string Status
        {
            get { return status; }
            set
            {
                if (value == null) value = "offline";
                Set(ref status, value);
            }
        }

        private string _avatarUrl;
        public string AvatarUrl
        {
            get
            {
                var avatar = GetParamsSetting(AvatarKey);
                if (avatar == null) avatar = "/ImageCollection/default_avatar.png";
                return avatar.ToString();
            }
            set
            {
                if (value == null) value = "/ImageCollection/default_avatar.png";
                SetParamsSetting(AvatarKey, value);
                Set(ref _avatarUrl, value);
            }
        }

        public string _login;
        public string Login
        {
            get
            {
                var profileName = GetParamsSetting(NickNameKey);
                if (profileName == null) profileName = "Войти";
                return profileName.ToString();
            }
            set
            {
                if (value == null) value = "Войти";
                SetParamsSetting(NickNameKey, value);
                Set(ref _login, value);
            }
        }

        private bool techSectionNewsFirstLoad = true;
        public bool TechSectionNewsFirstLoad
        {
            get
            {
                return techSectionNewsFirstLoad;
            }
            set
            {
                techSectionNewsFirstLoad = value;
            }
        }

        private bool peopleSectionNewsFirstLoad = true;
        public bool PeopleSectionNewsFirstLoad
        {
            get
            {
                return peopleSectionNewsFirstLoad;
            }
            set
            {
                peopleSectionNewsFirstLoad = value;
            }
        }

        private bool homeSectionNewsFirstLoad = true;
        public bool HomeSectionNewsFirstLoad
        {
            get
            {
                return homeSectionNewsFirstLoad;
            }
            set
            {
                homeSectionNewsFirstLoad = value;
            }
        }

        private bool autoSectionNewsFirstLoad = true;
        public bool AutoSectionNewsFirstLoad
        {
            get
            {
                return autoSectionNewsFirstLoad;
            }
            set
            {
                autoSectionNewsFirstLoad = value;
            }
        }
        #endregion
    }
}
