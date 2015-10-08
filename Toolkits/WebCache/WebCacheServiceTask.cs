using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCache
{
    public class RegexTransformConfigration
    {
        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        [JsonProperty("target_format")]
        public string TargetFormat { get; set; }
    }

    public class HtmlAttributeLookUpConfigration
    {
        [JsonProperty("xpath")]
        public string XPath { get; set; }

        [JsonProperty("attribute")]
        public string Attribute { get; set; }
    }

    public class PaginationConfigration
    {
        [JsonProperty("nav_uri_fmt")]
        public string NavigationUriFormat { get; set; }

        [JsonProperty("start_page")]
        public int StartPage { get; set; }

        [JsonProperty("page_length")]
        public int PageLength { get; set; }

        [JsonProperty("look_up")]
        public HtmlAttributeLookUpConfigration Lookup { get; set; }

        [JsonProperty("basic_uri")]
        public string BasicUri { get; set; }

        [JsonProperty("uri_filter")]
        public string UriFilter { get; set; }

        [JsonProperty("uri_transform")]
        public RegexTransformConfigration UriTransform { get; set; }
    }

    public class PageCacheConfigration
    {
        [JsonProperty("uri_to_path_transform")]
        public RegexTransformConfigration UriToPathTransform { get; set; }

        [JsonProperty("root_folder")]
        public string RootFolder { get; set; }
    }

    public class WebCacheTask
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("encoding")]
        public string Encoding { get; set; }

        [JsonProperty("pagination")]
        public PaginationConfigration Pagination { get; set; }

        [JsonProperty("cache")]
        public PageCacheConfigration Cache { get; set; }
    }
}
