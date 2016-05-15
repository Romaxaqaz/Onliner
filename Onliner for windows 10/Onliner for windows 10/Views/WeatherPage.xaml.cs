using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using OnlinerApp.ViewModel;

namespace OnlinerApp.Views
{
    public sealed partial class WeatherPage : Page
    {
        public WeatherPage()
        {
            this.InitializeComponent();
            this.DataContext = new WeatherViewModel();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (!Frame.CanGoBack) return;
            e.Handled = true;
            Frame.GoBack();
        }

    }

}
