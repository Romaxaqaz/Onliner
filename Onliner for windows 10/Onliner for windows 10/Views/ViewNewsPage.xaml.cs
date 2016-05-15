using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OnlinerApp.ViewModel;

namespace OnlinerApp.Views
{
    public sealed partial class ViewNewsPage : Page
    {
        private bool _downPosition;

        public ViewNewsPage()
        {
            InitializeComponent();
            DataContext = new ViewNewsViewModel();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (GridComments.Visibility == Visibility.Visible)
            {
                GridComments.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (!Frame.CanGoBack) return;
                e.Handled = true;
                Frame.GoBack();
            }
        }

        private void DownListButton_Click(object sender, RoutedEventArgs e)
        { 
            
            if(_downPosition==false)
            {
                CommentsListView.SelectedIndex = 0;
                CommentsListView.ScrollIntoView(CommentsListView.SelectedItem);
                _downPosition = true;
                DownListButton.Visibility = Visibility.Visible;
                UpListButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                CommentsListView.SelectedIndex = CommentsListView.Items.Count - 1;
                CommentsListView.ScrollIntoView(CommentsListView.SelectedItem);
                _downPosition = false;
                DownListButton.Visibility = Visibility.Collapsed;
                UpListButton.Visibility = Visibility.Visible;
            }
        }
    }
}
