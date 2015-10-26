using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAnalysis
{
    public class AskerAnalysisStep : IStep
    {
        private IMongoCollection<BsonDocument> _userCollection;

        private IMongoCollection<BsonDocument> _threadCollection;

        private IMongoCollection<BsonDocument> _askerActivities;

        private readonly string _repository;

        private readonly string _month;

        private IMongoDatabase _database;

        public string Description
        {
            get
            {
                return "Run asker analysis to extract the actions of askers";
            }
        }

        public AskerAnalysisStep(string repository, string month)
        {
            var client = new MongoClient("mongodb://app-svr.cloudapp.net:27017/" + repository);

            _repository = repository;

            _month = month;

            _database = client.GetDatabase(repository);

            _userCollection = _database.GetCollection<BsonDocument>("users");

            _threadCollection = _database.GetCollection<BsonDocument>(month + "_" + "threads");

            _askerActivities = _database.GetCollection<BsonDocument>("asker_activities");
        }

        public async Task RunAsync()
        {
            var result = new Dictionary<string, dynamic>();

            await MapBasicInfo(result);

            // clean all of the old data.
            var filter = Builders<BsonDocument>.Filter.Eq("month", _month);

            await _askerActivities.DeleteManyAsync(filter);

            result.Select(m => m.Value).ToList().ForEach(async (item)=> {

                item.month = _month;

                var line = Newtonsoft.Json.JsonConvert.SerializeObject(item, Formatting.Indented);

                using (var jsonReader = new MongoDB.Bson.IO.JsonReader(line))
                {
                    var context = BsonDeserializationContext.CreateRoot(jsonReader);

                    var document = _askerActivities.DocumentSerializer.Deserialize(context);

                    await _askerActivities.InsertOneAsync(document);
                }
            });
        }

        public async Task ExportAskers(string exportPath)
        {
            var result = new Dictionary<string, dynamic>();

            await MapBasicInfo(result);

            var text = Newtonsoft.Json.JsonConvert.SerializeObject(result.Select(m => m.Value), Formatting.Indented);

            File.WriteAllText(exportPath, text);
        }

        private async Task MapBasicInfo(Dictionary<string, dynamic> result)
        {
            await _threadCollection
                .Aggregate()
                .Group("{ _id : '$authorId', total: { $sum: 1 } }")
                .Sort("{total: -1}")
                .ForEachAsync(async (item) =>
                {
                    var userId = item.GetElement("_id").Value.AsString;

                    var total = item.GetElement("total").Value.AsInt32;

                    var user = await _userCollection.Find("{id: '" + userId + "'}").FirstOrDefaultAsync();

                    dynamic container;

                    if (result.ContainsKey(userId))
                    {
                        container = result[userId];
                    }
                    else
                    {
                        container = new ExpandoObject();

                        container.display_name = user.GetElement("display_name").Value.AsString;

                        container.id = userId;

                        result[userId] = container;
                    }

                    container.total = total;

                    var answered = await _threadCollection.Find(string.Format("{{ authorId: '{0}', answered: 'true' }}", userId)).CountAsync();

                    container.threads = await GetThreadsByUser(userId);

                    container.answered = answered;

                    // display the progress
                    Console.Write(".");
                });
        }

        private async Task<IEnumerable<dynamic>> GetThreadsByUser(string userId)
        {
            var list = new List<dynamic>();

            var client = new HttpClient();

            await _threadCollection.Aggregate()
                             .Match(string.Format("{{ authorId : '{0}' }}", userId))
                             .Project("{ _id: 0, id: 1, title: 1, url: 1, answered: 1 }")
                             .ForEachAsync(async data => {
                                 dynamic thread = new ExpandoObject();

                                 thread.id = data.GetElement("id").Value.AsString;
                                 thread.title = data.GetElement("title").Value.AsString;
                                 thread.url = data.GetElement("url").Value.AsString;
                                 thread.answered = bool.Parse(data.GetElement("answered").Value.AsString);

                                 var web_api = "http://analyzeit.azurewebsites.net/api/thread/{0}/detail";

                                 try
                                 {
                                     var json = await client.GetStringAsync(string.Format(web_api, thread.id));

                                     var obj = JsonConvert.DeserializeObject(json) as JObject;

                                     var tags = obj.GetValue("Tags") as JArray;

                                     thread.type_id = obj.GetValue("TypeId").Value<int>();

                                     if (tags != null)
                                     {
                                         thread.tags = tags.Select(m => m.ToString()).ToList();
                                     }
                                     
                                 }catch(Exception ex)
                                 {
                                     Console.Write("x");
                                 }

                                 list.Add(thread);
                             });

            return list;
        } 
    }
}
