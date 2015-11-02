using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Infrastructure.Cache
{
    public class LocalFileCacheClient : ICacheClient
    {
        Stream ICacheClient.GetCache(Uri resource)
        {
            throw new NotImplementedException();
        }

        void ICacheClient.SetCache(Uri resource, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
