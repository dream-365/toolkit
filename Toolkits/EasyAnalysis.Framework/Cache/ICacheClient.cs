using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework.Cache
{
    public interface ICacheClient
    {
        Stream GetCache(Uri resource);

        void SetCache(Uri resource, Stream stream);

        bool IsCached(Uri resource);
    }
}
