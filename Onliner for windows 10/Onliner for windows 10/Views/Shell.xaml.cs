using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Onliner_for_windows_10.Login;
using Onliner_for_windows_10.ProfilePage;
using Onliner_for_windows_10.Model.Message;
using Template10.Services.NavigationService;
using Template10.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        public Shell()
        {
            this.InitializeComponent();
            var fakeObject = Additionalinformation.Instance;
            ImageS.ImageSource = new BitmapImage(new Uri(fakeObject.AvatarUrl));
            Login.Text = fakeObject.Login;
            Instance = this;
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }
    }
}