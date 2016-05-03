using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Command;
using Onliner.ParsingHtml;
using Onliner.Http;
using Onliner.Model.News;
using Onliner.Model.DataTemplateSelector;
using static Onliner.Setting.SettingParams;

namespace Onliner_for_windows_10.View_Model
{
    public class ViewNewsViewModel : ViewModelBase
    {
        private ParsingFullNewsPage FullPageParser;
        private FullItemNews fullItem = new FullItemNews();
        private HttpRequest HttpRequest = new HttpRequest();
        private string loaderPage = string.Empty;
        private string NewsID = string.Empty;
        private string NewsUrl = string.Empty;

        #region Constructor
        public ViewNewsViewModel()
        {
            CommentsAdd = new RelayCommand<object>((obj) => AddComments(obj));
            SendButtonActive = new RelayCommand(() => ActiveSendButton());
            ChangeCommetnsGridVisible = new RelayCommand(() => VisibleCommentsGrid());
            OpenNewsInBrowser = new RelayCommand<object>(async (obj) => await OpenLink(obj));
            SaveNewsInDB = new RelayCommand<object>((obj) => SaveNewsDB(obj));
            UpdateCommentsList = new RelayCommand(async () => await UpdateCommets());
            AnswerCommentCommand = new RelayCommand<object>((obj) => AnswerComment(obj));
            AnswerQuoteCommentCommand = new RelayCommand<object>((obj) => AnswerQuoteComment(obj));
            LikeCommentsCommand = new RelayCommand<object>((obj) => LikeComments(obj));
            UserProfileCommand = new RelayCommand<object>((obj) => UserProfileNavigate(obj));

            var boolAuthorization = Convert.ToBoolean(GetParamsSetting(AuthorizationKey));
            if (boolAuthorization)
            {
                CommentsButtonVisible = true;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load all news data
        /// </summary>
        /// <param name="urlPage"></param>
        /// <returns></returns>
        private async Task LoadNewsData(string urlPage)
        {
            NewsUrl = urlPage;

            var htmlPage = await HttpRequest.GetRequestOnlinerAsync(NewsUrl);

            FullPageParser = new ParsingFullNewsPage(htmlPage, NewsUrl);
            //news data
            NewsItemContent = await FullPageParser.NewsMainInfo();
            //comments data
            Comments = await FullPageParser.CommentsMainInfo();

            CommentsProgressRing = false;
            await HttpRequest.ViewNewsSet(FullPageParser.NewsID);
        }

     
        private async Task UpdateCommets()
        {
            CommentsProgressRing = true;

            var htmlPage = await HttpRequest.GetRequestOnlinerAsync(NewsUrl);
            FullPageParser = new ParsingFullNewsPage(htmlPage, NewsUrl);

            Comments = await FullPageParser.CommentsMainInfo();

            CommentsProgressRing = false;
            await Task.CompletedTask;
        }

        private void ActiveSendButton() =>
            SendButton = !string.IsNullOrEmpty(Message) ? true : false;

        /// <summary>
        /// Open link in webBrowser
        /// </summary>
        /// <param name="obj">string url</param>
        /// <returns></returns>
        private async Task OpenLink(object obj)
        {
            string app = LinkNews;
            var uri = new Uri(app);
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        /// <summary>
        /// Save item News in DB
        /// </summary>
        /// <param name="obj">News item</param>
        private async void SaveNewsDB(object obj)
        {
            MessageDialog msg = new MessageDialog("Coming soon");
            await msg.ShowAsync();
        }

        /// <summary>
        /// Add comments
        /// </summary>
        /// <param name="obj">comments data</param>
        private async void AddComments(object obj)
        {
            var boolAuthorization = Convert.ToBoolean(GetParamsSetting(AuthorizationKey));

            if (boolAuthorization)
            {
                var result = await HttpRequest.AddComments(FullPageParser.NewsID, obj.ToString(), LinkNews);
                if (result)
                {
                    Comments comItem = new Onliner.Model.News.Comments();
                    comItem.Nickname = ShellViewModel.Instance.Login;
                    comItem.Image = ShellViewModel.Instance.AvatarUrl;
                    comItem.Time = "Только что";
                    comItem.Data = obj.ToString();
                    Comments.Add(comItem);
                }
                Message = string.Empty;
            }
            else
            {
                LogInMessageBox("Чтобы оставлять комментарии необходимо авторизоваться..");
            }
        }

        /// <summary>
        /// Generate default message dialog
        /// </summary>
        /// <param name="message"></param>
        private async void LogInMessageBox(string message)
        {
            var dialog = new MessageDialog(message);

            dialog.Commands.Add(new UICommand { Label = "авторизоваться", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "нет", Id = 1 });

            var result = await dialog.ShowAsync();

            if ((int)result.Id == 0)
            {
                NavigationService.Navigate(typeof(MainPage));
            }
        }

        /// <summary>
        /// Like
        /// </summary>
        /// <param name="obj"></param>
        private void LikeComments(object obj)
        {
            var boolAuthorization = Convert.ToBoolean(GetParamsSetting(AuthorizationKey));

            if (boolAuthorization)
            {
                var item = obj as IComments;
                if (item != null)
                    LikeSetter(item);
            }
            else
            {
                LogInMessageBox("Чтобы работать с комментарииями необходимо авторизоваться..");
            }

        }

        private async void LikeSetter(IComments commentItem)
        {
            if (!commentItem.Like)
            {
                var likeCount = await HttpRequest.LikeComment(commentItem.ID, LinkNews, LikeType.Like);
                commentItem.LikeCount = likeCount;
                commentItem.Like = true;
            }
            else
            {
                var likeCount = await HttpRequest.LikeComment(commentItem.ID, LinkNews, LikeType.UnLike);
                commentItem.LikeCount = likeCount;
                commentItem.Like = false;
            }
        }

        private async void AnswerQuoteComment(object obj)
        {
            var item = obj as Comments;
            var quoteMessage = await HttpRequest.QuoteComment(item.ID, LinkNews);
            Message = QuoteMessageGenerate(quoteMessage, item.Nickname);
        }

        private void AnswerComment(object obj)
        {
            var anwerUser = obj.ToString();
            Message = AnswerMessageGenerate(anwerUser);
        }

        private void VisibleCommentsGrid() =>
                         CommentsVisible = !CommentsVisible;

        private string AnswerMessageGenerate(string userName)
        {
            return $"[b]{userName}[/b], ";
        }

        private string QuoteMessageGenerate(string message, string userName)
        {
            return $"[quote=\"{userName}\"]{message}[/quote]";
        }

        private void UserProfileNavigate(object obj)
        {
           // var item = obj as Comments;
            NavigationService.Navigate(typeof(ProfilePage.ProfilePage), obj.ToString());
        }


        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            LinkNews = parameter?.ToString();

            if (HttpRequest.HasInternet())
            {
                await LoadNewsData(LinkNews);
                ProgressRing = false;
            }
            else
            {
                await HttpRequest.Message("Упс, вы не подключены к интернету :(");
            }
            await Task.CompletedTask;
        }
        #endregion

        #region Collections
        private ObservableCollection<ListViewItemSelectorModel> newsItem;
        private ObservableCollection<Comments> commentsItem;

        public ObservableCollection<ListViewItemSelectorModel> NewsItemContent
        {
            get { return newsItem; }
            set { Set(ref newsItem, value); }
        }
        public ObservableCollection<Comments> Comments
        {
            get { return commentsItem; }
            set { Set(ref commentsItem, value); }
        }
        #endregion

        #region Properties
        private string _Value = "Default";
        public string LinkNews { get { return _Value; } set { Set(ref _Value, value); } }

        private bool progressRing = true;
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

        private bool commentsProgressRing = true;
        public bool CommentsProgressRing
        {
            get
            {
                return this.commentsProgressRing;
            }
            set
            {
                Set(ref commentsProgressRing, value);
            }
        }

        private bool sendButton = false;
        public bool SendButton
        {
            get
            {
                return this.sendButton;
            }
            set
            {
                Set(ref sendButton, value);
            }
        }

        private bool commentsVisible = false;
        public bool CommentsVisible
        {
            get
            {
                return this.commentsVisible;
            }
            set
            {
                Set(ref commentsVisible, value);
            }
        }

        private bool commentsButtonVisible = false;
        public bool CommentsButtonVisible
        {
            get
            {
                return this.commentsButtonVisible;
            }
            set
            {
                Set(ref commentsButtonVisible, value);
            }
        }


        private string message;

        public string Message
        {
            get { return message; }
            set { Set(ref message, value); }
        }



        #endregion

        #region Commands
        public RelayCommand<object> CommentsAdd { get; private set; }
        public RelayCommand<object> OpenNewsInBrowser { get; private set; }
        public RelayCommand<object> SaveNewsInDB { get; private set; }
        public RelayCommand SendButtonActive { get; private set; }
        public RelayCommand ChangeCommetnsGridVisible { get; private set; }
        public RelayCommand UpdateCommentsList { get; private set; }
        public RelayCommand<object> AnswerCommentCommand { get; private set; }
        public RelayCommand<object> AnswerQuoteCommentCommand { get; private set; }
        public RelayCommand<object> LikeCommentsCommand { get; private set; }
        public RelayCommand<object> UserProfileCommand { get; private set; }
        #endregion
    }
}
