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
    public sealed partial class MessegeSenderControl : UserControl
    {

        public MessegeSenderControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public string Username
        {
            get { return NameUser.Text; }
            set { NameUser.Text = value; }
        }

        public string MessageHeader
        {
            get { return HeaderMessage.Text; }
            set { HeaderMessage.Text = value; }
        }

        public string MessageText
        {
            get { return TextMessage.Text; }
            set { TextMessage.Text = value; }
        }

        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            if (TextMessage.SelectedText != null)
            {
                string boldText = "[b]" + TextMessage.SelectedText + "[/b]";
                TextMessage.Text = TextMessage.Text.Replace(TextMessage.SelectedText, boldText);
            }
            else
            {
                TextMessage.Text = "[b][/b]";
            }
        }
    }
}
