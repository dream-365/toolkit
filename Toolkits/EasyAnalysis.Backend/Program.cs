using EasyAnalysis.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;
using EasyAnalysis.Framework.Analysis;

namespace EasyAnalysis.Backend
{
    class Program
    {
        static void Main(string[] args)
        {
            var steps = new List<Step>();

            //steps.Add(new Step {
            //    Action = "import-new-users",
            //    Parameters = new string[] {
            //        "uwp",
            //        "aug_threads",
            //        "users"
            //    }
            // });

            steps.Add(new Step
            {
                Action = "extract-user-activies",
                Parameters = new string[] {
                    "uwp",
                    "aug_threads"
                }
            });

            var factory = new DefaultActionFactory();

            foreach(var step in steps)
            {
                var action = factory.CreateInstance(step.Action);

                var task = action.RunAsync(step.Parameters);

                task.Wait();
            }
        }
    }
}
