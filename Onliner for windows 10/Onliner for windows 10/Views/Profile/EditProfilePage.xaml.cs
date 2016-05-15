using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using OnlinerApp.ViewModel.ProfileViewModels;

namespace OnlinerApp.Views.Profile
{
    public sealed partial class EditProfilePage : Page
    {
        public EditProfilePage()
        {
            this.InitializeComponent();
            this.DataContext = new EditProfileViewModel();
          
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.GoBack();
        }
    }
}
