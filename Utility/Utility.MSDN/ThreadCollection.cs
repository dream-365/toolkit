using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utility.MSDN
{
    public enum Community
    {
        MSDN,
        TECHNET
    };

    public class ThreadCollection
    {
        private readonly string _forum;

        private readonly Community _community;

        public const int PAGE_SIZE = 20;

        private const string MSDN_URL_FORMAT = "https://social.msdn.microsoft.com/Forums/en-US/home?forum={0}&filter=alltypes&sort=firstpostdesc&brandIgnore=true&page={1}";
        private const string TECHNET_URL_FORMAT = "https://social.technet.microsoft.com/Forums/en-us/home?forum={0}&filter=alltypes&sort=lastpostdesc&page={1}";


        public ThreadCollection(Community community, string forum)
        {
            _community = community;

            _forum = forum;
        }

        public async Task<IEnumerable<string>> NavigateToPageAsync(int index)
        {
            var result = new List<string>();

            var client = new HttpClient();

            string targetUrlFormat = string.Empty;

            if (_community == Community.MSDN)
            {
                targetUrlFormat = MSDN_URL_FORMAT;
            }
            else if (_community == Community.TECHNET)
            {
                targetUrlFormat = TECHNET_URL_FORMAT;
            }
            else
            {
                throw new Exception("This community type is not supported yet");
            }

            var message = await client.GetAsync(string.Format(targetUrlFormat, _forum, index));

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
