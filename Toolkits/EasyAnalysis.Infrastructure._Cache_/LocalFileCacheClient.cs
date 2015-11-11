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
        private readonly LocalFileCacheServcie _service;

        internal LocalFileCacheClient(LocalFileCacheServcie service)
        {
            _service = service;
        }

        public bool IsCached(Uri resource)
        {
            throw new NotImplementedException();
        }

        Stream ICacheClient.GetCache(Uri resource)
        {
            return _service.GetCache(resource);
        }

        void ICacheClient.SetCache(Uri resource, Stream stream)
        {
            _service.CacheOrUpdate(resource, stream);
        }
    }
}
