using HtmlAgilityPack;
using MyToolkit.Command;
using Onliner.Http;
using Onliner.Model.JsonModel.Profile;
using Onliner.Model.ProfileModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Onliner_for_windows_10.View_Model.ProfileViewModels
{
    public class EditProfileViewModel : ViewModelBase
    {
        private const string UrlProfileEdit = "https://profile.onliner.by/edit";

        private HttpRequest HttpRequest = new HttpRequest();
        private BirthDayDate bday = new BirthDayDate();
        private HtmlDocument resultat = new HtmlDocument();
        private List<object> ParamsList = new List<object>();

        #region Variables
        private string EditProfilePageContent = string.Empty;
        private string[] BirthdayView = new string[] { "yymmdd", "mmdd", "yyyy", "" };
        private string[] BirthdayViewName = new string[] { "полностью", "день и месяц", "только год", "не показывать" };
        #endregion

        #region Constructor
        public EditProfileViewModel()
        {
            SaveMainInforamtionCommand = new RelayCommand(() => SaveMainInforamtion());
            SaveAdditionalInforamtionCommand = new RelayCommand(() => SaveAdditionalInforamtion());
            ChangePasswordCommand = new RelayCommand(() => ChangePassword());
            ChangePivotCommand = new RelayCommand<object>((obj) => SelectedChangePivot(obj));
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
            string PmNotification = CheckBoolValue(PmNotificationCheckBox).ToString();
            string OnlineStatus = CheckBoolValue(HideOnlineStatusCheckBox).ToString();
            string ShowEmail = CheckBoolValue(ShowEmailCheckBox).ToString();
            string BirthDay = BirthdayView[BirthDayComboBox];
            HttpRequest.EditPreferencesProfile(PmNotification, OnlineStatus, ShowEmail, BirthDay);
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
            List<EditProfileData> profileDataEdit = new List<EditProfileData>();
            EditProfilePageContent = await HttpRequest.GetRequestOnlinerAsync(UrlProfileEdit);
            resultat.LoadHtml(EditProfilePageContent);

            List<HtmlNode> editDataList = resultat.DocumentNode.Descendants().Where
           (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("uprofile-form"))).ToList();

            var divList = editDataList[0].Descendants("div").ToList();
            foreach (var item in divList)
            {
                if (item.InnerHtml.Contains("<input"))
                {
                    ParamsList.Add(item.Descendants("input").FirstOrDefault().Attributes["value"].Value.ToString());
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
        private BirthDayDate GetBirthDay(List<HtmlNode> list)
        {
            BirthDayDate bDay = new BirthDayDate();
            bDay.Day = GetListParams(list[0].Descendants("option").ToList());
            bDay.Mounth = GetListParams(list[1].Descendants("option").ToList());
            bDay.Year = GetListParams(list[2].Descendants("option").ToList());
            return bDay;
        }


        private string GetListParams(List<HtmlNode> list)
        {
            string result = string.Empty;
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
            BirthdaDatePicker = new System.DateTime(int.Parse(bday.Year), System.DateTime.Parse("1." + bday.Mounth + " 2008").Month, int.Parse(bday.Day));
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

        private int CheckBoolValue(bool check)
        {
            int value = 0;
            return value = check == true ? 1 : 0;
        }

        private bool GetIsCheckValue(HtmlNode item, string checkParams)
        {
            bool result = false;
            if (item.InnerHtml.Contains(checkParams))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
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

        private string cityText;
        public string CityText
        {
            get { return cityText; }
            set { Set(ref cityText, value); }
        }

        private string occupationText;
        public string OccupationText
        {
            get { return occupationText; }
            set { Set(ref occupationText, value); }
        }

        private string interestsText;
        public string InterestsText
        {
            get { return interestsText; }
            set { Set(ref interestsText, value); }
        }

        private DateTime birthdaDatePicker = new DateTime();
        public DateTime BirthdaDatePicker
        {
            get { return birthdaDatePicker; }
            set { Set(ref birthdaDatePicker, value); }
        }

        private string jabberText;
        public string JabberText
        {
            get { return jabberText; }
            set { Set(ref jabberText, value); }
        }

        private string icqText;
        public string IcqText
        {
            get { return icqText; }
            set { Set(ref icqText, value); }
        }

        private string skypeText;
        public string SkypeText
        {
            get { return skypeText; }
            set { Set(ref skypeText, value); }
        }

        private string aimText;
        public string AimText
        {
            get { return aimText; }
            set { Set(ref aimText, value); }
        }

        private string websitetext;
        public string WebsiteText
        {
            get { return websitetext; }
            set { Set(ref websitetext, value); }
        }

        private string blogText;
        public string BlogText
        {
            get { return blogText; }
            set { Set(ref blogText, value); }
        }

        private string devicesText;
        public string DevicesText
        {
            get { return devicesText; }
            set { Set(ref devicesText, value); }
        }

        private string signatureText;
        public string SignatureText
        {
            get { return signatureText; }
            set { Set(ref signatureText, value); }
        }

        private string oldPassword;
        public string OldPassword
        {
            get { return oldPassword; }
            set { Set(ref oldPassword, value); }
        }

        private string newPassword;
        public string NewPassword
        {
            get { return newPassword; }
            set { Set(ref newPassword, value); }
        }

        private bool pmNotificationCheckBox;
        public bool PmNotificationCheckBox
        {
            get { return pmNotificationCheckBox; }
            set { Set(ref pmNotificationCheckBox, value); }
        }

        private bool hideOnlineStatusCheckBox;
        public bool HideOnlineStatusCheckBox
        {
            get { return hideOnlineStatusCheckBox; }
            set { Set(ref hideOnlineStatusCheckBox, value); }
        }

        private bool showEmailCheckBox;
        public bool ShowEmailCheckBox
        {
            get { return showEmailCheckBox; }
            set { Set(ref showEmailCheckBox, value); }
        }

        private int birthDayComboBox;
        public int BirthDayComboBox
        {
            get { return birthDayComboBox; }
            set { Set(ref birthDayComboBox, value); }
        }

        private int selectedIndexPivot = 0;

        public int SelectedIndexPivot
        {
            get { return selectedIndexPivot = 0; }
            set { Set(ref selectedIndexPivot, value); }
        }


        #endregion
    }
}
