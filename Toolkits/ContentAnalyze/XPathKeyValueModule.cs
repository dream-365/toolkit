using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContentAnalyze
{
    public class XPathKeyValueModule : IMetadataModule
    {
        private string _xpath;

        private string _pattern;

        private string _metadataKey;

        private const string DEFAULT_METADATA_KEY = "sentences_module_key";

        public void Init(IEnumerable<string> arguments)
        {
            var args = arguments.ToArray();

            _xpath = args.FirstOrDefault();

            _pattern = args.Length > 1 ? args[1]
                                       : string.Empty;

            _metadataKey = args.Length > 2 ? args[2]
                                           : DEFAULT_METADATA_KEY;
        }

        public void OnProcess(IDictionary<string, object> metadata, string content)
        {
            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(content);

            var node = htmlDocument.DocumentNode.SelectSingleNode(_xpath);

            if(node == null)
            {
                Console.WriteLine("No content by xpath: {0}", _xpath);

                return;
            }

            var rawText = node.InnerText;

            if (_pattern != string.Empty)
            {
                CaptureMatch(metadata, rawText);
            }
            else
            {
                metadata[_metadataKey] = node.InnerText;
            }            
        }

        private void CaptureMatch(IDictionary<string, object> metadata, string rawText)
        {
            var regex = new Regex(_pattern);

            var matches = regex.Matches(rawText);

            if (matches.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (Match match in matches)
                {
                    sb.AppendLine(match.Value);
                }

                metadata[_metadataKey] = sb.ToString();
            }
            else
            {
                Console.WriteLine("No matches in content");
            }
        }

        private IList<string> GetOrCreate(IDictionary<string, object> result)
        {
            if(result.ContainsKey(DEFAULT_METADATA_KEY))
            {
                return result[DEFAULT_METADATA_KEY] as IList<string>;
            }

            var list = new List<string>();

            result[DEFAULT_METADATA_KEY] = list;

            return list;
        }
    }
}
