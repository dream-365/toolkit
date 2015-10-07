using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentAnalyze
{
    class Program
    {
        static void Main(string[] args)
        {
            var configText = File.ReadAllText("content.analyze.tasks.json");

            var configrations =
                JsonConvert.DeserializeObject<IEnumerable<ContentProcessTaskConfigration>>(configText);

            var config = configrations.FirstOrDefault();

            Run(config);
        }

        private static void Run(ContentProcessTaskConfigration config)
        {
            var factory = new ModuleFactory();

            var encoding = Encoding.GetEncoding(config.Encoding);

            var al = factory.Create(config.Module, config.Arguments);

            var result = new Dictionary<string, object>();

            var rootFolder = new DirectoryInfo(config.RootFolder);

            var files = rootFolder.EnumerateFiles();

            Console.WriteLine("load file [{0}]", files.Count());

            foreach (var file in files)
            {
                try
                {
                    Process(encoding, al, result, file);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (result.ContainsKey(al.ResultKey))
            {
                var text = JsonConvert.SerializeObject(result[al.ResultKey], Formatting.Indented);

                File.WriteAllText(config.OutputTo, text);
            }
            else
            {
                Console.WriteLine("No result");
            }

        }

        private static void Process(Encoding encoding, IContentModule al, Dictionary<string, object> result, FileInfo file)
        {
            using (var memStream = new MemoryStream())
            {
                using (var stream = file.Open(FileMode.Open))
                {
                    stream.CopyTo(memStream);

                    memStream.Flush();

                    memStream.Position = 0;
                }

                using (var sr = new StreamReader(memStream, encoding))
                {
                    var content = sr.ReadToEnd();

                    al.OnProcess(result, content);
                }
            }
        }
    }
}
