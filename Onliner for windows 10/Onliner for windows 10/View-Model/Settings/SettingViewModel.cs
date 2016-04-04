using MyToolkit.Command;
using Onliner.SQLiteDataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Template10.Mvvm;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static Onliner.Setting.SettingParams;

namespace Onliner_for_windows_10.View_Model.Settings
{
    public class SettingViewModel : ViewModelBase
    {
        #region Constructor
        public SettingViewModel()
        {
            CheckUncheckAutoLoadnewsCommand = new RelayCommand<object>((obj) => CheckUncheckAutoLoadnews(obj));
            ChangedThemeAppCommand = new RelayCommand<object>((obj) => ChangedThemeApp(obj));
            PivotSelectedIndexCommand = new RelayCommand<object>((obj) => PivotSelectedIndexEvent(obj));
            ChangedNumberNewsCacheCommand = new RelayCommand(() => ChangedNumberNewsCache());
            ClearCacheCommand = new RelayCommand(() => CleadCache());
        }
        #endregion

        #region Methods
        private void PivotSelectedIndexEvent(object obj)
        {
            var index = (int)obj;
            switch(index)
            {
                case 0:
                    break;
                case 2:
                    CacheSize = BytesToDouble(SQLiteDB.GetSizeByteDB());
                    break;
            }
        }

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

        private void ChangedThemeApp(object obj)
        {
            int index = (int)obj;
            SetThemeApp(index);
        }

        private void ChangedNumberNewsCache()
        {
            var index = NumberNewsCache[NewsItemloadIndex];
            SetParamsSetting(NumberOfNewsitemsToTheCacheKey, index);
        }
        #endregion

        #region Theme App

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
                var statusBar = StatusBar.GetForCurrentView();
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

        #region Properties
        private void CheckUncheckAutoLoadnews(object obj)
        {
            object boolValue = null;

            switch ((string)obj)
            {
                case "AutoUpdateCheckBox":
                    boolValue = GetParamsSetting(AutoLoadNewsAtStartUpAppKey);
                    AutoUpdateCheked = ChangeBool(boolValue);
                    break;
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
                if (boolValue == null) boolValue = false;
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

        private int pivotSelectedIndex=0;
        public int PivotSelectedIndex
        {
            get { return pivotSelectedIndex; }
            set { Set(ref pivotSelectedIndex, value); }
        }


        #endregion

        #region Commands
        public RelayCommand<object> CheckUncheckAutoLoadnewsCommand { get; private set; }
        public RelayCommand<object> ChangedThemeAppCommand { get; private set; }
        public RelayCommand ChangedNumberNewsCacheCommand { get; private set; }
        public RelayCommand<object> PivotSelectedIndexCommand { get; private set; }
        public RelayCommand ClearCacheCommand { get; private set; }
        #endregion

        #region Collections
        public ObservableCollection<string> numberNewsCache = new ObservableCollection<string>() { "50", "100", "150" };

        public ObservableCollection<string> NumberNewsCache
        {
            get { return numberNewsCache; }
        }
        #endregion
    }
}
