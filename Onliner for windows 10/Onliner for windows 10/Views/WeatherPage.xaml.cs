using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.ProfilePage;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            Loaded += WeatherPage_Loaded;
        }

        private void WeatherPage_Loaded(object sender, RoutedEventArgs e)
        {
            Additionalinformation.Instance.NameActivePage = "Погода";
            var responsseObject = request.Weather();
            NowWeatherGrid.DataContext = responsseObject.Result.now;
            NowWeatherMoreGrid.DataContext = responsseObject.Result.now;

            if (responsseObject.Result.today.morning != null)
            {
                temperatureToday.Add(responsseObject.Result.today.morning);
            }
            temperatureToday.Add(responsseObject.Result.today.day);
            temperatureToday.Add(responsseObject.Result.today.evening);
            temperatureToday.Add(responsseObject.Result.today.night);

            TemperatureTodayListView.ItemsSource = temperatureToday;
            WeekWeather.ItemsSource = responsseObject.Result.forecast;
        }

        private void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            TownWeather.IsOpen = true;
        }
    }

}
