using MyToolkit.Command;
using Onliner_for_windows_10.Common;
using Onliner_for_windows_10.Model;
using Onliner_for_windows_10.ParsingHtml;
using Onliner_for_windows_10.SQLiteDataBase;
using Onliner_for_windows_10.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static Onliner_for_windows_10.SQLiteDataBase.SQLiteDB;

namespace Onliner_for_windows_10.View_Model
{
    public class NewsSectionPageViewModel : ViewModelBase
    {
        private ParsingNewsSection parsNewsSection = new ParsingNewsSection();

        #region url news
        private readonly string TechUrlNews = "http://tech.onliner.by/";
        private readonly string PeoplehUrlNews = "http://people.onliner.by/";
        private readonly string AutohUrlNews = "http://auto.onliner.by/";
        private readonly string RealtUrlNews = "http://realt.onliner.by/";
        private readonly string OpinionUrlNews = "http://people.onliner.by/category/opinions";
        #endregion

        #region private list 
        private ObservableCollection<ItemsNews> techNews;
        private ObservableCollection<ItemsNews> peopleNews;
        private ObservableCollection<ItemsNews> houseNews;
        private ObservableCollection<ItemsNews> autoNews;
        private ObservableCollection<OpinionModel> opinionsNews;
        #endregion

        #region commands
        public RelayCommand<object> PivotOrFlipViewSelectionChange { get; private set; }
        public RelayCommand<object> OpenFullNewsCommandNav { get; private set; }
        public RelayCommand FavoritePageCommand { get; private set; }
        public RelayCommand TestCommand { get; private set; }
        #endregion

        #region List news
        public ObservableCollection<ItemsNews> TechNewsList
        {
            get { return techNews; }
            set { Set(ref techNews, value); }
        }
        public ObservableCollection<ItemsNews> PeopleNewsList
        {
            get { return peopleNews; }
            set { Set(ref peopleNews, value); }
        }
        public ObservableCollection<ItemsNews> HouseNewsList
        {
            get { return houseNews; }
            set { Set(ref houseNews, value); }
        }
        public ObservableCollection<ItemsNews> AutoNewsList
        {
            get { return autoNews; }
            set { Set(ref autoNews, value); }
        }
        public ObservableCollection<OpinionModel> OpinionsNewsList
        {
            get { return opinionsNews; }
            set { Set(ref opinionsNews, value); }
        }
        #endregion

        #region Properties
        private ItemsNews selectedNews;
        public ItemsNews SelectedNews
        {
            get
            {
                return this.SelectedNews;
            }
            set
            {
                Set(ref selectedNews, value);
            }
        }

        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }
            set
            {
                Set(ref selectedIndex, value);
            }
        }

        private bool progressRing = false;
        public bool ProgressRing
        {
            get
            {
                return this.progressRing;
            }
            set
            {
                Set(ref progressRing, value);
            }
        }
        #endregion

        public NewsSectionPageViewModel()
        {
            PivotOrFlipViewSelectionChange = new RelayCommand<object>((obj) => ChangeNewsSection(obj));
            OpenFullNewsCommandNav = new RelayCommand<object>((obj) => DetailsPage(obj));
            FavoritePageCommand = new RelayCommand(() => FavoritePage());
            TestCommand = new RelayCommand(() => TestMethod());
        }

        private void TestMethod()
        {
            NavigationService.Navigate(typeof(ViewNewsPage), "https://people.onliner.by/2016/03/15/parom");
        }

        private async void ChangeNewsSection(object obj)
        {
            try
            {
                ProgressRing = true;
                SelectedIndex = (int)obj;
                await LoadNews();
            }
            catch(NullReferenceException)
            {
                await LoadNews();
            }
        }

        public async Task LoadNews()
        {
            switch (SelectedIndex)
            {
                case 0:
                    TechNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_TECH);
                    TechNewsList = await GetNewsCollection(TechNewsList, TechUrlNews, SQLiteDB.DB_PATH_TECH, SectionNewsDB.Tech);
                    break;
                case 1:
                    PeopleNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_PEOPLE);
                    PeopleNewsList = await GetNewsCollection(PeopleNewsList, PeoplehUrlNews, SQLiteDB.DB_PATH_PEOPLE, SectionNewsDB.People);
                    break;
                case 2:
                    AutoNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_AUTO);
                    AutoNewsList = await GetNewsCollection(AutoNewsList, AutohUrlNews, SQLiteDB.DB_PATH_AUTO, SectionNewsDB.Auto);
                    break;
                case 3:
                    HouseNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_HOUSE);
                    HouseNewsList =  await GetNewsCollection(HouseNewsList, RealtUrlNews, SQLiteDB.DB_PATH_HOUSE, SectionNewsDB.House);
                    break;
                case 4:
                    OpinionsNewsList = parsNewsSection.OpinionList(OpinionUrlNews);
                    break;
            }
            ProgressRing = false;
            await Task.CompletedTask;
        }

        private async Task<ObservableCollection<ItemsNews>> GetNewsCollection(IEnumerable<ItemsNews> collection, string urlNews, string sqlDBPath, SectionNewsDB sectionDB)
        {      
            var newListNews = await parsNewsSection.NewsItemList(urlNews, sectionDB);
            return await UpdateAndRetuntCollection(newListNews, sectionDB);
        }

        private void DetailsPage(object obj)
        {
            try
            {
                ItemsNews feedItem = obj as ItemsNews;
                NavigationService.Navigate(typeof(ViewNewsPage), feedItem.LinkNews);
            }
            catch (NullReferenceException) { }

        }

        private void FavoritePage() =>
            NavigationService.Navigate(typeof(Views.News.FavoriteNewsView));

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            ProgressRing = true;
            TechNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_TECH);
            if (TechNewsList == null)
            {
                TechNewsList = await UpdateAndRetuntCollection(await parsNewsSection.NewsItemList(TechUrlNews, SectionNewsDB.Tech), SectionNewsDB.Tech);
            }
            ProgressRing = false;
            await Task.CompletedTask;
        }
    }
}
