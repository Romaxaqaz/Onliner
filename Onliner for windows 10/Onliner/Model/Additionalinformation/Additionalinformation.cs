using Onliner.Model.LocalSetteing;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Onliner.Model.AdditionalInformation
{
    public class Additionalinformation : INotifyPropertyChanged
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private static Additionalinformation instance = new Additionalinformation();
        private Additionalinformation() { }

        public static Additionalinformation Instance
        {
            set { instance = value; }
            get { return instance; }
        }

        public string _login;
        public string _unreadMessage;
        public string _current;
        public string _wheather;
        public string _nameActivePage;
        public string _avatarUrl;

        public string UnreadeMessage
        {
            get { return _unreadMessage; }
            set
            {
                _unreadMessage = value;
                OnPropertyChanged("UnreadeMessage");
            }
        }
        public string Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnPropertyChanged("Current");
            }
        }
        public string Wheather
        {
            get { return _wheather; }
            set
            {
                _wheather = value;
                OnPropertyChanged("Wheather");
            }
        }

        public string NameActivePage
        {
            get { return _nameActivePage; }
            set
            {
                _nameActivePage = value;
                OnPropertyChanged("NameActivePage");
            }
        }

        public string AvatarUrl
        {
            get
            {
                    var avatar = localSettings.Values[LocalSettingParams.AvatarUrl];
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
                OnPropertyChanged("AvatarUrl");
            }
        }

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
                _login = value;
                OnPropertyChanged("Login");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
