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
        private readonly HttpRequest _httpRequest = new HttpRequest();
        private ObservableCollection<Forecast> _forecastItems;
        private ObservableCollection<object> _weatherTodayItems;
        public ObservableCollection<TownWeatherId> TownList { get; private set; }
        public Now now;

        public WeatherViewModel()
        {
            TownList = FillTownList;
            _forecastItems = new ObservableCollection<Forecast>();
            ChngeTown = new RelayCommand<object>(async (obj) => await GetTownId(obj));
        }

        #region Methods
        /// <summary>
        /// Get town ID out combobox
        /// </summary>
        /// <param name="obj">Combobox</param>
        private async Task GetTownId(object obj)
        {
            var combo = obj as ComboBox;
            var town = combo?.SelectedItem as TownWeatherId;
            if (town != null) await GeteWatherViewModel(town.Id);
        }

        /// <summary>
        /// Fill combobox item
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TownWeatherId> FillTownList
        {
            get
            {
                string fileContent;
                var fileStream = File.OpenRead("Files/Weather/" + "townWeather.txt");
                using (var reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
                return JsonConvert.DeserializeObject<ObservableCollection<TownWeatherId>>(fileContent);
            }
        }

        /// <summary>
        /// Get weather data
        /// </summary>
        /// <param name="townId">ID town</param>
        private async Task GeteWatherViewModel(string townId = "26850")
        {
            try
            {
                CurrentProgress = true;

                var temperatureToday = new ObservableCollection<object>();
                var responsseObject = await _httpRequest.Weather(townId);
                if (responsseObject.today.morning != null)
                {
                    temperatureToday.Add(responsseObject.today.morning);
                }
                temperatureToday.Add(responsseObject.today.day);
                temperatureToday.Add(responsseObject.today.evening);
                temperatureToday.Add(responsseObject.today.night);

                ForecastItems = responsseObject.forecast;
                WeatherTodayItems = temperatureToday;
                Now = responsseObject.now;

                CurrentProgress = false;
            }
            catch (Exception)
            {
                var msg = new MessageDialog("Упс, вы не подключены к интернету :(");
                await msg.ShowAsync();
            }
        }

        private int GetIndex(string townid)
        {
            int id = 0;
            foreach (var item in TownList)
            {
                if (item.Id.Equals(townid))
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
                return town == null ? 0 : GetIndex(town.ToString());
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
            get { return _forecastItems; }
            set { Set(ref _forecastItems, value); }
        }

        /// <summary>
        /// Today list
        /// </summary>
        public ObservableCollection<object> WeatherTodayItems
        {
            get { return _weatherTodayItems; }
            set { Set(ref _weatherTodayItems, value); }
        }
        #endregion

        #region Command
        /// <summary>
        /// Change town command
        /// </summary>
        public RelayCommand<object> ChngeTown { get; private set; }
        #endregion
    }

    public class TownWeatherId
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
