using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Onliner_for_windows_10
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Login.Request requestToApi = new Login.Request();
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == "") { var dialog = new Windows.UI.Popups.MessageDialog("Введите логин"); await dialog.ShowAsync(); }
            else if (PasswordBox.Password == "") { var dialog = new Windows.UI.Popups.MessageDialog("Введите пароль"); await dialog.ShowAsync(); }
            else
            {
                requestToApi.PostRequestUserApi(LoginTextBox.Text, PasswordBox.Password);
            }
        }

        private void LaterLogIn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage.ProfilePage));
        }
    }
}
