using HtmlAgilityPack;
using Onliner.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;

namespace Onliner_for_windows_10.View_Model
{
    public class KursViewModel : ViewModelBase
    {
        private readonly string KursUrlApi = "http://kurs.onliner.by/";

        private HttpRequest HttpRequest = new HttpRequest();
        private HtmlDocument resultat = new HtmlDocument();

        #region Methods

        /// <summary>
        /// Loading a page with currency
        /// </summary>
        private async void LoadKursOnliner()
        {
            var content = await HttpRequest.GetRequestOnlinerAsync(KursUrlApi);
            resultat.LoadHtml(content);

            List<HtmlNode> tagList = resultat.DocumentNode.Descendants().Where
                (x => (x.Name == "table" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains
                ("b-currency-table__best"))).ToList();

            Kurs kurs;
            for (int i = 0; i < 3; i++)
            {
                kurs = new Kurs();
                var tdlist = tagList[i].Descendants("tr").
                             Where(div => div.GetAttributeValue("class", string.Empty) == "tr-main").FirstOrDefault().Descendants("td").ToList();

                kurs.TypeMethod = GetContentTag(tdlist, 0, "abbr rate");
                kurs.BankBuy = GetContentTag(tdlist, 1, "value");
                kurs.BankSale = GetContentTag(tdlist, 2, "value");
                kurs.Nbrb = GetContentTag(tdlist, 3, "value fall");

                KursCollection.Add(kurs);
            }

            ProgressLoadRing = false;
        }

        /// <summary>
        /// Get data tag
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index">number td in tr</param>
        /// <param name="attributeName"></param>
        /// <param name="OrAttName"></param>
        /// <returns></returns>
        private string GetContentTag(List<HtmlNode> node, int index, string attributeName, string OrAttName = "")
        {
            var tagContent = node[index].Descendants("p").FirstOrDefault();
            return tagContent != null ? tagContent.InnerText : "";
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            LoadKursOnliner();
            await Task.CompletedTask;
        }
        #endregion

        #region Collections
        private ObservableCollection<Kurs> kursCollection = new ObservableCollection<Kurs>();
        public ObservableCollection<Kurs> KursCollection
        {
            get { return kursCollection; }
            set { Set(ref kursCollection, value); }
        }
        #endregion

        #region 
        private bool progressLoadRing = true;
        public bool ProgressLoadRing
        {
            get { return progressLoadRing; }
            set { Set(ref progressLoadRing, value); }
        }

        #endregion

    }

    public class Kurs
    {
        public string TypeMethod { get; set; }
        public string BankBuy { get; set; }
        public string BankSale { get; set; }
        public string Nbrb { get; set; }

        public Kurs()
        {
        }

        public Kurs(string type, string bankBuy, string bankSale, string nbrb)
        {
            TypeMethod = type;
            BankBuy = bankBuy;
            BankSale = bankSale;
            Nbrb = nbrb;
        }
    }
}
