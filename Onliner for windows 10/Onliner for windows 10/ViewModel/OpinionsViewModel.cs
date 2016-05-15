using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HtmlAgilityPack;
using MyToolkit.Command;
using Onliner.Http;
using Onliner.Model.OpinionsModel;

namespace OnlinerApp.ViewModel
{
    public class OpinionsViewModel
    {
        private HttpRequest HttpRequest = new HttpRequest();
        private OpinionModel opinionModel;
        private HtmlDocument HtmlDocument = new HtmlDocument();
        private ObservableCollection<OpinionModel> _opinionsItems = new ObservableCollection<OpinionModel>();

        private readonly string UrlApiOpinions = "http://people.onliner.by/category/opinions";
        private string ResultHtmlPage = string.Empty;

        public ObservableCollection<OpinionModel> IpinionsItems
        {
            get { return _opinionsItems; }
            set { _opinionsItems = value; }
        }

        public RelayCommand<IList<object>> ItemClickCommand
        {
            get
            {
                   var  selectionChangedCommand = new RelayCommand<IList<object>>(
                        items =>
                        {
                            var it = items as OpinionModel;
                }
                    );
                return selectionChangedCommand;
            }
        }

        public OpinionsViewModel()
        {
            if (HttpRequest.HasInternet()) return;
            HttpRequest.GetRequestOnliner(UrlApiOpinions);
            ResultHtmlPage = HttpRequest.ResultGetRequsetString;
            HtmlDocument.LoadHtml(ResultHtmlPage);

            var opinionsItems = HtmlDocument.DocumentNode.Descendants
                ().FirstOrDefault(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-opinions-body"))).Descendants("div").Where(y => y.Attributes["class"].Value.Contains("b-opinions-list-item")).ToList();
            foreach (var item in opinionsItems)
            {
                opinionModel = new OpinionModel();

                opinionModel.LinkNews = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").Descendants("a").FirstOrDefault().Attributes["href"].Value;
                opinionModel.Header = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").Descendants("a").FirstOrDefault().Descendants("h2").FirstOrDefault().InnerText;
                opinionModel.Body = item.Descendants("p").FirstOrDefault().InnerText.Trim();
                opinionModel.ImageUrl = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "person-portrait").Attributes["style"].Value;
                opinionModel.PersonAbout = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "person-about").InnerText.Trim();
                opinionModel.CommentsCount = item.Descendants("div").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").Descendants("a").LastOrDefault().Descendants("span").FirstOrDefault().InnerText;

                _opinionsItems.Add(opinionModel);
            }
        }

    }
}
