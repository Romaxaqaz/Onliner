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

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views
{
    public class MyItem
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }

        public MyItem(string title, string subtitle, string description)
        {
            Title = title;
            Subtitle = subtitle;
            Description = description;
        }
    }
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class NewsPage : Page
    {
        List<MyItem> myItems = new List<MyItem>();
        public NewsPage()
        {
            this.InitializeComponent();
            for (int i = 1; i < 100; i++)
            {
                myItems.Add(new MyItem(
                    "Title:" + i.ToString(), // Title.
                    "Sub:" + i.ToString(), // Subtitle.
                    "Desc:" + i.ToString())); // Description.                            
            }
            myGridView.ItemsSource = myItems;
        }

    }
}
