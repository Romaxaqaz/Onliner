using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using HtmlAgilityPack;
using Onliner.Http;
using Template10.Mvvm;

namespace OnlinerApp.ViewModel
{
    public class KursViewModel : ViewModelBase
    {
        private readonly string KursUrlApi = "http://kurs.onliner.by/";

        private readonly HttpRequest _httpRequest = new HttpRequest();
        private readonly HtmlDocument _resultat = new HtmlDocument();

        #region Methods

        /// <summary>
        /// Loading a page with currency
        /// </summary>
        private async void LoadKursOnliner()
        {
            var content = await _httpRequest.GetRequestOnlinerAsync(KursUrlApi);
            _resultat.LoadHtml(content);

            var tagList = _resultat.DocumentNode.Descendants().Where
                (x => (x.Name == "table" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains
                ("b-currency-table__best"))).ToList();

            for (var i = 0; i < 3; i++)
            {
                var kurs = new Kurs();
                var tdlist = tagList[i].Descendants("tr").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "tr-main").Descendants("td").ToList();

                kurs.TypeMethod = GetContentTag(tdlist, 0);
                kurs.BankBuy = GetContentTag(tdlist, 1);
                kurs.BankSale = GetContentTag(tdlist, 2);
                kurs.Nbrb = GetContentTag(tdlist, 3);

                KursCollection.Add(kurs);
            }

            ProgressLoadRing = false;
        }

        /// <summary>
        /// Get data tag
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index">number td in tr</param>
        /// <returns></returns>
        private string GetContentTag(List<HtmlNode> node, int index)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
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
        private ObservableCollection<Kurs> _kursCollection = new ObservableCollection<Kurs>();
        public ObservableCollection<Kurs> KursCollection
        {
            get { return _kursCollection; }
            set { Set(ref _kursCollection, value); }
        }
        #endregion

        #region 
        private bool _progressLoadRing = true;
        public bool ProgressLoadRing
        {
            get { return _progressLoadRing; }
            set { Set(ref _progressLoadRing, value); }
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
