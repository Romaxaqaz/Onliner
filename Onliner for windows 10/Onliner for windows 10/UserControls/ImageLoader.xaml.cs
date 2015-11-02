using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Onliner_for_windows_10.UserControls
{
    public sealed partial class ImageLoader : UserControl
    {

        public static DependencyProperty SourceProperty =
             DependencyProperty.Register("Source",
                 typeof(ImageSource),
                 typeof(ImageLoader),
                 new PropertyMetadata(default(ImageSource), OnSourceChanged));

        public ImageLoader()
        {
            this.InitializeComponent();
        }
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var image = (ImageLoader)sender;
            var path = (ImageSource)args.NewValue;
            image.Source = path;
        }
    }
}
