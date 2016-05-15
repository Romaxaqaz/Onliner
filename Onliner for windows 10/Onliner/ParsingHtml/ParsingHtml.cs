using HtmlAgilityPack;
using System.Linq;
using System.Text.RegularExpressions;

namespace Onliner.ParsingHtml
{
    public class ParsingHtml
    {
        /// <summary>
        /// Parsing html
        /// </summary>
        /// <example></example>
        /// <param name="tagName"></param>
        /// <param name="className"></param>
        /// <param name="elementTag"></param>
        /// <returns>html</returns>
        public string GetDataHtml(string tagName, string className, string elementTag)
        {
            var resultat = new HtmlDocument();
            var node = resultat.DocumentNode.
               Descendants(tagName).
               FirstOrDefault(div => div.GetAttributeValue(className, string.Empty) == elementTag);
            return node.InnerHtml;
        }

        /// <summary>
        /// Parsing html
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="className"></param>
        /// <param name="elementTag"></param>
        /// <returns>text</returns>
        public string GetDataText(string tagName, string className, string elementTag)
        {
            var resultat = new HtmlDocument();
            var node = resultat.DocumentNode.
               Descendants(tagName).
               FirstOrDefault(div => div.GetAttributeValue(className, string.Empty) == elementTag);
            return node.InnerText;
        }

        public string ParsElementHtml(string[] paramsPars, HtmlDocument htmldoc)
        {
            string finalResult;
            if (paramsPars[3] != "")
            {
                var htmlreg = new Regex(paramsPars[3]);
                var name = htmldoc.DocumentNode.
                   Descendants(paramsPars[0]).
                   FirstOrDefault(div => div.GetAttributeValue(paramsPars[1], string.Empty) == paramsPars[2]);
                var result = name.InnerHtml.Trim();
                finalResult = htmlreg.Match(result).ToString();
            }
            else
            {
                var name = htmldoc.DocumentNode.
                   Descendants(paramsPars[0]).
                   FirstOrDefault(div => div.GetAttributeValue(paramsPars[1], string.Empty) == paramsPars[2]);
                finalResult = name.InnerHtml.Trim();
            }
            return finalResult;
        }
    }
}
