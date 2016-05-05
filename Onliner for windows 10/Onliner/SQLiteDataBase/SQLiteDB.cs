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
    public static class SqLiteDb
    {
        public static readonly HttpRequest HttpRequest = new HttpRequest();
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
        public static async Task UpdateAndCollectionInDb(ObservableCollection<ItemsNews> itemsNews, string dbPath)
        {
            await CreateDatabase(itemsNews, dbPath);
        }

        public static async Task UpdateItemDb(ObservableCollection<ItemsNews> itemsNews, string dbPath)
        {
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), dbPath))
            {
                db.UpdateAll(itemsNews);
            }
            await Task.CompletedTask;
        }


        private async static Task<ObservableCollection<ItemsNews>> CreateDatabase(ObservableCollection<ItemsNews> itemsNews, string path)
        {
            if (itemsNews == null) throw new ArgumentNullException(nameof(itemsNews));
            var resultItems = new ObservableCollection<ItemsNews>();
            if (!HttpRequest.HasInternet()) return await GetAllNews(path);
            var maxCountNews = 25;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<ItemsNews>();
                db.GetMapping(typeof(ItemsNews));

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

                if (itemsCount > maxCountNews && itemsCount != 0)
                {
                    for (var i = itemsNews.Count-1; i > maxCountNews; i--)
                    {
                        var lasts = itemsNews[i];
                        itemsNews.Remove(lasts);
                        db.Delete(lasts);
                    }
                }
                db.InsertOrReplaceAll(itemsNews);
                var resultColl = (from p in db.Table<ItemsNews>()
                                  select p).ToList();
                return new ObservableCollection<ItemsNews>(resultColl);
            }
        }

        public static async Task<ObservableCollection<ItemsNews>> GetAllNews(string path)
        {
            await Task.CompletedTask;
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

        }

        public static long SizeByteDB
        {
            get
            {
                long dBsize = 0;
                foreach (var item in PathCollection)
                {
                    dBsize += SizeByteFile(item);
                }
                return dBsize;
            }
        }

        public static long SizeByteFile(string path)
        {
            long fileLength = -1;
            try
            {
                using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                using (var file = isf.OpenFile(path, FileMode.Open))
                    fileLength = file.Length;
            }
            catch
            {
                // ignored
            }

            return fileLength;
        }

        public enum SectionNewsDb
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
