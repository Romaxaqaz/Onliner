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
        private ParsingFullNewsPage _fullPageParser;
        private FullItemNews _fullItem = new FullItemNews();
        private readonly HttpRequest _httpRequest = new HttpRequest();
        private string _loaderPage = string.Empty;
        private string _newsId = string.Empty;
        private string _newsUrl = string.Empty;

        #region Constructor
        public ViewNewsViewModel()
        {
            CommentsAdd = new RelayCommand<object>(AddComments);
            SendButtonActive = new RelayCommand(ActiveSendButton);
            ChangeCommetnsGridVisible = new RelayCommand(VisibleCommentsGrid);
            OpenNewsInBrowser = new RelayCommand<object>(async (obj) => await OpenLink(obj));
            SaveNewsInDb = new RelayCommand<object>(SaveNewsDB);
            UpdateCommentsList = new RelayCommand(async () => await UpdateCommets());
            AnswerCommentCommand = new RelayCommand<object>(AnswerComment);
            AnswerQuoteCommentCommand = new RelayCommand<object>(AnswerQuoteComment);
            LikeCommentsCommand = new RelayCommand<object>(LikeComments);
            UserProfileCommand = new RelayCommand<object>(UserProfileNavigate);

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
            _newsUrl = urlPage;

            var htmlPage = await _httpRequest.GetRequestOnlinerAsync(_newsUrl);

            _fullPageParser = new ParsingFullNewsPage(htmlPage, _newsUrl);
            //news data
            NewsItemContent = await _fullPageParser.NewsMainInfo();
            //comments data
            Comments = await _fullPageParser.CommentsMainInfo();

            CommentsProgressRing = false;
            await _httpRequest.ViewNewsSet(_fullPageParser.NewsId);
        }

     
        private async Task UpdateCommets()
        {
            CommentsProgressRing = true;

            var htmlPage = await _httpRequest.GetRequestOnlinerAsync(_newsUrl);
            _fullPageParser = new ParsingFullNewsPage(htmlPage, _newsUrl);

            Comments = await _fullPageParser.CommentsMainInfo();

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
            var msg = new MessageDialog("Coming soon");
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
                var result = await _httpRequest.AddComments(_fullPageParser.NewsId, obj.ToString(), LinkNews);
                if (result)
                {
                    var comItem = new Onliner.Model.News.Comments
                    {
                        Nickname = ShellViewModel.Instance.Login,
                        Image = ShellViewModel.Instance.AvatarUrl,
                        Time = "Только что",
                        Data = obj.ToString()
                    };
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
                var likeCount = await _httpRequest.LikeComment(commentItem.ID, LinkNews, LikeType.Like);
                commentItem.LikeCount = likeCount;
                commentItem.Like = true;
            }
            else
            {
                var likeCount = await _httpRequest.LikeComment(commentItem.ID, LinkNews, LikeType.UnLike);
                commentItem.LikeCount = likeCount;
                commentItem.Like = false;
            }
        }

        private async void AnswerQuoteComment(object obj)
        {
            var item = obj as Comments;
            if (item == null) return;
            var quoteMessage = await _httpRequest.QuoteComment(item.ID, LinkNews);
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

            if (_httpRequest.HasInternet())
            {
                await LoadNewsData(LinkNews);
                ProgressRing = false;
            }
            else
            {
                await _httpRequest.Message("Упс, вы не подключены к интернету :(");
            }
            await Task.CompletedTask;
        }
        #endregion

        #region Collections
        private ObservableCollection<ListViewItemSelectorModel> _newsItem;
        private ObservableCollection<Comments> _commentsItem = new ObservableCollection<Onliner.Model.News.Comments>();

        public ObservableCollection<ListViewItemSelectorModel> NewsItemContent
        {
            get { return _newsItem; }
            set { Set(ref _newsItem, value); }
        }
        public ObservableCollection<Comments> Comments
        {
            get { return _commentsItem; }
            set { Set(ref _commentsItem, value); }
        }
        #endregion

        #region Properties
        private string _value = "Default";
        public string LinkNews { get { return _value; } set { Set(ref _value, value); } }

        private bool _progressRing = true;
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

        private bool _commentsProgressRing = true;
        public bool CommentsProgressRing
        {
            get
            {
                return this._commentsProgressRing;
            }
            set
            {
                Set(ref _commentsProgressRing, value);
            }
        }

        private bool _sendButton = false;
        public bool SendButton
        {
            get
            {
                return this._sendButton;
            }
            set
            {
                Set(ref _sendButton, value);
            }
        }

        private bool _commentsVisible = false;
        public bool CommentsVisible
        {
            get
            {
                return this._commentsVisible;
            }
            set
            {
                Set(ref _commentsVisible, value);
            }
        }

        private bool _commentsButtonVisible = false;
        public bool CommentsButtonVisible
        {
            get
            {
                return this._commentsButtonVisible;
            }
            set
            {
                Set(ref _commentsButtonVisible, value);
            }
        }


        private string _message;
        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }



        #endregion

        #region Commands
        public RelayCommand<object> CommentsAdd { get; private set; }
        public RelayCommand<object> OpenNewsInBrowser { get; private set; }
        public RelayCommand<object> SaveNewsInDb { get; private set; }
        public RelayCommand SendButtonActive { get; private set; }
        public RelayCommand ChangeCommetnsGridVisible { get; private set; }
        public RelayCommand UpdateCommentsList { get; private set; }
        public RelayCommand<object> AnswerCommentCommand { get; private set; }
        public RelayCommand<object> AnswerQuoteCommentCommand { get; private set; }
        public RelayCommand<object> LikeCommentsCommand { get; private set; }
        public RelayCommand<object> UserProfileCommand { get; private set; }

        public object Value
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
