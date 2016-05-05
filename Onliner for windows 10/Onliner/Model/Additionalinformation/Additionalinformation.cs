using Onliner.Model.LocalSetteing;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Onliner.Model.AdditionalInformation
{
    public class Additionalinformation : INotifyPropertyChanged
    {
        readonly Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private Additionalinformation() { }

        public static Additionalinformation Instance { set; get; } = new Additionalinformation();

        private string _login;
        private string _unreadMessage;
        private string _current;
        private string _wheather;
        private string _nameActivePage;
        private string _avatarUrl;

        public string UnreadeMessage
        {
            get { return _unreadMessage; }
            set
            {
                _unreadMessage = value;
                OnPropertyChanged();
            }
        }
        public string Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnPropertyChanged();
            }
        }
        public string Wheather
        {
            get { return _wheather; }
            set
            {
                _wheather = value;
                OnPropertyChanged();
            }
        }

        public string NameActivePage
        {
            get { return _nameActivePage; }
            set
            {
                _nameActivePage = value;
                OnPropertyChanged();
            }
        }

        public string AvatarUrl
        {
            get
            {
                    var avatar = _localSettings.Values[LocalSettingParams.AvatarUrl];
                    if (avatar != null)
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
                _avatarUrl = value;
                OnPropertyChanged();
            }
        }

        public string Login
        {
            get
            {
                var profileName = _localSettings.Values[LocalSettingParams.Login];
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
                _login = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            eventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
