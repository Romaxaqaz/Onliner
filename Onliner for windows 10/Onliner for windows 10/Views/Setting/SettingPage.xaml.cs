using Onliner.SQLiteDataBase;
using Onliner_for_windows_10.View_Model.Settings;
using System;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Onliner_for_windows_10.Views.Setting
{
    public sealed partial class SettingPage : Page
    {
        SettingViewModel viewModel = new SettingViewModel();

        public SettingPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

    }
}
