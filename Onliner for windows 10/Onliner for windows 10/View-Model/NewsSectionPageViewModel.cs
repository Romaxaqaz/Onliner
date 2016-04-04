﻿using MyToolkit.Command;
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

        #region Constructor
        public NewsSectionPageViewModel()
        {
            PivotOrFlipViewSelectionChange = new RelayCommand<object>((obj) => ChangeNewsSection(obj));
            OpenFullNewsCommandNav = new RelayCommand<object>(async (obj) => await DetailsPage(obj));
            FavoritePageCommand = new RelayCommand(() => FavoritePage());
            UpdateNewsSectionCommand = new RelayCommand(() => UpdateNewsSection());
        }
        #endregion

        #region Methods
        /// <summary>
        /// Selection change pivot event
        /// </summary>
        /// <param name="obj"></param>
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
                        await AddAndUpdateCollectionNews(TechNewsList, SQLiteDB.DB_PATH_TECH, TechUrlNews, SectionNewsDB.Tech);
                        ShellViewModel.Instance.TechSectionNewsFirstLoad = false;
                    }
                    else { TechNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_TECH); }
                    break;
                case 1:
                    if (ShellViewModel.Instance.PeopleSectionNewsFirstLoad)
                    {
                        await AddAndUpdateCollectionNews(PeopleNewsList, SQLiteDB.DB_PATH_PEOPLE, PeoplehUrlNews, SectionNewsDB.People);
                        ShellViewModel.Instance.PeopleSectionNewsFirstLoad = false;
                    }
                    else { PeopleNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_PEOPLE); }
                    break;
                case 2:
                    if (ShellViewModel.Instance.AutoSectionNewsFirstLoad)
                    {
                        await AddAndUpdateCollectionNews(AutoNewsList, SQLiteDB.DB_PATH_AUTO, AutohUrlNews, SectionNewsDB.Auto);
                        ShellViewModel.Instance.AutoSectionNewsFirstLoad = false;
                    }
                    else { AutoNewsList = await SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_AUTO); }
                    break;
                case 3:
                    if (ShellViewModel.Instance.HomeSectionNewsFirstLoad)
                    {
                        await AddAndUpdateCollectionNews(HouseNewsList, SQLiteDB.DB_PATH_HOUSE, RealtUrlNews, SectionNewsDB.House);
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

        /// <summary>
        /// Adds new and updates the old articles
        /// </summary>
        /// <param name="mainCollection">UI Collection</param>
        /// <param name="pathDB">SQlite DB path</param>
        /// <param name="urlNews"></param>
        /// <param name="section"></param>
        /// <returns>Update collection</returns>
        private async Task AddAndUpdateCollectionNews(ObservableCollection<ItemsNews> mainCollection, string pathDB, string urlNews, SectionNewsDB section)
        {
            if (mainCollection == null)
                mainCollection = await SQLiteDB.GetAllNews(pathDB);
            //get new item news
            var newItemsNews = await GetNewsCollection(mainCollection, urlNews, pathDB, section);
            if (newItemsNews.Count != 0 && mainCollection.Count != 0)
            {
                //Reverse to add at the beginning of the
                newItemsNews = new ObservableCollection<ItemsNews>(newItemsNews.Reverse());
                //Add at the beginning
                foreach (var item in newItemsNews)
                {
                    mainCollection.Insert(0, item);
                }
            }
            else if (newItemsNews.Count != 0 && mainCollection.Count == 0)
            {
                mainCollection = newItemsNews;
            }
            ProgressRing = false;
            //update item in collection and database
            mainCollection = await UpdateOldItemInCollection(mainCollection);
            //save database collection
            await SQLiteDB.UpdateAndRetuntCollection(mainCollection, section);
        }

        /// <summary>
        /// Updates only old news
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private async Task<ObservableCollection<ItemsNews>> UpdateOldItemInCollection(ObservableCollection<ItemsNews> items)
        {
            var oldCollection = parsNewsSection.OldNewsForUpdate;
            if (oldCollection.Count != 0)
            {
                foreach (var item in items)
                {
                    var it = oldCollection.FirstOrDefault(x => x.LinkNews.Contains(item.LinkNews));
                    item.CountViews = it.CountViews;
                    item.Footer = it.Footer;
                    item.Popularcount = it.Popularcount;
                }
                await SQLiteDB.UpdateItemDB(items, SectionNewsDB.Tech);
            }
            return items;
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

        /// <summary>
        /// Gets new articles
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="urlNews"></param>
        /// <param name="sqlDBPath"></param>
        /// <param name="sectionDB"></param>
        /// <returns></returns>
        private async Task<ObservableCollection<ItemsNews>> GetNewsCollection(IEnumerable<ItemsNews> collection, string urlNews, string sqlDBPath, SectionNewsDB sectionDB)
        {
            return await parsNewsSection.NewsItemList(urlNews, sectionDB);
        }

        /// <summary>
        /// Jumps to detail view news page
        /// </summary>
        /// <param name="obj">ItemNews</param>
        /// <returns></returns>
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

        #region Commands
        public RelayCommand<object> PivotOrFlipViewSelectionChange { get; private set; }
        public RelayCommand<object> OpenFullNewsCommandNav { get; private set; }
        public RelayCommand FavoritePageCommand { get; private set; }
        public RelayCommand TestCommand { get; private set; }
        public RelayCommand UpdateNewsSectionCommand { get; private set; }
        #endregion

        #region List news
        private ObservableCollection<ItemsNews> techNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<ItemsNews> peopleNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<ItemsNews> houseNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<ItemsNews> autoNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<OpinionModel> opinionsNews;
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
    }
}
