using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework
{
    public interface IURIDiscovery
    {
        event Action<string> OnNew;

        void Start();
    }
}
