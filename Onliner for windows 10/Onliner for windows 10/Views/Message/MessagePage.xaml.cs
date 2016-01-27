using Onliner_for_windows_10.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using System.Text;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Onliner_for_windows_10.ProfilePage;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Model.Message
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MessagePage : Page
    {
        private readonly string MessageMaskReadUrl = "https://profile.onliner.by/messages/markAsRead/";
        private readonly string ComposeMessageUrl = "https://profile.onliner.by/messages/compose";
        private readonly string SaveMessageUrl = "https://profile.onliner.by/messages/saveMessages";
        private readonly string DeleteMessage = "https://profile.onliner.by/messages/deleteMessages";
        private readonly string MarkSpamUrl = "https://profile.onliner.by/messages/markAsSpam";
        private readonly string HostMessage = "profile.onliner.by";
        private readonly string Origin = "https://profile.onliner.by";

        private Request request = new Request();
        private MessageList messageListforRawText;
        private List<string> MessageIdList = new List<string>();

        public MessagePage()
        {
            this.InitializeComponent();
            Loaded += MessagePage_Loaded;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
           e.Handled = true;
           Frame.GoBack();
        }

        private void MessagePage_Loaded(object sender, RoutedEventArgs e)
        {
            Additionalinformation.Instance.NameActivePage = "Личные сообщения";
        }

        private void PivotMessage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateItemSourceListView(((Pivot)sender).SelectedIndex);
        }

        private async void UpdateItemSourceListView(int index)
        {
            switch (index)
            {
                case 0:
                    var inMessage = await request.Message("0", "1");
                    if (inMessage != null)
                    {
                        InMessageContentListview.ItemsSource = inMessage.messages;
                    }
                    break;
                case 1:
                    var outMessage = await request.Message("-1", "1");
                    if (outMessage != null)
                    {
                        OutMessageContentListview.ItemsSource = outMessage.messages;
                    }
                    break;
                case 2:
                    var saveMessage = await request.Message("1", "1");
                    if (saveMessage != null)
                    {
                        SaveMessageContentListview.ItemsSource = saveMessage.messages;
                    }
                    break;
            }
        }

        private void ShowListViewItemContent(MessageList Item)
        {
            LogoImage.Source = new BitmapImage(new Uri("https://profile.onliner.by/content//user/avatar/80x80/" + Item.authorId));
            MessageUserInfo.DataContext = Item;
            messageListforRawText = Item;
            WebViewMessage.NavigateToString(Item.text.Replace("class=\"uncited\"", "style=\"border: 1px solid black; padding: 2\""));
            if (PivotMessage.SelectedIndex==2)
            {
                Save.Visibility = Visibility.Collapsed;
            }
            else
            {
                Save.Visibility = Visibility.Visible;
            }
            if (ViewMessageContent.Visibility == Visibility.Collapsed)
            {
                ViewMessageContent.Visibility = Visibility.Visible;
            }
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            PopupMessageSender.IsOpen = false;
            request.PostRequestFormData(ComposeMessageUrl, HostMessage, Origin, DataForAnswer().ToString());
        }

        private void SetParamsMessage(bool rawText)
        {
            MessageSender.Username = messageListforRawText.authorName;
            MessageSender.MessageHeader = messageListforRawText.subject;
            if (rawText)
            {
                MessageSender.MessageText = messageListforRawText.rawText;
            }
        }

        private StringBuilder DataForAnswer()
        {
            StringBuilder postData = new StringBuilder();
            if (messageListforRawText != null)
            {
                postData.Append("username=" + messageListforRawText.authorName + "&");
                postData.Append("subject=" + messageListforRawText.subject + "&");
                postData.Append("message=" + MessageSender.MessageText);
            }
            else
            {
                postData.Append("username=" + MessageSender.Username + "&");
                postData.Append("subject=" + MessageSender.MessageHeader + "&");
                postData.Append("message=" + MessageSender.MessageText);
            }
            return postData;
        }

        private void ChekcListItem_Checked(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            MessageIdList.Add(check.Tag.ToString());
        }

        private void ChekcListItem_Unchecked(object sender, RoutedEventArgs e)
        {
            var check = sender as CheckBox;
            MessageIdList.Remove(check.Tag.ToString());
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

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            if (!PopupMessageSender.IsOpen)
            {
                SetParamsMessage(false);
                PopupMessageSender.IsOpen = true;
            }
            else
            {
                PopupMessageSender.IsOpen = false;
            }
        }

        private void NewMessage_Click(object sender, RoutedEventArgs e)
        {
            PopupMessageSender.IsOpen = true;
        }

        private void AnswerQuote_Click(object sender, RoutedEventArgs e)
        {
            if (!PopupMessageSender.IsOpen)
            {
                SetParamsMessage(true);
                PopupMessageSender.IsOpen = true;
            }
            else
            {
                PopupMessageSender.IsOpen = false;
            }
        }

        private async void MarkRead_Click(object sender, RoutedEventArgs e)
        {
            request.PostRequestFormData(MessageMaskReadUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
            UpdateItemSourceListView(PivotMessage.SelectedIndex);
        }

        private async void SaveMarkMessage_Click(object sender, RoutedEventArgs e)
        {
            request.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, await GeneratePostDataMessageIdList());
            UpdateItemSourceListView(PivotMessage.SelectedIndex);
        }

        private void Spam_Click(object sender, RoutedEventArgs e)
        {
            request.PostRequestFormData(MarkSpamUrl, HostMessage, Origin, GeneratePostDataMessageId(messageListforRawText.id));
            UpdateItemSourceListView(PivotMessage.SelectedIndex);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            request.PostRequestFormData(SaveMessageUrl, HostMessage, Origin, GeneratePostDataMessageId(messageListforRawText.id));
            UpdateItemSourceListView(PivotMessage.SelectedIndex);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            request.PostRequestFormData(DeleteMessage, HostMessage, Origin, GeneratePostDataMessageId(messageListforRawText.id));
            WebViewMessage.NavigateToString(string.Empty);
            UpdateItemSourceListView(PivotMessage.SelectedIndex);
        }

        private async void DeleteMarkMessage_Click(object sender, RoutedEventArgs e)
        {
            request.PostRequestFormData(DeleteMessage, HostMessage, Origin, await GeneratePostDataMessageIdList());
            WebViewMessage.NavigateToString(string.Empty);
            UpdateItemSourceListView(PivotMessage.SelectedIndex);
        }

        private void MessageTypeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListView listView = sender as ListView;
                var item = (MessageList)listView.SelectedItems[0];
                if(item.unread=="1")
                {
                    request.PostRequestFormData(MessageMaskReadUrl, HostMessage, Origin, GeneratePostDataMessageId(item.id));
                    UpdateItemSourceListView(PivotMessage.SelectedIndex);
                }
                ShowListViewItemContent(item);
            }
        }

        private void UserHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            Frame.Navigate(typeof(ProfilePage.ProfilePage), hyperlinkButton.Content);
        }
    }
}
