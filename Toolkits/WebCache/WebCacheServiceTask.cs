using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCache
{
    public class WebCacheServiceTask
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("encoding")]
        public string Encoding { get; set; }

        [JsonProperty("page_nav_uri_fmt")]
        public string PageNavigationUriFormat { get; set; }

        [JsonProperty("html_node_xpath")]
        public string HtmlNodeXPath { get; set; }

        [JsonProperty("html_node_attr")]
        public string HtmlNodeAttribute { get; set; }

        [JsonProperty("basic_uri")]
        public string BasicUri { get; set; }

        [JsonProperty("uri_filter_pattern")]
        public string UriFilterPattern { get; set; }

        [JsonProperty("start_page")]
        public int StartPage { get; set; }

        [JsonProperty("page_length")]
        public int PageLength { get; set; }

        [JsonProperty("cache_provider_uri_pattern")]
        public string CacheProviderUriPattern { get; set; }

        [JsonProperty("cache_provider_path_format")]
        public string CacheProviderPathFormat { get; set; }

        [JsonProperty("cache_provider_root_folder")]
        public string CacheProviderRootFolder { get; set; }
    }
}
