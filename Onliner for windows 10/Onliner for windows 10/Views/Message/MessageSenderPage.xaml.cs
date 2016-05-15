using Windows.UI.Xaml.Controls;
using OnlinerApp.ViewModel.Message;

namespace OnlinerApp.Views.Message
{
    public sealed partial class MessageSenderPage : Page
    {
        public MessageSenderPage()
        {
            this.InitializeComponent();
            this.DataContext = new MessageSenderViewModel();
        }
    }
}
