using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MyToolkit.Media;
using MyToolkit.Multimedia;
using Onliner_for_windows_10.UserControls;
using Windows.UI.Xaml.Media.Imaging;
// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class testpage : Page
    {
        public testpage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void MediaElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MediaElement med = (MediaElement)sender;
            med.Play();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            YouTubePlayerControl pl = new YouTubePlayerControl();
            MainGrid.Children.Add(pl);
            
        }

        private void player_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
          
        }

        

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
           
        }

        private void PopUpTest_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
        }
    }
}

