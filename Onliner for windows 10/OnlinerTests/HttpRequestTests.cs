using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace OnlinerTests
{  
    [TestClass]
    public class HttpRequestTests
    {
        private Onliner.Http.HttpRequest request = new Onliner.Http.HttpRequest();

        [TestMethod]
        public async void Request_Bestrate_Method()
        {
            var bestrate = await request.Bestrate("USD", "nbrb");

            Assert.IsNotNull(bestrate);
        }
    }
}
