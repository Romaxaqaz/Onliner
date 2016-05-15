using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Command;
using Onliner.Http;
using Onliner.Model.Message;
using Template10.Mvvm;

namespace OnlinerApp.ViewModel.Message
{
    public class MessageSenderViewModel : ViewModelBase
    {
        private readonly HttpRequest _httpRequest = new HttpRequest();

        #region Variables
        private readonly string ComposeMessageUrl = "https://profile.onliner.by/messages/compose";
        private readonly string HostMessage = "profile.onliner.by";
        private readonly string Origin = "https://profile.onliner.by";
        #endregion

        #region Constructor
        public MessageSenderViewModel()
        {
            SendMessageCommand = new RelayCommand(SendMessage);
        }
        #endregion

        #region Properties
        private string _header;
        public string Header
        {
            get { return _header; }
            set { Set(ref _header, value); }
        }

        private string _userSend;
        public string UserSend
        {
            get { return _userSend; }
            set { Set(ref _userSend, value); }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { Set(ref _content, value); }
        }
        #endregion

        #region Command
        public RelayCommand SendMessageCommand { get; private set; }
        #endregion

        #region Methods
        private void SetModelMessage(IMessageModel messageModel)
        {
            Header = messageModel.Header ?? "";
            UserSend = messageModel.UserSend ?? "";
            Content = messageModel.Content ?? "";
        }

        private async void SendMessage()
        {
            if (string.IsNullOrEmpty(UserSend) || string.IsNullOrEmpty(Header) || string.IsNullOrEmpty(Content))
            {
                var message = new MessageDialog("Введены не все данные");
                await message.ShowAsync();
            }
            else
            {
                var postData = new StringBuilder();
                postData.Append("username=" + UserSend + "&");
                postData.Append("subject=" + Header + "&");
                postData.Append("message=" + Content);
                await _httpRequest.PostRequestFormData(ComposeMessageUrl, HostMessage, Origin, postData.ToString());
                NavigationService.GoBack();
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            IMessageModel model = parameter as IMessageModel;
            if (model != null)
                SetModelMessage(model);
            await Task.CompletedTask;
        }
        #endregion
    }
}
