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
        public RelayCommand<object> HeaderRadioButtonCommand { get; private set; }
        public RelayCommand<object> OpenFullNewsCommandNav { get; private set; }
        public RelayCommand FavoritePageCommand { get; private set; }
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

        private bool progressRing;
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
            //load data DB
            TechNewsList = SQLiteDB.GetAllNews();
            PivotOrFlipViewSelectionChange = new RelayCommand<object>(async (obj) => await ChangeNewsSection(obj));
            HeaderRadioButtonCommand = new RelayCommand<object>(async (obj) => await RadioButtonChoiseSection(obj));
            OpenFullNewsCommandNav = new RelayCommand<object>((obj) => DetailsPage(obj));
            FavoritePageCommand = new RelayCommand(() => FavoritePage());
        }

        private async Task RadioButtonChoiseSection(object obj)
        {
            var rad = obj as RadioButton;
            if (rad != null)
            {
                int index = Convert.ToInt32(rad.Tag);
                SelectedIndex = index;
                await LoadNews(index);
            }
        }

        private async Task ChangeNewsSection(object obj)
        {
            var flip = obj as FlipView;
            await LoadNews(flip.SelectedIndex);
        }

        public async Task LoadNews(int index)
        {
            ProgressRing = true;
            switch (index)
            {
                case 0:
                    var li = await SQLiteDB.CreateDatabase(parsNewsSection.NewsItemList(TechUrlNews));
                    AddNewItemList(TechNewsList, li);
                    break;
                case 1:
                    PeopleNewsList = parsNewsSection.NewsItemList(PeoplehUrlNews);
                    break;
                case 2:
                    AutoNewsList = parsNewsSection.NewsItemList(AutohUrlNews);
                    break;
                case 3:
                    HouseNewsList = parsNewsSection.NewsItemList(RealtUrlNews);
                    break;
                case 4:
                    OpinionsNewsList = parsNewsSection.OpinionList(OpinionUrlNews);
                    break;
            }
            ProgressRing = false;
        }

        private void DetailsPage(object obj)
        {
            ItemsNews feedItem = obj as ItemsNews;
            NavigationService.Navigate(typeof(ViewNewsPage), feedItem.LinkNews);
        }

        private void FavoritePage()
        {
            NavigationService.Navigate(typeof(Views.News.FavoriteNewsView));
        }

        private void AddNewItemList(ObservableCollection<ItemsNews> list, ObservableCollection<ItemsNews> newList)
        {
            foreach (var item in newList)
            {
                list.Insert(0, item);
            }
        }
    }
}
