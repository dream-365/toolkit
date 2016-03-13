using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStroage.Client
{
    public class OnlineStroageClient
    {
        private string _endpoint;

        private HttpClient _httpClient;

        public OnlineStroageClient(string endpoint)
        {
            _endpoint = endpoint;

            _httpClient = new HttpClient();
        }

        public async Task UploadStreamAsync(string path, Stream stream)
        {
            var multipartFormDataContent = new MultipartFormDataContent();

            var streamContent = new StreamContent(stream);

            multipartFormDataContent.Add(streamContent);

            var client = new HttpClient();

            var response = await _httpClient.PutAsync(_endpoint + "/blob/" + path, multipartFormDataContent);
        }

        public async Task<Stream> DownloadStrem(string path)
        {
            throw new NotImplementedException();
        }
    }
}
