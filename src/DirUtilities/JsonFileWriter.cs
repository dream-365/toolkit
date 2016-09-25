using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace DirUtilities
{
    public class JsonFileWriter<TRecord> : IDisposable
    {
        private string _path;

        private StreamWriter _writer;

        public JsonFileWriter(string path)
        {
            _path = path;

            _writer = new StreamWriter (path)
            {
                AutoFlush = true
            };
        }

        public void Write(TRecord record)
        {
            var text = JsonConvert.SerializeObject(record, new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            Console.Write(".");

            _writer.WriteLine(text);
        }

        public void Close()
        {
            _writer.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
