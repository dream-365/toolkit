using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAnalysis
{
    public class MapReduceMonthlyAskerTagsStep : IStep
    {
        private IMongoCollection<BsonDocument> _userActivityCollection;

        private IMongoDatabase _database;

        private readonly string _repository;

        private readonly string _month;

        public string Description
        {
            get
            {
                return "Map-reduce user tags";
            }
        }

        public MapReduceMonthlyAskerTagsStep(string repository, string month)
        {
            _repository = repository;

            _month = month;

            var client = new MongoClient("mongodb://app-svr.cloudapp.net:27017/" + repository);

            _database = client.GetDatabase(repository);

            _userActivityCollection = _database.GetCollection<BsonDocument>("asker_activities");
        }

        public async Task RunAsync()
        {
            BsonJavaScript map = @"function() {
                for (var idx = 0; idx < this.threads.length; idx++) {

                    var tags = this.threads[idx].tags;

                    if (tags === undefined) {
                        continue;
                    }

                    for (var j = 0; j < tags.length; j++) {
                        var key = {
                            user: this.id,
                            tag: tags[j],
                            month: '" + _month + @"'
                        };

                        var value = {
                            count: 1,
                        };

                        emit(key, value);
                    }
                }
            }";

            BsonJavaScript reduce = @"function(key, values) {
                reducedVal = {
                    count: 0
                };

                for (var idx = 0; idx < values.length; idx++) {
                    reducedVal.count += values[idx].count;
                }

                return reducedVal;
            }";

            var options = new MapReduceOptions<BsonDocument, BsonDocument>();

            options.OutputOptions = MapReduceOutputOptions.Merge("user_tags");

            options.Filter = "{month: '" + _month +"'}";

            await _userActivityCollection.MapReduceAsync(map, reduce, options);
        }
    }
}
