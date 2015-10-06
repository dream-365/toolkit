using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public interface IUriDiscovery
    {
        IEnumerable<Uri> Discover(string html);
    }
}
