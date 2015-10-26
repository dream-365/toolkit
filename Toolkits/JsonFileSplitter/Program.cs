using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFileSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("args: [input] [output] [start] [end]");

                return;
            }

            string input = args[0];
            string output = args[1];
            DateTime start = DateTime.Parse(args[2]);
            DateTime end = DateTime.Parse(args[3]);

            var text = File.ReadAllText(input);

            var obj = JsonConvert.DeserializeObject(text) as JArray;

            var export = obj.Where(m =>
            {
                var value = m.Value<DateTime>("createdOn");

                return value >= start && value < end;
            });

            var json = JsonConvert.SerializeObject(export, Formatting.Indented);

            File.WriteAllText(output, json);
        }
    }
}
