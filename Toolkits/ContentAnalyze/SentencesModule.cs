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
    public class SentencesModule : IContentModule
    {
        private string _xpath;

        private string _pattern;

        private const string RESULT_KEY = "sentences_module_key";

        public string ResultKey
        {
            get
            {
                return RESULT_KEY;
            }
        }

        public void Init(IEnumerable<string> arguments)
        {
            var args = arguments.ToArray();

            _xpath = args.FirstOrDefault();

            _pattern = args.Length > 1 ? args[1]
                                       : string.Empty;
        }

        public void OnProcess(IDictionary<string, object> result, string content)
        {
            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(content);

            var node = htmlDocument.DocumentNode.SelectSingleNode(_xpath);

            if(node == null)
            {
                Console.WriteLine("No content by xpath: {0}", _xpath);

                return;
            }

            var list = GetOrCreate(result);

            var rawText = node.InnerText;

            if (_pattern != string.Empty)
            {
                CaptureMatch(list, rawText);
            }
            else
            {
                list.Add(node.InnerText);
            }            
        }

        private void CaptureMatch(IList<string> list, string rawText)
        {
            var regex = new Regex(_pattern);

            var matches = regex.Matches(rawText);

            if (matches.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (Match match in matches)
                {
                    sb.Append(match.Value);
                }

                list.Add(sb.ToString());
            }
            else
            {
                Console.WriteLine("No matches in content");
            }
        }

        private IList<string> GetOrCreate(IDictionary<string, object> result)
        {
            if(result.ContainsKey(RESULT_KEY))
            {
                return result[RESULT_KEY] as IList<string>;
            }

            var list = new List<string>();

            result[RESULT_KEY] = list;

            return list;
        }
    }
}
