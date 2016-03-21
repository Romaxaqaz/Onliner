using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onliner_for_windows_10.Model;
using Onliner_for_windows_10.Login;
using HtmlAgilityPack;
using System.ComponentModel;
using MyToolkit.Command;
using Windows.UI.Xaml.Controls;

namespace Onliner_for_windows_10.View_Model
{
    public class OpinionsViewModel
    {
        private Request request = new Request();
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
            if (!request.HasInternet())
            {
                request.GetRequestOnliner(UrlApiOpinions);
                ResultHtmlPage = request.ResultGetRequsetString;
                HtmlDocument.LoadHtml(ResultHtmlPage);

                List<HtmlNode> opinionsItems = HtmlDocument.DocumentNode.Descendants().Where
                (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-opinions-body"))).
                FirstOrDefault().Descendants("div").Where(y => y.Attributes["class"].Value.Contains("b-opinions-list-item")).ToList();
                int step = 0;
                foreach (var item in opinionsItems)
                {
                    if (step == 0 || step % 2 == 0)
                    {
                        opinionModel = new OpinionModel();

                        opinionModel.LinkNews = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").FirstOrDefault().Descendants("a").FirstOrDefault().Attributes["href"].Value;
                        opinionModel.Header = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").FirstOrDefault().Descendants("a").FirstOrDefault().Descendants("h2").FirstOrDefault().InnerText;
                        opinionModel.Body = item.Descendants("p").FirstOrDefault().InnerText.Trim();
                        opinionModel.ImageUrl = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "person-portrait").FirstOrDefault().Attributes["style"].Value;
                        opinionModel.PersonAbout = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "person-about").FirstOrDefault().InnerText.Trim();
                        opinionModel.CommentsCount = item.Descendants("div").Where(div => div.GetAttributeValue("class", string.Empty) == "b-opinions-list-item__header").FirstOrDefault().Descendants("a").LastOrDefault().Descendants("span").FirstOrDefault().InnerText;

                        _opinionsItems.Add(opinionModel);
                    }
                    step++;
                }
            }
        }

    }
}
