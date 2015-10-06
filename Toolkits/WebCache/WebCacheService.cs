using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility.HttpCache;

namespace WebCache
{
    public class WebCacheService
    {
        public async Task RunAsync(WebCacheServiceTask task)
        {
            var httpClient = new HttpClient();

            var headers = httpClient.DefaultRequestHeaders;

            headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            headers.Add("Cache-Control", "keep-alive");
            headers.Add("Accept-Language", "en-US,en;q=0.8,zh-Hans-CN;q=0.5,zh-Hans;q=0.3");
            headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0");

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
                : new Regex(task.UriFilterPattern),
                task.BasicUri
                );

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
                    
                    try
                    {
                        if (!cacheProvider.IsCached(uri))
                        {
                            using (var stream = await httpClient.GetStreamAsync(uri))
                            {
                                cacheProvider.Cache(stream, uri);
                            }

                            Console.WriteLine(string.Format("[New]{0}", uri));
                        }
                        else
                        {
                            Console.WriteLine(string.Format("[Cached]{0}", uri));
                        }
                    }catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
