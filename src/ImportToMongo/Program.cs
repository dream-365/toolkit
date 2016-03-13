using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportToMongo
{
    class Program
    {
        const int BAT_SIZE = 100;

        /// <summary>
        /// [--server localhost] --db landing --collection cname --file file.json 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("[--server localhost] --db landing --collection cname --file file.json");

                return;
            }

            // parse parameters
            var parameters = new Dictionary<string, string>();

            int start = 0;

            while (start < args.Length)
            {
                var pair = args.Skip(start).Take(2);

                parameters.Add(pair.ElementAt(0).TrimStart('-'), pair.ElementAt(1));

                start = start + 2;
            }

            var filePath = parameters["file"];

            var server = parameters.ContainsKey("server") ? parameters["server"] : "localhost";

            var connectionString = string.Format("mongodb://{0}:27017", server);

            var dbName = parameters["db"];

            var collectionName = parameters["collection"];

            var client = new MongoClient(connectionString);

            var collection = client.GetDatabase(dbName).GetCollection<BsonDocument>(collectionName);

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
            {
                var documents = new List<BsonDocument>();

                while (!sr.EndOfStream)
                {
                    try
                    {
                        var text = sr.ReadLine();

                        using (var jsonReader = new JsonReader(text))
                        {
                            var context = BsonDeserializationContext.CreateRoot(jsonReader);

                            var document = collection.DocumentSerializer.Deserialize(context);

                            documents.Add(document);
                        }

                        if(documents.Count == BAT_SIZE)
                        {
                            collection.InsertMany(documents);

                            documents.Clear();

                            Console.Write(".");
                        }
                    }
                    catch(Exception ex)
                    {
                        var bak = Console.ForegroundColor;

                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine();

                        Console.WriteLine(ex.Message);

                        Console.ForegroundColor = bak;
                    }
                }

                Console.WriteLine();

                if (documents.Count > 0)
                {
                    collection.InsertMany(documents);
                }
            }
        }
    }
}
