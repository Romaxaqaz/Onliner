using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.ProfilePage;
using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using System.Threading.Tasks;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class WeatherPage : Page
    {
        private Request request = new Request();
        private List<object> temperatureToday = new List<object>();

        public WeatherPage()
        {
            this.InitializeComponent();
            this.Loaded += WeatherPage_Loaded;
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

        private async void WeatherPage_Loaded(object sender, RoutedEventArgs e)
        {
            Additionalinformation.Instance.NameActivePage = "Погода";
            try
            {
                //get weather
                var responsseObject = await request.Weather();

                NowWeatherGrid.DataContext = responsseObject.now;
                NowWeatherMoreGrid.DataContext = responsseObject.now;

                if (responsseObject.today.morning != null)
                {
                    temperatureToday.Add(responsseObject.today.morning);
                }
                temperatureToday.Add(responsseObject.today.day);
                temperatureToday.Add(responsseObject.today.evening);
                temperatureToday.Add(responsseObject.today.night);

                TemperatureTodayListView.ItemsSource = temperatureToday;
                WeekWeather.ItemsSource = responsseObject.forecast;

                WeatherProgressRing.IsActive = false;
            }
            catch (FormatException ex)
            {
                MessageDialog message = new MessageDialog(ex.ToString());
                await message.ShowAsync();
            }
        }

        private void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            TownWeather.IsOpen = true;
        }
    }

}
