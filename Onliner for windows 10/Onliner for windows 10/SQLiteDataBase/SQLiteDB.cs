using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using Windows.Storage;
using System.IO;
using Onliner_for_windows_10.Model;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Onliner_for_windows_10.Login;

namespace Onliner_for_windows_10.SQLiteDataBase
{
    public static class SQLiteDB
    {
        private static Request request = new Request();

        public static string DB_PATH_TECH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "TechNewsdb.sqlite");
        public static string DB_PATH_PEOPLE = Path.Combine(ApplicationData.Current.LocalFolder.Path, "PeopleNewsdb.sqlite");
        public static string DB_PATH_AUTO = Path.Combine(ApplicationData.Current.LocalFolder.Path, "AutoNewsdb.sqlite");
        public static string DB_PATH_HOUSE = Path.Combine(ApplicationData.Current.LocalFolder.Path, "HouseNewsdb.sqlite");
        public static string DB_PATH_OPINIONS = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Opiniom.sqlite");

        private static List<string> PathCollection = new List<string>() { DB_PATH_TECH, DB_PATH_PEOPLE, DB_PATH_AUTO, DB_PATH_HOUSE };

        private static SQLiteConnection DbConnection
        {
            get
            {
                return new SQLiteConnection(new SQLitePlatformWinRT(), (Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite")));
            }
        }


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

        private async static Task<ObservableCollection<ItemsNews>> CreateDatabase(ObservableCollection<ItemsNews> itemsNews, string path)
        {
            ObservableCollection<ItemsNews> resultItems = new ObservableCollection<ItemsNews>();
            if (!request.HasInternet()) return await GetAllNews(path);
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                var c = db.CreateTable<ItemsNews>();
                var info = db.GetMapping(typeof(ItemsNews));
                var items = (from p in db.Table<ItemsNews>()
                             select p).ToList();

                foreach (var item in itemsNews)
                {
                    var f = db.Table<ItemsNews>().Where(x => x.LinkNews.Contains(item.LinkNews)).Any();

                    if (items.Count < 50)
                    {
                        if (!f)
                        {
                            db.InsertOrReplace(item);
                        }
                    }
                    else
                    {
                        if (f)
                        {
                            db.Delete(items.Last());
                            db.InsertOrReplace(item);
                        }
                    }

                }
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
                                 select p).ToList();
                    resultItems = new ObservableCollection<ItemsNews>(items);
                }
            }
            catch (SQLite.Net.SQLiteException)
            { }
            await Task.CompletedTask;
            return resultItems;
        }

        public static void RemoveDataBase()
        {
            foreach (var item in PathCollection)
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), item))
                {
                    db.DeleteAll<ItemsNews>();
                    db.UpdateAll(null);
                }
            }
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
    }
}
