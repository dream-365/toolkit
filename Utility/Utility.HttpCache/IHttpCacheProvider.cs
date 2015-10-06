using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public interface IHttpCacheProvider
    {
        bool IsCached(Uri requestUri);

        void Cache(Stream content, Uri contentUri);

        Stream GetCache(Uri contentUri);
    }
}
