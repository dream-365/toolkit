using MongoDB.Bson;
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
    public class AskerAnalysis
    {
        private IMongoCollection<BsonDocument> _userCollection;

        private IMongoCollection<BsonDocument> _threadCollection;

        private IMongoDatabase _database;

        public AskerAnalysis(string collection)
        {
            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            _database = client.GetDatabase("forums");

            _userCollection = _database.GetCollection<BsonDocument>("users");

            _threadCollection = _database.GetCollection<BsonDocument>(collection);
        }

        public async Task ExportTop20Asker(string exportPath)
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
                .Limit(20)
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
                                     Console.WriteLine(ex.Message);
                                 }

                                 list.Add(thread);
                             });

            return list;
        } 
    }
}
