using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.ParsingHtml
{
    public class ParsingHtml
    {
        //Input parametr: <(1)div (2)class="(3)one"><(4)span></div>
        public string GetDataHtml(string tagName, string className, string elementTag)
        {
            HtmlDocument resultat = new HtmlDocument();
            var node = resultat.DocumentNode.Descendants(tagName).
               Where(div => div.GetAttributeValue(className, string.Empty) == elementTag).
               FirstOrDefault();
            return node.InnerHtml.ToString();
        }

        public string GetDataText(string tagName, string className, string elementTag)
        {
            HtmlDocument resultat = new HtmlDocument();
            var node = resultat.DocumentNode.Descendants(tagName).
               Where(div => div.GetAttributeValue(className, string.Empty) == elementTag).
               FirstOrDefault();
            return node.InnerText.ToString();
        }

        public string ParsElementHtml(string[] paramsPars, HtmlDocument htmldoc)
        {
            string finalResult = "";
            if (paramsPars[3] != "")
            {
                Regex htmlreg = new Regex(paramsPars[3]);
                var name = htmldoc.DocumentNode.Descendants(paramsPars[0]).
                   Where(div => div.GetAttributeValue(paramsPars[1], string.Empty) == paramsPars[2]).
                   FirstOrDefault();
                string result = name.InnerHtml.Trim();
                finalResult = htmlreg.Match(result).ToString();
            }
            else
            {
                var name = htmldoc.DocumentNode.Descendants(paramsPars[0]).
                   Where(div => div.GetAttributeValue(paramsPars[1], string.Empty) == paramsPars[2]).
                   FirstOrDefault();
                finalResult = name.InnerHtml.Trim();
            }
            return finalResult;
        }

    }
}
