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

namespace Framework
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

            EasyAnalysis.Framework.ConnectionStringProviders.IConnectionStringProvider mongoDBCSProvider =
                EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.CreateConnectionStringProvider(EasyAnalysis.Framework.ConnectionStringProviders.ConnectionStringProvider.ConnectionStringProviderType.MongoDBConnectionStringProvider);

            IList<IStep> steps = new List<IStep>
            {
                new ImportNewUsersStep(repository, month, mongoDBCSProvider),
                new AskerAnalysisStep(repository, month, mongoDBCSProvider),
                new MapReduceMonthlyAskerTagsStep(repository, month, mongoDBCSProvider),
                new UpdateMonthlyAskerTagsToAskerActivityStep(repository, month, mongoDBCSProvider)
            };

            foreach(var step in steps)
            {
                Console.WriteLine(step.Description);
                step.RunAsync().Wait();
            }
        }
       
    }
}
