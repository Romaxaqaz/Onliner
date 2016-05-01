using MyToolkit.Command;
using Onliner.Http;
using Onliner.Model.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;

namespace Onliner_for_windows_10.View_Model.Message
{
    public class MessageSenderViewModel : ViewModelBase
    {
        private HttpRequest HttpRequest = new HttpRequest();

        private readonly string ComposeMessageUrl = "https://profile.onliner.by/messages/compose";
        private readonly string HostMessage = "profile.onliner.by";
        private readonly string Origin = "https://profile.onliner.by";

        private string header;
        public string Header
        {
            get { return header; }
            set { Set(ref header, value); }
        }

        private string userSend;
        public string UserSend
        {
            get { return userSend; }
            set { Set(ref userSend, value); }
        }

        private string content;
        public string Content
        {
            get { return content; }
            set { Set(ref content, value); }
        }


        public MessageSenderViewModel()
        {
            SendMessageCommand = new RelayCommand(() => SendMessage());
        }

        private void SetModelMessage(IMessageModel messageModel)
        {
            Header = messageModel.Header == null ? "" : messageModel.Header;
            UserSend = messageModel.UserSend == null ? "" : messageModel.UserSend;
            Content = messageModel.Content == null ? "" : messageModel.Content;
        }

        private async void SendMessage()
        {
            StringBuilder postData = new StringBuilder();
            postData.Append("username=" + UserSend + "&");
            postData.Append("subject=" + Header + "&");
            postData.Append("message=" + Content);
            await HttpRequest.PostRequestFormData(ComposeMessageUrl, HostMessage, Origin, postData.ToString());
            NavigationService.GoBack();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            IMessageModel model = parameter as IMessageModel;
            if (model != null)
                SetModelMessage(model);
            await Task.CompletedTask;
        }

        public RelayCommand SendMessageCommand { get; private set; }
    }
}
