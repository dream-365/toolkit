using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTransform
{
    public class FunctionProvider
    {
        public ITextTransformFunction Get(string name)
        {
            if(name == "replace")
            {
                return new ReplaceFunction();
            }else if(name == "regex")
            {
                return new RegexFormatFunction();
            }

            return null;
        }
    }
}
