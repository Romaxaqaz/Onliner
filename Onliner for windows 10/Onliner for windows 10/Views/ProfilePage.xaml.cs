using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using Onliner_for_windows_10.View_Model;

namespace Onliner_for_windows_10.ProfilePage
{
    public sealed partial class ProfilePage : Page
    {
        private ProfileViewModel viewModel = new ProfileViewModel();

        public ProfilePage()
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
            Frame rootFrame = Window.Current.Content as Frame;
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }


    }
}



