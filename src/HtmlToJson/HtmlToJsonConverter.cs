using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlToJson
{
    public class HtmlToJsonConverter
    {
        private IEnumerable<XPathMapping> _mappings;

        public HtmlToJsonConverter(IEnumerable<XPathMapping> mappings)
        {
            _mappings = mappings;
        }

        public string Convert(string html)
        {
            var dict = new Dictionary<string, object>();

            var selector = new XPathSelector(html);

            foreach(var mp in _mappings)
            {
                var values = selector.Query(
                    xpath: mp.XPath, 
                    selectType: mp.SourceValueType, 
                    attributeName: mp.SourceAttributeName);

                if(!mp.IsArray)
                {
                    dict.Add(mp.TargetPropertyName, values.FirstOrDefault());
                }
                else
                {
                    dict.Add(mp.TargetPropertyName, values);
                }  
            }

            return JsonConvert.SerializeObject(value:dict, formatting:Formatting.Indented);
        }
    }
}
