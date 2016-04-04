using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using Onliner.Http;
using Onliner.Model.News;
using static Onliner.Setting.SettingParams;
using System;

namespace Onliner.SQLiteDataBase
{
    public static class SQLiteDB
    {
        private static HttpRequest HttpRequest = new HttpRequest();
        private static SQLiteConnection DbConnection
        {
            get
            {
                return new SQLiteConnection(new SQLitePlatformWinRT(), (Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite")));
            }
        }

        #region TablesName
        public static string DB_PATH_TECH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "TechNewsdb.sqlite");
        public static string DB_PATH_PEOPLE = Path.Combine(ApplicationData.Current.LocalFolder.Path, "PeopleNewsdb.sqlite");
        public static string DB_PATH_AUTO = Path.Combine(ApplicationData.Current.LocalFolder.Path, "AutoNewsdb.sqlite");
        public static string DB_PATH_HOUSE = Path.Combine(ApplicationData.Current.LocalFolder.Path, "HouseNewsdb.sqlite");
        public static string DB_PATH_OPINIONS = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Opiniom.sqlite");
        private static string DB_PATH_FAVORITE = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Favorite.sqlite");
        #endregion

        #region Collections
        public static List<string> FileNameCollection = new List<string>() { "TechNewsdb.sqlite", "PeopleNewsdb.sqlite", "AutoNewsdb.sqlite", "HouseNewsdb.sqlite" };
        private static List<string> PathCollection = new List<string>() { DB_PATH_TECH, DB_PATH_PEOPLE, DB_PATH_AUTO, DB_PATH_HOUSE };
        #endregion

        #region Methods
        public static async Task<ObservableCollection<ItemsNews>> UpdateAndRetuntCollection(ObservableCollection<ItemsNews> itemsNews, SectionNewsDB section)
        {
            ObservableCollection<ItemsNews> resultItems = new ObservableCollection<ItemsNews>();
            switch (section)
            {
                case SectionNewsDB.Tech:
                    resultItems = await CreateDatabase(itemsNews, DB_PATH_TECH);
                    break;
                case SectionNewsDB.People:
                    resultItems = await CreateDatabase(itemsNews, DB_PATH_PEOPLE);
                    break;
                case SectionNewsDB.Auto:
                    resultItems = await CreateDatabase(itemsNews, DB_PATH_AUTO);
                    break;
                case SectionNewsDB.House:
                    resultItems = await CreateDatabase(itemsNews, DB_PATH_HOUSE);
                    break;
            }
            return resultItems;
        }

        public static async Task UpdateItemDB(ObservableCollection<ItemsNews> itemsNews, SectionNewsDB section)
        {
            switch (section)
            {
                case SectionNewsDB.Tech:
                    await UpdateItemDB(itemsNews, DB_PATH_TECH);
                    break;
                case SectionNewsDB.People:
                    await UpdateItemDB(itemsNews, DB_PATH_PEOPLE);
                    break;
                case SectionNewsDB.Auto:
                    await UpdateItemDB(itemsNews, DB_PATH_AUTO);
                    break;
                case SectionNewsDB.House:
                    await UpdateItemDB(itemsNews, DB_PATH_HOUSE);
                    break;
            }
        }

        /// <summary>
        /// Update elemets in db
        /// </summary>
        /// <param name="itemsNews">Elements that are present in the database</param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static async Task UpdateItemDB(ObservableCollection<ItemsNews> itemsNews, string path)
        {
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                foreach (var item in itemsNews)
                {
                    var it = db.Table<ItemsNews>().FirstOrDefault(x => x.LinkNews.Contains(item.LinkNews));
                    if (it != null)
                    {
                        it.Popularcount = item.Popularcount;
                        it.Footer = item.Footer;
                        it.CountViews = item.CountViews;
                        db.Update(it);
                    }
                }

            }
            await Task.CompletedTask;
        }

        private async static Task<ObservableCollection<ItemsNews>> CreateDatabase(ObservableCollection<ItemsNews> itemsNews, string path)
        {
            ObservableCollection<ItemsNews> resultItems = new ObservableCollection<ItemsNews>();
            if (!HttpRequest.HasInternet()) return await GetAllNews(path);
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                var maxCountNews = Convert.ToInt32(GetParamsSetting(NumberOfNewsitemsToTheCacheKey));
                if (maxCountNews == 0) maxCountNews = 50;
                var c = db.CreateTable<ItemsNews>();
                var info = db.GetMapping(typeof(ItemsNews));

                var items = (from p in db.Table<ItemsNews>()
                             select p).ToList();

                var itemsCount = items.Count;

                if (itemsCount == 0)
                {
                    foreach (var item in itemsNews)
                    {
                        resultItems.Add(item);
                    }
                }
                if (itemsCount > 0 && itemsCount < maxCountNews)
                {
                    foreach (var item in items)
                    {
                        itemsNews.Add(item);
                    }
                    resultItems = itemsNews;
                }
                if (itemsCount >= maxCountNews && itemsCount != 0)
                {
                    var last = items.Last();
                    items.Remove(last);
                    db.Delete(last);
                    foreach (var item in items)
                    {
                        resultItems.Add(item);
                    }
                }
                db.InsertOrReplaceAll(resultItems);

            }
            await Task.CompletedTask;
            return await GetAllNews(path);
        }

        public static async Task<ObservableCollection<ItemsNews>> GetAllNews(string path)
        {
            ObservableCollection<ItemsNews> resultItems = null;
            // Create a new connection
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    var items = (from p in db.Table<ItemsNews>()
                                 select p);
                    return new ObservableCollection<ItemsNews>(items);
                }
            }
            catch (SQLite.Net.SQLiteException)
            {
                return new ObservableCollection<ItemsNews>();
            }
            await Task.CompletedTask;
            return resultItems;
        }

        public static long GetSizeByteDB()
        {
            long DBsize = 0;
            foreach (var item in PathCollection)
            {
                DBsize += SizeByteDB(item);
            }
            return DBsize;
        }

        private static long SizeByteDB(string path)
        {
            long fileLength = -1;
            try
            {
                using (IsolatedStorageFile ISF = IsolatedStorageFile.GetUserStoreForApplication())
                using (IsolatedStorageFileStream file = ISF.OpenFile(path, FileMode.Open))
                    fileLength = file.Length;
            }
            catch { }

            return fileLength;
        }

        public enum SectionNewsDB
        {
            Tech,
            Auto,
            People,
            House,
            Opinion
        }
        #endregion
    }
}
