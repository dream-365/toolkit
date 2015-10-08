using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentAnalyze
{
    public class ModuleFactory
    {
        public IMetadataModule Create(string name, IEnumerable<string> parameters)
        {
            IMetadataModule module;

            switch(name.ToLowerInvariant())
            {
                case "xpath-kv-module": module = new XPathKeyValueModule(); break;
                case "xpath-attribute-module": module = new XPathAttributeModule(); break;
                case "msdn-xml-module": module = new MSDNThreadMeatadataModule(); break;
                default: module = null; break;            
            }

            if(module == null)
            {
                throw new ArgumentException(string.Format("module[{0}] is not supported", name));
            }

            module.Init(parameters);

            return module;
        }
    }
}
