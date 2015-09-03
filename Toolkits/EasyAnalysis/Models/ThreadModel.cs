using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyAnalysis.Models
{
    public class ThreadModel
    {
        public String Id { get; set; }

        public String Title { get; set; }

        public DateTime CreateOn { get; set; }

        public String ForumId { get; set; }

        public String AuthorId { get; set; }
    }
}