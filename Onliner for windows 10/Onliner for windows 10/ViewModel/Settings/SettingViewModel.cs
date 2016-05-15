using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using MyToolkit.Command;
using Newtonsoft.Json;
using Onliner.Expansion;
using Onliner.Http;
using Onliner.Setting;
using Onliner.SQLiteDataBase;
using OnlinerApp.View_Model;
using Template10.Mvvm;

namespace OnlinerApp.ViewModel.Settings
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
            SettingParams.SetParamsSetting(SettingParams.CurrentTypeKey, current);
            CurrentType = current;
            UpdateCurrent();
        }

        private void BankActionChanged(object obj)
        {
            var type = obj.GetType();
            var value = type.GetProperty("Value");
            var valueObj = value.GetValue(obj, null);

            SettingParams.SetParamsSetting(SettingParams.BankActionKey, valueObj.ToString());
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
        private static void SetThemeApp(int index)
        {
            switch (index)
            {
                case 0:
                    SettingParams.SetParamsSetting(SettingParams.ThemeAppKey, index.ToString());
                    SetLigthThemeApp();
                    break;
                case 1:
                    SettingParams.SetParamsSetting(SettingParams.ThemeAppKey, index.ToString());
                    SetDarkThemeApp();
                    break;
            }
        }

        private static void SetDarkThemeApp()
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
            SetResourcesColor("BackGroundCustomColorBrush", Colors.Black);
            SetResourcesColor("BackGroundCustomNewsColorBrush", Colors.Black);
            SetResourcesColor("BackGroundCustomNewsItemColorBrush", Color.FromArgb(255, 43, 43, 43));
            SetResourcesColor("BackGroundCustomNewsHeaderColorBrush", Color.FromArgb(255, 76, 74, 75));
            SetResourcesColor("BackgroundCommentsColorBrush", Color.FromArgb(255, 43, 43, 43));

            SetResourcesColor("ForegroundCustomBlackBrush", Colors.Gray);
            SetResourcesColor("ForegroundCustomOtherBlackBrush", Colors.White);
            SetResourcesColor("BackGroundCustomYellowColorBrush", Color.FromArgb(255, 43, 43, 43));
            SetResourcesColor("BackGroundCustomHeaderColorBrush", Color.FromArgb(255, 43, 43, 43));
            SetResourcesColor("BackGroundCustomHeaderChildColorBrush", Color.FromArgb(255, 43, 43, 43));

            SetResourcesColor("ForegroundCustomWhiteBrush", Colors.Black);
            SetResourcesColor("ForegroundCustomTextOnBlackBlackBrush", Color.FromArgb(255, 170, 170, 170));
            SetResourcesColor("FillIcoCustomColorBrush", Colors.White);
            SetResourcesColor("BackGroundCustomMainElementColorBrush", Color.FromArgb(255, 43, 43, 43));
        }

        private static void SetLigthThemeApp()
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

            SetResourcesColor("BackGroundCustomColorBrush", Colors.White);
            SetResourcesColor("BackGroundCustomNewsColorBrush", Colors.Gainsboro);
            SetResourcesColor("BackGroundCustomNewsItemColorBrush", Colors.White);
            SetResourcesColor("BackGroundCustomNewsHeaderColorBrush", Colors.White);
            SetResourcesColor("BackgroundCommentsColorBrush", Color.FromArgb(255, 255, 247, 247));

            SetResourcesColor("ForegroundCustomBlackBrush", Colors.Black);
            SetResourcesColor("ForegroundCustomOtherBlackBrush", Colors.Black);
            SetResourcesColor("BackGroundCustomYellowColorBrush", Colors.Yellow);
            SetResourcesColor("BackGroundCustomHeaderColorBrush", Colors.Yellow);
            SetResourcesColor("BackGroundCustomHeaderChildColorBrush", Colors.White);

            SetResourcesColor("ForegroundCustomWhiteBrush", Colors.White);
            SetResourcesColor("ForegroundCustomTextOnBlackBlackBrush", Colors.Yellow);
            SetResourcesColor("FillIcoCustomColorBrush", Colors.Black);
            SetResourcesColor("BackGroundCustomMainElementColorBrush", Colors.LightBlue);

            SetResourcesColor("BackGroundCustomMainElementColorBrush", Colors.Yellow);
        }

        private static void SetResourcesColor(string key, Color value)
        {
            ((SolidColorBrush) Application.Current.Resources[key]).Color = value;
        }

        public void GetThemeApp()
        {
            var indexValue = SettingParams.GetParamsSetting(SettingParams.ThemeAppKey) ?? 0;
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
            var localFolder = ApplicationData.Current.LocalFolder;
            var sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
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
            var bytes = Math.Abs(byteCount);
            if (bytes == 0) return "0 " + suf[0];
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num) + suf[place];
        }

        private void ChangedNumberNewsCache()
        {
            var index = NumberNewsCache[NewsItemloadIndex];
            SettingParams.SetParamsSetting(SettingParams.NumberOfNewsitemsToTheCacheKey, index);
        }
        #endregion

        private static void PivotSelectedIndexEvent(object obj)
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
            SettingParams.SetParamsSetting(SettingParams.TownWeatherIdKey, town.Id);
            var weather = await _requset.Weather(town.Id);
            ShellViewModel.Instance.Weather = weather == null ? (string)SettingParams.GetParamsSetting(SettingParams.LastWeatherKey) : weather.now.temperature;
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
                    var boolValue = SettingParams.GetParamsSetting(SettingParams.LoadImageKey);
                    LoadImage = ChangeBool(boolValue);
                    break;
            }
        }

        private static bool ChangeBool(object boolValue)
        {
            if (boolValue == null) boolValue = false;
            var value = Convert.ToBoolean(boolValue);
            value = value == false;
            return value;
        }

        private bool _autoUpdateCheked;
        public bool AutoUpdateCheked
        {
            get
            {
                var boolValue = SettingParams.GetParamsSetting(SettingParams.AutoLoadNewsAtStartUpAppKey) ?? true;
                _autoUpdateCheked = Convert.ToBoolean(boolValue);
                return _autoUpdateCheked;
            }
            set
            {
                SettingParams.SetParamsSetting(SettingParams.AutoLoadNewsAtStartUpAppKey, value.ToString());
                Set(ref _autoUpdateCheked, value);
            }
        }

        private bool _loadImage;
        public bool LoadImage
        {
            get
            {
                var boolValue = SettingParams.GetParamsSetting(SettingParams.LoadImageKey) ?? false;
                var value = Convert.ToBoolean(boolValue);
                _loadImage = value;
                return _loadImage;
            }
            set
            {
                SettingParams.SetParamsSetting(SettingParams.LoadImageKey, value.ToString());
                Set(ref _loadImage, value);
            }
        }

        private int _themeAppIndex;
        public int ThemeAppIndex
        {
            get
            {
                var indexValue = SettingParams.GetParamsSetting(SettingParams.ThemeAppKey) ?? 0;
                _themeAppIndex = Convert.ToInt32(indexValue);
                return _themeAppIndex;
            }
            set
            {
                SettingParams.SetParamsSetting(SettingParams.ThemeAppKey, value.ToString());
                Set(ref _themeAppIndex, value);
            }
        }

        private int _newsItemloadIndex;
        public int NewsItemloadIndex
        {
            get
            {
                var indexValue = SettingParams.GetParamsSetting(SettingParams.NumberOfNewsitemsToTheCacheKey) ?? "50";
                _newsItemloadIndex = NumberNewsCache.IndexOf((string)indexValue);
                return _newsItemloadIndex;
            }
            set
            {
                SettingParams.SetParamsSetting(SettingParams.NumberOfNewsitemsToTheCacheKey, NumberNewsCache[value]);
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
                var town = SettingParams.GetParamsSetting(SettingParams.TownWeatherIdKey);
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
                var toggle = SettingParams.GetParamsSetting(SettingParams.ToggleSwitchNewsDataTemplateTypeKey);
                if (toggle == null) return true;
                _toggleSwitchNewsDataTemplateType = Convert.ToBoolean(toggle);
                return _toggleSwitchNewsDataTemplateType;
            }
            set
            {
                SetDataTemplateType(value);
                SettingParams.SetParamsSetting(SettingParams.ToggleSwitchNewsDataTemplateTypeKey, value.ToString());
                Set(ref _toggleSwitchNewsDataTemplateType, value);
            }
        }

        private static void SetDataTemplateType(bool value) => 
            SettingParams.SetParamsSetting(SettingParams.NewsDataTemplateKey, value ? SettingParams.TileDataTemplate : SettingParams.ListDataTemplate);

        public bool ToggleSwitchNewsDataTemplateTypeIsEnable => DeviceType.IsMobile;

        private string _currentType;
        public string CurrentType
        {
            get
            {
                var current = SettingParams.GetParamsSetting(SettingParams.CurrentTypeKey);
                return current == null ? "USD" : current.ToString();
            }
            set { Set(ref _currentType, value); }
        }

        private string _bankAction;
        public string BankAction
        {
            get
            {
                var bankAction = SettingParams.GetParamsSetting(SettingParams.BankActionKey);
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
                var town = SettingParams.GetParamsSetting(SettingParams.CurrentTypeKey);
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
                var town = SettingParams.GetParamsSetting(SettingParams.BankActionKey);
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
