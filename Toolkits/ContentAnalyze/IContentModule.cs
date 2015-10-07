using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentAnalyze
{
    public interface IContentModule
    {
        string ResultKey { get; }

        void Init(IEnumerable<string> arguments);

        void OnProcess(IDictionary<string, object> result, string content);
    }
}
