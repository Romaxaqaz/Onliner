using HtmlAgilityPack;
using Onliner_for_windows_10.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Onliner_for_windows_10;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Media.Imaging;
// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.ProfilePage
{
    
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        ProfileData profile = new ProfileData();
        HtmlDocument resultat = new HtmlDocument();
        Request request = new Request();

        public string ProfileListData { get; set; }
        private string[] accauntNameParsParam = new string[4] { "h1", "class", "m-title", "(^([A-Za-z0-9-_]+))" };
        private string[] accauntImageParsParam = new string[4] { "div", "class", "uprofile-ph", "(?<=src=\").*?(?=\")" };
        private string[] accauntStatusParsParam = new string[4] { "p", "class", "uprofile-connect user-status", "" };

        public ProfilePage()
        {
            this.InitializeComponent();
            Showh();
        }

        public void Showh()
        {
            ParsingHtml.ParsingHtml parsHtml = new ParsingHtml.ParsingHtml();
            request.GetRequestOnliner("https://profile.onliner.by");
            string resultGetRequest = request.ResultGetRequsetString;
            resultat.LoadHtml(resultGetRequest);

            profile.AccauntName = parsHtml.ParsElementHtml(accauntNameParsParam, resultat);
            profile.Avatar = parsHtml.ParsElementHtml(accauntImageParsParam, resultat);
            profile.Status = parsHtml.ParsElementHtml(accauntStatusParsParam, resultat);

            List<ProfilePataList> _profileDataList = new List<ProfilePataList>();
            stackPanelAccauntInfo.DataContext = profile;
            List<HtmlNode> titleList = resultat.DocumentNode.Descendants().Where
            (x => (x.Name == "dl" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("uprofile-info"))).ToList();
            var infolist = titleList[0].Descendants("dt").ToList();
            var valuelist = titleList[0].Descendants("dd").ToList();
            for (int i = 0; i < infolist.Count; i++)
            {
                _profileDataList.Add(new ProfilePataList
                {
                    Info = infolist[i].InnerText,
                    Value = valuelist[i].InnerText
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
            var myimg = new BitmapImage(new Uri("ms-appx:///Image/123.jpg"));
            Application.Current.Resources["name"] = myimg;
            
        }
    }
}



