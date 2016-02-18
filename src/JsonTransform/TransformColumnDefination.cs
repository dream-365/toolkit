using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTransform
{
    public class TransformColumnDefination
    {
        public string SourcePropertyName { get; set; }

        public string TargetPropertyName { get; set; }

        public string Transform { get; set; }
    }
}
