using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentAnalyze
{
    public class ModuleConfigration
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("arguments")]
        public IEnumerable<string> Arguments { get; set; }
    }

    public class ContentProcessTaskConfigration
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("root_folder")]
        public string RootFolder { get; set; }

        [JsonProperty("encoding")]
        public string Encoding { get; set; }

        [JsonProperty("file_name_pattern")]
        public string FileNamePattern { get; set; }

        [JsonProperty("output_to")]
        public string OutputTo { get; set; }

        [JsonProperty("modules")]
        public IEnumerable<ModuleConfigration> Modules { get; set; }
    }
}
