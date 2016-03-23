using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using HtmlAgilityPack;
using Onliner_for_windows_10.Views.Profile;
using Onliner_for_windows_10.Model.Message;
using Onliner.Model.ProfileModel;
using Onliner.Http;
using Onliner.Model.AdditionalInformation;
using Onliner.Model.LocalSetteing;

namespace Onliner_for_windows_10.ProfilePage
{
    public sealed partial class ProfilePage : Page
    {
        private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        private readonly string NameButtonSendMessage = "Мои сообщения";

        private ProfileData profile = new ProfileData();
        private HtmlDocument resultat = new HtmlDocument();
        private HttpRequest HttpRequest = new HttpRequest();
        private BirthDayDate bday = new BirthDayDate();
        private List<object> ParamsList = new List<object>();
        public string ProfileListData { get; set; }

        private string[] accauntNameParsParam = new string[4] { "h1", "class", "m-title", "(^([A-Za-z0-9-_]+))" };
        private string[] accauntImageParsParam = new string[4] { "div", "class", "uprofile-ph", "(?<=src=\").*?(?=\")" };
        private string[] accauntStatusParsParam = new string[4] { "p", "class", "uprofile-connect user-status", "" };
        private string[] accauntNumbersParsParam = new string[4] { "sup", "class", "nfm", "" };
        private string[] accauntMoneyParsParam = new string[4] { "span", "class", "b-pay-widgetbalance-info", "" };

        public ProfilePage()
        {
            this.InitializeComponent();
            this.Loaded += ProfilePage_Loaded;
            ButtonsControlStackPanel.DataContext = Additionalinformation.Instance;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            string key = (string)e.Parameter;
            if (HttpRequest.HasInternet())
            {
                Task taskProfileLoad = ProfileDataLoad(key);
                await taskProfileLoad;
            }
            else
            {
                HttpRequest.Message("Упс, вы не подключены к интернету :(");
            }

        }

        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            Additionalinformation.Instance.NameActivePage = "Профиль";
        }

        private async Task ProfileDataLoad(string param)
        {
            try
            {
                if (param == null || param == "")
                {
                    ShowProfileInfo(string.Empty);
                }
                else
                {
                    string prifileUrl = "https://profile.onliner.by/user/" + param;
                    ShowProfileInfo(prifileUrl);
                }
            }
            catch (FormatException ex)
            {
                MessageDialog message = new MessageDialog(ex.ToString());
                await message.ShowAsync();
            }
        }

        public async void ShowProfileInfo(string profileUrl)
        {
            bool changeInfo = false;
            if (profileUrl == string.Empty)
            {
                profileUrl = "https://profile.onliner.by";
                changeInfo = true;
            }

            var parsHtml = new  Onliner.ParsingHtml.ParsingHtml();
            string resultGetHttpRequest = await HttpRequest.GetRequestOnlinerAsync(profileUrl);
            resultat.LoadHtml(resultGetHttpRequest);

            string loginUser = parsHtml.ParsElementHtml(accauntNameParsParam, resultat);
            string avatarImage = parsHtml.ParsElementHtml(accauntImageParsParam, resultat);

            profile.AccauntName = loginUser;
            profile.Avatar = avatarImage;
            profile.Status = parsHtml.ParsElementHtml(accauntStatusParsParam, resultat);


            //profile.ProfileNumbers = parsHtml.ParsElementHtml(accauntNumbersParsParam, resultat);
            if (changeInfo)
            {
                profile.Money = parsHtml.ParsElementHtml(accauntMoneyParsParam, resultat);
                //save setting data
                localSettings.Values[LocalSettingParams.AvatarUrl] = avatarImage;
                localSettings.Values[LocalSettingParams.Login] = loginUser;

                Additionalinformation.Instance.AvatarUrl = avatarImage;
                Additionalinformation.Instance.Login = loginUser;
                SendMessageTextBox.Text = NameButtonSendMessage;
            }

            List<ProfilePataList> _profileDataList = new List<ProfilePataList>();
            stackPanelAccauntInfo.DataContext = profile;
            List<HtmlNode> titleList = resultat.DocumentNode.Descendants().Where
            (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("profp-col-2-i"))).ToList();

            var infolist = titleList[0].Descendants("dt").ToList();
            var valuelist = titleList[0].Descendants("dd").ToList();
            for (int i = 0; i < infolist.Count - 1; i++)
            {
                _profileDataList.Add(new ProfilePataList
                {
                    Info = infolist[i].InnerText,
                    Value = valuelist[i].InnerText.Trim()
                });
            }
            DataProfileList.ItemsSource = _profileDataList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SendMessageTextBox.Text == NameButtonSendMessage)
            {
                Frame.Navigate(typeof(MessagePage));
            }
            else
            {

            }
        }

        private void EditProfilePivotItemHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditProfilePage));
        }

        private void ProfileFlipview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
        }

        private void ProfileTabButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radBut = sender as RadioButton;
        }

        private void PinWindow_Click(object sender, RoutedEventArgs e)
        {
            //SquareTileControl.DataContext = new TileData()
            //{
            //    Title = Additionalinformation.Instance.Login,
            //    Img = Additionalinformation.Instance.AvatarUrl,
            //    Number = 100,
            //    BackgroundColour = new SolidColorBrush(Colors.Transparent)

            //};
            //WideTileControl.DataContext = new TileData()
            //{
            //    Title = Additionalinformation.Instance.Login,
            //    Img = Additionalinformation.Instance.AvatarUrl,
            //    Number = 100,
            //    BackgroundColour = new SolidColorBrush(Colors.Transparent)

            //};
            //await LiveTileBuilder.ConvertControlsToTiles(WideTileControl, SquareTileControl);
            Frame.Navigate(typeof(SearchUserPage));
        }

        private void ExitProfile_Click(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            LocalSettingParams.RemoveAllParams();
            HttpRequest.Remoovecookie("");
            Frame.Navigate(typeof(MainPage));
        }
    }
}



