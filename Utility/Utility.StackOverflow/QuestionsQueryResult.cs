using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.StackOverflow
{

    public class Question
    {
        [JsonProperty("is_answered")]
        public bool IsAnswered { get; set; }

        [JsonProperty("view_count")]
        public int ViewCount { get; set; }

        [JsonProperty("answer_count")]
        public int AnswerCount { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("last_activity_date")]
        [JsonConverter(typeof(SimpleJavascriptDateTimeConverter))]
        public DateTime LastActivityDate { get; set; }

        [JsonProperty("creation_date")]
        [JsonConverter(typeof(SimpleJavascriptDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        [JsonProperty("last_edit_date")]
        [JsonConverter(typeof(SimpleJavascriptDateTimeConverter))]
        public DateTime LastEditDate { get; set; }

        [JsonProperty("question_id")]
        public int QuestionId { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("title")]
        [JsonConverter(typeof(HtmlStringConverter))]
        public string Title { get; set; }

        [JsonProperty("tags")]
        public IEnumerable<string> Tags { get; set; }
    }

    public class QuestionsQueryResult
    {
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("quota_max")]
        public int QuotaMax { get; set; }

        [JsonProperty("quota_remaining")]
        public int QuotaRemaining { get; set; }

        [JsonProperty("items")]
        public IEnumerable<Question> Items { get; set; }
    }
}
