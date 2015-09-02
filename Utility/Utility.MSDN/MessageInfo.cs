using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utility.MSDN
{
    public class MessageInfo
    {
        public String Id { get; set; }

        public UserInfo Author { get; set; }

        public DateTime CreateOn { get; set; }

        public string Body { get; set; }
    }
}