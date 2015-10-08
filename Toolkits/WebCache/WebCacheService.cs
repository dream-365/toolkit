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
        public async Task RunAsync(WebCacheTask task)
        {
            var httpClient = new HttpClient();

            Utility.SetGeneralHttpHeaders(httpClient);

            var frame = new IndexPageNavigation(task.Pagination.NavigationUriFormat);

            var encoding = Encoding.GetEncoding(task.Encoding);

            var cacheProvider = DefaultLocalFileSystemHttpCacheProvider.Current;

            Regex urlRegex = new Regex(task.Cache.UriToPathTransform.Pattern);

            var resolver = new RegexPathResolver(urlRegex, task.Cache.UriToPathTransform.TargetFormat);

            cacheProvider.Configure(task.Cache.RootFolder, resolver);

            var discovery = new XPathUriDiscovery(task.Pagination.Lookup.XPath,
                task.Pagination.Lookup.Attribute,
                string.IsNullOrEmpty(task.Pagination.UriFilter)
                ? null
                : new Regex(task.Pagination.UriFilter),
                task.Pagination.BasicUri
                );

            for (int i = task.Pagination.StartPage; 
                 i < task.Pagination.StartPage + task.Pagination.PageLength; 
                 i++)
            {
                frame.NavigateTo(i);

                string text = string.Empty;

                using (var content = await frame.GetAsync())
                using (var sr = new StreamReader(content, encoding))
                {
                    text = await sr.ReadToEndAsync();
                }

                var uris = discovery.Discover(text);

                uris = TransformIfNeeded(task, uris);

                await CacheAll(httpClient, cacheProvider, uris);
            }
        }

        private static async Task CacheAll(HttpClient httpClient, DefaultLocalFileSystemHttpCacheProvider cacheProvider, IEnumerable<Uri> uris)
        {
            foreach (var uri in uris)
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
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static IEnumerable<Uri> TransformIfNeeded(WebCacheTask task, IEnumerable<Uri> uris)
        {
            if (task.Pagination.UriTransform != null)
            {
                var tfg = task.Pagination.UriTransform;

                var regex = new Regex(tfg.Pattern);

                var tranformedUris = new List<Uri>();

                uris.ToList().ForEach((uri) =>
                {
                    string output;
                    if (TryTransform(regex, tfg.TargetFormat, uri.ToString(), out output))
                    {
                        tranformedUris.Add(new Uri(output));
                    }
                });

                uris = tranformedUris;
            }

            return uris;
        }

        private static bool TryTransform(Regex pattern, 
            string expression, 
            string value,
            out string output)
        {
            var match = pattern.Match(value);

            if(!match.Success)
            {
                output = string.Empty;

                return false;
            }

            var groupValueList = new List<string>();

            foreach (Group group in match.Groups)
            {
                groupValueList.Add(group.Value);
            }

            output = string.Format(expression, groupValueList.ToArray());

            return true;
        }
    }
}
