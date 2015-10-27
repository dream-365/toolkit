using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EasyAnalysis.Backend.Algorithm;

namespace EasyAnalysis.Backend
{
    public class DuplicateDetection
    {
        public void Run()
        {
            // {{ parameters:

            string repository = "uwp";

            DateTime start = DateTime.Parse("2015-9-1");

            DateTime end = DateTime.Parse("2015-10-1");

            // }}

            var distance = new LevenshteinDistance();

            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var result = new List<string>();

            using (var connection = new System.Data.SqlClient.SqlConnection(cs))
            {
                var titles = connection.Query(SqlQueryFactory.Instance.Get("get_thread_profile"),
                new
                {
                    repository = repository.ToUpper(),
                    start = start,
                    end = end
                }).Select(m => m.Title as string).ToList();

               for (int i = 0; i < titles.Count - 1; i++)
               {
                    for(int j = i + 1; j < titles.Count; j++)
                    {
                        var left = titles[i].ToLower();

                        var right = titles[j].ToLower();

                        var percentage = distance.LevenshteinDistancePercent(left, right) * 100;

                        // list all the percentage >= 50%
                        if(percentage >= 50m)
                        {
                            result.Add(string.Format("Left: {0}\r\nRight: {1}\r\nPercertage: {2}%", left, right, percentage));
                        }
                    }

                    Console.Write(".");
                }

                foreach(var item in result)
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
}
