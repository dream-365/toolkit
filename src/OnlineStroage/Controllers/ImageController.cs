using OnlineStroage.Services;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace OnlineStroage.Controllers
{
    public class ImageController : ApiController
    {
        private ImageResizeService _imageResizeService = new ImageResizeService();

        public HttpResponseMessage Get(string pathInfo)
        {
            if(string.IsNullOrEmpty(pathInfo))
            {
                var message = new HttpResponseMessage(HttpStatusCode.OK);

                message.Content = new StringContent("the string path can not be null");

                return message;
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            var stream = LocalBlob.Inst.Get(pathInfo);

            var extention = Path.GetExtension(pathInfo).ToLower();

            string mediaTypeHeaderValue = string.Empty;

            switch(extention)
            {
                case ".jpg": mediaTypeHeaderValue = "image/jpg"; break;
                default: mediaTypeHeaderValue = "application/octet-stream"; break;
            }

            result.Content = new StreamContent(stream);

            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue(mediaTypeHeaderValue);

            return result;
        }

        public HttpResponseMessage Get(string pathInfo, int? width, int? height)
        {
            if (string.IsNullOrEmpty(pathInfo))
            {
                var message = new HttpResponseMessage(HttpStatusCode.OK);

                message.Content = new StringContent("the string path can not be null");

                return message;
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            var stream = LocalBlob.Inst.Get(pathInfo);

            var thubmailStream = _imageResizeService.Resize(stream, width, height);

            stream.Close();

            var extention = Path.GetExtension(pathInfo).ToLower();

            string mediaTypeHeaderValue = string.Empty;

            switch (extention)
            {
                case ".jpg": mediaTypeHeaderValue = "image/jpg"; break;
                default: mediaTypeHeaderValue = "application/octet-stream"; break;
            }

            result.Content = new StreamContent(thubmailStream);

            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue(mediaTypeHeaderValue);

            return result;
        }
    }
}
