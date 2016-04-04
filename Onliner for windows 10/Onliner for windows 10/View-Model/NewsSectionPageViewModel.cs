using MyToolkit.Command;
using Onliner.Model.News;
using Onliner.Model.OpinionsModel;
using Onliner.ParsingHtml;
using Onliner.SQLiteDataBase;
using Onliner_for_windows_10.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using static Onliner.SQLiteDataBase.SQLiteDB;

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
        private ObservableCollection<ItemsNews> techNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<ItemsNews> peopleNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<ItemsNews> houseNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<ItemsNews> autoNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<OpinionModel> opinionsNews;
        #endregion

        #region Commands
        public RelayCommand<object> PivotOrFlipViewSelectionChange { get; private set; }
        public RelayCommand<object> OpenFullNewsCommandNav { get; private set; }
        public RelayCommand FavoritePageCommand { get; private set; }
        public RelayCommand TestCommand { get; private set; }
        public RelayCommand UpdateNewsSectionCommand { get; private set; }
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
            OpenFullNewsCommandNav = new RelayCommand<object>(async (obj) => await DetailsPage(obj));
            FavoritePageCommand = new RelayCommand(() => FavoritePage());
            UpdateNewsSectionCommand = new RelayCommand(() => UpdateNewsSection());
        }

        #region Methods
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
            catch (NullReferenceException)
            {

            }
        }

        /// <summary>
        /// Loading news. Crap code - improved!!
        /// </summary>
        /// <returns></returns>
        public async Task LoadNews()
        {
            switch (SelectedIndex)
            {
                case 0:
                    if (ShellViewModel.Instance.TechSectionNewsFirstLoad)
                    {
                        await AddUpdateCollectionNews(TechNewsList, SQLiteDB.DB_PATH_TECH, TechUrlNews, SectionNewsDB.Tech);
                        ShellViewModel.Instance.TechSectionNewsFirstLoad = false;
                    }
                    else { TechNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_TECH); }
                    break;
                case 1:
                    if (ShellViewModel.Instance.PeopleSectionNewsFirstLoad)
                    {
                        await AddUpdateCollectionNews(PeopleNewsList, SQLiteDB.DB_PATH_PEOPLE, PeoplehUrlNews, SectionNewsDB.People);
                        ShellViewModel.Instance.PeopleSectionNewsFirstLoad = false;
                    }
                    else { PeopleNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_PEOPLE); }
                    break;
                case 2:
                    if (ShellViewModel.Instance.AutoSectionNewsFirstLoad)
                    {
                        await AddUpdateCollectionNews(AutoNewsList, SQLiteDB.DB_PATH_AUTO, AutohUrlNews, SectionNewsDB.Auto);
                        ShellViewModel.Instance.AutoSectionNewsFirstLoad = false;
                    }
                    else { AutoNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_AUTO); }
                    break;
                case 3:
                    if (ShellViewModel.Instance.HomeSectionNewsFirstLoad)
                    { 
                        await AddUpdateCollectionNews(HouseNewsList, SQLiteDB.DB_PATH_HOUSE, RealtUrlNews, SectionNewsDB.House);
                        ShellViewModel.Instance.HomeSectionNewsFirstLoad = false;
                    }
                    else { HouseNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_HOUSE); }
                    break;
                case 4:
                    OpinionsNewsList = parsNewsSection.OpinionList(OpinionUrlNews);
                    break;
            }
            ProgressRing = false;
            await Task.CompletedTask;
        }

        private async Task AddUpdateCollectionNews(ObservableCollection<ItemsNews> mainCollection, string pathDB, string urlNews, SectionNewsDB section)
        {
            if (mainCollection==null)
                mainCollection = await SQLiteDB.GetAllNews(pathDB);
            //get new item news
            var newT = await GetNewsCollection(mainCollection, urlNews, pathDB, section);
            if (newT.Count != 0 && mainCollection.Count != 0)
            {
                //Reverse to add at the beginning of the
                 newT = new ObservableCollection<ItemsNews>(newT.Reverse());
                //Add at the beginning
                foreach (var item in newT)
                {
                    mainCollection.Insert(0, item);
                }
            }
            else if (newT.Count != 0 && mainCollection.Count == 0)
            {
                mainCollection = newT;
            }
            ProgressRing = false;
            //save database collection
            await SQLiteDB.UpdateAndRetuntCollection(mainCollection, section);
            //update item in collection and database
            await UpdateOldItemInCollection(mainCollection);
        }

        private async Task UpdateOldItemInCollection(ObservableCollection<ItemsNews> items)
        {
            if (items == null) await Task.CompletedTask;
            var oldCol = parsNewsSection.OldNewsForUpdate;
            if (oldCol.Count != 0)
            {
                foreach (var item in items)
                {
                    var it = oldCol.FirstOrDefault(x => x.LinkNews.Contains(item.LinkNews));
                    item.CountViews = it.CountViews;
                    item.Footer = it.Footer;
                    item.Popularcount = it.Popularcount;
                }
                await SQLiteDB.UpdateItemDB(items, SectionNewsDB.Tech);
            } 
        }

        /// <summary>
        /// Update section news
        /// </summary>
        private async void UpdateNewsSection()
        {
            ProgressRing = true;
            switch (SelectedIndex)
            {
                case 0:
                    ShellViewModel.Instance.TechSectionNewsFirstLoad = true;
                    break;
                case 1:
                    ShellViewModel.Instance.PeopleSectionNewsFirstLoad = true;
                    break;
                case 2:
                    ShellViewModel.Instance.AutoSectionNewsFirstLoad = true;
                    break;
                case 3:
                    ShellViewModel.Instance.HomeSectionNewsFirstLoad = true;
                    break;
            }
            await LoadNews();
        }

        private async Task<ObservableCollection<ItemsNews>> GetNewsCollection(IEnumerable<ItemsNews> collection, string urlNews, string sqlDBPath, SectionNewsDB sectionDB)
        {
            return await parsNewsSection.NewsItemList(urlNews, sectionDB);
        }

        private async Task DetailsPage(object obj)
        {
            ItemsNews feedItem = obj as ItemsNews;
            NavigationService.Navigate(typeof(ViewNewsPage), feedItem.LinkNews);
            await Task.CompletedTask;
        }

        private void FavoritePage() =>
            NavigationService.Navigate(typeof(Views.News.FavoriteNewsView));

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            ProgressRing = true;
            await LoadNews();
            await Task.CompletedTask;
        }
        #endregion
    }
}
