using HtmlToJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlConveter
{
    class Program
    {
        const int TASKS_BUFFER_SIZE = 100;

        const int CONCURRENCY_THREADS = 5;

        /// <summary>
        /// commands:
        /// -mapping maping.json -encoding utf8 -html source.html -json target.json
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("-mapping maping.json -encoding utf8 -html source.html -json target.json");

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

            var mappingFilePath = parameters["mapping"];

            var htmlFilePath = parameters["html"];

            var jsonFilePath = parameters["json"];

            var encoding = Encoding.GetEncoding(
                parameters.ContainsKey("encoding") 
                ? parameters["encoding"] 
                : "utf8");

            var mappingFileText = File.ReadAllText(mappingFilePath);

            var mappings = JsonConvert.DeserializeObject<IEnumerable<XPathMapping>>(
                value: mappingFileText, 
                settings:new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            var converter = new HtmlToJsonConverter(mappings);

            var startTime = DateTime.Now;

            if(Directory.Exists(htmlFilePath))
            {
                var dir = new DirectoryInfo(htmlFilePath);

                LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(CONCURRENCY_THREADS);

                TaskFactory factory = new TaskFactory(lcts);

                CancellationTokenSource cts = new CancellationTokenSource();

                List<Task> tasks = new List<Task>();

                Object IO_LOCK = new Object();

                using (var fs = new FileStream(jsonFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;

                    foreach (var file in dir.EnumerateFiles())
                    {
                        var task = factory.StartNew((
                            ) => {
                                var content = File.ReadAllText(file.FullName, encoding);

                                var line = converter.Convert(content, false);

                                lock(IO_LOCK)
                                {
                                    sw.WriteLine(line);

                                    Console.Write(".");
                                }
                            }, cts.Token);

                        tasks.Add(task);

                        // when the buffer size arrived, wait and clear the buffer
                        if(tasks.Count > TASKS_BUFFER_SIZE)
                        {
                            Task.WaitAll(tasks.ToArray());

                            tasks.Clear();
                        }
                    }

                    // wait the rest tasks

                    if(tasks.Count > 0)
                    {
                        Task.WaitAll(tasks.ToArray());
                    }                  

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine();
                }
            }else if(File.Exists(htmlFilePath))
            {
                var html = File.ReadAllText(htmlFilePath, encoding);

                var jsonText = converter.Convert(html, true);

                File.WriteAllText(jsonFilePath, jsonText);
            }
            else
            {
                Console.WriteLine("The specified html path is invalid");
            }

            var endTime = DateTime.Now;

            Console.WriteLine("{0} - {1} ({2})", startTime, endTime, endTime - startTime);
        }
    }
}
