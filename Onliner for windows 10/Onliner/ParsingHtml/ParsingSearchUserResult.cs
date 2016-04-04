using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Onliner.Model.ProfileModel;
using System.Collections.ObjectModel;

namespace Onliner.ParsingHtml
{
    public class ParsingSearchUserResult
    {
        private HtmlDocument htmlDoc = new HtmlDocument();
        private ObservableCollection<ProfileSearchModel> listUsers;

        public string AllUsers { get; set; }

        public ObservableCollection<ProfileSearchModel> GetResultList(string htmlPage)
        {
            htmlDoc.LoadHtml(htmlPage);
            try
            {
                AllUsers = htmlDoc.DocumentNode.Descendants().Where
                (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-hdtopic"))).FirstOrDefault().
                Descendants("script").FirstOrDefault().InnerText;
            }
            catch (NullReferenceException)
            {
                AllUsers = "1";
            }

            List<HtmlNode> userList = htmlDoc.DocumentNode.Descendants().Where
                (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("b-userlist"))).FirstOrDefault().
                Descendants("table").FirstOrDefault().
                Descendants("tr").ToList();

            AllUsers = PagesResult(AllUsers);

                listUsers = new ObservableCollection<ProfileSearchModel>();
                foreach (var item in userList)
                {
                    listUsers.Add(new ProfileSearchModel(
                        item.Descendants("td").Where(div => div.GetAttributeValue("class", string.Empty) == "rate-stat").FirstOrDefault().Descendants("span").FirstOrDefault().InnerText,
                        item.Descendants("td").Where(div => div.GetAttributeValue("class", string.Empty) == "ph").FirstOrDefault().Descendants("a").FirstOrDefault().Descendants("img").FirstOrDefault().Attributes["src"].Value,
                        item.Descendants("td").Where(div => div.GetAttributeValue("class", string.Empty) == "user").FirstOrDefault().Descendants("strong").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText,
                        "",
                        item.Descendants("td").Where(div => div.GetAttributeValue("class", string.Empty) == "a-c user-status member-search-user-status").FirstOrDefault().Descendants("span").FirstOrDefault().InnerText,
                        "",
                        item.Descendants("td").Where(div => div.GetAttributeValue("class", string.Empty) == "u-msgs").FirstOrDefault().InnerText
                        ));
                }
                return listUsers;
        }

        private string PagesResult(string str)
        {
            if (str == string.Empty) return "0";
            string pattern = @"(PaginationSlider.init)(.\d+)(,)";
            string match = Regex.Match(str, pattern).ToString();
            string result = match.Replace("PaginationSlider.init(", "").Replace(",", "");
            return result;
        }

         
    }
}
