
using Onliner.Model.News;
using Onliner.SQLiteDataBase;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Onliner_for_windows_10.Views.News
{
    public sealed partial class FavoriteNewsView : Page
    {
        private ObservableCollection<ItemsNews> news;
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
