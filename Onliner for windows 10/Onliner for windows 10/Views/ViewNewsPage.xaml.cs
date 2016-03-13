using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Onliner_for_windows_10.ParsingHtml;
using Onliner_for_windows_10.Model;
using Windows.UI.Xaml.Controls.Primitives;
using Onliner_for_windows_10.Login;
using System;
using Windows.UI.Popups;
using Windows.Phone.UI.Input;
using Onliner_for_windows_10.Model.DataTemplateSelector;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Onliner_for_windows_10.View_Model;

namespace Onliner_for_windows_10.Views
{
    public sealed partial class ViewNewsPage : Page
    {
        private ViewNewsPageViewModel viewModel = new ViewNewsPageViewModel();

        public ViewNewsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
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
                Frame rootFrame = Window.Current.Content as Frame;
                if (Frame.CanGoBack)
                {
                    e.Handled = true;
                    Frame.GoBack();
                }
            }
        }

        private async void NewsListInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItemSelectorModel feedItem = e.AddedItems[0] as ListViewItemSelectorModel;
            MessageDialog msg = new MessageDialog(feedItem.Type);
            await msg.ShowAsync();
        }
    }
}
