using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyAnalysis.Infrastructure.Cache
{
    class UrlPattern
    {
        private readonly Regex _pattern;

        private readonly string _expression;

        public UrlPattern(Regex pattern, string expression)
        {
            _pattern = pattern;

            _expression = expression;
        }

        public string Resolve(Uri contentUri)
        {
            var host = contentUri.Host;

            var match = _pattern.Match(contentUri.AbsolutePath);

            if (!match.Success)
            {
                throw new ArgumentException(
                    string.Format("The argument '{0}' does not match the pattern {1}",
                    contentUri.AbsolutePath,
                    _pattern));
            }

            var groupValueList = new List<string>();

            foreach (Group group in match.Groups)
            {
                groupValueList.Add(group.Value);
            }

            var fmtText = string.Format(_expression, groupValueList.ToArray());

            return System.IO.Path.Combine(host, fmtText);
        }
    }
}
