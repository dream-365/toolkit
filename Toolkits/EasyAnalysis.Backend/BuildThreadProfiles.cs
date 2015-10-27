using Dapper;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Backend
{
    public class BuildThreadProfiles
    {
        public void Run()
        {
            // {{ parameters:

            string repository = "uwp";

            DateTime start = DateTime.Parse("2015-9-1");

            DateTime end = DateTime.Parse("2015-10-1");

            // }}

            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var connection = new System.Data.SqlClient.SqlConnection(cs);

            var client = new MongoClient("mongodb://app-svr.cloudapp.net:27017/" + repository);

            var database = client.GetDatabase(repository);

            var threadProfiles = database.GetCollection<BsonDocument>("thread_profiles");

            var threads = database.GetCollection<BsonDocument>("sep_threads");

            var prifiles = connection.Query(SqlQueryFactory.Instance.Get("get_thread_profile"),
                new
                {
                    repository = repository.ToUpper(),
                    start = start,
                    end = end
                });

            foreach (dynamic profile in prifiles)
            {
                var tags = connection.Query<string>(SqlQueryFactory.Instance.Get("get_thread_tags"), new { ThreadId = profile.Id });

                var key = Builders<BsonDocument>.Filter.Eq("id", (profile.Id as string).Trim());

                var task = threads.Find(key).Project("{url: 1, messages: 1, _id: 0}").SingleOrDefaultAsync();

                task.Wait();

                var match = task.Result;

                var updateAction = Builders<BsonDocument>.Update
                                     .Set("create_on", (DateTime)profile.CreateOn)
                                     .Set("category", profile.Category as string)
                                     .Set("title", profile.Title as string)
                                     .Set("type", profile.Type as string);

                if (match != null)
                {
                    var html = match.GetValue("messages")
                                    .AsBsonArray
                                    .FirstOrDefault()
                                    .AsBsonDocument
                                    .GetValue("body").AsString;

                    var document = new HtmlAgilityPack.HtmlDocument();

                    document.LoadHtml(html);

                    var text = document.DocumentNode.InnerText;

                    var excerpt = text.Substring(0, Math.Min(256, text.Length));

                    updateAction = updateAction.Set("url", match.GetValue("url").AsString)
                                               .Set("excerpt", excerpt);
                }

                if (tags != null)
                {
                    var tagArray = new BsonArray(tags.Select(m => m.ToLower()).ToList());

                    updateAction = updateAction.Set("tags", tagArray);
                }

                threadProfiles.UpdateOneAsync("{'_id': '" + (profile.Id as string).Trim() + "'}",
                    updateAction,
                    new UpdateOptions { IsUpsert = true });

                Console.Write(".");
            }
        }
    }
}
