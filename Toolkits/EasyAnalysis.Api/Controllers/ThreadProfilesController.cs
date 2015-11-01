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
    public class ThreadProfilesController : ApiController
    {
        [Route("api/ThreadProfiles/relatedtags")]
        public async Task<HttpResponseMessage> GetRelatedTags(
            [FromUri] string repository,
            [FromUri] DateTime? start,
            [FromUri] DateTime? end,
            [FromUri] string tags)
        {
            EasyAnalysis.Framework.ConnectionStringProviders.IConnectionStringProvider mongoDBCSProvider =
                EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MongoDBConnectionStringProvider);
            var client = new MongoClient(mongoDBCSProvider.GetConnectionString(repository));//"mongodb://app-svr.cloudapp.net:27017/" + repository);

            var database = client.GetDatabase(repository);

            var threadProfiles = database.GetCollection<BsonDocument>("thread_profiles");

            var builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = "{}";

            List<string> wellKnownTags = new List<string> {
                "uwp",
                "wp8.1",
                "w8.1",
                "u8.1",
                "wpsl",
                "c#",
                "c++",
                "vb",
                "javascript"
            };

            if (start.HasValue && end.HasValue)
            {
                filter = filter & builder.Gte("create_on", start) & builder.Lt("create_on", end);
            }

            if (!string.IsNullOrWhiteSpace(tags))
            {
                var array = tags.Split('|');

                wellKnownTags.AddRange(array);

                wellKnownTags = wellKnownTags.Distinct().ToList();

                filter = filter & builder.All("tags", array);
            }

            var result = await threadProfiles.Aggregate()
                .Match(filter)
                .Project("{ _id: 0, tags: 1 }")
                .Unwind("tags")
                .Group("{ _id: '$tags', freq: { $sum: 1 } }")
                .Project("{ _id: 0, name: '$_id', freq: 1 }")
                .Sort("{ freq: -1 }")
                .ToListAsync();

            IList<BsonDocument> resultToRemove = new List<BsonDocument>();

            foreach(var item in result)
            {
                if(wellKnownTags.Contains(item.GetValue("name").AsString))
                {
                    resultToRemove.Add(item);
                }
            }

            foreach(var itemToRemove in resultToRemove)
            {
                result.Remove(itemToRemove);
            }

            result = result.Take(20).ToList();

            var response = Request.CreateResponse();

            response.StatusCode = HttpStatusCode.OK;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            response.Content = new StringContent(result.ToJson(jsonWriterSettings), Encoding.UTF8, "application/json");

            return response;
        }

        // GET: api/ThreadProfiles
        public async Task<HttpResponseMessage> Get(
            [FromUri] string repository,
            [FromUri] int page,
            [FromUri] int length,
            [FromUri] DateTime? start, 
            [FromUri] DateTime? end,
            [FromUri] string tags)
        {
            EasyAnalysis.Framework.ConnectionStringProviders.IConnectionStringProvider mongoDBCSProvider =
                   EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MongoDBConnectionStringProvider);
            var client = new MongoClient(mongoDBCSProvider.GetConnectionString(repository));//"mongodb://app-svr.cloudapp.net:27017/" + repository);

            var database = client.GetDatabase(repository);

            var threadProfiles = database.GetCollection<BsonDocument>("thread_profiles");

            var builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = "{}";

            if (start.HasValue && end.HasValue)
            {
                filter = filter & builder.Gte("create_on", start) & builder.Lt("create_on", end);
            }

            if (!string.IsNullOrWhiteSpace(tags))
            {
                var array = tags.Split('|');

                filter = filter & builder.All("tags", array);
            }

            var result = await threadProfiles
                .Find(filter)
                .Sort("{create_on: -1}")
                .Skip((page - 1) * length)
                .Limit(length)
                .ToListAsync();

            var response = Request.CreateResponse();

            response.StatusCode = HttpStatusCode.OK;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            response.Content = new StringContent(result.ToJson(jsonWriterSettings), Encoding.UTF8, "application/json");

            return response;
        }

        // GET: api/ThreadProfiles/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ThreadProfiles
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ThreadProfiles/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ThreadProfiles/5
        public void Delete(int id)
        {
        }
    }
}
