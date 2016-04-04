using HtmlAgilityPack;
using MyToolkit.Command;
using Onliner.Http;
using Onliner.Model.ProfileModel;
using Onliner_for_windows_10.Model.Message;
using Onliner_for_windows_10.Views.Profile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using static Onliner.Setting.SettingParams;

namespace Onliner_for_windows_10.View_Model
{
    public class ProfileViewModel : ViewModelBase
    {
        private ProfileData profile = new ProfileData();
        private HtmlDocument resultat = new HtmlDocument();
        private HttpRequest HttpRequest = new HttpRequest();
        private BirthDayDate bday = new BirthDayDate();

        #region Variables
        private bool TypeAccaunt = false;
        private const string DefaultProfileUrl = "https://profile.onliner.by";
        private const string DefaultUserUrl = "https://profile.onliner.by/user/";
        private string[] accauntNameParsParam = new string[4] { "h1", "class", "m-title", "(^([A-Za-z0-9-_]+))" };
        private string[] accauntImageParsParam = new string[4] { "div", "class", "uprofile-ph", "(?<=src=\").*?(?=\")" };
        private string[] accauntStatusParsParam = new string[4] { "p", "class", "uprofile-connect user-status", "" };
        private string[] accauntNumbersParsParam = new string[4] { "sup", "class", "nfm", "" };
        private string[] accauntMoneyParsParam = new string[4] { "span", "class", "b-pay-widgetbalance-info", "" };
        #endregion

        #region Collection
        private ObservableCollection<ProfilePataList> profileDataCollection = new ObservableCollection<ProfilePataList>();
        public ObservableCollection<ProfilePataList> ProfileDataCollection
        {
            get { return profileDataCollection; }
            set { Set(ref profileDataCollection, value);}
        }
        #endregion

        #region Constructor
        public ProfileViewModel()
        {
            ActionWithMessageCommand = new RelayCommand(() => ActionWithMessage());
            EditProfileCommand = new RelayCommand(() => EditProfile());
            ExitProfileCommand = new RelayCommand(() => ExitProfile());
            SearchUsersCommand = new RelayCommand(() => SearchUsers());
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
            if (HttpRequest.HasInternet())
            {
                var parsHtml = new Onliner.ParsingHtml.ParsingHtml();
                string resultGetHttpRequest = await HttpRequest.GetRequestOnlinerAsync(profileUrl);
                resultat.LoadHtml(resultGetHttpRequest);

                NickName = parsHtml.ParsElementHtml(accauntNameParsParam, resultat);
                Avatar = parsHtml.ParsElementHtml(accauntImageParsParam, resultat);
                Status = parsHtml.ParsElementHtml(accauntStatusParsParam, resultat);

                if (updateShell)
                {
                    ShellViewModel.Instance.AvatarUrl = Avatar;
                    ShellViewModel.Instance.Login = NickName;
                    ShellViewModel.Instance.Status = Status;
                    CommandButtonVisibility = true;
                }

                SetProfileDataCollection(resultat);
                ProgressRingIsActive = false;
            }
            else
            {
                await HttpRequest.Message("Упс");
            }
        }

        /// <summary>
        /// Additional informaton
        /// </summary>
        /// <param name="document"></param>
        private void SetProfileDataCollection(HtmlDocument document)
        {
            List<HtmlNode> titleList = document.DocumentNode.Descendants().Where
            (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("profp-col-2-i"))).ToList();
            //name
            var infolist = titleList[0].Descendants("dt").ToList();
            //value
            var valuelist = titleList[0].Descendants("dd").ToList();

            for (int i = 0; i < infolist.Count - 1; i++)
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

            if ((int)result.Id == 0)
            {
                //remove the file with cookie
                HttpRequest.Remoovecookie();
                //reset splitview profile button content
                ResetShellData();
                NavigationService.Navigate(typeof(MainPage));
            }
        }

        private void ActionWithMessage()
        {
            if (TypeAccaunt)
            {

            }
            else
            {
                NavigationService.Navigate(typeof(MessagePage));
            }
        }

        private void MessageButtonContent() =>
            TextButtonMessageContent = TypeAccaunt == false ? "Мои сообщения" : "Отправить сообщение";

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
            if(!authBool)
            {
                NavigationService.Navigate(typeof(MainPage));
            }
            else
            {
                //if there is an option key
                if(parameter!= null)
                {
                    string profileUrl = DefaultUserUrl + (string)parameter;
                    TypeAccaunt = true;
                    LoadProfileInfo(false, profileUrl);
                }
                else
                {
                    TypeAccaunt = false;
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

        private string nickName;
        public string NickName
        {
            get { return nickName; }
            set { Set(ref nickName, value); }
        }

        private string avatar;
        public string Avatar
        {
            get { return avatar; }
            set { Set(ref avatar, value); }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { Set(ref status, value); }
        }

        private string money;
        public string Money
        {
            get { return money; }
            set { Set(ref money, value); }
        }

        private string textButtonMessageContent;
        public string TextButtonMessageContent
        {
            get { return textButtonMessageContent; }
            set { Set(ref textButtonMessageContent, value); }
        }

        private bool progressRingIsActive = false;

        public bool ProgressRingIsActive
        {
            get { return progressRingIsActive; }
            set { Set(ref progressRingIsActive, value); }
        }

        private bool commandButtonVisibility = false;
        public bool CommandButtonVisibility
        {
            get { return commandButtonVisibility; }
            set { Set(ref commandButtonVisibility, value); }
        }

        #endregion
    }
}
