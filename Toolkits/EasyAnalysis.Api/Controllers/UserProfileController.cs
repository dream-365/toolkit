using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyAnalysis.Api.Controllers
{
    [EnableCors("*", "*", "*")]
    public class UserProfileController : ApiController
    {
        // GET: api/UserProfiles
        public async Task<HttpResponseMessage> Get(
            [FromUri] string repository,
            [FromUri] string display_name,
            [FromUri] string month
            )
        {
            EasyAnalysis.Framework.ConnectionStringProviders.IConnectionStringProvider mongoDBCSProvider =
                   EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MongoDBConnectionStringProvider);
            var client = new MongoClient(mongoDBCSProvider.GetConnectionString(repository));

            var database = client.GetDatabase(repository);

            var userProfiles = database.GetCollection<BsonDocument>("asker_activities");

            var builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = "{}";

            if (!string.IsNullOrEmpty(display_name))
            {
                filter = filter & builder.Eq("display_name", display_name);
            }

            if (!string.IsNullOrEmpty(month))
            {
                filter = filter & builder.Eq("month", month);
            }
            
            var result = await userProfiles
                .Find(filter)
                .Sort("{ total: -1 }")
                .ToListAsync();

            var response = Request.CreateResponse();

            response.StatusCode = HttpStatusCode.OK;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            response.Content = new StringContent(result.ToJson(jsonWriterSettings), Encoding.UTF8, "application/json");

            return response;
        }

        // GET: api/UserProfiles
        public async Task<HttpResponseMessage> Get(
            [FromUri] string repository,
            [FromUri] string month,
            [FromUri] int length
            )
        {
            EasyAnalysis.Framework.ConnectionStringProviders.IConnectionStringProvider mongoDBCSProvider =
                   EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MongoDBConnectionStringProvider);
            var client = new MongoClient(mongoDBCSProvider.GetConnectionString(repository));

            var database = client.GetDatabase(repository);

            var userProfiles = database.GetCollection<BsonDocument>("asker_activities");

            var builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = "{}";

            if (!string.IsNullOrEmpty(month))
            {
                filter = filter & builder.Eq("month", month);
            }

            var result = await userProfiles
                .Find(filter)
                .Sort("{ total: -1 }")
                .Limit(length)
                .ToListAsync();

            var response = Request.CreateResponse();

            response.StatusCode = HttpStatusCode.OK;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            response.Content = new StringContent(result.ToJson(jsonWriterSettings), Encoding.UTF8, "application/json");

            return response;
        }
    }
}
