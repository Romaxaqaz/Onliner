using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MyToolkit.Command;
using Template10.Mvvm;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Onliner.Http;
using Onliner.Model.ProfileModel;
using Onliner_for_windows_10.Model.Message;
using Onliner_for_windows_10.Views.Profile;
using static Onliner.Setting.SettingParams;
using Onliner.Model.Message;
using Onliner_for_windows_10.Views.Message;

namespace Onliner_for_windows_10.View_Model
{
    public class ProfileViewModel : ViewModelBase
    {
        private ProfileData _profile = new ProfileData();
        private readonly HtmlDocument _resultat = new HtmlDocument();
        private readonly HttpRequest _httpRequest = new HttpRequest();
        private BirthDayDate _bday = new BirthDayDate();

        #region Variables
        private bool _typeAccaunt;
        private const string DefaultProfileUrl = "https://profile.onliner.by";
        private const string DefaultUserUrl = "https://profile.onliner.by/user/";
        private readonly string[] _accauntNameParsParam = new string[4] { "h1", "class", "m-title", "(^([A-Za-z0-9-_]+))" };
        private readonly string[] _accauntImageParsParam = new string[4] { "div", "class", "uprofile-ph", "(?<=src=\").*?(?=\")" };
        private readonly string[] _accauntStatusParsParam = new string[4] { "p", "class", "uprofile-connect user-status", "" };
        private string[] _accauntNumbersParsParam = new string[4] { "sup", "class", "nfm", "" };
        private string[] _accauntMoneyParsParam = new string[4] { "span", "class", "b-pay-widgetbalance-info", "" };
        #endregion

        #region Collection
        private ObservableCollection<ProfilePataList> _profileDataCollection = new ObservableCollection<ProfilePataList>();
        public ObservableCollection<ProfilePataList> ProfileDataCollection
        {
            get { return _profileDataCollection; }
            set { Set(ref _profileDataCollection, value); }
        }
        #endregion

        #region Constructor
        public ProfileViewModel()
        {
            ActionWithMessageCommand = new RelayCommand(ActionWithMessage);
            EditProfileCommand = new RelayCommand(EditProfile);
            ExitProfileCommand = new RelayCommand(ExitProfile);
            SearchUsersCommand = new RelayCommand(SearchUsers);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load the profile information
        /// </summary>
        /// <param name="updateShell"></param>
        /// <param name="profileUrl"></param>
        public async void LoadProfileInfo(bool updateShell, string profileUrl = DefaultProfileUrl)
        {
            ProgressRingIsActive = true;
            if (_httpRequest.HasInternet())
            {
                var parsHtml = new Onliner.ParsingHtml.ParsingHtml();
                var resultGetHttpRequest = await _httpRequest.GetRequestOnlinerAsync(profileUrl);
                _resultat.LoadHtml(resultGetHttpRequest);

                NickName = parsHtml.ParsElementHtml(_accauntNameParsParam, _resultat);
                Avatar = parsHtml.ParsElementHtml(_accauntImageParsParam, _resultat);
                Status = parsHtml.ParsElementHtml(_accauntStatusParsParam, _resultat);

                if (updateShell)
                {
                    ShellViewModel.Instance.AvatarUrl = Avatar;
                    ShellViewModel.Instance.Login = NickName;
                    ShellViewModel.Instance.Status = Status;
                    CommandButtonVisibility = true;
                }

                SetProfileDataCollection(_resultat);
                ProgressRingIsActive = false;
            }
            else
            {
                await _httpRequest.Message("Упс");
            }
        }

        /// <summary>
        /// Additional informaton
        /// </summary>
        /// <param name="document"></param>
        private void SetProfileDataCollection(HtmlDocument document)
        {
            var titleList = document.DocumentNode.Descendants().Where
            (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("profp-col-2-i"))).ToList();
            //name
            var infolist = titleList[0].Descendants("dt").ToList();
            //value
            var valuelist = titleList[0].Descendants("dd").ToList();

            for (var i = 0; i < infolist.Count - 1; i++)
            {
                ProfileDataCollection.Add(new ProfilePataList
                {
                    Info = infolist[i].InnerText,
                    Value = valuelist[i].InnerText.Trim()
                });
            }
        }

        /// <summary>
        /// Exip profile. Remove cookie
        /// </summary>
        private async void ExitProfile()
        {
            var dialog = new MessageDialog("Вы действительно хотите выйти?");

            dialog.Commands.Add(new UICommand { Label = "да", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "нет", Id = 1 });

            var result = await dialog.ShowAsync();

            if (NewMethod(result) != 0) return;
            //remove the file with cookie
            _httpRequest.Remoovecookie();
            SetParamsSetting(AuthorizationKey, "false");
            //reset splitview profile button content
            ResetShellData();
            NavigationService.Navigate(typeof(MainPage));
        }

        private static int NewMethod(IUICommand result)
        {
            return (int)result.Id;
        }

        private void ActionWithMessage()
        {
            IMessageModel message = new MessageModel(NickName, "", "");
            NavigationService.Navigate(typeof(MessageSenderPage), message);
        }

        private void MessageButtonContent() =>
            TextButtonMessageContent = _typeAccaunt == false ? "Мои сообщения" : "Отправить сообщение";

        private void EditProfile() =>
           NavigationService.Navigate(typeof(EditProfilePage));

        private void SearchUsers() =>
            NavigationService.Navigate(typeof(SearchUserPage));

        private void ResetShellData()
        {
            ShellViewModel.Instance.AvatarUrl = null;
            ShellViewModel.Instance.Login = null;
            ShellViewModel.Instance.Status = null;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            var auth = GetParamsSetting(AuthorizationKey);
            var authBool = Convert.ToBoolean(auth);
            //if not logged in go to the login page
            if (!authBool)
            {
                NavigationService.Navigate(typeof(MainPage));
            }
            else
            {
                //if there is an option key
                if (parameter != null)
                {
                    var profileUrl = DefaultUserUrl + (string)parameter;
                    _typeAccaunt = true;
                    LoadProfileInfo(false, profileUrl);
                }
                else
                {
                    _typeAccaunt = false;
                    LoadProfileInfo(true);
                }
            }
            MessageButtonContent();
            await Task.CompletedTask;
        }
        #endregion

        #region Command
        public RelayCommand ActionWithMessageCommand { get; private set; }
        public RelayCommand EditProfileCommand { get; private set; }
        public RelayCommand ExitProfileCommand { get; private set; }
        public RelayCommand SearchUsersCommand { get; private set; }
        #endregion

        #region Properties

        private string _nickName;
        public string NickName
        {
            get { return _nickName; }
            set { Set(ref _nickName, value); }
        }

        private string _avatar;
        public string Avatar
        {
            get { return _avatar; }
            set { Set(ref _avatar, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        private string _money;
        public string Money
        {
            get { return _money; }
            set { Set(ref _money, value); }
        }

        private string _textButtonMessageContent;
        public string TextButtonMessageContent
        {
            get { return _textButtonMessageContent; }
            set { Set(ref _textButtonMessageContent, value); }
        }

        private bool _progressRingIsActive = false;
        public bool ProgressRingIsActive
        {
            get { return _progressRingIsActive; }
            set { Set(ref _progressRingIsActive, value); }
        }

        private bool _commandButtonVisibility = false;
        public bool CommandButtonVisibility
        {
            get { return _commandButtonVisibility; }
            set { Set(ref _commandButtonVisibility, value); }
        }

        public bool ExitButtonVisibility => !_typeAccaunt;

        #endregion
    }
}
