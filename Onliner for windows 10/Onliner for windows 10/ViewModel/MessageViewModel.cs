using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Command;
using Onliner.Http;
using Onliner.Model.JsonModel.Message;
using Onliner.Model.Message;
using OnlinerApp.Views.Message;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Universal.UI.Xaml.Controls;

namespace OnlinerApp.ViewModel
{
    public class MessageViewModel : ViewModelBase
    {
        #region Url Api
        private readonly string MessageMaskReadUrl = "https://profile.onliner.by/messages/markAsRead/";
        private readonly string ComposeMessageUrl = "https://profile.onliner.by/messages/compose";
        private readonly string SaveMessageUrl = "https://profile.onliner.by/messages/saveMessages";
        private readonly string DeleteMessage = "https://profile.onliner.by/messages/deleteMessages";
        private readonly string MarkSpamUrl = "https://profile.onliner.by/messages/markAsSpam";
        private readonly string HostMessage = "profile.onliner.by";
        private readonly string Origin = "https://profile.onliner.by";
        #endregion

        #region Variables
        private readonly string _keyPosts = "msgid=";
        private readonly string _keyMessagesToTheList = "msgid%5B%5D=";

        private bool _returnBackPageNavigation = true;
        #endregion

        private readonly HttpRequest _httpRequest = new HttpRequest();
        private MessageList _messageListforRawText;
        private readonly List<string> _messageIdList = new List<string>();

        private DelegateCommand<ItemSwipeEventArgs> _itemSwipeCommand;

        #region Constructor
        public MessageViewModel()
        {
            PivotSelectionChange = new RelayCommand(async () => await UpdateItemSourceList());
            ListMessageSelectionChange = new RelayCommand<object>(ShowMessageContent);
            CheckListItemChecked = new RelayCommand<object>(ChekcListItemAdd);
            CheckListItemUnChecked = new RelayCommand<object>(ChekcListItemRemove);
            SendMessageCommand = new RelayCommand(async () => await SendMessage());
            AnswerSendMessageCommand = new RelayCommand(AnswerSendMessage);
            AnswerSendMessageReplyQuoteCommand = new RelayCommand(AnswerSendMessageReplyQuote);
            NewMessageCreate = new RelayCommand(CreateMessage);
            MarkReadMessageCommand = new RelayCommand(async () => await MarkReadMessage());
            SaveMarkMessageCommand = new RelayCommand(async () => await SaveMarkMessage());
            SpamMarkMessageCommand = new RelayCommand(async () => await SpamMarkMessage());
            SpamMessageCommand = new RelayCommand(async () => await SpamMessage());
            SaveMessageCommand = new RelayCommand(async () => await SaveOneMessage());
            DeleteMessageCommand = new RelayCommand(async () => await DeleteOneMessage());
            DeleteMarkMessageCommand = new RelayCommand(async () => await DeleteMarkMessage());
            CheckListItemCommand = new RelayCommand(CheckListLissageItem);
            CheckAllListItemCommand = new RelayCommand<object>(CheckAllListLissageItem);
            UserProfileCommand = new RelayCommand<object>(UserProfileNavigate);
        }

        #endregion

        #region Methods

        #region Check MessageList Item
        /// <summary>
        /// Check all list item id
        /// </summary>
        /// <param name="obj"></param>
        private void CheckAllListLissageItem(object obj)
        {
            var value = Convert.ToBoolean(obj);
            if (value)
            {
                MessageList.ToList().ForEach(x => x.IsChecked = true);
                foreach (var item in MessageList)
                {
                    _messageIdList.Add(item.id);
                }
            }
            else
            {
                MessageList.ToList().ForEach(x => x.IsChecked = false);
                _messageIdList.Clear();
            }
        }

        /// <summary>
        /// Visibility checkbox listview
        /// </summary>
        private void CheckListLissageItem() =>
            ListCheckBoxVisible = !ListCheckBoxVisible;
        #endregion

        #region Delete Message
        /// <summary>
        /// Delete selected messages
        /// </summary>
        /// <returns></returns>
        private async Task DeleteMarkMessage()
        {
            await _httpRequest.PostRequestFormData(DeleteMessage, HostMessage, Origin, await GeneratePostDataMessageIdList());
            HtmlMessageContent = string.Empty;
            RemoveMarkMessageInMessageList();
        }

        /// <summary>
        /// Delete read message
        /// </summary>
        /// <returns></returns>
        private async Task DeleteOneMessage()
        {
            await _httpRequest.PostRequestFormData(DeleteMessage, HostMessage, Origin, GeneratePostDataMessageId(_messageListforRawText.id));
            HtmlMessageContent = string.Empty;
            RemoveMessageListItem();
        }
        #endregion

        #region Spam Message
        /// <summary>
        /// Spam message
        /// </summary>
        /// <returns></returns>
        private async Task SpamMessage()
        {
            await _httpRequest.PostRequestFormData(MarkSpamUrl, HostMessage, Origin, GeneratePostDataMessageId(_messageListforRawText.id));
            RemoveMessageListItem();
        }

        /// <summary>
        /// Mark as spam
        /// </summary>
        /// <returns></returns>
        private async Task SpamMarkMessage()
        {
            await _httpRequest.PostRequestFormData(MarkSpamUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
            RemoveMarkMessageInMessageList();
        }
        #endregion

        #region Save Message
        /// <summary>
        /// Save read message
        /// </summary>
        /// <returns></returns>
        private async Task SaveOneMessage()
        {
            await _httpRequest.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, GeneratePostDataMessageId(_messageListforRawText.id));
            RemoveMessageListItem();
        }

        /// <summary>
        /// Save read message
        /// </summary>
        /// <returns></returns>
        private async Task SaveMarkMessage()
        {
            await _httpRequest.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
        }
        #endregion

        #region Mark Message
        /// <summary>
        /// Mark as read
        /// </summary>
        /// <returns></returns>
        private async Task MarkReadMessage()
        {
            await _httpRequest.PostRequestFormData(MessageMaskReadUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
            var item = MessageList.FirstOrDefault(x => x.id.Equals(_messageListforRawText.id));
            if (item != null)
                item.unread = "0";
        }
        #endregion

        #region Answer Message
        /// <summary>
        /// Answer on read message
        /// </summary>
        private void AnswerSendMessage()
        {
            NavigationService.Navigate(typeof(MessageSenderPage), CreateMessagePass(false));
        }

        /// <summary>
        /// Answer message quote
        /// </summary>
        private void AnswerSendMessageReplyQuote()
        {
            NavigationService.Navigate(typeof(MessageSenderPage), CreateMessagePass(true));
        }

        /// <summary>
        /// Creates a message to reply
        /// </summary>
        /// <param name="quote">Create qoute message contetn</param>
        /// <returns></returns>
        private IMessageModel CreateMessagePass(bool quote)
        {
            _returnBackPageNavigation = false;

            string quoteMessage = null;
            if (quote)
            {
                quoteMessage = $"[quote=\"{_messageListforRawText.authorName}\"]{_messageListforRawText.rawText}[/quote] ";
            }
            return new MessageModel(_messageListforRawText.authorName, _messageListforRawText.subject, quoteMessage);
        }
        #endregion

        private void UserProfileNavigate(object obj)
        {
            NavigationService.Navigate(typeof(Views.ProfilePage), obj.ToString());
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <returns></returns>
        private async Task SendMessage()
        {
            PopUpSender = false;
            await _httpRequest.PostRequestFormData(ComposeMessageUrl, HostMessage, Origin, DataForAnswer().ToString());
        }

        /// <summary>
        /// Show for more detailed message
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void ShowMessageContent(object obj)
        {
            var item = obj as MessageList;
            if (item != null)
            {
                if (!ListCheckBoxVisible)
                {
                    if (item.unread == "1")
                    {
                        await _httpRequest.PostRequestFormData(MessageMaskReadUrl, HostMessage, Origin, GeneratePostDataMessageId(item.id));
                        await _httpRequest.MessageUnread();
                    }
                    ShowListViewItemContent(item);
                }
                else
                {
                    if (item.IsChecked)
                    {
                        item.IsChecked = false;
                        _messageIdList.Remove(item.id);
                    }
                    else
                    {
                        item.IsChecked = true;
                        _messageIdList.Add(item.id);
                    }
                }
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get message type lsit
        /// </summary>
        /// <returns></returns>
        private async Task UpdateItemSourceList()
        {
            ProgressRingMessage = true;
            switch (SelectedIndex)
            {
                case 0:
                    MessageListType("0", "1");
                    break;
                case 1:
                    MessageListType("-1", "1");
                    break;
                case 2:
                    MessageListType("1", "1");
                    break;
            }
            ProgressRingMessage = false;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Remove marked items in the ListView
        /// </summary>
        private void RemoveMarkMessageInMessageList()
        {
            foreach (var item in _messageIdList)
            {
                var itemMessageList = MessageList.FirstOrDefault(x => x.Equals(item));
                if (itemMessageList != null)
                    MessageList.Remove(itemMessageList);
            }
        }

        /// <summary>
        /// Message lsit
        /// </summary>
        /// <param name="idOne">input, output</param>
        /// <param name="idTwo"></param>
        private async void MessageListType(string idOne, string idTwo)
        {
            try
            {
                var saveMessage = await _httpRequest.Message(idOne, idTwo);
                if (saveMessage != null)
                {
                    MessageList = new ObservableCollection<MessageList>(saveMessage.messages);
                }
            }
            catch (ArgumentException)
            {
               
            }
        }

        /// <summary>
        /// Generate html for WebView  BETA
        /// </summary>
        /// <param name="item"></param>
        private void ShowListViewItemContent(MessageList item)
        {
            MessageLogoContent = false;
            _messageListforRawText = item;
            AuthorName = item.authorName;
            TimeAuthor = item.time;
            SubjectMessage = item.subject;
            AuthorImage = $"https://content.onliner.by/user/avatar/80x80/{item.authorId}";
            HtmlMessageContent = item.text.Replace("class=\"uncited\"", "style=\"border: 1px solid black; padding: 2\"");
            SaveButtonVisible = SelectedIndex != 2;
            if (!ViewContentMessage)
            {
                ViewContentMessage = true;
            }
        }

        private string GeneratePostDataMessageId(string id)
        {
            return _keyPosts + id;
        }

        /// <summary>
        /// Create list message for request
        /// </summary>
        /// <returns></returns>
        private async Task<string> GeneratePostDataMessageIdList()
        {
            StringBuilder postData = new StringBuilder();
            if (_messageIdList != null)
            {
                for (int i = 0; i < _messageIdList.Count; i++)
                {
                    if (i == _messageIdList.Count - 1)
                    {
                        postData.Append(_keyMessagesToTheList + _messageIdList[i]);
                    }
                    else
                    {
                        postData.Append(_keyMessagesToTheList + _messageIdList[i] + "&");
                    }
                }
            }
            else
            {
                MessageDialog message = new MessageDialog("Элементы не выбраны");
                await message.ShowAsync();
            }

            return postData.ToString();
        }

        /// <summary>
        /// Create message object and navigation to sender page
        /// </summary>
        private void CreateMessage()
        {
            IMessageModel message = new MessageModel(UserNameRecipient, MessageHeader, MessageText);
            NavigationService.Navigate(typeof(MessageSenderPage), message);
        }

        private StringBuilder DataForAnswer()
        {
            StringBuilder postData = new StringBuilder();
            if (_messageListforRawText != null)
            {
                postData.Append("username=" + _messageListforRawText.authorName + "&");
                postData.Append("subject=" + _messageListforRawText.subject + "&");
                postData.Append("message=" + MessageText);
            }
            else
            {
                postData.Append("username=" + UserNameRecipient + "&");
                postData.Append("subject=" + MessageHeader + "&");
                postData.Append("message=" + MessageText);
            }
            return postData;
        }

        #region CheckBox Command Method
        private void ChekcListItemRemove(object obj) =>
            _messageIdList.Remove(obj.ToString());

        private void ChekcListItemAdd(object obj) =>
            _messageIdList.Add(obj.ToString());
        #endregion

        /// <summary>
        /// Remove list item message
        /// </summary>
        private void RemoveMessageListItem()
        {
            var item = MessageList.FirstOrDefault(x => x.id.Equals(_messageListforRawText.id));
            if (item != null)
                MessageList.Remove(item);
        }

        private async void ItemSwipeCommandExecute(ItemSwipeEventArgs e)
        {
            var item = e.SwipedItem as MessageList;
            if (item == null) return;
            if (e.Direction == SwipeListDirection.Left)
            {
                await _httpRequest.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, GeneratePostDataMessageId(item.id));
                MessageList.Remove(item);
            }
            else
            {
                await _httpRequest.PostRequestFormData(DeleteMessage, HostMessage, Origin, GeneratePostDataMessageId(item.id));
                HtmlMessageContent = string.Empty;
                MessageList.Remove(item);
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await UpdateItemSourceList();
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            if (ViewContentMessage)
            {
                ViewContentMessage = false;
                args.Cancel = _returnBackPageNavigation;
            }
            else
            {
                args.Cancel = false;
            }
            await Task.CompletedTask;
        }

        #endregion

        #region Properties
        private int _selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                Set(ref _selectedIndex, value);
            }
        }

        private bool _saveButtonVisible;
        public bool SaveButtonVisible
        {
            get
            {
                return _saveButtonVisible;
            }
            set
            {
                Set(ref _saveButtonVisible, value);
            }
        }

        private bool _popUpSender;
        public bool PopUpSender
        {
            get
            {
                return _popUpSender;
            }
            set
            {
                Set(ref _popUpSender, value);
            }
        }

        private bool _viewContentMessage;
        public bool ViewContentMessage
        {
            get
            {
                return _viewContentMessage;
            }
            set
            {
                Set(ref _viewContentMessage, value);
            }
        }

        private string _htmlMessageContent = string.Empty;
        public string HtmlMessageContent
        {
            get
            {
                return _htmlMessageContent;
            }
            set
            {
                Set(ref _htmlMessageContent, value);
            }
        }

        private bool _messageLogoContent = true;
        public bool MessageLogoContent
        {
            get
            {
                return _messageLogoContent;
            }
            set
            {
                Set(ref _messageLogoContent, value);
            }
        }

        private string _authorName;
        public string AuthorName
        {
            get { return _authorName; }
            set { Set(ref _authorName, value); }
        }

        private string _authorImage;
        public string AuthorImage
        {
            get { return _authorImage; }
            set { Set(ref _authorImage, value); }
        }

        private string _timeAuthor;
        public string TimeAuthor
        {
            get { return _timeAuthor; }
            set { Set(ref _timeAuthor, value); }
        }

        private string _subjectMessage;
        public string SubjectMessage
        {
            get { return _subjectMessage; }
            set { Set(ref _subjectMessage, value); }
        }

        private bool _listCheckBoxVisible;
        public bool ListCheckBoxVisible
        {
            get
            {
                return _listCheckBoxVisible;
            }
            set
            {
                Set(ref _listCheckBoxVisible, value);
            }
        }

        private bool _progressRingMessage = true;
        public bool ProgressRingMessage
        {
            get
            {
                return _progressRingMessage;
            }
            set
            {
                Set(ref _progressRingMessage, value);
            }
        }
        #endregion

        #region MessageSenderData
        private string _userNameRecipient = "";
        public string UserNameRecipient
        {
            get
            {
                return _userNameRecipient;
            }
            set
            {
                Set(ref _userNameRecipient, value);
            }
        }
        private string _messageHeader = "";
        public string MessageHeader
        {
            get
            {
                return _messageHeader;
            }
            set
            {
                Set(ref _messageHeader, value);
            }
        }
        private string _messageText = "";
        public string MessageText
        {
            get
            {
                return _messageText;
            }
            set
            {
                Set(ref _messageText, value);
            }
        }
        #endregion

        #region Collection
        private ObservableCollection<MessageList> _messageMessage = new ObservableCollection<MessageList>();
        public ObservableCollection<MessageList> MessageList
        {
            get { return _messageMessage; }
            set { Set(ref _messageMessage, value); }
        }
        #endregion

        #region Command
        public RelayCommand PivotSelectionChange { get; private set; }
        public RelayCommand<object> ListMessageSelectionChange { get; private set; }
        public RelayCommand<object> CheckListItemChecked { get; private set; }
        public RelayCommand<object> CheckListItemUnChecked { get; private set; }

        public RelayCommand SendMessageCommand { get; private set; }
        public RelayCommand AnswerSendMessageCommand { get; private set; }
        public RelayCommand AnswerSendMessageReplyQuoteCommand { get; private set; }
        public RelayCommand NewMessageCreate { get; private set; }
        public RelayCommand MarkReadMessageCommand { get; private set; }
        public RelayCommand SaveMarkMessageCommand { get; private set; }
        public RelayCommand SpamMarkMessageCommand { get; private set; }
        public RelayCommand SpamMessageCommand { get; private set; }
        public RelayCommand SaveMessageCommand { get; private set; }
        public RelayCommand DeleteMessageCommand { get; private set; }
        public RelayCommand DeleteMarkMessageCommand { get; private set; }
        public RelayCommand<object> UserProfileCommand { get; private set; }

        public RelayCommand CheckListItemCommand { get; private set; }
        public RelayCommand<object> CheckAllListItemCommand { get; private set; }

        public ICommand ItemSwipeCommand
        {
            get { return _itemSwipeCommand = _itemSwipeCommand ?? new DelegateCommand<ItemSwipeEventArgs>(ItemSwipeCommandExecute); }
        }
        #endregion
    }
}
