using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility.MSDN;

namespace ThreadDiscovery
{
    public class ForumSnapshot
    {
        private readonly string _forum;

        private readonly Community _community;

        private IList<ThreadInfo> _storage;

        public ForumSnapshot(Community community, string forum)
        {
            _forum = forum;

            _community = community;

            _storage = new List<ThreadInfo>();
        }

        public async Task TakeAsync(int pageSize)
        {
            var collection = new ThreadCollection(_community, _forum);

            for (int i = 1; i < pageSize + 1; i++)
            {
                var threads = await collection.NavigateToPageAsync(i);

                foreach (var url in threads)
                {
                    var threadId = ExtractUuid(url);

                    if(_storage.FirstOrDefault(m => m.Id.Equals(threadId)) != null)
                    {
                        continue;
                    }

                    var parser = new ThreadParser(Guid.Parse(threadId));

                    try
                    {

                        var info = await parser.ReadThreadInfoAsync();

                        _storage.Add(info);

                        Log(threadId);
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }

                }
            }
        }

        public void Save(string fileName)
        {
            var text = JsonConvert.SerializeObject(_storage);

            File.WriteAllText(fileName, text);
        }

        public void Load(string fileName)
        {
            var text = File.ReadAllText(fileName);

            _storage = JsonConvert.DeserializeObject<IList<ThreadInfo>>(text);
        }

        public IEnumerable<ThreadInfo> GetData()
        {
            return _storage;
        }

        internal static void Log(string info)
        {
            Console.WriteLine(info);
        }

        internal static string ExtractUuid(string value)
        {
            string identifier = string.Empty;

            Regex urlRegex = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");

            var match = urlRegex.Match(value);

            if (match.Success)
            {
                identifier = match.Groups[0].ToString();
            }

            return identifier;
        }
    }
}
