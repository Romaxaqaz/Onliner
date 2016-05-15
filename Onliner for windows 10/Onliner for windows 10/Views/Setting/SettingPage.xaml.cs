using System;
using Windows.ApplicationModel.Email;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OnlinerApp.ViewModel.Settings;

namespace OnlinerApp.Views.Setting
{
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
            this.DataContext = new SettingViewModel();
        }

        private async void RecallButton_Click(object sender, RoutedEventArgs e)
        {
            const string app = "https://www.microsoft.com/store/apps/9nblggh645s7";
            var uri = new Uri(app);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Developer_Click(object sender, RoutedEventArgs e)
        {
            var emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("win10app@outlook.com"));
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);

        }

        private void PrivatePolicy_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PrivacyPolicy));
        }

    }
}
