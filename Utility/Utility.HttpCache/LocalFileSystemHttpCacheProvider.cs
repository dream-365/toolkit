using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public class DefaultLocalFileSystemHttpCacheProvider
        : IHttpCacheProvider
    {
        private string _cahcheFolder;

        private IPathResolver _pathResolver;

        public static DefaultLocalFileSystemHttpCacheProvider Current = new DefaultLocalFileSystemHttpCacheProvider();

        DefaultLocalFileSystemHttpCacheProvider()
        {

        }

        public void Configure(string cahcheFolder, IPathResolver pathResolver)
        {
            _cahcheFolder = cahcheFolder;

            _pathResolver = pathResolver;
        }

        public void Init()
        {

        }

        public void Cache(Stream content, Uri contentUri)
        {
            string absolutePath = CalculateAbsolutePath(contentUri);

            var dir = Path.GetDirectoryName(absolutePath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var stream = new FileStream(absolutePath
                , FileMode.CreateNew))
            {
                content.CopyTo(stream);

                stream.Flush();
            }
        }

        private string CalculateAbsolutePath(Uri contentUri)
        {
            var path = _pathResolver.Resolve(contentUri);

            var absolutePath = Path.Combine(_cahcheFolder, path);

            return absolutePath;
        }

        public Stream GetCache(Uri contentUri)
        {
            if(!IsCached(contentUri))
            {
                return null;
            }

            var path = CalculateAbsolutePath(contentUri);

            var mem = new MemoryStream();

            using (var fs = new FileStream(path, FileMode.Open))
            {
                fs.CopyTo(mem);

                mem.Flush();

                mem.Position = 0;
            }

            return mem;
        }

        public bool IsCached(Uri requestUri)
        {
            var path = CalculateAbsolutePath(requestUri);

            return File.Exists(path);
        }
    }
}
