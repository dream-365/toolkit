using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public class XPathUriDiscovery : IUriDiscovery
    {
        private readonly string _xpath;

        private readonly string _attribute;

        private readonly Regex _filterPattern;

        private readonly Uri _baseUri;

        public XPathUriDiscovery(string xpath, string attribute, Regex filterPattern, string baseUri)
        {
            _xpath = xpath;

            _attribute = attribute;

            _filterPattern = filterPattern;

            if(!string.IsNullOrEmpty((baseUri)))
            {
                _baseUri = new Uri(baseUri);
            } 
        }

        public IEnumerable<Uri> Discover(string content)
        {
            var result = new List<Uri>();

            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(content);

            var nodes = htmlDocument.DocumentNode.SelectNodes(_xpath);

            if (nodes == null)
            {
                Console.WriteLine("No nodes found in the current content");

                return result;
            }

            foreach (HtmlNode node in nodes)
            {
                var link = node.GetAttributeValue(_attribute, "");

                if(link != null && (
                    _filterPattern == null ||
                    _filterPattern.IsMatch(link)))
                {
                    Uri uri;

                    if(!link.ToLower().StartsWith("http"))
                    {
                        uri = new Uri(_baseUri, link);
                    }
                    else
                    {
                        uri = new Uri(link);
                    }

                    result.Add(uri);
                }
                else
                {
                    Console.WriteLine("Link[{0}] does not match the condiation", link);
                }
            }

            return result;
        }
    }
}
