using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ContentAnalyze
{
    public class MSDNThreadMeatadataModule : IMetadataModule
    {
        public void Init(IEnumerable<string> arguments)
        {
            
        }

        public void OnProcess(IDictionary<string, object> metadata, string content)
        {
            using (Stream stream = new MemoryStream())
            using (StreamWriter sr = new StreamWriter(stream))
            {
                sr.Write(content);

                sr.Flush();

                stream.Position = 0;

                FillMetadata(metadata, stream);
            }
        }

        private static void FillMetadata(IDictionary<string, object> metadata, Stream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                var xDoc = XDocument.Load(reader);

                var xUsers = xDoc.Element("root").Element("users").Descendants("user");

                var users = (from u in xUsers
                             select new Dictionary<string, string>
                             {
                                 { "id", u.Attribute("id").Value },
                                 { "display_name", u.Element("displayName").Value },
                                 { "msft", u.Element("msft").Value },
                                 { "mscs", u.Element("mscs").Value },
                                 { "mvp", u.Element("mvp").Value },
                                 { "partner", u.Element("partner").Value },
                                 { "mcc", u.Element("mcc").Value },
                             }).ToList();

                var xMessages = xDoc.Element("root").Element("messages").Descendants("message");

                var messages = (from c in xMessages
                                select new Dictionary<string, string>
                                {
                                    { "id", c.Attribute("id").Value },
                                    { "authorId", c.Attribute("authorId").Value },
                                    { "createdOn", c.Element("createdOn").Value },
                                    { "body", c.Element("body").Value },
                                    { "is_answer", c.Element("answer") == null ? "false" : c.Element("answer").Value}
                                }).ToList();

                var xThread = xDoc.Element("root").Element("thread");

                var thread = new Dictionary<string, object>
                {
                    { "id", xThread.Attribute("id") == null ? string.Empty : xThread.Attribute("id").Value},
                    { "authorId", xThread.Attribute("authorId") == null ? string.Empty : xThread.Attribute("authorId").Value },
                    { "title", xThread.Element("topic") == null ? string.Empty : xThread.Element("topic").Value },
                    { "url", xThread.Element("url") == null ? string.Empty : xThread.Element("url").Value},
                    { "createdOn", xThread.Element("createdOn") == null ? string.Empty : xThread.Element("createdOn").Value},
                    { "answered", xThread.Attribute("answered") == null ? "false" : xThread.Attribute("answered").Value},
                    { "views", int.Parse(xThread.Attribute("views") == null ? string.Empty : xThread.Attribute("views").Value) },
                    { "forumId", xThread.Attribute("discussionGroupId") == null ? string.Empty : xThread.Attribute("discussionGroupId").Value},
                    { "messages", messages},
                    { "users", users}
                };

                // push to result
                foreach (var kv in thread)
                {
                    metadata.Add(kv);
                }
            }
        }
    }
}
