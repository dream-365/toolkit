using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            var anslysis = new AskerAnalysis("uwp_sep_threads");

            anslysis.ExportTop20Asker("top_20_askers.json").Wait();
        }

        private static void ExportJsonData()
        {
            var text = File.ReadAllText(@"D:\temp\all_in_one.json");

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(text) as JArray;

            var export = obj.Where(m =>
            {
                var value = m.Value<DateTime>("createdOn");

                return value >= DateTime.Parse("2015-8-1") && value < DateTime.Parse("2015-9-1");
            });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(export, Formatting.Indented);

            File.WriteAllText(@"D:\temp\uwp_aug_threads.json", json);
        }

        private static void RunAskerAnslysis()
        {
            var example = new UserAnalysisExample();

            var task = example.RunAskerAnalysis();

            task.Wait();

            var result = task.Result.Select(m => m.Value)
                .OrderByDescending(m => m.total)
                .ToList();

            var text = Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented);

            File.WriteAllText("top_20_askers.json", text);

            Console.WriteLine(text);
        }

        private static void AnswerAnalysis()
        {
            var example = new UserAnalysisExample();

            var task = example.RunAnswerAnalysis("mscs");

            task.Wait();

            var result = task.Result
                .Select(m => m.Value).ToList()
                .Where(m => m.Total > 0)
                .OrderByDescending(m => m.Total).ToList();

            var text = Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented);

            Console.WriteLine(text);
        }

        static async Task ListAllMSCS()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("msft", "true");

            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");

            var users = forums.GetCollection<BsonDocument>("users");

            var result = await users.Find(filter).ToListAsync();

            Console.WriteLine(result.ToJson());
        }

        static async Task AddUsers()
        {
            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");

            var users = forums.GetCollection<BsonDocument>("users");

            var threads = forums.GetCollection<BsonDocument>("uwp_aug_threads");

            await threads.Find(new BsonDocument()).ForEachAsync(async (thread)  => {
                foreach(var user in thread.GetElement("users").Value.AsBsonArray)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("id", user.ToBsonDocument().GetElement("id").Value);
                    var count = await users.Find(filter).CountAsync();

                    if (count == 0)
                    {
                        await users.InsertOneAsync(user.ToBsonDocument());
                    }
                }
            });
        }

        static async Task ArrayInAll()
        {
            IMongoCollection<BsonDocument> threads = GetCollection();

            // this could resovle the tag problem
            var filter = Builders<BsonDocument>.Filter.All("users.id", new List<string>
            { "43213545-0bdc-470b-a7b1-767939c0d76b", "08708526-2dae-43bf-9cc5-29ce9b646442" });

            var result = await threads.Find(filter).Limit(10).ToListAsync();
        }

        static async Task ElementMatch()
        {
            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");
         
            var threads = forums.GetCollection<BsonDocument>("uwp_threads");

            var match = Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>("messages", "{authorId: \"08708526-2dae-43bf-9cc5-29ce9b646442\"}");

            var result = await threads.Find(match).FirstOrDefaultAsync();

            var author = result.GetElement("author");

        }

        static async Task Porjection()
        {
            IMongoCollection<BsonDocument> threads = GetCollection();

            // alternative way to build projection
            //ProjectionDefinition<BsonDocument> projection =
            //    Builders<BsonDocument>
            //    .Projection
            //    .Include("title")
            //    .Exclude("_id");

            ProjectionDefinition<BsonDocument> projection = "{ title: 1, _id: 0, views: 1, msgs: { $size: \"$messages\" }}";

            var result = await threads.Aggregate()
                                    .Project(projection)
                                    .Sort("{views: -1}")
                                    .Limit(10)
                                    .ToListAsync();
        }

        static async Task GroupBy()
        {
            var threads = GetCollection();

            var resut = await threads.Aggregate().Group(new BsonDocument
                            {
                                { "_id", new BsonDocument
                                             {
                                                 {
                                                     "author","$author.display_name"
                                                 }
                                             }
                                },
                                {
                                    "total", new BsonDocument
                                                 {
                                                     {
                                                         "$sum", 1
                                                     }
                                                 }
                                }
                            }).Sort("{total: 1}").ToListAsync();
        }


        static async Task  MappingAuthor()
        {
            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");

            var users = forums.GetCollection<BsonDocument>("users");

            var threads = forums.GetCollection<BsonDocument>("uwp_threads");

            var updates = new List<WriteModel<BsonDocument>>();

            await threads.Find(new BsonDocument())
                .ForEachAsync(async (thread) => {
                    var filter = Builders<BsonDocument>.Filter.Eq("id", thread.GetElement("authorId").Value);

                    var author = await users.Find(filter).FirstOrDefaultAsync();

                    var dbref = new MongoDBRef("users", author.GetElement("_id").Value.AsObjectId);
                   
                    updates.Add(new UpdateOneModel<BsonDocument>(new BsonDocument("_id", thread.GetElement("_id").Value), new BsonDocument("$set", new BsonDocument("author", dbref.ToBsonDocument()))));
                });

            await threads.BulkWriteAsync(updates);
        }

        private static IMongoCollection<BsonDocument> GetCollection()
        {
            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");

            var threads = forums.GetCollection<BsonDocument>("uwp_threads");

            return threads;
        }
    }
}
