using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utility.MSDN
{
    public class ThreadInfo
    {
        public String Id { get; set; }

        public String Title { get; set; }

        public bool Answered { get; set; }

        public DateTime CreateOn { get; set; }

        public String Url { get; set; }

        public string AuthorId { get; set; }

        public string ForumId { get; set; }

        public UserInfo Author { get; set; }

        public virtual ICollection<MessageInfo> Messages { get; set; }
    }
}