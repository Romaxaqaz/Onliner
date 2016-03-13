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
using Windows.UI;
using Onliner_for_windows_10.Login;
using HtmlAgilityPack;
using MyToolkit.Controls;
// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class testpage : Page
    {

        public class CommentsContent
        {
            public string Tag { get; set; }
            public string Content { get; set; }

            public CommentsContent(string tag, string content)
            {
                this.Tag = tag;
                this.Content = content;
            }
        }

        Request request = new Request();
        private HtmlDocument htmlDoc = new HtmlDocument();
        List<CommentsContent> liststring = new List<CommentsContent>();

        private int countStep = 0;
        private int countBlock = 0;
        //private string httml = "<div class=\"comment-content\" id=\"message_1857162\"><blockquote class=\"uncited\"><div><cite>VINNIVINNI:</cite><em>\"За основу действительно были взяты StG-44\"</em> - это какой ёж родил такую мысль?<br>Да просто на картинку посмотрите:<br><a href = \"http://www.ak-info.ru/spravka/akvsstg/akvsstg05.jpg\" class=\"postlink\" target=\"_blank\">разобранные АК и StG рядышком</a><p></p><p>Автомат Булкина ничего не напоминает?<br><a href = \"https://upload.wikimedia.org/wikipedia/ru/1/15/%D0%A2%D0%9A%D0%91-415_%28%D0%90%D0%B2%D1%82%D0%BE%D0%BC%D0%B0%D1%82_%D0%91%D1%83%D0%BB%D0%BA%D0%B8%D0%BD%D0%B0%29.jpg\" class=\"postlink\" target=\"_blank\">Изображение здесь</a></p></div></blockquote><blockquote class=\"uncited\"><div><cite>VINNIVINNI:</cite>\"За основу действительно были взяты StG-44\" - это какой ёж родил такую мысль?<br>Да просто на картинку посмотрите:<br>разобранные АК и StG рядышком</div></blockquote><p>Украл, не украл, взял за основу, похож, не похож.Победитель имеет право, история так сложилась.Мое мнение - есть значительные внешние различия, но главное - это та маленькая деталька, что возле затворной рамы(за которую тянут для перезарядки) - собственно затвор.Принцип этого узла абсолютно одинаков у АК и StG.</p>    </div>";
        //private string httml = "<div class=\"comment-content\" id=\"message_1857169\"><blockquote class=\"uncited\"><div><cite>voit-cs:</cite><blockquote class=\"uncited\"><div><cite>GalaG3:</cite><blockquote class=\"uncited\"><div><cite>Rooke:</cite>B-4-1<br>Для тех кто понял<br>))</div></blockquote><p>Ак в-4-2))</p></div></blockquote><p>Это в 1.6</p></div></blockquote><p>В GO он тоже 4-2/</p>    </div>";
        //private string httml = "<div class=\"comment-content\" id=\"message_1857209\"><p>Поржал Спасибо )))))))))))))))))))))))</p><blockquote class=\"uncited\"><div>телескопическим прицелом</div></blockquote><p> - Просто убило. Из трех фото М16 только на одной оная.</p><blockquote class=\"uncited\"><div>АК-47</div></blockquote><p> - нет такого автомата, есть АК, учите мат часть.А сравнивать АК с STG - 44 это непрофессионально и крайне глупо!<br>У АК запирание ствола происходит поворотом у StG-44 перекосом, абсолютно разные УСМ.Это совсем разные технологии. И если вы в чем-то не разбираетесь лучше статей совсем не писать!<br>Ну и под конец, как понимаю автор решил блеснуть и выкинуть фото АК-12, но как-то не получилось видимо, запутался в стволах и выкинул АК в обвесе.<br>Учитесь и читайте, а то страшно за белорусскую журналистику. А то скоро будете рассказывать, что вторая мировая закончилась 9-го мая!</p>    </div>";


        public testpage()
        {
            this.InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            
        }

        private Border ReturnBorderComplete(string s)
        {
            Border mainBorder = new Border();
            Border buff = new Border();
            Grid grid = new Grid(); 
           
            if (countStep < countBlock)
            {
                countStep++;
                htmlDoc.LoadHtml(s);
                mainBorder.BorderThickness = new Thickness(2);
                mainBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 60, 100, 50));
                mainBorder.Margin = new Thickness(10, 5, 2, 2);
                grid.HorizontalAlignment = HorizontalAlignment.Stretch;
                grid.VerticalAlignment = VerticalAlignment.Stretch;
                grid.Background = new SolidColorBrush(Color.FromArgb(255, 176, 224, 230));

                RowDefinition rowDefinition1 = new RowDefinition();
                RowDefinition rowDefinition2 = new RowDefinition();
                RowDefinition rowDefinition3 = new RowDefinition();
                rowDefinition1.Height = new GridLength();
                rowDefinition2.Height = new GridLength();
                rowDefinition3.Height = new GridLength();
                grid.RowDefinitions.Add(rowDefinition1);
                grid.RowDefinitions.Add(rowDefinition2);
                grid.RowDefinitions.Add(rowDefinition3);

                if (countStep == countBlock)
                {
                    HtmlView t11 = new HtmlView();
                    t11.Html = htmlDoc.DocumentNode.Descendants("div").LastOrDefault().InnerText;
                    Grid.SetRow(t11, 2);
                    grid.Children.Add(t11);
                }
                else
                {
                    HtmlView t1 = new HtmlView();
                    t1.Html = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault().Descendants("cite").FirstOrDefault().InnerText;
                    Grid.SetRow(t1, 0);
                    grid.Children.Add(t1);

                    HtmlView t11 = new HtmlView();
                    t11.Html = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault().Descendants("p").LastOrDefault().InnerText;
                    Grid.SetRow(t11, 2);
                    grid.Children.Add(t11);
                }
                try
                {
                    buff = ReturnBorderComplete(htmlDoc.DocumentNode.Descendants("div").FirstOrDefault().Descendants("blockquote").FirstOrDefault().InnerHtml);
                    Grid.SetRow(buff, 1);
                    grid.Children.Add(buff);
                }
                catch(NullReferenceException)
                {
                    ReturnBorderComplete("");
                }

            }
            mainBorder.Child = grid;
            return mainBorder;
        }



        private void MediaElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MediaElement med = (MediaElement)sender;
            med.Play();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            YouTubePlayerControl pl = new YouTubePlayerControl();
            //MainGrid.Children.Add(pl);

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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }
    }
}

