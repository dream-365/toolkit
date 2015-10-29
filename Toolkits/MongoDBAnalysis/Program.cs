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
            if (args.Length < 2)
            {
                Console.WriteLine("arguments: {repository} {month}");
                return;
            }

            string repository = args[0];

            string month = args[1];

            MongoDBAnalysis.ConnectionStringProviders.IConnectionStringProvider mongoDBDataProvider = 
                MongoDBAnalysis.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(MongoDBAnalysis.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MongoDBConnectionStringProvider);

            IList<IStep> steps = new List<IStep>
            {
                new ImportNewUsersStep(repository, month, mongoDBDataProvider),
                new AskerAnalysisStep(repository, month, mongoDBDataProvider),
                new MapReduceMonthlyAskerTagsStep(repository, month, mongoDBDataProvider),
                new UpdateMonthlyAskerTagsToAskerActivityStep(repository, month, mongoDBDataProvider)
            };

            foreach(var step in steps)
            {
                Console.WriteLine(step.Description);
                step.RunAsync().Wait();
            }
        }
       
    }
}
