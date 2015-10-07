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

            var modules = new List<IMetadataModule>();

            foreach(var moduleConfig in config.Modules)
            {
                var al = factory.Create(moduleConfig.Name, moduleConfig.Arguments);

                modules.Add(al);
            }

            var rootFolder = new DirectoryInfo(config.RootFolder);

            var files = rootFolder.EnumerateFiles();

            var result = new List<Dictionary<string, object>>();

            Console.WriteLine("load file [{0}]", files.Count());

            foreach (var file in files)
            {
                var meatadata = new Dictionary<string, object>();

                meatadata["file_name"] = file.Name;

                try
                {
                    foreach (var module in modules)
                    {
                        Process(encoding, module, meatadata, file);
                    }

                    result.Add(meatadata);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            var text = JsonConvert.SerializeObject(result, Formatting.Indented);

            File.WriteAllText(config.OutputTo, text);
        }

        private static void Process(Encoding encoding, IMetadataModule al, Dictionary<string, object> result, FileInfo file)
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
