using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public class DefaultPathResolver : IPathResolver
    {
        public string Resolve(Uri contentUri)
        {
            var host = contentUri.Host;

            var path = contentUri.AbsolutePath;

            var query = contentUri.Query;

            var resolvedPath = path.Replace("/", "#");

            var resolvedQuery = query.Replace("?", "#QM#");

            return System.IO.Path.Combine(host, resolvedPath, resolvedQuery);
        }
    }
}
