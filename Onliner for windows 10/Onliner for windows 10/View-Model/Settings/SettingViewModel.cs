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
        private HttpRequest requset = new HttpRequest();

        #region Constructor
        public SettingViewModel()
        {
            CheckUncheckAutoLoadnewsCommand = new RelayCommand<object>((obj) => CheckUncheckAutoLoadnews(obj));
            ChangedThemeAppCommand = new RelayCommand<object>((obj) => ChangedThemeApp(obj));
            PivotSelectedIndexCommand = new RelayCommand<object>((obj) => PivotSelectedIndexEvent(obj));
            ChangedNumberNewsCacheCommand = new RelayCommand(() => ChangedNumberNewsCache());
            ClearCacheCommand = new RelayCommand(() => CleadCache());
            ChangeTown = new RelayCommand<object>((obj) => SetTownIDWeather(obj));
            CurrentTypeCommand = new RelayCommand<object>((obj) => CurrentTypeChanged(obj));
            BankActionCommand = new RelayCommand<object>((obj) => BankActionChanged(obj));
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
            var key = type.GetProperty("Key");
            var value = type.GetProperty("Value");
            var keyObj = key.GetValue(obj, null);
            var valueObj = value.GetValue(obj, null);

            SetParamsSetting(BankActionKey, valueObj.ToString());
            BankAction = valueObj.ToString();
            UpdateCurrent();
        }

        private async void UpdateCurrent()
        {
            var newCurent = await requset.Bestrate(CurrentType, BankAction);
            NewCurrent = newCurent.amount;
            ShellViewModel.Instance.Current = newCurent.amount;
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
            int index = (int)obj;
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
            var indexValue = GetParamsSetting(ThemeAppKey);
            if (indexValue == null) indexValue = 0;
            themeAppIndex = Convert.ToInt32(indexValue);
            SetThemeApp(themeAppIndex);

        }
        #endregion

        #region Cache News
        /// <summary>
        /// Clear cache news
        /// </summary>
        private void CleadCache()
        {
            foreach (var item in SQLiteDB.FileNameCollection)
            {
                RemoveFile(item);
            }
            CacheSize = BytesToDouble(SQLiteDB.GetSizeByteDB()).ToString();
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
        private async void SetTownIDWeather(object obj)
        {
            TownWeatherID town = obj as TownWeatherID;
            SetParamsSetting(TownWeatherIdKey, town.ID);
            var weather = await requset.Weather(town.ID);
            ShellViewModel.Instance.Weather = weather == null ? (string)GetParamsSetting(LastWeatherKey) : weather.now.temperature;
        }

        /// <summary>
        /// Index city
        /// </summary>
        /// <param name="townid"></param>
        /// <returns></returns>
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
        #endregion

        #region Properties
        private void CheckUncheckAutoLoadnews(object obj)
        {
            object boolValue = null;
            switch ((string)obj)
            {
                case "LoadImageCheckBox":
                    boolValue = GetParamsSetting(LoadImageKey);
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

        private bool autoUpdateCheked = false;
        public bool AutoUpdateCheked
        {
            get
            {
                var boolValue = GetParamsSetting(AutoLoadNewsAtStartUpAppKey);
                if (boolValue == null) boolValue = true;
                autoUpdateCheked = Convert.ToBoolean(boolValue);
                return autoUpdateCheked;
            }
            set
            {
                SetParamsSetting(AutoLoadNewsAtStartUpAppKey, value.ToString());
                Set(ref autoUpdateCheked, value);
            }
        }

        private bool loadImage = false;
        public bool LoadImage
        {
            get
            {
                var boolValue = GetParamsSetting(LoadImageKey);
                if (boolValue == null) boolValue = false;
                var value = Convert.ToBoolean(boolValue);
                loadImage = value;
                return loadImage;
            }
            set
            {
                SetParamsSetting(LoadImageKey, value.ToString());
                Set(ref loadImage, value);
            }
        }

        private int themeAppIndex = 0;
        public int ThemeAppIndex
        {
            get
            {
                var indexValue = GetParamsSetting(ThemeAppKey);
                if (indexValue == null) indexValue = 0;
                themeAppIndex = Convert.ToInt32(indexValue);
                return themeAppIndex;
            }
            set
            {
                SetParamsSetting(ThemeAppKey, value.ToString());
                Set(ref themeAppIndex, value);
            }
        }

        private int newsItemloadIndex;
        public int NewsItemloadIndex
        {
            get
            {
                var indexValue = GetParamsSetting(NumberOfNewsitemsToTheCacheKey);
                if (indexValue == null) indexValue = "50";
                newsItemloadIndex = NumberNewsCache.IndexOf((string)indexValue);
                return newsItemloadIndex;
            }
            set
            {
                SetParamsSetting(NumberOfNewsitemsToTheCacheKey, NumberNewsCache[value]);
                Set(ref newsItemloadIndex, value);
            }
        }

        private string cacheSize;
        public string CacheSize
        {
            get { return cacheSize; }
            set { Set(ref cacheSize, value); }
        }

        private int pivotSelectedIndex = 0;
        public int PivotSelectedIndex
        {
            get { return pivotSelectedIndex; }
            set { Set(ref pivotSelectedIndex, value); }
        }

        private int townWeatherIdindex;
        public int TownWeatherIdindex
        {
            get
            {
                var town = GetParamsSetting(TownWeatherIdKey);
                if (town == null) return 0;
                townWeatherIdindex = GetIndex(town.ToString());
                return townWeatherIdindex;
            }
            set
            {
                Set(ref townWeatherIdindex, value);
            }
        }

        private bool toggleSwitchNewsDataTemplateType = true;
        public bool ToggleSwitchNewsDataTemplateType
        {
            get
            {
                var toggle = GetParamsSetting(ToggleSwitchNewsDataTemplateTypeKey);
                if (toggle == null) return true;
                toggleSwitchNewsDataTemplateType = Convert.ToBoolean(toggle);
                return toggleSwitchNewsDataTemplateType;
            }
            set
            {
                SetDataTemplateType(value);
                SetParamsSetting(ToggleSwitchNewsDataTemplateTypeKey, value.ToString());
                Set(ref toggleSwitchNewsDataTemplateType, value);
            }
        }

        private void SetDataTemplateType(bool value)
        {
            if (value == true)
            {
                SetParamsSetting(NewsDataTemplateKey, TileDataTemplate);
            }
            else
            {
                SetParamsSetting(NewsDataTemplateKey, ListDataTemplate);
            }
        }

        public bool ToggleSwitchNewsDataTemplateTypeIsEnable
        {
            get
            {
                if (DeviceType.IsMobile)
                    return true;
                return false;
            }
        }

        private string currentType;
        public string CurrentType
        {
            get
            {
                var current = GetParamsSetting(CurrentTypeKey);
                if (current == null) return "USD";
                return current.ToString();
            }
            set { Set(ref currentType, value); }
        }

        private string bankAction;
        public string BankAction
        {
            get
            {
                var bankAction = GetParamsSetting(BankActionKey);
                if (bankAction == null) return "nbrb";
                return bankAction.ToString();
            }
            set { Set(ref bankAction, value); }
        }

        private string newCurrent;
        public string NewCurrent
        {
            get { return newCurrent; }
            set { Set(ref newCurrent, value); }
        }

        private int currentIndex;
        public int CurrentIndex
        {
            get
            {
                var town = GetParamsSetting(CurrentTypeKey);
                if (town == null) return 0;
                currentIndex = currentTypeList.IndexOf(town.ToString());
                return currentIndex;
            }
            set
            {
                Set(ref currentIndex, value);
            }
        }

        private int bankActionIndex;
        public int BankActionIndex
        {
            get
            {
                int index = 0;
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
                return bankActionIndex;
            }
            set
            {
                Set(ref bankActionIndex, value);
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
        public ObservableCollection<string> numberNewsCache = new ObservableCollection<string>() { "50", "100", "150" };
        public ObservableCollection<string> NumberNewsCache
        {
            get { return numberNewsCache; }
        }

        public ObservableCollection<TownWeatherID> TownList
        {
            get
            {
                string fileContent;
                var fileStream = File.OpenRead("Files/Weather/" + "townWeather.txt");
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
                return JsonConvert.DeserializeObject<ObservableCollection<TownWeatherID>>(fileContent);
            }
        }

        private Dictionary<string, string> bankActionDictionary = new Dictionary<string, string>
        {
            ["НБРБ"] = "nbrb",
            ["Продажа"] = "sale",
            ["Покупка"] = "buy"
        };

        public Dictionary<string, string> BankActionDictionary
        {
            get { return bankActionDictionary; }
        }

        private List<string> currentTypeList = new List<string> { "USD", "EUR", "RUB" };
        public List<string> CurrentTypeList
        {
            get { return currentTypeList; }
        }
        #endregion
    }
}
