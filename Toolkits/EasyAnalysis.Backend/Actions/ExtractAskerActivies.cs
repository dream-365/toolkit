using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace EasyAnalysis.Backend.Actions
{
    public class ExtractAskerActivies : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public ExtractAskerActivies(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public string Description
        {
            get
            {
                return "Run asker analysis to extract the actions of askers";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">[repository] [user collection name] [thread collection name] [target collection name]</param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            var repository = args[0];

            var threadCollectionName = args[1];

            var targetCollectionName = args[2];

            var monthPrefix = args[3];

            var client = new MongoClient(_connectionStringProvider.GetConnectionString(repository));
           
            var database = client.GetDatabase(repository);

            var threadCollection = database.GetCollection<BsonDocument>(threadCollectionName);

            var targetCollection = database.GetCollection<BsonDocument>(targetCollectionName);

            await MapBasicInfo(threadCollection, targetCollection, monthPrefix);

        }


        private async Task MapBasicInfo(IMongoCollection<BsonDocument> threadCollection, IMongoCollection<BsonDocument> targetCollection, string monthPrefix)
        {
            await threadCollection
                .Aggregate()
                .Group("{ _id : '$authorId', total: { $sum: 1 } }")
                .Sort("{total: -1}")
                .ForEachAsync((item) =>
                {
                    var userId = item.GetElement("_id").Value.AsString;

                    var total = item.GetElement("total").Value.AsInt32;

                    var filter = Builders<BsonDocument>.Filter;

                    var updateAction = Builders<BsonDocument>.Update.Set("total", total);
                });
        }
    }
}
