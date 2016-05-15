using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OnlinerApp.ViewModel;

namespace OnlinerApp.Views.Message
{
    public sealed partial class MessagePage : Page
    {
       private MessageViewModel viewModel = new MessageViewModel();

        public MessagePage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (viewModel.ViewContentMessage)
            {
                viewModel.ViewContentMessage = false;
            }
            else
            {
                FrameGoBack();
            }
        }

        private void FrameGoBack()
        {
            var rootFrame = Window.Current.Content as Frame;
            if (!Frame.CanGoBack) return;
            Frame.GoBack();
        }

    }
}
