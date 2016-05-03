using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using Onliner_for_windows_10.View_Model;

namespace Onliner_for_windows_10.Model.Message
{

    public sealed partial class MessagePage : Page
    {
       private MessageViewModel viewModel = new MessageViewModel();

        public MessagePage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {

            if (viewModel.ViewContentMessage)
            {
                viewModel.ViewContentMessage = false;
            }
            else
            {
                FrameGoBack();
            }
        }

        private void FrameGoBack()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void UserHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            Frame.Navigate(typeof(ProfilePage.ProfilePage), hyperlinkButton.Content);
        }
    }
}
