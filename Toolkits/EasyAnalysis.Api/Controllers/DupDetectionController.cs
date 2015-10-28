using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyAnalysis.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class DupDetectionController : ApiController
    {
        // GET: api/DupDetection
        public async Task<HttpResponseMessage> Get(
            [FromUri] string repository)
        {
            var client = new MongoClient("mongodb://app-svr.cloudapp.net:27017/" + repository);

            var database = client.GetDatabase(repository);

            var dupDetection = database.GetCollection<BsonDocument>("dup_detection");

            var result = await dupDetection
                                .Find("{}")
                                .Sort("{percentage: -1}")
                                .ToListAsync();

            var response = Request.CreateResponse();

            response.StatusCode = HttpStatusCode.OK;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            response.Content = new StringContent(result.ToJson(jsonWriterSettings), Encoding.UTF8, "application/json");

            return response;
        }

        // GET: api/DupDetection/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DupDetection
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/DupDetection/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DupDetection/5
        public void Delete(int id)
        {
        }
    }
}
