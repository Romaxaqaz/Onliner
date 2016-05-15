using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OnlinerApp.ViewModel;

namespace OnlinerApp.Views
{
    public sealed partial class NewsPage : Page
    {
        public NewsPage()
        {
            this.InitializeComponent();
            this.DataContext = new NewsSectionViewModel();
            this.Loaded += MainOageLoaded;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (!Frame.CanGoBack) return;
            e.Handled = true;
            Frame.GoBack();
        }

        private void MainOageLoaded(object sender, RoutedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void myGridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var grid = sender as GridView;
            var panel = (ItemsWrapGrid)grid.ItemsPanelRoot;

            if (e.NewSize.Width <= 500)
            {
                panel.MaximumRowsOrColumns = 1;
                panel.ItemWidth = e.NewSize.Width;

            }
            if (e.NewSize.Width > 500 && e.NewSize.Width <= 640)
            {
                panel.MaximumRowsOrColumns = 2;
                panel.ItemWidth = e.NewSize.Width / 2;

            }
            else
            if (e.NewSize.Width > 640 && e.NewSize.Width <= 1024)
            {
                panel.MaximumRowsOrColumns = 3;
                panel.ItemWidth = (e.NewSize.Width) / 3;
            }
            else
            if (e.NewSize.Width > 1024)
            {
                panel.MaximumRowsOrColumns = 4;
                panel.ItemWidth = (e.NewSize.Width) / 4;
            }
        }


        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //change datatemplate
            //  TechGridView.ItemTemplate = (DataTemplate)this.Resources["ListViewMobileTrigger"];
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            TechGridView.ItemTemplate = (DataTemplate)this.Resources["CompactNewsDataTemplate"];
        }
    }
}
