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

namespace Onliner_for_windows_10.SQLiteDataBase
{
    public static class SQLiteDB
    {
        public static string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");

        private static SQLiteConnection DbConnection
        {
            get
            {
                return new SQLiteConnection(new SQLitePlatformWinRT(), (Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite")));
            }
        }

        public async static Task<ObservableCollection<ItemsNews>> CreateDatabase(ObservableCollection<ItemsNews> itemsNews)
        {
            ObservableCollection<ItemsNews> resultItems = new ObservableCollection<ItemsNews>();
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH))
            {
                // Create the table if it does not exist
 
                var c = db.CreateTable<ItemsNews>();

                var info = db.GetMapping(typeof(ItemsNews));


                foreach (var item in itemsNews)
                {
                    var f = db.Table<ItemsNews>().Where(x => x.LinkNews.Contains(item.LinkNews)).Any();
                    var items = (from p in db.Table<ItemsNews>()
                                 select p).ToList();

                    if(items.Count<50)
                    {
                        if (!f)
                        {
                            db.InsertOrReplace(item);
                            resultItems.Insert(0, item);
                        }
                    }
                    else
                    {
                        if (!f)
                        {
                            db.Delete(items.Last());
                            db.InsertOrReplace(item);
                            resultItems.Insert(0, item);
                        }
                    }

                }
            }
            await Task.Delay(0);
            return resultItems;
        }

        public static ObservableCollection<ItemsNews> GetAllNews()
        {
            ObservableCollection<ItemsNews> resultItems;
            // Create a new connection
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH))
            {

            var items = (from p in db.Table<ItemsNews>()
                          select p).ToList();
                resultItems = new ObservableCollection<ItemsNews>(items);
            }
            return resultItems;
        }

        public static void RemoveDataBase()
        {
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH))
            {

                db.DeleteAll<ItemsNews>();
            }
        }

        public class TechListItemModel : ItemsNews { }
        public class AutoListItemModel : ItemsNews { }
        public class PeopleListItemModel : ItemsNews { }
        public class HouseListItemModel : ItemsNews { }
        public class OpinionListItemModel : ItemsNews { }

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
