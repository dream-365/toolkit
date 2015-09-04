using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace EasyAnalysis
{
    public class Utils
    {
        public static IEnumerable<string> DetectTagsFromTitle(string title)
        {
            var result = new List<string>();

            var pattern = new Regex(@"\[([^\]]+)\]");

            var collection = pattern.Matches(title);

            foreach(Match match in collection)
            {
                var value = match.Groups[1].Value;

                result.Add(value);
            }

            return result;
        }
    }
}