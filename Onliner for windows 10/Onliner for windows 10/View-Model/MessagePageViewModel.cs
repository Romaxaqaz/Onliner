﻿using MyToolkit.Command;
using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

namespace Onliner_for_windows_10.View_Model
{
    public class MessagePageViewModel : ViewModelBase
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

        private Request request = new Request();
        private MessageList messageListforRawText;
        private List<string> MessageIdList = new List<string>();

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
        private ObservableCollection<MessageList> incomingMessage;
        private ObservableCollection<MessageList> outgoingMessage;
        private ObservableCollection<MessageList> savedMessage;
        public ObservableCollection<MessageList> IncomingMessage
        {
            get { return incomingMessage; }
            set { Set(ref incomingMessage, value); }
        }
        public ObservableCollection<MessageList> OutgoingMessage
        {
            get { return outgoingMessage; }
            set { Set(ref outgoingMessage, value); }
        }
        public ObservableCollection<MessageList> SavedMessage
        {
            get { return savedMessage; }
            set { Set(ref savedMessage, value); }
        }
        #endregion

        #region Command
        public RelayCommand<object> PivotSelectionChange { get; private set; }
        public RelayCommand<object> ListMessageSelectionChange { get; private set; }
        public RelayCommand<object> CheckListItemChecked { get; private set; }
        public RelayCommand<object> CheckListItemUnChecked { get; private set; }

        public RelayCommand SendMessageCommand { get; private set; }
        public RelayCommand AnswerSendMessageCommand { get; private set; }
        public RelayCommand NewMessageCreate { get; private set; }
        public RelayCommand MarkReadMessageCommand { get; private set; }
        public RelayCommand SaveMarkMessageCommand { get; private set; }
        public RelayCommand SpamMarkMessageCommand { get; private set; }
        public RelayCommand SaveMessageCommand { get; private set; }
        public RelayCommand DeleteMessageCommand { get; private set; }
        public RelayCommand DeleteMarkMessageCommand { get; private set; }
        #endregion

        public MessagePageViewModel()
        {
            PivotSelectionChange = new RelayCommand<object>(async (obj) => await UpdateItemSourceList(SelectedIndex));
            ListMessageSelectionChange = new RelayCommand<object>(async (obj) => await ShowMessageContent(obj));
            CheckListItemChecked = new RelayCommand<object>((obj) => ChekcListItemAdd(obj));
            CheckListItemUnChecked = new RelayCommand<object>((obj) => ChekcListItemRemove(obj));
            SendMessageCommand = new RelayCommand(async() => await SendMessage());
            AnswerSendMessageCommand = new RelayCommand(() =>  AnswerSendMessage());
            NewMessageCreate = new RelayCommand(() => CreateMessage());
            MarkReadMessageCommand = new RelayCommand(async () => await MarkReadMessage());
            SaveMarkMessageCommand = new RelayCommand(async () => await SaveMarkMessage());
            SpamMarkMessageCommand = new RelayCommand(async () => await SpamMarkMessage());
            SaveMessageCommand = new RelayCommand(async () => await SaveOneMessage());
            DeleteMessageCommand = new RelayCommand(async () => await DeleteOneMessage());
            DeleteMarkMessageCommand = new RelayCommand(async () => await DeleteMarkMessage());
        }

        private async Task DeleteMarkMessage()
        {
            await request.PostRequestFormData(DeleteMessage, HostMessage, Origin, await GeneratePostDataMessageIdList());
            HtmlMessageContent = string.Empty;
            await UpdateItemSourceList(SelectedIndex);
        }

        private async Task DeleteOneMessage()
        {
            await request.PostRequestFormData(DeleteMessage, HostMessage, Origin, GeneratePostDataMessageId(messageListforRawText.id));
            HtmlMessageContent = string.Empty;
            await UpdateItemSourceList(SelectedIndex);
        }

        private async Task SaveOneMessage()
        {
            await request.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, GeneratePostDataMessageId(messageListforRawText.id));
             await UpdateItemSourceList(SelectedIndex);
        }

        private async Task SpamMarkMessage()
        {
            await request.PostRequestFormData(MarkSpamUrl, HostMessage, Origin, GeneratePostDataMessageId(messageListforRawText.id));
            await UpdateItemSourceList(SelectedIndex);
        }

        private async Task SaveMarkMessage()
        {
            await request.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
            await UpdateItemSourceList(SelectedIndex);
        }

        private async Task MarkReadMessage()
        {
            await request.PostRequestFormData(MessageMaskReadUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
            await UpdateItemSourceList(SelectedIndex);
        }

        private void AnswerSendMessage()
        {
            if (!PopUpSender)
            {
                SetParamsMessage(false);
                PopUpSender = true;
            }
            else
            {
                PopUpSender = false;
            }
        }

        private async Task SendMessage()
        {
            PopUpSender = false;
            await request.PostRequestFormData(ComposeMessageUrl, HostMessage, Origin, DataForAnswer().ToString());
        }

        private void SetParamsMessage(bool rawText)
        {
            UserNameRecipient = messageListforRawText.authorName;
            MessageHeader = messageListforRawText.subject;
            if (rawText)
            {
                MessageText = messageListforRawText.rawText;
            }
        }

        private StringBuilder DataForAnswer()
        {
            StringBuilder postData = new StringBuilder();
            if (messageListforRawText != null)
            {
                postData.Append("username=" + messageListforRawText.authorName + "&");
                postData.Append("subject=" + messageListforRawText.subject + "&");
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

        private async Task ShowMessageContent(object obj)
        {
            var item = (MessageList)obj;
            if (item.unread == "1")
            {
                request.PostRequestFormData(MessageMaskReadUrl, HostMessage, Origin, GeneratePostDataMessageId(item.id));
                await UpdateItemSourceList(SelectedIndex);
                request.MessageUnread();
            }
            ShowListViewItemContent(item);
        }

        private async Task UpdateItemSourceList(int index)
        {
            switch (index)
            {
                case 0:
                    var inMessage = await request.Message("0", "1");
                    if (inMessage != null)
                    {
                        IncomingMessage = new ObservableCollection<MessageList>(inMessage.messages);
                    }
                    break;
                case 1:
                    var outMessage = await request.Message("-1", "1");
                    if (outMessage != null)
                    {
                        OutgoingMessage = new ObservableCollection<MessageList>(outMessage.messages);
                    }
                    break;
                case 2:
                    var saveMessage = await request.Message("1", "1");
                    if (saveMessage != null)
                    {
                        SavedMessage = new ObservableCollection<MessageList>(saveMessage.messages);
                    }
                    break;
            }
        }

        private void ShowListViewItemContent(MessageList Item)
        {
            messageListforRawText = Item;
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
            return "msgid=" + id;
        }

        private async Task<string> GeneratePostDataMessageIdList()
        {
            StringBuilder postData = new StringBuilder();
            if (MessageIdList != null)
            {
                for (int i = 0; i < MessageIdList.Count; i++)
                {
                    if (i == MessageIdList.Count - 1)
                    {
                        postData.Append("msgid%5B%5D=" + MessageIdList[i]);
                    }
                    else
                    {
                        postData.Append("msgid%5B%5D=" + MessageIdList[i] + "&");
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

        private void CreateMessage() =>
           PopUpSender = true;

        private void ChekcListItemRemove(object obj) =>
            MessageIdList.Remove(obj.ToString());

        private void ChekcListItemAdd(object obj) =>
            MessageIdList.Add(obj.ToString());

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            await Task.CompletedTask;
        }
    }
}