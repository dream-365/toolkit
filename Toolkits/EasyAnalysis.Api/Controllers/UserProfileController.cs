using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
                filter = filter & builder.Regex("display_name", BsonRegularExpression.Create(new Regex("^" + display_name, RegexOptions.IgnoreCase)));
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

        private BsonArray MergeAndCleanArray(BsonArray ba)
        {
            try
            {
                BsonArray newArray = new BsonArray();
                foreach (var item in ba)
                {
                    bool isContainTheSame = false;
                    int index = 0;
                    foreach (var newitem in newArray)
                    {
                        if (newitem["name"].AsString.ToLower() == item["name"].AsString.ToLower())
                        {
                            isContainTheSame = true;
                            break;
                        }
                        index++;
                    }

                    if (isContainTheSame)
                    {
                        BsonValue t = newArray.Values.Where(x => x["name"].AsString.ToLower() == item["name"].AsString.ToLower()).FirstOrDefault();
                        t["count"] = t["count"].AsDouble + item["count"].AsDouble;
                    }
                    else
                    {
                        newArray.Add(item);
                    }
                }
                return newArray;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        List<string> wellKnownPlatformTags = new List<string> {
                "uwp",
                "wp8.1",
                "w8.1",
                "u8.1",
                "wpsl"
            };

        List<string> wellKnownLanguageTags = new List<string> {
                "c#",
                "c++",
                "vb",
                "javascript"
            };

        // GET: api/UserProfiles
        public async Task<HttpResponseMessage> Get(
            [FromUri] string repository,
            [FromUri] string display_name,
            [FromUri] List<string> months
            )
        {
            EasyAnalysis.Framework.ConnectionStringProviders.IConnectionStringProvider mongoDBCSProvider =
                   EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MongoDBConnectionStringProvider);
            var client = new MongoClient(mongoDBCSProvider.GetConnectionString(repository));

            var database = client.GetDatabase(repository);

            var userProfiles = database.GetCollection<BsonDocument>("asker_activities");

            var builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = "{}";
            
            if (months.Count > 0)
            {
                filter = filter & builder.In("month", months);
            }

            if (!string.IsNullOrEmpty(display_name))
            {
                filter = filter & builder.Regex("display_name", BsonRegularExpression.Create(new Regex("^" + display_name, RegexOptions.IgnoreCase)));
            }

            IList<BsonDocument> result = new List<BsonDocument>();

            var cursor = await userProfiles.Aggregate()
                .Match(filter)
                .Group("{_id: '$id', asked: {$sum:'$total'}, answered: {$sum:'$answered'}, marked: {$sum: '$marked'}, months:{$push:'$month' }}")
                .Project("{_id: 0, id: '$_id', asked: 1, answered: 1, marked: 1, months:1}")
                //.Match("{asked: {$gte:7}")
                .Sort("{ asked: -1 }")
                .ToListAsync();

            foreach (var data in cursor)
            {
                //BsonArray _tags = new BsonArray();
                BsonArray _platform_tags = new BsonArray();
                BsonArray _language_tags = new BsonArray();
                BsonArray _other_tags = new BsonArray();
                BsonArray _threads = new BsonArray();
                BsonArray _marked_threads = new BsonArray();

                FilterDefinition<BsonDocument> filterid = filter & builder.Eq("id", data.GetValue("id").AsString);

                var op = await userProfiles
                    .Find(filterid)
                    .ToListAsync();

                foreach (var item in op)
                {
                    data["display_name"] = item.GetValue("display_name").AsString;

                    // Unpacking Tags
                    foreach (var tag in item.GetValue("tags").AsBsonArray)
                    {
                        if (wellKnownPlatformTags.Contains(tag["name"].AsString.ToLower()))
                            _platform_tags.Add(tag);
                        else if (wellKnownLanguageTags.Contains(tag["name"].AsString.ToLower()))
                            _language_tags.Add(tag);
                        else
                            _other_tags.Add(tag);
                        //_tags.Add(tag);
                    }
                    data["platform_tags"] = MergeAndCleanArray(_platform_tags);
                    data["language_tags"] = MergeAndCleanArray(_language_tags);
                    data["other_tags"] = MergeAndCleanArray(_other_tags);
                    //data["tags"] = _tags;

                    ////Unpacking Threads
                    //foreach (var thread in item.GetValue("threads").AsBsonArray)
                    //{
                    //    _threads.Add(thread);
                    //}

                    ////Unpacking Marked Threads
                    //if (item.GetValue("marked_threads")!=null)
                    //{
                    //    foreach (var thread in item.GetValue("marked_threads").AsBsonArray)
                    //    {
                    //        _marked_threads.Add(thread);
                    //    }
                    //}

                    //data["threads"] = _threads;
                    //data["marked_threads"] = _marked_threads;
                }

                result.Add(data);
            }

            var response = Request.CreateResponse();

            response.StatusCode = HttpStatusCode.OK;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            response.Content = new StringContent(result.ToJson(jsonWriterSettings), Encoding.UTF8, "application/json");

            return response;
        }

        // GET: api/UserProfiles
        public async Task<HttpResponseMessage> Get(
            [FromUri] string repository,
            [FromUri] int length,
            [FromUri] List<string> months
            )
        {
            EasyAnalysis.Framework.ConnectionStringProviders.IConnectionStringProvider mongoDBCSProvider =
                   EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MongoDBConnectionStringProvider);
            var client = new MongoClient(mongoDBCSProvider.GetConnectionString(repository));

            var database = client.GetDatabase(repository);

            var userProfiles = database.GetCollection<BsonDocument>("asker_activities");

            var builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = "{}";

            if (months.Count > 0)
            {
                filter = filter & builder.In("month", months);
            }
            
            IList<BsonDocument> result = new List<BsonDocument>();

            var cursor = await userProfiles.Aggregate()
                .Match(filter)
                .Group("{_id: '$id', asked: {$sum:'$total'}, answered: {$sum:'$answered'}, marked: {$sum: '$marked'}, months:{$push:'$month' }}")
                .Project("{_id: 0, id: '$_id', asked: 1, answered: 1, marked: 1, months:1}")
                //.Match("{asked: {$gte:7}")
                .Sort("{ asked: -1 }")
                .Limit(length)
                .ToListAsync();

            foreach (var data in cursor)
            {
                //BsonArray _tags = new BsonArray();
                BsonArray _platform_tags = new BsonArray();
                BsonArray _language_tags = new BsonArray();
                BsonArray _other_tags = new BsonArray();
                BsonArray _threads = new BsonArray();
                BsonArray _marked_threads = new BsonArray();

                FilterDefinition<BsonDocument> filterid = filter & builder.Eq("id", data.GetValue("id").AsString);

                var op = await userProfiles
                    .Find(filterid)
                    .ToListAsync();

                foreach (var item in op)
                {
                    data["display_name"] = item.GetValue("display_name").AsString;

                    // Unpacking Tags
                    foreach (var tag in item.GetValue("tags").AsBsonArray)
                    {
                        if (wellKnownPlatformTags.Contains(tag["name"].AsString.ToLower()))
                            _platform_tags.Add(tag);
                        else if (wellKnownLanguageTags.Contains(tag["name"].AsString.ToLower()))
                            _language_tags.Add(tag);
                        else
                            _other_tags.Add(tag);
                    }
                    data["platform_tags"] = MergeAndCleanArray(_platform_tags);
                    data["language_tags"] = MergeAndCleanArray(_language_tags);
                    data["other_tags"] = MergeAndCleanArray(_other_tags);
                }

                result.Add(data);
            }

            var response = Request.CreateResponse();

            response.StatusCode = HttpStatusCode.OK;

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            response.Content = new StringContent(result.ToJson(jsonWriterSettings), Encoding.UTF8, "application/json");

            return response;
        }
    }
}
