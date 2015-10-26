using EasyAnalysis.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;

namespace EasyAnalysis.Backend
{
    class Program
    {
        static void Main(string[] args)
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

            var prifiles = connection.Query(SqlQueryFactory.Instance.Get("get_thread_profile"), 
                new {
                    repository = repository.ToUpper(),
                    start = start,
                    end = end
                });

            foreach(dynamic profile in prifiles)
            {
                var tags = connection.Query<string>(SqlQueryFactory.Instance.Get("get_thread_tags"), new { ThreadId = profile.Id });

                var updateAction = Builders<BsonDocument>.Update
                                     .Set("create_on", (DateTime)profile.CreateOn)
                                     .Set("category", profile.Category as string)
                                     .Set("title", profile.Title as string)
                                     .Set("type", profile.Type as string);

                if (tags != null)
                {
                    var tagArray = new BsonArray(tags.Select(m => m.ToLower()).ToList());

                    updateAction = updateAction.Set("tags", tagArray);
                }

                threadProfiles.UpdateOneAsync("{'_id': '" + profile.Id + "'}",
                    updateAction,
                    new UpdateOptions { IsUpsert = true });

                Console.Write(".");
            }
        }

        static void OldFashion()
        {
            string uwpFromId = "b5bc7d55-43e1-4b26-918e-ed1b8d373b3a";

            string repository = "uwp";

            DateTime start = DateTime.Parse("2015-9-1");

            DateTime end = DateTime.Parse("2015-10-1");

            using (var context = new DefaultDbConext())
            {
                var client = new MongoClient("mongodb://app-svr.cloudapp.net:27017/" + repository);

                var database = client.GetDatabase(repository);

                var threadProfiles = database.GetCollection<BsonDocument>("thread_profiles");

                context.Threads
                       .Include("Tags")
                       .Where(m => m.ForumId.Equals(uwpFromId))
                       .Where(m => m.CreateOn >= start && m.CreateOn < end)
                       .ToList()
                       .ForEach((thread) => {
                           var tags = new BsonArray(thread.Tags.Select(m => m.Name.ToLower()).ToList());

                           var updateAction = Builders<BsonDocument>.Update
                                                .Set("tags", tags)
                                                .Set("title", thread.Title);

                           threadProfiles.UpdateOneAsync("{'_id': '" + thread.Id + "'}",
                               updateAction,
                               new UpdateOptions { IsUpsert = true });

                           Console.Write(".");
                       });

            }
        }

    }
}
