using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentAnalyze
{
    public class HtmlAttributesLookUp
    {
        public const string InnerText = "InnerText";

        public IEnumerable<string> Execute(string html, string xpath, string lookup)
        {
            var htmlDoc = new HtmlDocument();

            List<string> result;

            htmlDoc.LoadHtml(html);

            HtmlNodeCollection filterNodes = htmlDoc.DocumentNode.SelectNodes(xpath);

            if (filterNodes == null)
            {
                return null;
            }

            if (InnerText.Equals(lookup))
            {
                result =
                    (from filterNode in filterNodes
                     select filterNode.InnerText into text
                     where text != null
                     select text).ToList();
            }
            else
            {
                result =
                (from filterNode in filterNodes
                 select filterNode.Attributes[lookup]
                     into attr
                 where attr != null
                 select attr.Value).ToList();
            }

            htmlDoc = null;

            return result;
        }

    }
}
