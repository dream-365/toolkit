using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utility.MSDN
{
    public enum ThreadType
    {
        Discussion,
        Question
    }

    public class ThreadInfo
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int Views { get; set; }

        public bool Answered { get; set; }

        public DateTime CreateOn { get; set; }

        public string Url { get; set; }

        public string AuthorId { get; set; }

        public string ForumId { get; set; }

        public UserInfo Author { get; set; }

        public virtual ICollection<MessageInfo> Messages { get; set; }
    }
}