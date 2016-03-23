using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using Onliner.Http;
using Onliner.Model.ProfileModel;
using Onliner.Model.JsonModel.Profile;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views.Profile
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class EditProfilePage : Page
    {
        private HttpRequest HttpRequest = new HttpRequest();
        private BirthDayDate bday = new BirthDayDate();
        private HtmlDocument resultat = new HtmlDocument();
        private List<object> ParamsList = new List<object>();

        private string EditProfilePageContent = string.Empty;

        private string[] BirthdayView = new string[] { "yymmdd", "mmdd", "yyyy", "" };
        private string[] BirthdayViewName = new string[] { "полностью", "день и месяц", "только год", "не показывать" };

        public EditProfilePage()
        {
            this.InitializeComponent();
            Loaded += EditProfilePage_Loaded;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.GoBack();
        }

        private void EditProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            ShowParamsProfile();
        }

        public void ShowParamsProfile()
        {
            ParamsList.Clear();
            List<EditProfileData> profileDataEdit = new List<EditProfileData>();
            HttpRequest.GetRequestOnliner("https://profile.onliner.by/edit");
            EditProfilePageContent = HttpRequest.ResultGetRequsetString;
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

        public void ShowPreferencesParams()
        {
            List<HtmlNode> editDataList = resultat.DocumentNode.Descendants().Where
           (x => (x.Name == "ul" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("uprofile-form-chks"))).ToList();

            var divList = editDataList[0].Descendants("li").ToList();
            foreach (var item in divList)
            {
                if (item.InnerHtml.Contains("<input") && item.InnerHtml.Contains("name=\"pmNotification\""))
                {
                    pmNotificationCheckBox.IsChecked = GetIsCheckValue(item, "checked=\"checked\"");
                }
                else
                if (item.InnerHtml.Contains("<input") && item.InnerHtml.Contains("name=\"pmNotification\""))
                {

                    hideOnlineStatusCheckBox.IsChecked = GetIsCheckValue(item, "checked=\"checked\"");
                }
                else
                if(item.InnerHtml.Contains("<input") && item.InnerHtml.Contains("name=\"showEmail\""))
                {
                    showEmailCheckBox.IsChecked = GetIsCheckValue(item, "checked=\"checked\"");
                }
                else
                if (item.InnerHtml.Contains("<select"))
                {
                    var selectList = item.Descendants("option").ToList();
                    BirthDayComboBox.SelectedIndex = BdayParamsIndex(GetListParams(selectList));
                }

            }
        }

        private void InputParamsProfile()
        {
            CityTextBox.Text = ParamsList[0].ToString();
            OccupationTextBox.Text = ParamsList[1].ToString();
            InterestsTextBox.Text = ParamsList[2].ToString();
            BirthdaDatePicker.Date = new System.DateTime(int.Parse(bday.Year), System.DateTime.Parse("1." + bday.Mounth + " 2008").Month, int.Parse(bday.Day));
            JabberTextBox.Text = ParamsList[3].ToString();
            IcqTextBox.Text = ParamsList[4].ToString();
            SkypeTextBox.Text = ParamsList[5].ToString();
            AimTextBox.Text = ParamsList[6].ToString();
            WebsiteTextBox.Text = ParamsList[7].ToString();
            BlogTextBox.Text = ParamsList[8].ToString();
            DevicesTextBox.Text = ParamsList[9].ToString();
            SignatureTextBox.Text = ParamsList[10].ToString();
        }

        private void SetInputParamsProfile()
        {
            ParamsList[0] = CityTextBox.Text;
            ParamsList[1] = OccupationTextBox.Text;
            ParamsList[2] = InterestsTextBox.Text;
            bday.Year = BirthdaDatePicker.Date.Year.ToString();
            bday.Mounth = BirthdaDatePicker.Date.Month.ToString();
            bday.Day = BirthdaDatePicker.Date.Day.ToString();
            ParamsList[3] = JabberTextBox.Text;
            ParamsList[4] = IcqTextBox.Text;
            ParamsList[5] = SkypeTextBox.Text;
            ParamsList[6] = AimTextBox.Text;
            ParamsList[7] = WebsiteTextBox.Text;
            ParamsList[8] = BlogTextBox.Text;
            ParamsList[9] = DevicesTextBox.Text;
            ParamsList[10] = SignatureTextBox.Text;
        }

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

        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            SetInputParamsProfile();
            HttpRequest.SaveEditProfile(ParamsList, bday);
        }

        private void SaveDDayEdit_Click(object sender, RoutedEventArgs e)
        {
            string PmNotification = CheckBoxValue(pmNotificationCheckBox).ToString();
            string OnlineStatus = CheckBoxValue(hideOnlineStatusCheckBox).ToString();
            string ShowEmail = CheckBoxValue(showEmailCheckBox).ToString();
            string BirthDay = BirthdayView[BirthDayComboBox.SelectedIndex];
            HttpRequest.EditPreferencesProfile(PmNotification, OnlineStatus, ShowEmail, BirthDay);
 
        }

        private int CheckBoxValue(CheckBox checkbox)
        {
            int value = 0;
            if (checkbox.IsChecked == true)
            {
                value = 1;
            }
            return value;
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

        private void DataProfilePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 1:
                    break;
                case 2:
                    ShowPreferencesParams();
                    break;
                case 3:

                    break;
            }
        }

        private void ChangePassButton_Click(object sender, RoutedEventArgs e)
        {
            HttpRequest.Changepass(OldPassTextBox.Text,RepeatPassTextBox.Text,NewsPassTextBox.Text);
            Frame.Navigate(typeof(MainPage));
            Frame.BackStack.Clear();
        }
    }
}
