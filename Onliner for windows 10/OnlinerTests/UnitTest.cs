using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using Onliner.SQLiteDataBase;

namespace OnlinerTests
{
    [TestClass]
    public class UnitTest1
    {
        private Onliner.Http.HttpRequest request = new Onliner.Http.HttpRequest();

        [TestMethod]
        public void Request_Message_Method()
        {
            var messageCount = request.MessageUnread();
            Assert.IsFalse(string.IsNullOrEmpty(messageCount.Result));
        }

        [TestMethod]
        public void Request_Bestrate_Method()
        {
            var bestrate = SQLiteDB.GetAllNews(SQLiteDB.DB_PATH_PEOPLE);
            int x = 0;
        }
    }
}
