using EasyAnalysis.Backend.Algorithm;
using EasyAnalysis.Framework.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Driver;
using MongoDB.Bson;

namespace EasyAnalysis.Backend.Actions
{
    public class DetectDuplicates : IAction
    {
        private const string QUERY_GET_THREAD_PROFILE = "get_thread_profile";

        private IConnectionStringProvider _mssqlconnectionStringProvider;
        private IConnectionStringProvider _mongoconnectionStringProvider;

        public string Description
        {
            get
            {
                return "detect the duplicates among threads and export to mongodb";
            }
        }

        public DetectDuplicates(IConnectionStringProvider mssqlconnectionStringProvider, IConnectionStringProvider mongoconnectionStringProvider)
        {
            _mssqlconnectionStringProvider = mssqlconnectionStringProvider;
            _mongoconnectionStringProvider = mongoconnectionStringProvider;
        }


        public async Task RunAsync(string[] args)
        {
            // {{ parameters:

            string repository = args[0];

            DateTime start = DateTime.Parse(args[1]);

            DateTime end = DateTime.Parse(args[2]);

            string targetCollection = args[3];

            // }}

            var distance = new LevenshteinDistance();

            var client = new MongoClient(_mongoconnectionStringProvider.GetConnectionString(repository));

            var database = client.GetDatabase(repository);

            var dup_detection = database.GetCollection<BsonDocument>(targetCollection);

            using (var connection = new System.Data.SqlClient.SqlConnection(_mssqlconnectionStringProvider.GetConnectionString()))
            {
                var threads = connection.Query(SqlQueryFactory.Instance.Get(QUERY_GET_THREAD_PROFILE),
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
                    for (int j = i + 1; j < threads.Count; j++)
                    {
                        var left = (threads[i].Title as string).ToLower();

                        var right = (threads[j].Title as string).ToLower();

                        var percentage = distance.LevenshteinDistancePercent(left, right) * 100;

                        // list all the percentage >= 50%
                        if (percentage >= 50m)
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
                }
            }
        }
    }
}
