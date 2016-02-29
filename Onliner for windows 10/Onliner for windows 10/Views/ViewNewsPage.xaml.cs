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

namespace Onliner_for_windows_10.Views
{
    public sealed partial class ViewNewsPage : Page
    {
        private ParsingFullNewsPage fullPagePars;
        private FullItemNews fullItem = new FullItemNews();
        private Request request = new Request();
        private string loaderPage = string.Empty;
        private string NewsID = string.Empty;

        private List<ListViewItemSelectorModel> NewsListItem = new List<ListViewItemSelectorModel>();

        public ViewNewsPage()
        {
            this.InitializeComponent();
            Loaded += ViewNewsPage_Loaded;
            //back button mobile event
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

        private async void ViewNewsPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                fullPagePars = new ParsingFullNewsPage(loaderPage);
                var fullItem = await fullPagePars.NewsMainInfo();
                NewsListInfo.ItemsSource = fullItem;
                CommentsListView.ItemsSource = await fullPagePars.CommentsMainInfo();
            }
            catch (FormatException ex)
            {
                MessageDialog message = new MessageDialog(ex.ToString());
                await message.ShowAsync();
            }

            //comments data

            NewsProgressRing.IsActive = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get page
            loaderPage = e.Parameter.ToString();
        }


        // visibility comments grid
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (GridComments.Visibility != Visibility.Visible)
            {
                GridComments.Visibility = Visibility.Visible;
            }
            else
            {
                GridComments.Visibility = Visibility.Collapsed;
            }
        }

        // add comments button
        private void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            request.ResponceResult += Request_ResponceResult;
            request.AddComments(NewsID, ContentCommentTextBox.Text);
        }

        private async void Request_ResponceResult(string ok)
        {
            if(ok=="ok")
            {
                MessageDialog messageExeption = new MessageDialog("Add");
                await messageExeption.ShowAsync();
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
