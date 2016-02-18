using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextTransform;

namespace JsonTransform
{
    public class ColumnMap
    {
        public string SourcePropertyName { get; set; }

        public string TargetPropertyName { get; set; }

        public Func<string, string> Transform { get; set; }
    }
}
