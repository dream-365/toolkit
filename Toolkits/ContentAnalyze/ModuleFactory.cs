using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentAnalyze
{
    public class ModuleFactory
    {
        public IContentModule Create(string name, IEnumerable<string> parameters)
        {
            IContentModule module;

            switch(name.ToLowerInvariant())
            {
                case "sentences-module": module = new SentencesModule(); break;
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
