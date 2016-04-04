using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using Onliner.Http;
using Onliner.Model.ProfileModel;
using Onliner.Model.JsonModel.Profile;
using Onliner_for_windows_10.View_Model.ProfileViewModels;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views.Profile
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class EditProfilePage : Page
    {
        EditProfileViewModel viewModel = new EditProfileViewModel();

        public EditProfilePage()
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
            e.Handled = true;
            Frame.GoBack();
        }

      
    }
}
