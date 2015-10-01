using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility.MSDN;

namespace ThreadDiscovery
{
    class Program
    {
        static void Main(string[] args)
        {
            var snapshoot = new ForumSnapshot(Community.MSDN, "wpdevelop");

            snapshoot.Load(@"D:\Archive\uwp_page_0_35.json");

            var data = snapshoot.GetData();

            int count = 0;

            foreach (ThreadInfo info in data)
            {
                var body = info.Messages.First().Body;

                if (
                    info.Messages.First().Body.ToLowerInvariant().Contains("winjs") ||
                    info.Title.ToLowerInvariant().Contains("winjs"))
                {
                    Console.WriteLine(info.Title);

                    count++;
                }
            }

            Console.WriteLine("Total: {0}", count);
        }

        private static void SaveSanpshot()
        {
            var snapshoot = new ForumSnapshot(Community.MSDN, "wpdevelop");

            var task = snapshoot.TakeAsync(35);

            task.Wait();

            snapshoot.Save(@"D:\Archive\uwp_page_0_35.json");
        }

        static void ImportThreads()
        {
            for (int i = 1; i < 5; i++)
            {
                //Run(Community.MSDN, "wpdevelop", i).Wait();
                //Run(Community.TECHNET, "Office2016setupdeploy%2COffice2016ITPro", i).Wait();
                // Run(Community.MSDN, "WindowsIoT", i).Wait();
                Run(Community.MSDN, "appsforoffice", i).Wait();
            }
        }

        static async Task Run(Community community, string forum, int index)
        {
            var collection = new ThreadCollection(community, forum);

            var threads = await collection.NavigateToPageAsync(index);

            var client = new HttpClient();

            foreach (var thread in threads)
            {
                var body = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                                { new KeyValuePair<string, string>("", thread)});

                var message = await client.PostAsync("http://analyzeit.azurewebsites.net/api/thread", body);

                var result = await message.Content.ReadAsStringAsync();

                Console.WriteLine(result);
            }
        }
    }
}
