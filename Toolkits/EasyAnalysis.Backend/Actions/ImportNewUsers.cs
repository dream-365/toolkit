using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace EasyAnalysis.Backend.Analysis
{
    public class ImportNewUsers : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "Import new users to current users collection";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringProvider"></param>
        public ImportNewUsers(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">[repository] [source collection name] [target collection name]</param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            var repository = args[0];

            var sourceCollectionName = args[1];

            var targetCollectionName = args[2];

            var client = new MongoClient(_connectionStringProvider.GetConnectionString(repository));

            IMongoDatabase _database = client.GetDatabase(repository);

            IMongoCollection<BsonDocument> targetCollection = _database.GetCollection<BsonDocument>(targetCollectionName);

            IMongoCollection<BsonDocument> sourceCollection = _database.GetCollection<BsonDocument>(sourceCollectionName);

            await sourceCollection.Find(new BsonDocument()).ForEachAsync(async (thread) => {
                foreach (var user in thread.GetElement(targetCollectionName).Value.AsBsonArray)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("id", user.ToBsonDocument().GetElement("id").Value);

                    var count = await targetCollection.Find(filter).CountAsync();

                    if (count == 0)
                    {
                        await targetCollection.InsertOneAsync(user.ToBsonDocument());
                    }
                }
            });
        }
    }
}
