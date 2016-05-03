using System;
using Windows.UI.Popups;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Onliner_for_windows_10.View_Model;
using Onliner.Model.DataTemplateSelector;
using Windows.UI.ViewManagement;

namespace Onliner_for_windows_10.Views
{
    public sealed partial class ViewNewsPage : Page
    {
        private ViewNewsViewModel viewModel = new ViewNewsViewModel();
        private bool DownPosition = false;
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

        private void DownListButton_Click(object sender, RoutedEventArgs e)
        { 
            
            if(DownPosition==false)
            {
                CommentsListView.SelectedIndex = 0;
                CommentsListView.ScrollIntoView(CommentsListView.SelectedItem);
                DownPosition = true;
                DownListButton.Visibility = Visibility.Visible;
                UpListButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                CommentsListView.SelectedIndex = CommentsListView.Items.Count - 1;
                CommentsListView.ScrollIntoView(CommentsListView.SelectedItem);
                DownPosition = false;
                DownListButton.Visibility = Visibility.Collapsed;
                UpListButton.Visibility = Visibility.Visible;
            }
        }
    }
}
