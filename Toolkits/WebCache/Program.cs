using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCache
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("webcache.tasks.json");

            var tasks = JsonConvert
                .DeserializeObject<IEnumerable<WebCacheTask>>(text);

            int index = 0;

            foreach (var task in tasks)
            {
                Console.WriteLine("[{0}]: {1}", index, task.Name);

                index++;
            }

            var input = Console.ReadLine();

            int number;

            WebCacheTask selected = null;

            if (int.TryParse(input, out number)
                && number > -1
                && number < tasks.Count())
            {
                selected = tasks.ElementAt(number);
            }
            else
            {
                Console.WriteLine("Invalid Input");

                return;
            }


            Console.WriteLine("Run Service Task [{0}]", selected.Name);

            var service = new WebCacheService();

            var serviceTask = service.RunAsync(selected);

            serviceTask.Wait();
        }
    }
}
