using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public class HttpContentCacheClient
    {
        private readonly HttpClient _internalClient;

        private IHttpCacheProvider _cacheProvider;

        HttpContentCacheClient()
        {
            _internalClient = new HttpClient();

            _cacheProvider = DefaultLocalFileSystemHttpCacheProvider.Current;
        }
 
        public Task<System.IO.Stream> GetStreamAsync(Uri requestUri)
        {
            return _internalClient.GetStreamAsync(requestUri);
        }
    }
}
