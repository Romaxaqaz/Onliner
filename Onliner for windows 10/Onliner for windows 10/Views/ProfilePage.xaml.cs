using HtmlAgilityPack;
using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.Views.Profile;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.Phone.UI.Input;
using Onliner_for_windows_10.Model.Message;
using Onliner_for_windows_10.Tile;
using LiveTileUWP;
using Onliner_for_windows_10.Tiles.TileTemplates;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Onliner_for_windows_10.ProfilePage
{
    public sealed partial class ProfilePage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        private readonly string NameButtonSendMessage = "Мои сообщения";

        private ProfileData profile = new ProfileData();
        private HtmlDocument resultat = new HtmlDocument();
        private Request request = new Request();
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
            Loaded += ProfilePage_Loaded1;
            ButtonsControlStackPanel.DataContext = Additionalinformation.Instance;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string key = (string)e.Parameter;
            if (key == null || key == "")
            {
                Showh(string.Empty);
            }
            else
            {
                string prifileUrl = "https://profile.onliner.by/user/" + (string)e.Parameter;
                Showh(prifileUrl);
            }

        }

        private void ProfilePage_Loaded1(object sender, RoutedEventArgs e)
        {
            Additionalinformation.Instance.NameActivePage = "Профиль";
        }

        public void Showh(string profileUrl)
        {
            bool changeInfo = false;
            if (profileUrl == string.Empty)
            {
                profileUrl = "https://profile.onliner.by";
                changeInfo = true;
            }
            var parsHtml = new ParsingHtml.ParsingHtml();
            request.GetRequestOnliner(profileUrl);
            string resultGetRequest = request.ResultGetRequsetString;
            resultat.LoadHtml(resultGetRequest);
            string loginUser = parsHtml.ParsElementHtml(accauntNameParsParam, resultat);
            string avatarImage = parsHtml.ParsElementHtml(accauntImageParsParam, resultat);

            profile.AccauntName = loginUser;
            profile.Avatar = avatarImage;
            profile.Status = parsHtml.ParsElementHtml(accauntStatusParsParam, resultat);
            //profile.ProfileNumbers = parsHtml.ParsElementHtml(accauntNumbersParsParam, resultat);
            if (changeInfo)
            {
                profile.Money = parsHtml.ParsElementHtml(accauntMoneyParsParam, resultat);
                Additionalinformation.Instance.AvatarUrl = avatarImage;
                Additionalinformation.Instance.Login = loginUser;
                AppBarForMobile.Visibility = Visibility.Visible;
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

        private void ellipse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("Autorization");
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
            if (ProfileFlipview.SelectedIndex == 0)
            {
                ProfileTabButton.IsChecked = true;
                ProfileTabButtonOther.IsChecked = false;
            }
            else
            {
                ProfileTabButton.IsChecked = false;
                ProfileTabButtonOther.IsChecked = true;
            }
        }

        private void ProfileTabButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radBut = sender as RadioButton;
            if (radBut.Name == "ProfileTabButton")
            {
                ProfileFlipview.SelectedIndex = 0;
            }
            else
            {
                ProfileFlipview.SelectedIndex = 1;
            }
        }

        private async void PinWindow_Click(object sender, RoutedEventArgs e)
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
        }
    }
}



