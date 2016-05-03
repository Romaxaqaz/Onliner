using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Command;
using Newtonsoft.Json;
using Template10.Mvvm;
using Onliner.Http;
using Onliner.Model.JsonModel.Weather;
using static Onliner.Setting.SettingParams;

namespace Onliner_for_windows_10.View_Model
{
    public class WeatherViewModel : ViewModelBase
    {
        private bool _currentProgress = true;
        private HttpRequest HttpRequest = new HttpRequest();
        private ObservableCollection<Forecast> forecastItems;
        private ObservableCollection<object> weatherTodayItems;
        public ObservableCollection<TownWeatherID> TownList { get; private set; }
        public Now now;

        public WeatherViewModel()
        {
            TownList = FillTownList();
            forecastItems = new ObservableCollection<Forecast>();
            ChngeTown = new RelayCommand<object>(async (obj) => await GetTownID(obj));
        }

        #region Methods
        /// <summary>
        /// Get town ID out combobox
        /// </summary>
        /// <param name="obj">Combobox</param>
        private async Task GetTownID(object obj)
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
        private async Task GeteWatherViewModel(string townID = "26850")
        {
            try
            {
                CurrentProgress = true;

                var _temperatureToday = new ObservableCollection<object>();
                var responsseObject = await HttpRequest.Weather(townID);
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
            catch (Exception)
            {
                MessageDialog msg = new MessageDialog("Упс, вы не подключены к интернету :(");
                await msg.ShowAsync();
            }
        }

        private int GetIndex(string townid)
        {
            int id = 0;
            foreach (var item in TownList)
            {
                if (item.ID.Equals(townid))
                {
                    return id;
                }
                id++;
            }
            return id;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await GeteWatherViewModel();
            await Task.CompletedTask;
        }
        #endregion

        #region Properties
        public int SelectedIndexItem
        {
            get
            {
                var town = GetParamsSetting(TownWeatherIdKey);
                if (town == null) return 0;
                return GetIndex(town.ToString());
            }
        }

        /// <summary>
        /// Progress indicator
        /// </summary>
        public bool CurrentProgress
        {
            get { return _currentProgress; }
            set { Set(ref _currentProgress, value); }
        }

        /// <summary>
        /// Now weather
        /// </summary>
        public Now Now
        {
            get { return now; }
            set { Set(ref now, value); }
        }

        #endregion

        #region Collections
        /// <summary>
        /// Forecast list 
        /// </summary>
        public ObservableCollection<Forecast> ForecastItems
        {
            get { return forecastItems; }
            set { Set(ref forecastItems, value); }
        }

        /// <summary>
        /// Today list
        /// </summary>
        public ObservableCollection<object> WeatherTodayItems
        {
            get { return weatherTodayItems; }
            set { Set(ref weatherTodayItems, value); }
        }
        #endregion

        #region Command
        /// <summary>
        /// Change town command
        /// </summary>
        public RelayCommand<object> ChngeTown { get; private set; }
        #endregion
    }

    public class TownWeatherID
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
