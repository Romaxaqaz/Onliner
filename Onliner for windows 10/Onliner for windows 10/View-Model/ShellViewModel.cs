using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using Onliner.Http;
using Onliner.Model.LocalSetteing;

namespace Onliner_for_windows_10.View_Model
{
    public class ShellViewModel : ViewModelBase
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private HttpRequest HttpRequest = new HttpRequest();

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
            set { Set(ref weather, value); }
        }

        private string message = "0";
        public string Message
        {
            get { return message; }
            set { Set(ref message, value); }
        }

        private string current = "0";
        public string Current
        {
            get { return current; }
            set { Set(ref current, value); }
        }

        public string _avatarUrl;
        public string AvatarUrl
        {
            get
            {
                var avatar = localSettings.Values[LocalSettingParams.AvatarUrl];
                if (avatar != null && HttpRequest.HasInternet())
                {
                    _avatarUrl = avatar.ToString();
                }
                else
                {
                    return "/ImageCollection/default_avatar.png";
                }
                return _avatarUrl;
            }
            set
            {
                Set(ref _avatarUrl, value);
            }
        }

        public string _login;
        public string Login
        {
            get
            {
                var profileName = localSettings.Values[LocalSettingParams.Login];
                if (profileName != null)
                {
                    _login = profileName.ToString();
                }
                else
                {
                    return "Войти";
                }
                return _login;
            }
            set
            {
                Set(ref _login, value);
            }
        }

        public ShellViewModel()
        {
            Parallel.Invoke(
                async () => await GetWeatherNow(),
                async () => await GetCurrent(),
                async () => await GetMessage()
                );
        }

        private async Task GetWeatherNow()
        {
            var weather = await HttpRequest.Weather();       
            Weather = weather == null ? "-" : weather.now.temperature;
        }

        private async Task GetCurrent()
        {
            var current = await HttpRequest.Bestrate();
            Current = current == null ? "-" : current.amount;
        }

        private async Task GetMessage()
        {
            var msg = await HttpRequest.MessageUnread();
            Message = msg == null ? "-" : msg;
        }

        private async Task GetCartCount()
        {
          // Shop = await HttpRequest.ShopCount("543687");
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await Task.CompletedTask;
        }
    }

}
