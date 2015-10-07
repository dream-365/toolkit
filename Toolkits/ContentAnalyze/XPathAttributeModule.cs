using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentAnalyze
{
    public class XPathAttributeModule : IMetadataModule
    {
        private string _xpath;

        private string _pattern;

        private string _metadataKey;

        private string _htmlElementAttribute;

        private const string DEFAULT_METADATA_KEY = "attribute_module_key";

        public void Init(IEnumerable<string> arguments)
        {
            var args = arguments.ToArray();

            if(args.Length < 2)
            {
                throw new ArgumentException("At least 2 arguments for XPathAttributeModule");
            }

            _xpath = args[0];

            _htmlElementAttribute = args[1];

            _pattern = args.Length > 2 ? args[2]
                                       : string.Empty;

            _metadataKey = args.Length > 3 ? args[3]
                                           : DEFAULT_METADATA_KEY;
        }

        public void OnProcess(IDictionary<string, object> metadata, string content)
        {
            var lookup = new HtmlAttributesLookUp();

            var result = lookup.Execute(content, _xpath, _htmlElementAttribute);

            if(null == result || result.Count() < 1)
            {
                Console.WriteLine("no attributes find by xpath: {0}", _xpath);

                return;
            }

            metadata[_metadataKey] = result;
        }
    }
}
