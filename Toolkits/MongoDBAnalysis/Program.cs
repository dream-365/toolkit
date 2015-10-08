using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Porjection().Wait();
        }

        static async Task Porjection()
        {
            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");

            var threads = forums.GetCollection<BsonDocument>("uwp_threads");

            // alternative way to build projection
            //ProjectionDefinition<BsonDocument> projection =
            //    Builders<BsonDocument>
            //    .Projection
            //    .Include("title")
            //    .Exclude("_id");

            ProjectionDefinition<BsonDocument> projection = "{ title: 1, _id: 0, msgs: { $size: \"$messages\" }}";

            var result = await threads.Aggregate().Project(projection).Limit(10).ToListAsync();
        }

        static async Task GroupBy()
        {
            var client = new MongoClient("mongodb://rpt:[password]@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");

            var threads = forums.GetCollection<BsonDocument>("uwp_threads");

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
                            }).ToListAsync();

        }


        static async Task  MappingAuthor()
        {
            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");

            var threads = forums.GetCollection<BsonDocument>("uwp_threads");

            var firstOne = await threads.Find(new BsonDocument()).FirstOrDefaultAsync();

            Console.WriteLine(firstOne);

            var updates = new List<WriteModel<BsonDocument>>();

            await threads.Find(new BsonDocument())
                .ForEachAsync((thread) => {
                    var users = thread.GetElement("users").Value.AsBsonArray;

                    var author = users
                    .FirstOrDefault(m => m.AsBsonDocument.GetElement("id").Value == thread.GetElement("authorId").Value);

                    updates.Add(new UpdateOneModel<BsonDocument>(new BsonDocument("_id", thread.GetElement("_id").Value), new BsonDocument("$set", new BsonDocument("author", author))));
                });

            await threads.BulkWriteAsync(updates);
        }
    }
}
