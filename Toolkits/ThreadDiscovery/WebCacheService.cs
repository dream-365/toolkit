using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility.HttpCache;

namespace ThreadDiscovery
{
    public class WebCacheService
    {
        public async Task RunAsync(WebCacheServiceTask task)
        {
            var frame = new IndexPageNavigation(task.PageNavigationUriFormat);

            var encoding = Encoding.GetEncoding(task.Encoding);

            var cacheProvider = DefaultLocalFileSystemHttpCacheProvider.Current;

            Regex urlRegex = new Regex(task.CacheProviderUriPattern);

            var resolver = new RegexPathResolver(urlRegex, task.CacheProviderPathFormat);

            cacheProvider.Configure(task.CacheProviderRootFolder, resolver);

            var discovery = new XPathUriDiscovery(task.HtmlNodeXPath,
                task.HtmlNodeAttribute,
                string.IsNullOrEmpty(task.UriFilterPattern)
                ? null
                : new Regex(task.UriFilterPattern));

            for (int i = task.StartPage; i < task.StartPage + task.PageLength; i++)
            {
                frame.NavigateTo(i);

                string text = string.Empty;

                using (var content = await frame.GetAsync())
                using (var sr = new StreamReader(content, encoding))
                {
                    text = await sr.ReadToEndAsync();
                }

                var uris = discovery.Discover(text);

                foreach(var uri in uris)
                {
                    Console.WriteLine(uri);
                }
            }
        }
    }
}
