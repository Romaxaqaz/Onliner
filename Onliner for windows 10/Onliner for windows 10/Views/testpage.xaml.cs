using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;
using System.Text.RegularExpressions;

using Onliner.Http;
using Onliner_for_windows_10.Views;

namespace Onliner_for_windows_10
{

    public sealed partial class testpage : Page
    {
        static HttpRequest req = new HttpRequest();

        public testpage()
        {
            this.InitializeComponent();
        }

        public static async void OAuthVk()
        {
            const string vkUri = "https://oauth.vk.com/authorize?client_id=4589307&scope=email&redirect_uri=https%3A%2F%2Fuser.api.onliner.by%2Fsocials%2Fvkontakte%3Fsession_id%3DLr4BKpZHRwEJstOhgXGN&response_type=code";
            Uri requestUri = new Uri(vkUri);
            Uri callbackUri = new Uri("https://oauth.vk.com");

            string res = string.Empty;
            WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None, requestUri, callbackUri);

            switch (result.ResponseStatus)
            {
                case WebAuthenticationStatus.ErrorHttp:
                    MessageDialog dialogError = new MessageDialog("Не удалось открыть страницу сервиса\n" +
                "Попробуйте войти в приложения позже!", "Ошибка");
                    await dialogError.ShowAsync();
                    break;
                case WebAuthenticationStatus.Success:
                    string responseString = result.ResponseData;
                    res = responseString;
                    MessageDialog dialogSuccess = new MessageDialog(responseString);
                    await dialogSuccess.ShowAsync();
                    break;
            }




            var regex = new Regex(@"(session_id[-A-Za-z0-9+@#\/%?=~_|!:,.;]+&)", RegexOptions.Compiled | RegexOptions.Multiline);         
            var myString = regex.Match(res);
            var s = myString.ToString();
            s = s.Replace("session_id%3", "").Replace("&", "");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //  OAuthVk();
            Frame.Navigate(typeof(NewsPage));
        }
    }
}


