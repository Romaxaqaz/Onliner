using MyToolkit.Command;
using MyToolkit.Mvvm;
using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Onliner_for_windows_10.View_Model
{
    public class WeatherViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private Request request = new Request();
        public ObservableCollection<Forecast> Items { get; private set; }
        public ObservableCollection<object> WeatherTodayItems { get; private set; }
        public Now Now { get; set; }
        public RelayCommand ChangeWeatherTown { get; set; }

        private bool _currentProgress = true;

        public async void GeteWatherViewModel()
        {
            try
            {
                var _temperatureToday = new ObservableCollection<object>();
                var responsseObject = await request.Weather();

                if (responsseObject.today.morning != null)
                {
                    _temperatureToday.Add(responsseObject.today.morning);
                }
                _temperatureToday.Add(responsseObject.today.day);
                _temperatureToday.Add(responsseObject.today.evening);
                _temperatureToday.Add(responsseObject.today.night);

                Items = responsseObject.forecast;
                WeatherTodayItems = _temperatureToday;
                Now = responsseObject.now;
                CurrentProgress = false;
            }
            catch (Exception)
            { }
        }

        public bool CurrentProgress
        {
            get { return _currentProgress; }
            private set
            {
                _currentProgress = value;
                OnPropertyChanged("CurrentProgress");
            }
        }

        public event PropertyChangedEventHandler PropertyProgressChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyProgressChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
