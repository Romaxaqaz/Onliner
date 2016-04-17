using Onliner_for_windows_10.View_Model.Settings;
using System;
using Windows.ApplicationModel.Email;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Onliner_for_windows_10.Views.Setting
{
    public sealed partial class SettingPage : Page
    {
        SettingViewModel viewModel = new SettingViewModel();

        public SettingPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        private async void RecallButton_Click(object sender, RoutedEventArgs e)
        {
            string app = "https://www.microsoft.com/store/apps/9nblggh645s7";
            var uri = new Uri(app);
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void Developer_Click(object sender, RoutedEventArgs e)
        {
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("win10app@outlook.com"));
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);

        }

        private void PrivatePolicy_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PrivacyPolicy));
        }
    }
}
