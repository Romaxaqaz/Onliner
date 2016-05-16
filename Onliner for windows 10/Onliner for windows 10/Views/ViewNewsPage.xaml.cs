using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OnlinerApp.ViewModel;
using WinRTXamlToolkit.Controls.Extensions;

namespace OnlinerApp.Views
{
    public sealed partial class ViewNewsPage : Page
    {
        private readonly ViewNewsViewModel viewModel = new ViewNewsViewModel();
        private ScrollViewer scrollViewer = new ScrollViewer();

        private double HalfHeigthLstview;
        private double PositionScrolling;


        public ViewNewsPage()
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.CommentsOpen += ViewModel_CommentsOpen;
            CommentsListView.Loaded += CommentsListView_Loaded;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            } 
        }

        private void ViewModel_CommentsOpen()
        {
            LayoutUpdated += ViewNewsPage_LayoutUpdated;
        }

        /// <summary>
        /// For mobile state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewNewsPage_LayoutUpdated(object sender, object e)
        {
            if (scrollViewer == null)
            {
                scrollViewer = CommentsListView.GetFirstDescendantOfType<ScrollViewer>();
                scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            }
        }

        private void CommentsListView_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = CommentsListView.GetFirstDescendantOfType<ScrollViewer>();
            if (scrollViewer == null) return;
            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
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
            if (scrollViewer == null)
            {
                scrollViewer = CommentsListView.GetFirstDescendantOfType<ScrollViewer>();
                scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            }
            scrollViewer.ScrollToVerticalOffset(PositionScrolling < scrollViewer.ExtentHeight / 2 ? scrollViewer.ExtentHeight : 0);
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            PositionScrolling = scrollViewer.VerticalOffset;
            if (PositionScrolling < scrollViewer.ExtentHeight / 2)
            {
                DownListButton.Visibility = Visibility.Visible;
                UpListButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                DownListButton.Visibility = Visibility.Collapsed;
                UpListButton.Visibility = Visibility.Visible;
            }
        }

    }
}
