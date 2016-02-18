using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextTransform;

namespace JsonTransform
{
    class Program
    {
        /// <summary>
        /// -transform trans.trf -source raw.json -target out.json
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("-transform trans.trf -source raw.json -target out.json");

                return;
            }

            // parse parameters
            var parameters = new Dictionary<string, string>();

            int start = 0;

            while (start < args.Length)
            {
                var pair = args.Skip(start).Take(2);

                parameters.Add(pair.ElementAt(0).TrimStart('-'), pair.ElementAt(1));

                start = start + 2;
            }

            var sourceFilePah = parameters["source"];

            var transformMappingFilePath = parameters["transform"];

            var targetFilePath = parameters["target"];

            var functinProvier = new FunctionProvider();

            IEnumerable<TransformColumnDefination> mappings = JsonConvert
                .DeserializeObject<IEnumerable<TransformColumnDefination>>(
                File.ReadAllText(transformMappingFilePath), 
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var columnMappings = new List<ColumnMap>();

            foreach(var column in mappings)
            {
                var map = new ColumnMap
                {
                    SourcePropertyName = column.SourcePropertyName,
                    TargetPropertyName = column.TargetPropertyName
                };

                if(!string.IsNullOrWhiteSpace(column.Transform))
                {
                    var pattern = new Regex(@"^([A-z]+)\((.+)\)");

                    var match = pattern.Match(column.Transform);

                    if(!match.Success)
                    {
                        throw new InvalidDataException("invalid transform: " + column.Transform);
                    }

                    var funName = match.Groups[1].Value;

                    var funArgs = match.Groups[2].Value.Split(',').Select(v => v.Trim().Trim('\'')).ToArray();

                    ITextTransformFunction fun = functinProvier.Get(funName);

                    if(fun == null)
                    {
                        throw new InvalidDataException("invalid function name: " + funName);
                    }

                    map.Transform = (value) => {
                        return fun.Call(value, funArgs);
                    };
                }

                columnMappings.Add(map);
            }

            using (var fs = new FileStream(sourceFilePah, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
            using (var targetFs = new FileStream(targetFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            using (var sw = new StreamWriter(targetFs))
            {
                while (!sr.EndOfStream)
                {
                    var text = sr.ReadLine();

                    JObject item = JsonConvert.DeserializeObject(text) as JObject;

                    JObject newItem = new JObject();

                    foreach (var mapping in columnMappings)
                    {
                        var value = item.GetValue(mapping.SourcePropertyName);

                        if (mapping.Transform != null)
                        {
                            var textValue = value.ToString();

                            var newValue = mapping.Transform(textValue);

                            newItem.Add(mapping.TargetPropertyName, new JValue(newValue));
                        }
                        else
                        {
                            newItem.Add(mapping.TargetPropertyName, value);
                        }
                    }

                    sw.WriteLine(JsonConvert.SerializeObject(newItem));
                    // save the new item
                }
            }
        }
    }
}
