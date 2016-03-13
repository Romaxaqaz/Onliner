using Onliner_for_windows_10.ProfilePage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using Onliner_for_windows_10.View_Model;
using System.Threading.Tasks;
using System;

namespace Onliner_for_windows_10.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class WeatherPage : Page
    {
        private WeatherViewModel viewModel = new WeatherViewModel();

        public WeatherPage()
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
