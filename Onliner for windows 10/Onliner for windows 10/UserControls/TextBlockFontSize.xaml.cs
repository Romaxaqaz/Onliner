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
    public sealed partial class TextBlockFontSize : UserControl
    {
        private int fontSize = 12;

        public static DependencyProperty FontSizeProperty { private set; get; }

        static TextBlockFontSize()
        {
            FontSizeProperty = DependencyProperty.Register("FontSize",
                typeof(int),
                typeof(TextBlockFontSize),
                new PropertyMetadata(TextBlock.FontSizeProperty, null));
        }

        public TextBlockFontSize()
        {
            this.InitializeComponent();
        }
        public int FontSize
        {
            set { SetValue(FontSizeProperty, value); }
            get { return (int)GetValue(FontSizeProperty); }
        }

        private void UpFontSize_Click(object sender, RoutedEventArgs e)
        {
            this.FontSize++;
        }

        private void DownFontSize_Click(object sender, RoutedEventArgs e)
        {
            this.FontSize = 12;
        }

        private void DefaultFontSize_Click(object sender, RoutedEventArgs e)
        {
            this.FontSize--;
        }
    }
}
