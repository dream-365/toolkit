using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utility.StackOverflow
{
    public class SimpleJavascriptDateTimeConverter : DateTimeConverterBase
    {
        private const long InitialJavaScriptDateTicks = 621355968000000000;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            long ticks = (long)reader.Value;

            DateTime dateTime = new DateTime((ticks * 10000000) + InitialJavaScriptDateTicks, DateTimeKind.Utc);

            return dateTime;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotFiniteNumberException();
        }
    }
}
