using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utility.MSDN
{
    public class ThreadCollection
    {
        private readonly string _forum;

        public const int PAGE_SIZE = 20;

        private const string URL_FORMAT = "https://social.msdn.microsoft.com/Forums/en-US/home?forum={0}&filter=alltypes&sort=firstpostdesc&brandIgnore=true&page={1}";

        public ThreadCollection(string forum)
        {
            _forum = forum;
        }

        public async Task<IEnumerable<string>> NavigateToPageAsync(int index)
        {
            var result = new List<string>();

            var client = new HttpClient();

            var message = await client.GetAsync(string.Format(URL_FORMAT, _forum, index));

            var content = await message.Content.ReadAsStringAsync();

            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(content);

            var nodes = htmlDocument.DocumentNode.SelectNodes(@"//*[@id=""threadList""]/li/div/a");

            if (nodes == null)
            {
                return result;
            }

            foreach (HtmlNode node in nodes)
            {
                var link = node.GetAttributeValue("href", "");

                result.Add(link);
            }

            return result.Take(PAGE_SIZE).ToList();
        }
    }
}
