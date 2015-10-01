using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.MSDN;

namespace ThreadDiscovery
{
    public class KeyWordCountAnalyze : IThreadAnalyze
    {
        private readonly string _keyword;

        private const string COUNT_RESULT_FORMAT = "{0}_total_count";

        private readonly string _countResultKey;

        public KeyWordCountAnalyze(string keyword)
        {
            _keyword = keyword.ToLowerInvariant();

            _countResultKey = string.Format(COUNT_RESULT_FORMAT, _keyword);
        }

        public void Analyze(IDictionary<string, object> result, ThreadInfo info)
        {
            var body = info.Messages.First().Body;

            if (
                info.Messages.First().Body.ToLowerInvariant().Contains(_keyword) ||
                info.Title.ToLowerInvariant().Contains(_keyword))
            {
                if (!result.ContainsKey(_countResultKey))
                {
                    result.Add(_countResultKey, 0);
                }

                var count = (int)result[_countResultKey];

                result[_countResultKey] = count + 1;
            }
        }
    }
}
