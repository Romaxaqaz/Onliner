using Onliner_for_windows_10.Model.Catalog;
using Onliner_for_windows_10.ProfilePage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace Onliner_for_windows_10.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CatalogPage : Page
    {
        ObservableCollection<ElectronicSection> catL;

        public CatalogPage()
        {
            this.InitializeComponent();
            Loaded += CatalogPage_Loaded;
        }

        private void CatalogPage_Loaded(object sender, RoutedEventArgs e)
        {
            Additionalinformation.Instance.NameActivePage = "Каталог";
            catL = new ObservableCollection<ElectronicSection>();
            catL.Add(new ElectronicSection() { Category = "Телефония и связь", CategoryType = "Мобильные телефоны" });
            catL.Add(new ElectronicSection() { Category = "Телефония и связь", CategoryType = "Умные часы" });
            catL.Add(new ElectronicSection() { Category = "Телефония и связь", CategoryType = "Радиотелефоны DECT" });
            catL.Add(new ElectronicSection() { Category = "Телефония и связь", CategoryType = "Факсы" });
            catL.Add(new ElectronicSection() { Category = "Телефония и связь", CategoryType = "Проводные телефоны" });
            catL.Add(new ElectronicSection() { Category = "Телефония и связь", CategoryType = "3G-модемы" });
            catL.Add(new ElectronicSection() { Category = "Телефония и связь", CategoryType = "Портативные радиостанции" });
            catL.Add(new ElectronicSection() { Category = "Телефония и связь", CategoryType = "Автомобильные радиостанции" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Чехлы для телефонов" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Аккумуляторы" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Портативные аккумуляторы (Powerbank)" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Bluetooth-гарнитуры" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Автомобильные держатели" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Палки для селфи" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Беспроводные и портативные колонки" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Музыкальные док-станции" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Зарядные устройства" });
            catL.Add(new ElectronicSection() { Category = "Аксессуары для телефонов", CategoryType = "Data-кабели" });
            catL.Add(new ElectronicSection() { Category = "Минздрав предупреждает", CategoryType = "Электронные парогенераторы" });
            catL.Add(new ElectronicSection() { Category = "Планшеты, электронные книги", CategoryType = "Планшеты" });
            catL.Add(new ElectronicSection() { Category = "Планшеты, электронные книги", CategoryType = "Чехлы для планшетов" });
            catL.Add(new ElectronicSection() { Category = "Планшеты, электронные книги", CategoryType = "Электронные книги" });
            catL.Add(new ElectronicSection() { Category = "Планшеты, электронные книги", CategoryType = "Обложки для электронных книг" });
            catL.Add(new ElectronicSection() { Category = "Планшеты, электронные книги", CategoryType = "Внешние аккумуляторы (Powerbank)" });
            catL.Add(new ElectronicSection() { Category = "Планшеты, электронные книги", CategoryType = "Беспроводные и портативные колонки" });
            catL.Add(new ElectronicSection() { Category = "Планшеты, электронные книги", CategoryType = "Музыкальные док-станции" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Фотоаппараты" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Видеокамеры" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Экшен-камеры" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Объективы" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Светофильтры и конвертеры" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Вспышки и лампы" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Штативы" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Палки для селфи" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Сумки для фото/видео" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Аккумуляторы для фото/видео" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Батарейки, аккумуляторы, зарядные" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Карты памяти" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Карт-ридеры" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Фотопринтеры" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Цифровые фоторамки" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Бинокли и подзорные трубы" });
            catL.Add(new ElectronicSection() { Category = "Фото- и видеотехника", CategoryType = "Телескопы" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Телевизоры" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Приемники цифрового ТВ" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "ТВ-антенны" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Кронштейны для ТВ" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Звуковые панели и колонки для ТВ" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Домашние кинотеатры" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Плееры DVD и Blu-ray" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Медиаплееры" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Спутниковые ресиверы" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "ТВ-тюнеры" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Аксессуары для ТВ" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Проекторы" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Проекционные экраны" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Кронштейны для проекторов" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Кабели для AV-аппаратуры" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "Универсальные пульты ДУ" });
            catL.Add(new ElectronicSection() { Category = "Телевидение и видео", CategoryType = "" });
            catL.Add(new ElectronicSection() { Category = "Музыкальное оборудование", CategoryType = "Синтезаторы и рабочие станции" });
            catL.Add(new ElectronicSection() { Category = "Музыкальное оборудование", CategoryType = "Студийные мониторы" });
            catL.Add(new ElectronicSection() { Category = "Музыкальное оборудование", CategoryType = "Аудиоинтерфейсы" });
            catL.Add(new ElectronicSection() { Category = "Музыкальное оборудование", CategoryType = "Гитарные комбоусилители" });
            catL.Add(new ElectronicSection() { Category = "Музыкальное оборудование", CategoryType = "Концертная акустика" });

            var carCategoryList = new List<CategoryListViewModel>();
            var carGroups = catL.OrderBy(x => x.Category).GroupBy(x => x.Category);
            foreach (System.Linq.IGrouping<string, ElectronicSection> item in carGroups)
            {
                carCategoryList.Add(new CategoryListViewModel() { CategoryName = item.Key, ElectronicSection = item.ToList<ElectronicSection>() });
            }
            groupItemsViewSource.Source = carCategoryList;
        }

        private void Sort()
        {

        }

        private void GridViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (OneListView.Visibility == Visibility.Collapsed)
            {
                OneListView.Visibility = Visibility.Visible;
            }
            else
            {
                OneListView.Visibility = Visibility.Collapsed;
            }
        }
    }
}
