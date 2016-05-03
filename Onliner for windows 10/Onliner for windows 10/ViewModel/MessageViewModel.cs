using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using MyToolkit.Command;
using Universal.UI.Xaml.Controls;
using Onliner.Http;
using Onliner.Model.JsonModel.Message;
using Onliner.Model.Message;
using Onliner_for_windows_10.Views.Message;

namespace Onliner_for_windows_10.View_Model
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
        private readonly string KeyPosts = "msgid=";
        private readonly string KeyMessagesToTheList = "msgid%5B%5D=";

        private bool ReturnBackPageNavigation = true;
        #endregion

        private HttpRequest HttpRequest = new HttpRequest();
        private MessageList MessageListforRawText;
        private List<string> MessageIdList = new List<string>();

        private DelegateCommand<ItemSwipeEventArgs> _itemSwipeCommand;

        #region Constructor
        public MessageViewModel()
        {
            PivotSelectionChange = new RelayCommand(async () => await UpdateItemSourceList());
            ListMessageSelectionChange = new RelayCommand<object>((obj) => ShowMessageContent(obj));
            CheckListItemChecked = new RelayCommand<object>((obj) => ChekcListItemAdd(obj));
            CheckListItemUnChecked = new RelayCommand<object>((obj) => ChekcListItemRemove(obj));
            SendMessageCommand = new RelayCommand(async () => await SendMessage());
            AnswerSendMessageCommand = new RelayCommand(() => AnswerSendMessage());
            AnswerSendMessageReplyQuoteCommand = new RelayCommand(() => AnswerSendMessageReplyQuote());
            NewMessageCreate = new RelayCommand(() => CreateMessage());
            MarkReadMessageCommand = new RelayCommand(async () => await MarkReadMessage());
            SaveMarkMessageCommand = new RelayCommand(async () => await SaveMarkMessage());
            SpamMarkMessageCommand = new RelayCommand(async () => await SpamMarkMessage());
            SpamMessageCommand = new RelayCommand(async () => await SpamMessage());
            SaveMessageCommand = new RelayCommand(async () => await SaveOneMessage());
            DeleteMessageCommand = new RelayCommand(async () => await DeleteOneMessage());
            DeleteMarkMessageCommand = new RelayCommand(async () => await DeleteMarkMessage());
            CheckListItemCommand = new RelayCommand(() => CheckListLissageItem());
            CheckAllListItemCommand = new RelayCommand<object>((obj) => CheckAllListLissageItem(obj));
            UserProfileCommand = new RelayCommand<object>((obj) => UserProfileNavigate(obj));
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
                MessageList.ToList().ForEach(x => x.IsChecked = value);
                foreach (var item in MessageList)
                {
                    MessageIdList.Add(item.id);
                }
            }
            else
            {
                MessageList.ToList().ForEach(x => x.IsChecked = value);
                MessageIdList.Clear();
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
            await HttpRequest.PostRequestFormData(DeleteMessage, HostMessage, Origin, await GeneratePostDataMessageIdList());
            HtmlMessageContent = string.Empty;
            RemoveMarkMessageInMessageList();
        }

        /// <summary>
        /// Delete read message
        /// </summary>
        /// <returns></returns>
        private async Task DeleteOneMessage()
        {
            await HttpRequest.PostRequestFormData(DeleteMessage, HostMessage, Origin, GeneratePostDataMessageId(MessageListforRawText.id));
            HtmlMessageContent = string.Empty;
            RemoveMessageListItem(MessageListforRawText.id);
        }
        #endregion

        #region Spam Message
        /// <summary>
        /// Spam message
        /// </summary>
        /// <returns></returns>
        private async Task SpamMessage()
        {
            await HttpRequest.PostRequestFormData(MarkSpamUrl, HostMessage, Origin, GeneratePostDataMessageId(MessageListforRawText.id));
            RemoveMessageListItem(MessageListforRawText.id);
        }

        /// <summary>
        /// Mark as spam
        /// </summary>
        /// <returns></returns>
        private async Task SpamMarkMessage()
        {
            await HttpRequest.PostRequestFormData(MarkSpamUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
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
            await HttpRequest.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, GeneratePostDataMessageId(MessageListforRawText.id));
            RemoveMessageListItem(MessageListforRawText.id);
        }

        /// <summary>
        /// Save read message
        /// </summary>
        /// <returns></returns>
        private async Task SaveMarkMessage()
        {
            await HttpRequest.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
        }
        #endregion

        #region Mark Message
        /// <summary>
        /// Mark as read
        /// </summary>
        /// <returns></returns>
        private async Task MarkReadMessage()
        {
            await HttpRequest.PostRequestFormData(MessageMaskReadUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
            var item = MessageList.Where(x => x.id.Equals(MessageListforRawText.id)).FirstOrDefault();
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
            ReturnBackPageNavigation = false;

            string quoteMessage = string.Empty;
            if (true)
            {
                quoteMessage = $"[quote=\"{MessageListforRawText.authorName}\"]{MessageListforRawText.rawText}[/quote] ";
            }
            return new MessageModel(MessageListforRawText.authorName, MessageListforRawText.subject, quoteMessage);
        }
        #endregion

        private void UserProfileNavigate(object obj)
        {
            NavigationService.Navigate(typeof(ProfilePage.ProfilePage), obj.ToString());
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <returns></returns>
        private async Task SendMessage()
        {
            PopUpSender = false;
            await HttpRequest.PostRequestFormData(ComposeMessageUrl, HostMessage, Origin, DataForAnswer().ToString());
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
                        await HttpRequest.PostRequestFormData(MessageMaskReadUrl, HostMessage, Origin, GeneratePostDataMessageId(item.id));
                        await HttpRequest.MessageUnread();
                    }
                    ShowListViewItemContent(item);
                }
                else
                {
                    if (item.IsChecked)
                    {
                        item.IsChecked = false;
                        MessageIdList.Remove(item.id);
                    }
                    else
                    {
                        item.IsChecked = true;
                        MessageIdList.Add(item.id);
                    }
                }
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get message type lsit
        /// </summary>
        /// <param name="index"></param>
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
            foreach (var item in MessageIdList)
            {
                var itemMessageList = MessageList.Where(x => x.Equals(item)).FirstOrDefault();
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
                var saveMessage = await HttpRequest.Message(idOne, idTwo);
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
        /// <param name="Item"></param>
        private void ShowListViewItemContent(MessageList Item)
        {
            MessageLogoContent = false;
            MessageListforRawText = Item;
            AuthorName = Item.authorName;
            TimeAuthor = Item.time;
            SubjectMessage = Item.subject;
            AuthorImage = $"https://content.onliner.by/user/avatar/80x80/{Item.authorId}";
            HtmlMessageContent = Item.text.Replace("class=\"uncited\"", "style=\"border: 1px solid black; padding: 2\"");
            if (SelectedIndex == 2)
            {
                SaveButtonVisible = false;
            }
            else
            {
                SaveButtonVisible = true;
            }
            if (!ViewContentMessage)
            {
                ViewContentMessage = true;
            }
        }

        private string GeneratePostDataMessageId(string id)
        {
            return KeyPosts + id;
        }

        /// <summary>
        /// Create list message for request
        /// </summary>
        /// <returns></returns>
        private async Task<string> GeneratePostDataMessageIdList()
        {
            StringBuilder postData = new StringBuilder();
            if (MessageIdList != null)
            {
                for (int i = 0; i < MessageIdList.Count; i++)
                {
                    if (i == MessageIdList.Count - 1)
                    {
                        postData.Append(KeyMessagesToTheList + MessageIdList[i]);
                    }
                    else
                    {
                        postData.Append(KeyMessagesToTheList + MessageIdList[i] + "&");
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
            if (MessageListforRawText != null)
            {
                postData.Append("username=" + MessageListforRawText.authorName + "&");
                postData.Append("subject=" + MessageListforRawText.subject + "&");
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
            MessageIdList.Remove(obj.ToString());

        private void ChekcListItemAdd(object obj) =>
            MessageIdList.Add(obj.ToString());
        #endregion

        /// <summary>
        /// Remove list item message
        /// </summary>
        /// <param name="id">message ID</param>
        private void RemoveMessageListItem(string id)
        {
            var item = MessageList.Where(x => x.id.Equals(MessageListforRawText.id)).FirstOrDefault();
            if (item != null)
                MessageList.Remove(item);
        }

        private async void ItemSwipeCommandExecute(ItemSwipeEventArgs e)
        {
            var item = e.SwipedItem as MessageList;
            if (item != null)
            {
                if (e.Direction == SwipeListDirection.Left)
                {
                    await HttpRequest.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, GeneratePostDataMessageId(item.id));
                    MessageList.Remove(item);
                }
                else
                {
                    await HttpRequest.PostRequestFormData(DeleteMessage, HostMessage, Origin, GeneratePostDataMessageId(item.id));
                    HtmlMessageContent = string.Empty;
                    MessageList.Remove(item);
                }
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
                args.Cancel = ReturnBackPageNavigation;
            }
            else
            {
                args.Cancel = false;
            }
            await Task.CompletedTask;
        }

        #endregion

        #region Properties
        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                Set(ref selectedIndex, value);
            }
        }

        private bool saveButtonVisible;
        public bool SaveButtonVisible
        {
            get
            {
                return saveButtonVisible;
            }
            set
            {
                Set(ref saveButtonVisible, value);
            }
        }

        private bool popUpSender = false;
        public bool PopUpSender
        {
            get
            {
                return popUpSender;
            }
            set
            {
                Set(ref popUpSender, value);
            }
        }

        private bool viewContentMessage;
        public bool ViewContentMessage
        {
            get
            {
                return viewContentMessage;
            }
            set
            {
                Set(ref viewContentMessage, value);
            }
        }

        private string htmlMessageContent = string.Empty;
        public string HtmlMessageContent
        {
            get
            {
                return htmlMessageContent;
            }
            set
            {
                Set(ref htmlMessageContent, value);
            }
        }

        private bool messageLogoContent = true;
        public bool MessageLogoContent
        {
            get
            {
                return messageLogoContent;
            }
            set
            {
                Set(ref messageLogoContent, value);
            }
        }

        private string authorName;
        public string AuthorName
        {
            get { return authorName; }
            set { Set(ref authorName, value); }
        }

        private string authorImage;

        public string AuthorImage
        {
            get { return authorImage; }
            set { Set(ref authorImage, value); }
        }

        private string timeAuthor;
        public string TimeAuthor
        {
            get { return timeAuthor; }
            set { Set(ref timeAuthor, value); }
        }

        private string subjectMessage;
        public string SubjectMessage
        {
            get { return subjectMessage; }
            set { Set(ref subjectMessage, value); }
        }

        private bool listCheckBoxVisible = false;
        public bool ListCheckBoxVisible
        {
            get
            {
                return listCheckBoxVisible;
            }
            set
            {
                Set(ref listCheckBoxVisible, value);
            }
        }

        private bool progressRingMessage = true;
        public bool ProgressRingMessage
        {
            get
            {
                return progressRingMessage;
            }
            set
            {
                Set(ref progressRingMessage, value);
            }
        }
        #endregion

        #region MessageSenderData
        private string userNameRecipient = "";
        public string UserNameRecipient
        {
            get
            {
                return userNameRecipient;
            }
            set
            {
                Set(ref userNameRecipient, value);
            }
        }
        private string messageHeader = "";
        public string MessageHeader
        {
            get
            {
                return messageHeader;
            }
            set
            {
                Set(ref messageHeader, value);
            }
        }
        private string messageText = "";
        public string MessageText
        {
            get
            {
                return messageText;
            }
            set
            {
                Set(ref messageText, value);
            }
        }
        #endregion

        #region Collection
        private ObservableCollection<MessageList> messageMessage = new ObservableCollection<Onliner.Model.JsonModel.Message.MessageList>();
        public ObservableCollection<MessageList> MessageList
        {
            get { return messageMessage; }
            set { Set(ref messageMessage, value); }
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
