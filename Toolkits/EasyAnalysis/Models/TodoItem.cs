using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyAnalysis.Models
{
    public class TodoItem
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("id")]
        public string Id { get; set;}

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("createOn")]
        public DateTime CreateOn { get; set; }
    }
}