using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utility.StackOverflow
{
    public class QuestionCollection
    {
        private const string api_format = "https://api.stackexchange.com/2.2/questions?page={0}&pagesize={1}&order=desc&sort=creation&tagged={2}&site=stackoverflow";

        private const int page_size = 100;

        private readonly string _tag;

        public QuestionCollection(string tag)
        {
            _tag = tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">1 based page index</param>
        /// <returns></returns>
        public async Task<IEnumerable<Question>> NavigateToPageAsync(int index)
        {
            var client = new HttpClient();

            var message = await client.GetAsync(string.Format(api_format, index, page_size, _tag));

            var content = await message.Content.ReadAsStreamAsync();

            using (Stream decompressed = new GZipStream(content, CompressionMode.Decompress))
            using (StreamReader reader = new StreamReader(decompressed))
            {
                string text = reader.ReadToEnd();

                var result = JsonConvert.DeserializeObject<QuestionsQueryResult>(text);

                return result.Items;
            }
        }
    }
}
