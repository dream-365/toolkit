using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextTransform
{
    public class RegexFormatFunction : ITextTransformFunction
    {
        public string Call(string value, string[] parameters)
        {
            EnsureArgument.LengthEqual(parameters, 2);

            var pattern = new Regex(parameters[0]);

            var expression = parameters[1];

            var match = pattern.Match(value);

            if (!match.Success)
            {
                return string.Empty;
            }

            var groupValueList = new List<string>();

            foreach (Group group in match.Groups)
            {
                groupValueList.Add(group.Value);
            }

            return string.Format(expression, groupValueList.ToArray());
        }
    }
}
