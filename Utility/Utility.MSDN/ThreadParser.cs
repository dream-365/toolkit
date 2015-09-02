using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Utility.MSDN
{
    public class ThreadParser
    {
        private const String MSDN_URL_FMT = "http://social.msdn.microsoft.com/Forums/en-US/{0}?outputAs=xml";

        private readonly Guid _threadId;

        public ThreadParser(Guid uuid)
        {
            _threadId = uuid;
        }

        public async Task<ThreadInfo> ReadThreadInfoAsync()
        {
            ThreadInfo ret = null;

            var url = String.Format(MSDN_URL_FMT, _threadId);

            try
            {
                WebRequest request = WebRequest.Create(url);

                request.Method = "GET";

                var response = await request.GetResponseAsync();

                using (Stream stream = response.GetResponseStream())
                {
                    ret = MapInfoToModel(stream);
                }
            }
            catch (WebException ex)
            {
                // TBD: hanlde web exception
            }

            return ret;
        }

        private ThreadInfo MapInfoToModel(Stream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                var xDoc = XDocument.Load(reader);

                var xUsers = xDoc.Element("root").Element("users").Descendants("user");

                var users = (from u in xUsers
                             select new UserInfo
                             {
                                 Id = u.Attribute("id").Value,
                                 DisplayName = u.Element("displayName").Value
                             }).ToList();

                var xMessages = xDoc.Element("root").Element("messages").Descendants("message");

                var messages = (from c in xMessages
                                select new MessageInfo
                                {
                                    Id = c.Attribute("id").Value,
                                    Author = users.Find(m => m.Id.Equals(c.Attribute("authorId").Value)),
                                    CreateOn = DateTime.Parse(c.Element("createdOn").Value),
                                    Body = c.Element("body").Value
                                }).ToList();

                var xThread = xDoc.Element("root").Element("thread");

                var thread = new ThreadInfo
                {
                    Id = xThread.Attribute("id") == null ? string.Empty : xThread.Attribute("id").Value,
                    AuthorId = xThread.Attribute("authorId") == null ? string.Empty : xThread.Attribute("authorId").Value,
                    Title = xThread.Element("topic") == null ? string.Empty : xThread.Element("topic").Value,
                    Url = xThread.Element("url") == null ? string.Empty : xThread.Element("url").Value,
                    CreateOn = DateTime.Parse(xThread.Element("createdOn") == null ? string.Empty : xThread.Element("createdOn").Value), 
                    Answered = Boolean.Parse(xThread.Attribute("answered") == null ? "false" : xThread.Attribute("answered").Value),
                    ForumId = xThread.Attribute("discussionGroupId") == null ? string.Empty : xThread.Attribute("discussionGroupId").Value,
                    Messages = messages
                };

                thread.Author = users.Find(x => x.Id.Equals(thread.AuthorId));

                return thread;
            }
        }
    }
}
