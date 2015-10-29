using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EasyAnalysis.Backend.Algorithm;
using MongoDB.Driver;
using MongoDB.Bson;

namespace EasyAnalysis.Backend
{
    public class DuplicateDetection
    {
        public async Task RunAsync()
        {
            // {{ parameters:

            string repository = "uwp";

            DateTime start = DateTime.Parse("2015-9-1");

            DateTime end = DateTime.Parse("2015-10-1");

            // }}
            var distance = new LevenshteinDistance();

            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            Framework.ConnectionStringProviders.IConnectionStringProvider mongoDBDataProvider =
                Framework.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(Framework.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MongoDBConnectionStringProvider);

            var client = new MongoClient(mongoDBDataProvider.GetConnectionString(repository));

            var database = client.GetDatabase(repository);

            var dup_detection = database.GetCollection<BsonDocument>("dup_detection");

            using (var connection = new System.Data.SqlClient.SqlConnection(cs))
            {
                var threads = connection.Query(SqlQueryFactory.Instance.Get("get_thread_profile"),
                new
                {
                    repository = repository.ToUpper(),
                    start = start,
                    end = end
                })
                .Select(m => new { Title = m.Title, Id = m.Id })
                .ToList();

               for (int i = 0; i < threads.Count - 1; i++)
               {
                    for(int j = i + 1; j < threads.Count; j++)
                    {
                        var left = (threads[i].Title as string).ToLower();

                        var right = (threads[j].Title as string).ToLower();

                        var percentage = distance.LevenshteinDistancePercent(left, right) * 100;

                        // list all the percentage >= 50%
                        if(percentage >= 50m)
                        {
                            var md5 = Utils.ComputeStringPairMD5Hash(left, right);

                            var count = await dup_detection.Find("{_id: '" + md5 + "'}").CountAsync();

                            if (count == 0)
                            {
                                var dict = new Dictionary<string, object>()
                                {
                                    { "_id", md5 },
                                    { "left", new Dictionary<string, string> {
                                        { "thread_id", threads[i].Id as string },
                                        { "text", left as string}
                                    }},
                                    { "right", new Dictionary<string, string> {
                                        { "thread_id", threads[j].Id as string},
                                        { "text", right as string}
                                    }},
                                    { "percentage", (int)percentage }
                                };

                                var document = new BsonDocument(dict);

                                await dup_detection.InsertOneAsync(document);
                            }
                        }
                    }

                    Console.Write(".");
                }
            }
        }
    }
}
