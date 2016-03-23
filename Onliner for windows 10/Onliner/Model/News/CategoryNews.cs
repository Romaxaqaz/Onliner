using System.Collections.Generic;

namespace Onliner.Model.News
{
    public class CategoryNews
    {
        public string NameCategory { get; set; }
        public string ValueCategory { get; set; }

        public List<CategoryNews> GetTechCategory()
        {
            List<CategoryNews> techCategory = new List<CategoryNews>();
            techCategory.Add(new CategoryNews { NameCategory = "Apple", ValueCategory = "" });
            techCategory.Add(new CategoryNews { NameCategory = "Гаджеты и вендоры", ValueCategory = "" });
            techCategory.Add(new CategoryNews { NameCategory = "Операторы", ValueCategory = "" });
            techCategory.Add(new CategoryNews { NameCategory = "Интернет", ValueCategory = "" });
            techCategory.Add(new CategoryNews { NameCategory = "Игры", ValueCategory = "" });
            techCategory.Add(new CategoryNews { NameCategory = "Наука", ValueCategory = "" });
            techCategory.Add(new CategoryNews { NameCategory = "Обзоры техники", ValueCategory = "" });
            techCategory.Add(new CategoryNews { NameCategory = "Фотосюжет", ValueCategory = "" });

            return techCategory;
        }
        public List<CategoryNews> GetPeopleCategory()
        {
            List<CategoryNews> peopleCategory = new List<CategoryNews>();
            peopleCategory.Add(new CategoryNews { NameCategory = "Социум", ValueCategory = "" });
            peopleCategory.Add(new CategoryNews { NameCategory = "Бизнес", ValueCategory = "" });
            peopleCategory.Add(new CategoryNews { NameCategory = "Культура", ValueCategory = "" });
            peopleCategory.Add(new CategoryNews { NameCategory = "Еда", ValueCategory = "" });
            peopleCategory.Add(new CategoryNews { NameCategory = "Мнения", ValueCategory = "" });

            return peopleCategory;
        }
        public List<CategoryNews> GetAutoCategory()
        {
            List<CategoryNews> autoCategory = new List<CategoryNews>();
            autoCategory.Add(new CategoryNews { NameCategory = "Аварии", ValueCategory = "" });
            autoCategory.Add(new CategoryNews { NameCategory = "Дороги", ValueCategory = "" });
            autoCategory.Add(new CategoryNews { NameCategory = "Авто моё", ValueCategory = "" });
            autoCategory.Add(new CategoryNews { NameCategory = "Тест-драйвы", ValueCategory = "" });

            return autoCategory;
        }
        public List<CategoryNews> GetHouseCategory()
        {
            List<CategoryNews> houseCategory = new List<CategoryNews>();
            houseCategory.Add(new CategoryNews { NameCategory = "Цены", ValueCategory = "" });
            houseCategory.Add(new CategoryNews { NameCategory = "Городская жизнь", ValueCategory = "" });
            houseCategory.Add(new CategoryNews { NameCategory = "Проблемы", ValueCategory = "" });
            houseCategory.Add(new CategoryNews { NameCategory = "Архитектура", ValueCategory = "" });
            houseCategory.Add(new CategoryNews { NameCategory = "Интерьер", ValueCategory = "" });
            return houseCategory;
        }
    }
}
