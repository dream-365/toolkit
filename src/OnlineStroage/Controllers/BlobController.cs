using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace OnlineStroage.Controllers
{
    public class BlobController : ApiController
    {
        public HttpResponseMessage Get(string pathInfo)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            var stream = LocalBlob.Inst.Get(pathInfo);

            result.Content = new StreamContent(stream);

            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }

        public async Task<string> Put(string pathInfo)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = await Request.Content.ReadAsMultipartAsync();

            var content = provider.Contents.FirstOrDefault();

            using (var stream = await content.ReadAsStreamAsync())
            {
                LocalBlob.Inst.Set(pathInfo, stream);
            }

            return string.Empty;
        }
    }
}
