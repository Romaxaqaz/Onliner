using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OnlinerApp.ViewModel;

namespace OnlinerApp.Views
{
    public sealed partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            this.InitializeComponent();
            this.DataContext = new ProfileViewModel();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (!Frame.CanGoBack) return;
            e.Handled = true;
            Frame.GoBack();
        }


    }
}



