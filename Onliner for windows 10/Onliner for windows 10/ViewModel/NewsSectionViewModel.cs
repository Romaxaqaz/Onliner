using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Command;
using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Onliner.Model.News;
using Onliner.Model.OpinionsModel;
using Onliner.ParsingHtml;
using Onliner_for_windows_10.Views;
using static Onliner.SQLiteDataBase.SqLiteDb;
using static Onliner.Setting.SettingParams;

namespace Onliner_for_windows_10.View_Model
{
    public class NewsSectionViewModel : ViewModelBase
    {
        private readonly ParsingNewsSection _parsNewsSection = new ParsingNewsSection();
        private readonly CoreDispatcher _dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

        #region url news
        private readonly string TechUrlNews = "http://tech.onliner.by/";
        private readonly string PeoplehUrlNews = "http://people.onliner.by/";
        private readonly string AutohUrlNews = "http://auto.onliner.by/";
        private readonly string RealtUrlNews = "http://realt.onliner.by/";
        private readonly string OpinionUrlNews = "http://people.onliner.by/category/opinions";
        #endregion

        #region Constructor
        public NewsSectionViewModel(RelayCommand testCommand)
        {
            TestCommand = testCommand;

            PivotOrFlipViewSelectionChange = new RelayCommand<object>(ChangeNewsSection);
            OpenFullNewsCommandNav = new RelayCommand<object>(async (obj) => await DetailsPage(obj));
            FavoritePageCommand = new RelayCommand(FavoritePage);
            UpdateNewsSectionCommand = new RelayCommand(UpdateNewsSection);
            AddToDataBaseCommand = new RelayCommand<object>(AddItemToDataBas);
            ChangeDataTemplateNewsCommand = new RelayCommand(ChangeDataTemplateNews);
        }

        public NewsSectionViewModel()
        {
        }

        private void AddItemToDataBas(object obj)
        {
            var x = obj;
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
            catch (InvalidCastException)
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
                        await AddAndUpdateCollectionNews(TechNewsList, DB_PATH_TECH, TechUrlNews);
                        ShellViewModel.Instance.TechSectionNewsFirstLoad = false;
                    }
                    else
                    {
                        TechNewsList = await GetAllNews(DB_PATH_TECH);
                    }
                    break;
                case 1:
                    PeopleNewsList = await GetAllNews(DB_PATH_PEOPLE);
                    if (ShellViewModel.Instance.PeopleSectionNewsFirstLoad)
                    {
                        await AddAndUpdateCollectionNews(PeopleNewsList, DB_PATH_PEOPLE, PeoplehUrlNews);
                        ShellViewModel.Instance.PeopleSectionNewsFirstLoad = false;
                    }
                    break;
                case 2:
                    AutoNewsList = await GetAllNews(DB_PATH_AUTO);
                    if (ShellViewModel.Instance.AutoSectionNewsFirstLoad)
                    {
                        await AddAndUpdateCollectionNews(AutoNewsList, DB_PATH_AUTO, AutohUrlNews);
                        ShellViewModel.Instance.AutoSectionNewsFirstLoad = false;
                    }
                    break;
                case 3:
                    HouseNewsList = await GetAllNews(DB_PATH_HOUSE);
                    if (ShellViewModel.Instance.HomeSectionNewsFirstLoad)
                    {
                        await AddAndUpdateCollectionNews(HouseNewsList, DB_PATH_HOUSE, RealtUrlNews);
                        ShellViewModel.Instance.HomeSectionNewsFirstLoad = false;
                    }
                    break;
            }
            NewsSectionIndex = SelectedIndex;
            ProgressRing = false;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Adds new and updates the old articles
        /// </summary>
        /// <param name="mainCollection">UI Collection</param>
        /// <param name="pathDb">SQlite DB path</param>
        /// <param name="urlNews"></param>
        /// <returns>Update collection</returns>
        private async Task AddAndUpdateCollectionNews(ObservableCollection<ItemsNews> mainCollection, string pathDb, string urlNews)
        {
            if (mainCollection == null)
                mainCollection = await GetAllNews(pathDb);
            //get new item news
            var newItemsNews = await GetNewsCollection(urlNews, pathDb);
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
                foreach (var item in newItemsNews)
                {
                    mainCollection.Add(item);
                }
            }

            ProgressRing = false;
            //update item in collection and database
            mainCollection = await UpdateOldItemInCollection(mainCollection, pathDb);
            //save database collection
            await UpdateAndCollectionInDb(mainCollection, pathDb); 
        }

        /// <summary>
        /// Updates only old news
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pathDb"></param>
        /// <returns></returns>
        private async Task<ObservableCollection<ItemsNews>> UpdateOldItemInCollection(ObservableCollection<ItemsNews> items, string pathDb)
        {
            var oldCollection = _parsNewsSection.OldNewsForUpdate;

            if (oldCollection.Count == 0) return items;
            foreach (var item in items)
            {
                var oldItem = oldCollection.FirstOrDefault(x => x.LinkNews.Contains(item.LinkNews));
                if (oldItem == null) continue;

                item.CountViews = oldItem.CountViews;
                item.Footer = oldItem.Footer;
                item.Popularcount = oldItem.Popularcount;
            }
            await UpdateItemDb(items, pathDb);
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
        /// <param name="urlNews"></param>
        /// <param name="pathDb"></param>
        /// <returns></returns>
        private async Task<ObservableCollection<ItemsNews>> GetNewsCollection(string urlNews, string pathDb)
        {
            return await _parsNewsSection.NewsItemList(urlNews, pathDb);
        }

        /// <summary>
        /// Jumps to detail view news page
        /// </summary>
        /// <param name="obj">ItemNews</param>
        /// <returns></returns>
        private async Task DetailsPage(object obj)
        {
            var feedItem = obj as ItemsNews;
            if (feedItem != null)
            {
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => NavigationService.Navigate(typeof(ViewNewsPage), feedItem.LinkNews));
            }
        }

        private void ChangeDataTemplateNews()
        {
            var defDataTemplate = GetParamsSetting(NewsDataTemplateKey);
            if (defDataTemplate == null || defDataTemplate.Equals(TileDataTemplate))
            {
                SetParamsSetting(NewsDataTemplateKey, ListDataTemplate);
                NewsDataTemplate = (DataTemplate)App.Current.Resources[ListDataTemplate];
            }
            if(defDataTemplate != null && defDataTemplate.Equals(ListDataTemplate))
            {
                SetParamsSetting(NewsDataTemplateKey, TileDataTemplate);
                NewsDataTemplate = (DataTemplate)App.Current.Resources[TileDataTemplate];
            }
        }

        private bool MobileType { get; } = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");

        private void FavoritePage() =>
            NavigationService.Navigate(typeof(Views.News.FavoriteNewsView));

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            ProgressRing = true;
            SelectedIndex = NewsSectionIndex;
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
        public RelayCommand<object> AddToDataBaseCommand { get; private set; }
        public RelayCommand ChangeDataTemplateNewsCommand { get; private set; }
        #endregion

        #region List news
        private ObservableCollection<ItemsNews> _techNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<ItemsNews> _peopleNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<ItemsNews> _houseNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<ItemsNews> _autoNews = new ObservableCollection<ItemsNews>();
        private ObservableCollection<OpinionModel> _opinionsNews;
        public ObservableCollection<ItemsNews> TechNewsList
        {
            get { return _techNews; }
            set { Set(ref _techNews, value); }
        }
        public ObservableCollection<ItemsNews> PeopleNewsList
        {
            get { return _peopleNews; }
            set { Set(ref _peopleNews, value); }
        }
        public ObservableCollection<ItemsNews> HouseNewsList
        {
            get { return _houseNews; }
            set { Set(ref _houseNews, value); }
        }
        public ObservableCollection<ItemsNews> AutoNewsList
        {
            get { return _autoNews; }
            set { Set(ref _autoNews, value); }
        }
        public ObservableCollection<OpinionModel> OpinionsNewsList
        {
            get { return _opinionsNews; }
            set { Set(ref _opinionsNews, value); }
        }
        #endregion

        #region Properties
        private ItemsNews _selectedNews;
        public ItemsNews SelectedNews
        {
            get
            {
                return SelectedNews;
            }
            set
            {
                Set(ref _selectedNews, value);
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return this._selectedIndex;
            }
            set
            {
                Set(ref _selectedIndex, value);
            }
        }

        private bool _progressRing;
        public bool ProgressRing
        {
            get
            {
                return this._progressRing;
            }
            set
            {
                Set(ref _progressRing, value);
            }
        }

        private bool _dataTemplateToggle = true;
        public bool DataTemplateToggle
        {
            get
            {
                return this._dataTemplateToggle;
            }
            set
            {
                Set(ref _dataTemplateToggle, value);
            }
        }

        private DataTemplate _newsDataTemplate;
        public DataTemplate NewsDataTemplate
        {
            get {
                var defDataTemplate = GetParamsSetting(NewsDataTemplateKey);
                if (!MobileType)
                {
                    DataTemplateToggle = false;
                    _newsDataTemplate = (DataTemplate)App.Current.Resources[TileDataTemplate];
                }
                else
                {
                    if (defDataTemplate == null)
                    {
                        _newsDataTemplate = (DataTemplate)App.Current.Resources[TileDataTemplate];
                    }
                    else
                    {
                        _newsDataTemplate = (DataTemplate)App.Current.Resources[defDataTemplate];
                    }
                }
                return _newsDataTemplate;
            }
            set { Set(ref _newsDataTemplate, value); }
        }
        #endregion
    }
}
