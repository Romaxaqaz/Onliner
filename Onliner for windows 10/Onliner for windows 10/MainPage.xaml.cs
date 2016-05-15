using Windows.UI.Xaml.Controls;
using OnlinerApp.ViewModel;

namespace OnlinerApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
