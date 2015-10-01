using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.MSDN;

namespace ThreadDiscovery
{
    public interface IThreadAnalyze
    {
        void Analyze(IDictionary<string, object> result, ThreadInfo info);
    }
}
