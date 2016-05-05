using System.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using MyToolkit.Command;
using Template10.Mvvm;
using Newtonsoft.Json;
using Onliner.Expansion;
using Onliner.SQLiteDataBase;
using Onliner.Http;
using static Onliner.Setting.SettingParams;

namespace Onliner_for_windows_10.View_Model.Settings
{
    public class SettingViewModel : ViewModelBase
    {
        private readonly HttpRequest _requset = new HttpRequest();

        #region Constructor
        public SettingViewModel()
        {
            CheckUncheckAutoLoadnewsCommand = new RelayCommand<object>(CheckUncheckAutoLoadnews);
            ChangedThemeAppCommand = new RelayCommand<object>(ChangedThemeApp);
            PivotSelectedIndexCommand = new RelayCommand<object>(PivotSelectedIndexEvent);
            ChangedNumberNewsCacheCommand = new RelayCommand(ChangedNumberNewsCache);
            ClearCacheCommand = new RelayCommand(CleadCache);
            ChangeTown = new RelayCommand<object>(SetTownIdWeather);
            CurrentTypeCommand = new RelayCommand<object>(CurrentTypeChanged);
            BankActionCommand = new RelayCommand<object>(BankActionChanged);
        }

        private void CurrentTypeChanged(object obj)
        {
            var current = obj.ToString();
            SetParamsSetting(CurrentTypeKey, current);
            CurrentType = current;
            UpdateCurrent();
        }

        private void BankActionChanged(object obj)
        {
            var type = obj.GetType();
            var value = type.GetProperty("Value");
            var valueObj = value.GetValue(obj, null);

            SetParamsSetting(BankActionKey, valueObj.ToString());
            BankAction = valueObj.ToString();
            UpdateCurrent();
        }

        private async void UpdateCurrent()
        {
            var newCurent = await _requset.Bestrate(CurrentType, BankAction);
            NewCurrent = newCurent.Amount;
            ShellViewModel.Instance.Current = newCurent.Amount;
        }

        #endregion

        #region Methods

        #region Theme App
        /// <summary>
        /// Change the theme
        /// </summary>
        /// <param name="obj"></param>
        private void ChangedThemeApp(object obj)
        {
            var index = (int)obj;
            SetThemeApp(index);
        }

        /// <summary>
        /// Set theme
        /// </summary>
        /// <param name="index">Ligth = 0, Dark = 1</param>
        private void SetThemeApp(int index)
        {
            switch (index)
            {
                case 0:
                    SetParamsSetting(ThemeAppKey, index.ToString());
                    SetLigthThemeApp();
                    break;
                case 1:
                    SetParamsSetting(ThemeAppKey, index.ToString());
                    SetDarkThemeApp();
                    break;
            }
        }

        private void SetDarkThemeApp()
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = Windows.UI.Colors.Black;
                statusBar.ForegroundColor = Windows.UI.Colors.White;
                statusBar.BackgroundOpacity = 1;
            }
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Black;
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.BackgroundColor = Colors.Black;
                    titleBar.ForegroundColor = Colors.White;
                }
            }

            (Application.Current.Resources["BackGroundCustomColorBrush"] as SolidColorBrush).Color = Colors.Black;
            (Application.Current.Resources["BackGroundCustomNewsColorBrush"] as SolidColorBrush).Color = Colors.Black;
            (Application.Current.Resources["BackGroundCustomNewsItemColorBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 43, 43, 43);
            (Application.Current.Resources["BackGroundCustomNewsHeaderColorBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 76, 74, 75);
            (Application.Current.Resources["BackgroundCommentsColorBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 43, 43, 43);

            (Application.Current.Resources["ForegroundCustomBlackBrush"] as SolidColorBrush).Color = Colors.Gray;
            (Application.Current.Resources["ForegroundCustomOtherBlackBrush"] as SolidColorBrush).Color = Colors.White;
            (Application.Current.Resources["BackGroundCustomYellowColorBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 43, 43, 43);
            (Application.Current.Resources["BackGroundCustomHeaderColorBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 43, 43, 43);
            (Application.Current.Resources["BackGroundCustomHeaderChildColorBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 43, 43, 43);

            (Application.Current.Resources["ForegroundCustomWhiteBrush"] as SolidColorBrush).Color = Colors.Black;
            (Application.Current.Resources["ForegroundCustomTextOnBlackBlackBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 170, 170, 170);
            (Application.Current.Resources["FillIcoCustomColorBrush"] as SolidColorBrush).Color = Colors.White;
            (Application.Current.Resources["BackGroundCustomMainElementColorBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 43, 43, 43);
        }

        private void SetLigthThemeApp()
        {
            //mobile customization
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = Windows.UI.Colors.Yellow;
                statusBar.ForegroundColor = Windows.UI.Colors.Black;
                statusBar.BackgroundOpacity = 1;
            }
            //pc customiztion
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Yellow;
                    titleBar.ButtonForegroundColor = Colors.Black;
                    titleBar.BackgroundColor = Colors.Yellow;
                    titleBar.ForegroundColor = Colors.Black;
                }
            }

            (Application.Current.Resources["BackGroundCustomColorBrush"] as SolidColorBrush).Color = Colors.White;
            (Application.Current.Resources["BackGroundCustomNewsColorBrush"] as SolidColorBrush).Color = Colors.Gainsboro;
            (Application.Current.Resources["BackGroundCustomNewsItemColorBrush"] as SolidColorBrush).Color = Colors.White;
            (Application.Current.Resources["BackGroundCustomNewsHeaderColorBrush"] as SolidColorBrush).Color = Colors.White;
            (Application.Current.Resources["BackgroundCommentsColorBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 255, 247, 247);

            (Application.Current.Resources["ForegroundCustomBlackBrush"] as SolidColorBrush).Color = Colors.Black;
            (Application.Current.Resources["ForegroundCustomOtherBlackBrush"] as SolidColorBrush).Color = Colors.Black;
            (Application.Current.Resources["BackGroundCustomYellowColorBrush"] as SolidColorBrush).Color = Colors.Yellow;
            (Application.Current.Resources["BackGroundCustomHeaderColorBrush"] as SolidColorBrush).Color = Colors.Yellow;
            (Application.Current.Resources["BackGroundCustomHeaderChildColorBrush"] as SolidColorBrush).Color = Colors.White;

            (Application.Current.Resources["ForegroundCustomWhiteBrush"] as SolidColorBrush).Color = Colors.White;
            (Application.Current.Resources["ForegroundCustomTextOnBlackBlackBrush"] as SolidColorBrush).Color = Colors.Black;
            (Application.Current.Resources["FillIcoCustomColorBrush"] as SolidColorBrush).Color = Colors.LightBlue;
            (Application.Current.Resources["BackGroundCustomMainElementColorBrush"] as SolidColorBrush).Color = Colors.Yellow;
        }

        public void GetThemeApp()
        {
            var indexValue = GetParamsSetting(ThemeAppKey) ?? 0;
            _themeAppIndex = Convert.ToInt32(indexValue);
            SetThemeApp(_themeAppIndex);

        }
        #endregion

        #region Cache News
        /// <summary>
        /// Clear cache news
        /// </summary>
        private void CleadCache()
        {
            foreach (var item in SqLiteDb.FileNameCollection)
            {
                RemoveFile(item);
            }
            //CacheSize = BytesToDouble(SqLiteDb.SizeByteFile).ToString();
        }

        /// <summary>
        /// Remove files DB
        /// </summary>
        /// <param name="filename"></param>
        public async void RemoveFile(string filename)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await sampleFile.DeleteAsync();
        }

        /// <summary>
        /// Conver byte to string data
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        private string BytesToDouble(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            long bytes = Math.Abs(byteCount);
            if (bytes == 0) return "0 " + suf[0];
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        private void ChangedNumberNewsCache()
        {
            var index = NumberNewsCache[NewsItemloadIndex];
            SetParamsSetting(NumberOfNewsitemsToTheCacheKey, index);
        }
        #endregion

        private void PivotSelectedIndexEvent(object obj)
        {
            var index = (int)obj;
        }

        /// <summary>
        /// Set for city weather
        /// </summary>
        /// <param name="obj"></param>
        private async void SetTownIdWeather(object obj)
        {
            var town = obj as TownWeatherId;
            if (town == null) return;
            SetParamsSetting(TownWeatherIdKey, town.Id);
            var weather = await _requset.Weather(town.Id);
            ShellViewModel.Instance.Weather = weather == null ? (string)GetParamsSetting(LastWeatherKey) : weather.now.temperature;
        }

        /// <summary>
        /// Index city
        /// </summary>
        /// <param name="townid"></param>
        /// <returns></returns>
        private int GetIndex(string townid)
        {
            var id = 0;
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
        #endregion

        #region Properties
        private void CheckUncheckAutoLoadnews(object obj)
        {
            switch ((string)obj)
            {
                case "LoadImageCheckBox":
                    var boolValue = GetParamsSetting(LoadImageKey);
                    LoadImage = ChangeBool(boolValue);
                    break;
            }
        }

        private bool ChangeBool(object boolValue)
        {
            if (boolValue == null) boolValue = false;
            var value = Convert.ToBoolean(boolValue);
            value = value == false ? true : false;
            return value;
        }

        private bool _autoUpdateCheked;
        public bool AutoUpdateCheked
        {
            get
            {
                var boolValue = GetParamsSetting(AutoLoadNewsAtStartUpAppKey) ?? true;
                _autoUpdateCheked = Convert.ToBoolean(boolValue);
                return _autoUpdateCheked;
            }
            set
            {
                SetParamsSetting(AutoLoadNewsAtStartUpAppKey, value.ToString());
                Set(ref _autoUpdateCheked, value);
            }
        }

        private bool _loadImage;
        public bool LoadImage
        {
            get
            {
                var boolValue = GetParamsSetting(LoadImageKey) ?? false;
                var value = Convert.ToBoolean(boolValue);
                _loadImage = value;
                return _loadImage;
            }
            set
            {
                SetParamsSetting(LoadImageKey, value.ToString());
                Set(ref _loadImage, value);
            }
        }

        private int _themeAppIndex;
        public int ThemeAppIndex
        {
            get
            {
                var indexValue = GetParamsSetting(ThemeAppKey) ?? 0;
                _themeAppIndex = Convert.ToInt32(indexValue);
                return _themeAppIndex;
            }
            set
            {
                SetParamsSetting(ThemeAppKey, value.ToString());
                Set(ref _themeAppIndex, value);
            }
        }

        private int _newsItemloadIndex;
        public int NewsItemloadIndex
        {
            get
            {
                var indexValue = GetParamsSetting(NumberOfNewsitemsToTheCacheKey) ?? "50";
                _newsItemloadIndex = NumberNewsCache.IndexOf((string)indexValue);
                return _newsItemloadIndex;
            }
            set
            {
                SetParamsSetting(NumberOfNewsitemsToTheCacheKey, NumberNewsCache[value]);
                Set(ref _newsItemloadIndex, value);
            }
        }

        private string _cacheSize;
        public string CacheSize
        {
            get { return _cacheSize; }
            set { Set(ref _cacheSize, value); }
        }

        private int _pivotSelectedIndex;
        public int PivotSelectedIndex
        {
            get { return _pivotSelectedIndex; }
            set { Set(ref _pivotSelectedIndex, value); }
        }

        private int _townWeatherIdindex;
        public int TownWeatherIdindex
        {
            get
            {
                var town = GetParamsSetting(TownWeatherIdKey);
                if (town == null) return 0;
                _townWeatherIdindex = GetIndex(town.ToString());
                return _townWeatherIdindex;
            }
            set
            {
                Set(ref _townWeatherIdindex, value);
            }
        }

        private bool _toggleSwitchNewsDataTemplateType = true;
        public bool ToggleSwitchNewsDataTemplateType
        {
            get
            {
                var toggle = GetParamsSetting(ToggleSwitchNewsDataTemplateTypeKey);
                if (toggle == null) return true;
                _toggleSwitchNewsDataTemplateType = Convert.ToBoolean(toggle);
                return _toggleSwitchNewsDataTemplateType;
            }
            set
            {
                SetDataTemplateType(value);
                SetParamsSetting(ToggleSwitchNewsDataTemplateTypeKey, value.ToString());
                Set(ref _toggleSwitchNewsDataTemplateType, value);
            }
        }

        private void SetDataTemplateType(bool value) => 
            SetParamsSetting(NewsDataTemplateKey, value == true ? TileDataTemplate : ListDataTemplate);

        public bool ToggleSwitchNewsDataTemplateTypeIsEnable
        {
            get
            {
                if (DeviceType.IsMobile)
                    return true;
                return false;
            }
        }

        private string _currentType;
        public string CurrentType
        {
            get
            {
                var current = GetParamsSetting(CurrentTypeKey);
                return current == null ? "USD" : current.ToString();
            }
            set { Set(ref _currentType, value); }
        }

        private string _bankAction;
        public string BankAction
        {
            get
            {
                var bankAction = GetParamsSetting(BankActionKey);
                return bankAction == null ? "nbrb" : bankAction.ToString();
            }
            set { Set(ref _bankAction, value); }
        }

        private string _newCurrent;
        public string NewCurrent
        {
            get { return _newCurrent; }
            set { Set(ref _newCurrent, value); }
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get
            {
                var town = GetParamsSetting(CurrentTypeKey);
                if (town == null) return 0;
                _currentIndex = CurrentTypeList.IndexOf(town.ToString());
                return _currentIndex;
            }
            set
            {
                Set(ref _currentIndex, value);
            }
        }

        private int _bankActionIndex;
        public int BankActionIndex
        {
            get
            {
                var index = 0;
                var town = GetParamsSetting(BankActionKey);
                if (town == null) return 0;
                foreach (var item in BankActionDictionary)
                {
                    if (item.Value.Equals(town.ToString()))
                    {
                        return index;
                    }
                    index++;
                }
                return _bankActionIndex;
            }
            set
            {
                Set(ref _bankActionIndex, value);
            }
        }

        #endregion

        #region Commands
        public RelayCommand<object> CheckUncheckAutoLoadnewsCommand { get; private set; }
        public RelayCommand<object> ChangedThemeAppCommand { get; private set; }
        public RelayCommand ChangedNumberNewsCacheCommand { get; private set; }
        public RelayCommand<object> PivotSelectedIndexCommand { get; private set; }
        public RelayCommand ClearCacheCommand { get; private set; }
        public RelayCommand<object> ChangeTown { get; private set; }
        public RelayCommand<object> CurrentTypeCommand { get; private set; }
        public RelayCommand<object> BankActionCommand { get; private set; }
        #endregion

        #region Collections

        public ObservableCollection<string> NumberNewsCache { get; } = new ObservableCollection<string>() { "50", "100", "150" };

        public ObservableCollection<TownWeatherId> TownList
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

        public Dictionary<string, string> BankActionDictionary { get; } = new Dictionary<string, string>
        {
            ["НБРБ"] = "nbrb",
            ["Продажа"] = "sale",
            ["Покупка"] = "buy"
        };

        public List<string> CurrentTypeList { get; } = new List<string> { "USD", "EUR", "RUB" };

        #endregion
    }
}
