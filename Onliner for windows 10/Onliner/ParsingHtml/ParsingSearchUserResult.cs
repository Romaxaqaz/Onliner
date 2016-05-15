using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Onliner.Model.ProfileModel;
using System.Collections.ObjectModel;

namespace Onliner.ParsingHtml
{
    public class ParsingSearchUserResult
    {
        private readonly HtmlDocument _htmlDoc = new HtmlDocument();
        private ObservableCollection<ProfileSearchModel> _listUsers;

        public string AllUsers { get; set; }

        public ObservableCollection<ProfileSearchModel> GetResultList(string htmlPage)
        {
            _htmlDoc.LoadHtml(htmlPage);
            try
            {
                AllUsers = _htmlDoc.DocumentNode.Descendants
                ().FirstOrDefault(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-hdtopic"))).
                Descendants("script").FirstOrDefault().InnerText;
            }
            catch (NullReferenceException)
            {
                AllUsers = "1";
            }

            var userList = _htmlDoc.DocumentNode.Descendants
                ().FirstOrDefault(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-userlist"))).
                Descendants("table").FirstOrDefault().
                Descendants("tr").ToList();

            AllUsers = PagesResult(AllUsers);

                _listUsers = new ObservableCollection<ProfileSearchModel>();
                foreach (var item in userList)
                {
                    _listUsers.Add(new ProfileSearchModel(
                        item.Descendants("td").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "rate-stat").Descendants("span").FirstOrDefault().InnerText,
                        item.Descendants("td").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "ph").Descendants("a").FirstOrDefault().Descendants("img").FirstOrDefault().Attributes["src"].Value,
                        item.Descendants("td").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "user").Descendants("strong").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText,
                        "",
                        item.Descendants("td").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "a-c user-status member-search-user-status").Descendants("span").FirstOrDefault().InnerText,
                        "",
                        item.Descendants("td").FirstOrDefault(div => div.GetAttributeValue("class", string.Empty) == "u-msgs").InnerText
                        ));
                }
                return _listUsers;
        }

        private string PagesResult(string str)
        {
            if (str == string.Empty) return "0";
            var pattern = @"(PaginationSlider.init)(.\d+)(,)";
            var match = Regex.Match(str, pattern).ToString();
            var result = match.Replace("PaginationSlider.init(", "").Replace(",", "");
            return result;
        }

         
    }
}
