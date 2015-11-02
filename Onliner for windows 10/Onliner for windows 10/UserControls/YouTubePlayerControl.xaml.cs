using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Onliner_for_windows_10.UserControls
{
    public sealed partial class YouTubePlayerControl : UserControl
    {

        public YouTubePlayerControl()
        {
            this.InitializeComponent();
           
            Loaded += YouTubePlayerControl_Loaded;

        }
       
        private async void YouTubePlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            var pathUri = await YouTube.GetVideoUriAsync("szVb8-c9oH4", YouTubeQuality.Quality480P);
            YouMElement.Source = pathUri.Uri;
        }

    }
}
