
using Onliner_for_windows_10.Model;
using Onliner_for_windows_10.SQLiteDataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views.News
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class FavoriteNewsView : Page
    {
        ObservableCollection<ItemsNews> news;
        public FavoriteNewsView()
        {
            this.InitializeComponent();
           // news = SQLiteDB.GetAllNews("");
            TestGridView.ItemsSource = news;
        }

        private void suggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {
                var term = suggestBox.Text.ToLower();
                var results = news.Where(i => i.Title.Contains(term)).ToList();
                suggestBox.ItemsSource = results;
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            SQLiteDB.RemoveDataBase();
        }
    }
}
