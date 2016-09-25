using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace HtmlToJson
{
    public enum SelectType
    {
        Attribute = 0,
        InnerText = 1,
        Html = 2
    }

    public class XPathSelector
    {
        private HtmlDocument _document;

        public XPathSelector(string html)
        {
            _document = new HtmlDocument();

            _document.LoadHtml(html);
        }

        public IEnumerable<string> Query(string xpath, SelectType selectType = SelectType.InnerText, string attributeName = "")
        {
            HtmlNodeCollection matchedNodes = _document.DocumentNode.SelectNodes(xpath);

            if(matchedNodes == null)
            {
                return new List<string>();
            }

            List<string> result = new List<string>();

            if (selectType == SelectType.InnerText)
            {
                result =
                    (from filterNode in matchedNodes
                     select filterNode.InnerText into text
                     where text != null
                     select text).ToList();
            }
            else if(selectType == SelectType.Attribute)
            {
                result =
                (from filterNode in matchedNodes
                 select filterNode.Attributes[attributeName]
                     into attr
                 where attr != null
                 select attr.Value).ToList();
            }else if(selectType == SelectType.Html)
            {
                result =
                (from filterNode in matchedNodes
                 select filterNode.InnerHtml
                     into html
                 where html != null
                 select html).ToList();
            }

            return result;
        }
    }
}
