using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentAnalyze
{
    public class JsonDocument
    {
        private readonly IEnumerable<IDictionary<string, object>> _data;

        public JsonDocument(string filePath)
        {
           
        }

        public void Count(string key)
        {
            IDictionary<string, int> countResult = new Dictionary<string, int>();

            foreach(var item in _data)
            {
                if(!item.ContainsKey(key))
                {
                    continue;
                }

                var value = item[key] as string;

                if(countResult.ContainsKey(value))
                {
                    var count = countResult[value];

                    countResult[value] = count + 1;
                }
                else
                {
                    countResult[value] = 1;
                }
            }
        }
    }
}
