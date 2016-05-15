using Windows.UI.Xaml.Controls;
using OnlinerApp.ViewModel;

namespace OnlinerApp.Views.KursCurrent
{
    public sealed partial class KursPage : Page
    {
        public KursPage()
        {
            this.InitializeComponent();
            this.DataContext = new KursViewModel();
        }
    }
}
