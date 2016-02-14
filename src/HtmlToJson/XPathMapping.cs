using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlToJson
{
    public class XPathMapping
    {
        public string XPath { get; set; }

        public SelectType SourceValueType { get; set; }

        public string SourceAttributeName { get; set; }

        public string TargetPropertyName { get; set; }

        public bool IsArray { get; set; }
    }
}
