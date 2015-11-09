using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace EasyAnalysis.Backend.Actions
{
    public class BuildThreadProfiles : IAction
    {
        private IConnectionStringProvider _mssqlconnectionStringProvider;
        private IConnectionStringProvider _mongoconnectionStringProvider;

        public string Description
        {
            get
            {
                return "Build the thread profiles";
            }
        }

        public BuildThreadProfiles(IConnectionStringProvider mssqlconnectionStringProvider, IConnectionStringProvider mongoconnectionStringProvider)
        {
            _mssqlconnectionStringProvider = mssqlconnectionStringProvider;
            _mongoconnectionStringProvider = mongoconnectionStringProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">[repository] [start date] [end date] [thread collection name] [target collection name]</param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            string repository = args[0];

            DateTime start = DateTime.Parse(args[1]);

            DateTime end = DateTime.Parse(args[2]);

            string threadCollectionName = args[3];

            string targetCollectionName = args[4];

            using (var connection = new SqlConnection(_mssqlconnectionStringProvider.GetConnectionString()))
            {
                var mongoClient = new MongoClient(_mongoconnectionStringProvider.GetConnectionString(repository));

                var database = mongoClient.GetDatabase(repository);

                var threadProfileCollection = database.GetCollection<BsonDocument>(targetCollectionName);

                var threadCollection = database.GetCollection<BsonDocument>(threadCollectionName);

                var prifiles = connection.Query(SqlQueryFactory.Instance.Get("get_thread_profile"),
                    new
                    {
                        repository = repository.ToUpper(),
                        start = start,
                        end = end
                    });

                foreach (dynamic profile in prifiles)
                {
                    await FillData(profile, connection, threadCollection, threadProfileCollection);
                }
            }
        }

        private static async Task FillData(
            dynamic profile,
            SqlConnection connection,
            IMongoCollection<BsonDocument> threadCollection,
            IMongoCollection<BsonDocument> threadProfiles)
        {
            var tags = connection.Query<string>(SqlQueryFactory.Instance.Get("get_thread_tags"), new { ThreadId = profile.Id });

            var key = Builders<BsonDocument>.Filter.Eq("id", (profile.Id as string).Trim());

            var match = await threadCollection.Find(key).Project("{url: 1, messages: 1, _id: 0}").SingleOrDefaultAsync();

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

            await threadProfiles.UpdateOneAsync("{'_id': '" + (profile.Id as string).Trim() + "'}",
                updateAction,
                new UpdateOptions { IsUpsert = true });
        }
    }
}
