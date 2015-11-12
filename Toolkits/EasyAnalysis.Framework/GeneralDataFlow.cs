using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework
{
    public class GeneralDataFlow
    {
        ICacheService _cacheService;

        IActionFactory _actionFactory;

        IURIDiscovery _URIDiscovery;

        public GeneralDataFlow(IURIDiscovery uriDiscovery, ICacheService cacheServcie, IActionFactory actionFactory)
        {
            _URIDiscovery = uriDiscovery;

            _cacheService = cacheServcie;

            _actionFactory = actionFactory;
        }

        public void Run()
        {
            _URIDiscovery.OnNew += URIDiscovery_OnNew;

            _URIDiscovery.Start();
        }

        private void URIDiscovery_OnNew(string url)
        {
            
        }
    }
}
