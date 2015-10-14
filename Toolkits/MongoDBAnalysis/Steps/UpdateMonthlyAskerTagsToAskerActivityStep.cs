using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAnalysis
{
    public class UpdateMonthlyAskerTagsToAskerActivityStep : IStep
    {
        private IMongoCollection<BsonDocument> _askerTagCollection;

        private IMongoCollection<BsonDocument> _askerActivityCollection;

        private IMongoDatabase _database;

        private readonly string _repository;

        private readonly string _month;

        public string Description
        {
            get
            {
                return "Update monthly asker tags to asker activity";
            }
        }

        public UpdateMonthlyAskerTagsToAskerActivityStep(string database, string repository, string month)
        {
            _repository = repository;

            _month = month;

            var client = new MongoClient("mongodb://app-svr.cloudapp.net:27017/" + database);

            _database = client.GetDatabase(database);

            _askerTagCollection = _database.GetCollection<BsonDocument>(_repository + "_user_tags");

            _askerActivityCollection = _database.GetCollection<BsonDocument>(_repository + "_asker_activities");
        }

        public async Task RunAsync()
        {
            await _askerActivityCollection.Find("{month: '" + _month + "'}")
                .Project("{_id: 1, id: 1}")
                .ForEachAsync(async (item)=> {
                    var objId = item.GetElement("_id").Value.AsObjectId;
                    var id = item.GetElement("id").Value.AsString;

                    var document = new BsonArray();

                    await _askerTagCollection.Find("{'_id.user': '" + id + "', '_id.month': '" + _month + "'}")
                                       .ForEachAsync((userTag) => {
                                           var temp = new BsonDocument();

                                           var _id = userTag.GetElement("_id").Value.AsBsonDocument;

                                           var value = userTag.GetElement("value").Value.AsBsonDocument;

                                           var tag = _id.GetElement("tag").Value;

                                           var count = value.GetElement("count").Value;

                                           temp.Set("name", tag);

                                           temp.Set("count", count);

                                           document.Add(temp);
                                       });

                    var lookup = Builders<BsonDocument>.Filter.Eq("_id", objId);

                    var set = Builders<BsonDocument>.Update.Set("tags", document);

                    await _askerActivityCollection.UpdateOneAsync(lookup, set);

                    Console.WriteLine("user id: " + id);
                });
        }
    }
}
