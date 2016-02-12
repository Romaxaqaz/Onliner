using Onliner_for_windows_10.ProfilePage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using Onliner_for_windows_10.View_Model;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class WeatherPage : Page
    {

        public WeatherPage()
        {
            this.InitializeComponent();
            Loaded += (s, e) =>
            {
                Additionalinformation.Instance.NameActivePage = "Погода";
                var viewModel = new WeatherViewModel();
                viewModel.GeteWatherViewModel();
                this.DataContext = viewModel;

            };
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

        private void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            TownWeather.IsOpen = true;
        }

    }

}
