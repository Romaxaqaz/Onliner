using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Onliner_for_windows_10.SQLiteDataBase.SQLiteDB;
// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views.Setting
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = Windows.UI.Colors.Black;
                statusBar.ForegroundColor = Windows.UI.Colors.White;
                statusBar.BackgroundOpacity = 1;
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
            (Application.Current.Resources["ForegroundCustomWhiteBrush"] as SolidColorBrush).Color = Colors.Black;
            (Application.Current.Resources["ForegroundCustomTextOnBlackBlackBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 170, 170, 170);
            (Application.Current.Resources["FillIcoCustomColorBrush"] as SolidColorBrush).Color = Colors.White;
            (Application.Current.Resources["BackGroundCustomMainElementColorBrush"] as SolidColorBrush).Color = Color.FromArgb(255, 76, 74, 75);


            
        }

        private void Cash_Click(object sender, RoutedEventArgs e)
        {
            Size.Text = BytesToString(GetSizeByteDB());
        }

        private string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        private double BytesToDouble(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB;
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num);
        }

        private void SettingPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = sender as Pivot;
            switch(pivot.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    int max = 50;
                    int maxangle = 359;
                    int nowAngle = (maxangle * (int)BytesToDouble(GetSizeByteDB())) / max;
                    RingSlice.EndAngle = nowAngle;
                    Size.Text = BytesToString(GetSizeByteDB());
                    break;
            }
        }

        private void ClearDB_Click(object sender, RoutedEventArgs e)
        {
            RemoveDataBase();
            Size.Text = BytesToString(GetSizeByteDB());
        }
    }
}
