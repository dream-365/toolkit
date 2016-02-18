using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextTransform
{
    public class ReplaceFunction : ITextTransformFunction
    {
        public string Call(string text, string [] parameters)
        {
            EnsureArgument.LengthEqual(parameters, 2);

            var oldValue = parameters[0];

            var newValue = parameters[1];

            return text.Replace(oldValue, newValue);
        }
    }
}
