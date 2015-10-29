using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class ImportNewUsersStep : IStep
    {
        private IMongoCollection<BsonDocument> _userCollection;

        private IMongoCollection<BsonDocument> _threadCollection;

        private IMongoDatabase _database;

        private readonly string _repository;

        private readonly string _month;

        public ImportNewUsersStep(string repository, string month, EasyAnalysis.Framework.ConnectionStringProviders.IConnectionStringProvider mongoDBDataProvider)
        {
            _repository = repository;

            _month = month;
            
            var client = new MongoClient(mongoDBDataProvider.GetConnectionString(repository));

            _database = client.GetDatabase(repository);

            _userCollection = _database.GetCollection<BsonDocument>("users");

            _threadCollection = _database.GetCollection<BsonDocument>(_month + "_" + "threads");
        }

        public string Description
        {
            get
            {
                return "Import new users to current users collection";
            }
        }

        public async Task RunAsync()
        {
            await _threadCollection.Find(new BsonDocument()).ForEachAsync(async (thread) => {
                foreach (var user in thread.GetElement("users").Value.AsBsonArray)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("id", user.ToBsonDocument().GetElement("id").Value);

                    var count = await _userCollection.Find(filter).CountAsync();

                    if (count == 0)
                    {
                        await _userCollection.InsertOneAsync(user.ToBsonDocument());
                    }
                }
            });
        }
    }
}
