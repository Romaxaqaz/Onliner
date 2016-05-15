using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using HtmlAgilityPack;
using MyToolkit.Command;
using Onliner.Http;
using Onliner.Model.JsonModel.Profile;
using Onliner.Model.ProfileModel;
using Template10.Mvvm;

namespace OnlinerApp.ViewModel.ProfileViewModels
{
    public class EditProfileViewModel : ViewModelBase
    {
        private const string UrlProfileEdit = "https://profile.onliner.by/edit";

        private HttpRequest HttpRequest = new HttpRequest();
        private BirthDayDate bday = new BirthDayDate();
        private HtmlDocument resultat = new HtmlDocument();
        private List<object> ParamsList = new List<object>();

        #region Variables
        private string EditProfilePageContent;
        private readonly string[] BirthdayView = new string[] { "yymmdd", "mmdd", "yyyy", "" };
        private readonly string[] BirthdayViewName = new string[] { "полностью", "день и месяц", "только год", "не показывать" };
        #endregion

        #region Constructor
        public EditProfileViewModel()
        {
            SaveMainInforamtionCommand = new RelayCommand(SaveMainInforamtion);
            SaveAdditionalInforamtionCommand = new RelayCommand(SaveAdditionalInforamtion);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            ChangePivotCommand = new RelayCommand<object>(SelectedChangePivot);
        }
        #endregion

        #region Methods
        private void SelectedChangePivot(object obj)
        {
        }

        /// <summary>
        /// Change password
        /// </summary>
        private void ChangePassword()
        {
            SetInputParamsProfile();
            HttpRequest.SaveEditProfile(ParamsList, bday);
        }

        /// <summary>
        /// Save additional information
        /// </summary>
        private void SaveAdditionalInforamtion()
        {
            var pmNotification = CheckBoolValue(PmNotificationCheckBox).ToString();
            var onlineStatus = CheckBoolValue(HideOnlineStatusCheckBox).ToString();
            var showEmail = CheckBoolValue(ShowEmailCheckBox).ToString();
            var birthDay = BirthdayView[BirthDayComboBox];
            HttpRequest.EditPreferencesProfile(pmNotification, onlineStatus, showEmail, birthDay);
        }

        /// <summary>
        /// Save main information
        /// </summary>
        private void SaveMainInforamtion()
        {
            SetInputParamsProfile();
            HttpRequest.SaveEditProfile(ParamsList, bday);
        }

        /// <summary>
        /// Show main information profile
        /// </summary>
        /// <returns></returns>
        public async Task ShowParamsProfile()
        {
            ParamsList.Clear();
            EditProfilePageContent = await HttpRequest.GetRequestOnlinerAsync(UrlProfileEdit);
            resultat.LoadHtml(EditProfilePageContent);

            var editDataList = resultat.DocumentNode.Descendants().Where
           (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("uprofile-form"))).ToList();

            var divList = editDataList[0].Descendants("div").ToList();
            foreach (var item in divList)
            {
                if (item.InnerHtml.Contains("<input"))
                {
                    ParamsList.Add(item.Descendants("input").FirstOrDefault().Attributes["value"].Value);
                }
                else if (item.InnerHtml.Contains("<textarea"))
                {
                    ParamsList.Add(item.Descendants("textarea").FirstOrDefault().InnerText);
                }
                else if (item.InnerHtml.Contains("<select"))
                {
                    var selectList = item.Descendants("select").ToList();
                    bday = GetBirthDay(selectList);
                }

            }
            InputParamsProfile();
        }

        /// <summary>
        /// Show additional information profile
        /// </summary>
        public void ShowPreferencesParams()
        {
            List<HtmlNode> editDataList = resultat.DocumentNode.Descendants().Where
           (x => (x.Name == "ul" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("uprofile-form-chks"))).ToList();

            var divList = editDataList[0].Descendants("li").ToList();
            foreach (var item in divList)
            {
                if (item.InnerHtml.Contains("<input") && item.InnerHtml.Contains("name=\"pmNotification\""))
                {
                    PmNotificationCheckBox = GetIsCheckValue(item, "checked=\"checked\"");
                }
                else
                if (item.InnerHtml.Contains("<input") && item.InnerHtml.Contains("name=\"pmNotification\""))
                {

                    HideOnlineStatusCheckBox = GetIsCheckValue(item, "checked=\"checked\"");
                }
                else
                if (item.InnerHtml.Contains("<input") && item.InnerHtml.Contains("name=\"showEmail\""))
                {
                    ShowEmailCheckBox = GetIsCheckValue(item, "checked=\"checked\"");
                }
                else
                if (item.InnerHtml.Contains("<select"))
                {
                    var selectList = item.Descendants("option").ToList();
                    BirthDayComboBox = BdayParamsIndex(GetListParams(selectList));
                }

            }
        }

        /// <summary>
        /// Pars date information
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static BirthDayDate GetBirthDay(IReadOnlyList<HtmlNode> list)
        {
            var bDay = new BirthDayDate
            {
                Day = GetListParams(list[0].Descendants("option").ToList()),
                Mounth = GetListParams(list[1].Descendants("option").ToList()),
                Year = GetListParams(list[2].Descendants("option").ToList())
            };
            return bDay;
        }

        private static string GetListParams(IEnumerable<HtmlNode> list)
        {
            var result = string.Empty;
            foreach (var item in list)
            {
                if ((item.Name == "option") && (item.OuterHtml.Contains("selected")))
                {
                    result = item.NextSibling.InnerText;
                }
            }
            return result;
        }

        /// <summary>
        /// Set params profile for UI
        /// </summary>
        private void InputParamsProfile()
        {
            CityText = ParamsList[0].ToString();
            OccupationText = ParamsList[1].ToString();
            InterestsText = ParamsList[2].ToString();
            BirthdaDatePicker = new DateTime(int.Parse(bday.Year), DateTime.Parse("1." + bday.Mounth + " 2008").Month, int.Parse(bday.Day));
            JabberText = ParamsList[3].ToString();
            IcqText = ParamsList[4].ToString();
            SkypeText = ParamsList[5].ToString();
            AimText = ParamsList[6].ToString();
            WebsiteText = ParamsList[7].ToString();
            BlogText = ParamsList[8].ToString();
            DevicesText = ParamsList[9].ToString();
            SignatureText = ParamsList[10].ToString();
        }

        /// <summary>
        /// Set params profile for request
        /// </summary>
        private void SetInputParamsProfile()
        {
            ParamsList[0] = CityText;
            ParamsList[1] = OccupationText;
            ParamsList[2] = InterestsText;
            bday.Year = BirthdaDatePicker.Date.Year.ToString();
            bday.Mounth = BirthdaDatePicker.Date.Month.ToString();
            bday.Day = BirthdaDatePicker.Date.Day.ToString();
            ParamsList[3] = JabberText;
            ParamsList[4] = IcqText;
            ParamsList[5] = SkypeText;
            ParamsList[6] = AimText;
            ParamsList[7] = WebsiteText;
            ParamsList[8] = BlogText;
            ParamsList[9] = DevicesText;
            ParamsList[10] = SignatureText;
        }

        private int BdayParamsIndex(string nameParams)
        {
            int paramsBdayIndex = 0;
            for (int i = 0; i < BirthdayViewName.Length; i++)
            {
                if (BirthdayViewName[i] == nameParams)
                {
                    paramsBdayIndex = i;
                }
            }
            return paramsBdayIndex;
        }

        private static int CheckBoolValue(bool check)
        {
            return check ? 1 : 0;
        }

        private static bool GetIsCheckValue(HtmlNode item, string checkParams)
        {
            return item.InnerHtml.Contains(checkParams);
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await ShowParamsProfile();
             ShowPreferencesParams();
            await Task.CompletedTask;
        }

        #endregion

        #region Command
        public RelayCommand SaveMainInforamtionCommand { get; private set; }
        public RelayCommand SaveAdditionalInforamtionCommand { get; private set; }
        public RelayCommand ChangePasswordCommand { get; private set; }
        public RelayCommand<object> ChangePivotCommand { get; private set; }
        #endregion

        #region Properties

        private string _cityText;
        public string CityText
        {
            get { return _cityText; }
            set { Set(ref _cityText, value); }
        }

        private string _occupationText;
        public string OccupationText
        {
            get { return _occupationText; }
            set { Set(ref _occupationText, value); }
        }

        private string _interestsText;
        public string InterestsText
        {
            get { return _interestsText; }
            set { Set(ref _interestsText, value); }
        }

        private DateTime _birthdaDatePicker;
        public DateTime BirthdaDatePicker
        {
            get { return _birthdaDatePicker; }
            set { Set(ref _birthdaDatePicker, value); }
        }

        private string _jabberText;
        public string JabberText
        {
            get { return _jabberText; }
            set { Set(ref _jabberText, value); }
        }

        private string _icqText;
        public string IcqText
        {
            get { return _icqText; }
            set { Set(ref _icqText, value); }
        }

        private string _skypeText;
        public string SkypeText
        {
            get { return _skypeText; }
            set { Set(ref _skypeText, value); }
        }

        private string _aimText;
        public string AimText
        {
            get { return _aimText; }
            set { Set(ref _aimText, value); }
        }

        private string _websitetext;
        public string WebsiteText
        {
            get { return _websitetext; }
            set { Set(ref _websitetext, value); }
        }

        private string _blogText;
        public string BlogText
        {
            get { return _blogText; }
            set { Set(ref _blogText, value); }
        }

        private string _devicesText;
        public string DevicesText
        {
            get { return _devicesText; }
            set { Set(ref _devicesText, value); }
        }

        private string _signatureText;
        public string SignatureText
        {
            get { return _signatureText; }
            set { Set(ref _signatureText, value); }
        }

        private string _oldPassword;
        public string OldPassword
        {
            get { return _oldPassword; }
            set { Set(ref _oldPassword, value); }
        }

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set { Set(ref _newPassword, value); }
        }

        private bool _pmNotificationCheckBox;
        public bool PmNotificationCheckBox
        {
            get { return _pmNotificationCheckBox; }
            set { Set(ref _pmNotificationCheckBox, value); }
        }

        private bool _hideOnlineStatusCheckBox;
        public bool HideOnlineStatusCheckBox
        {
            get { return _hideOnlineStatusCheckBox; }
            set { Set(ref _hideOnlineStatusCheckBox, value); }
        }

        private bool _showEmailCheckBox;
        public bool ShowEmailCheckBox
        {
            get { return _showEmailCheckBox; }
            set { Set(ref _showEmailCheckBox, value); }
        }

        private int _birthDayComboBox;
        public int BirthDayComboBox
        {
            get { return _birthDayComboBox; }
            set { Set(ref _birthDayComboBox, value); }
        }

        private int _selectedIndexPivot;

        public int SelectedIndexPivot
        {
            get { return _selectedIndexPivot = 0; }
            set { Set(ref _selectedIndexPivot, value); }
        }


        #endregion
    }
}
