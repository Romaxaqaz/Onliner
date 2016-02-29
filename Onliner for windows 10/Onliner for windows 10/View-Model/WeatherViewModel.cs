using MyToolkit.Command;
using MyToolkit.Mvvm;
using Newtonsoft.Json;
using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Onliner_for_windows_10.View_Model
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        private bool _currentProgress = true;
        private Request request = new Request();
        private ObservableCollection<Forecast> forecastItems;
        private ObservableCollection<object> weatherTodayItems;
        public ObservableCollection<TownWeatherID> TownList { get; private set; }
        public Now now;

        public int SelectedIndexItem { get { return 0; } }

        /// <summary>
        /// Change town command
        /// </summary>
        public RelayCommand<object> ChngeTown { get; private set; }

        /// <summary>
        /// Forecast list 
        /// </summary>
        public ObservableCollection<Forecast> ForecastItems
        {
            get { return forecastItems; }
            set
            {
                forecastItems = value;
                OnPropertyChanged("ForecastItems");
            }
        }

        /// <summary>
        /// Today list
        /// </summary>
        public ObservableCollection<object> WeatherTodayItems
        {
            get { return weatherTodayItems; }
            set
            {
                weatherTodayItems = value;
                OnPropertyChanged("WeatherTodayItems");
            }
        }

        /// <summary>
        /// Now weather
        /// </summary>
        public Now Now
        {
            get { return now; }
            set
            {
                now = value;
                OnPropertyChanged("Now");
            }
        }

        public WeatherViewModel()
        {
            TownList = FillTownList();
            forecastItems = new ObservableCollection<Forecast>();
            ChngeTown = new RelayCommand<object>((obj) => GetTownID(obj));
        }

        /// <summary>
        /// Get town ID out combobox
        /// </summary>
        /// <param name="obj">Combobox</param>
        private async void GetTownID(object obj)
        {
            ComboBox combo = obj as ComboBox;
            TownWeatherID town = combo.SelectedItem as TownWeatherID;
            await GeteWatherViewModel(town.ID);
        }

        /// <summary>
        /// Fill combobox item
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<TownWeatherID> FillTownList()
        {
            string fileContent;
            var fileStream = File.OpenRead("Files/Weather/" + "townWeather.txt");
            using (StreamReader reader = new StreamReader(fileStream))
            {
                fileContent = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<ObservableCollection<TownWeatherID>>(fileContent);
        }

        /// <summary>
        /// Get weather data
        /// </summary>
        /// <param name="townID">ID town</param>
        public async Task GeteWatherViewModel(string townID = "26850")
        {
            try
            {
                CurrentProgress = true;

                var _temperatureToday = new ObservableCollection<object>();
                var responsseObject = await request.Weather(townID);

                if (responsseObject.today.morning != null)
                {
                    _temperatureToday.Add(responsseObject.today.morning);
                }
                _temperatureToday.Add(responsseObject.today.day);
                _temperatureToday.Add(responsseObject.today.evening);
                _temperatureToday.Add(responsseObject.today.night);

                ForecastItems = responsseObject.forecast;
                WeatherTodayItems = _temperatureToday;
                Now = responsseObject.now;

                CurrentProgress = false;
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog(ex.ToString());
                await msg.ShowAsync();
            }
        }

        /// <summary>
        /// Progress indicator
        /// </summary>
        public bool CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                OnPropertyChanged("CurrentProgress");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public class TownWeatherID
    {
        public string ID { get; set; }
        public string Name { get; set; }

    }
}
