using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public class IndexPageNavigation : IWebPageNavigation
    {
        private readonly string _uriFormat;

        private HttpClient _httpClient;

        private int _currentIndex = 0;

        public IndexPageNavigation(string uriFormat)
        {
            _uriFormat = uriFormat;

            _httpClient = new HttpClient();

            var headers = _httpClient.DefaultRequestHeaders;

            headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            headers.Add("Cache-Control", "keep-alive");
            headers.Add("Accept-Language", "en-US,en;q=0.8,zh-Hans-CN;q=0.5,zh-Hans;q=0.3");
            headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0");
        }

        public Task<Stream> GetAsync()
        {
            if(_currentIndex == 0)
            {
                return null;
            }

            var uriString = string.Format(_uriFormat, _currentIndex);

            var uri = new Uri(uriString);

            return _httpClient.GetStreamAsync(uri);
        }

        public void NavigateTo(int pageIndex)
        {
            _currentIndex = pageIndex;
        }

        public void Next()
        {
            _currentIndex++;
        }

        public void Previous()
        {
            if(_currentIndex > 1)
            {
                _currentIndex--;
            }
        }
    }
}
