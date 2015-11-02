using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Infrastructure.Cache
{
    public class LocalFileCacheServcie : ICacheService
    {
        private bool _isInitialized = false;

        private string _rootFolder;

        public ICacheClient CreateClient()
        {
            Initialize();

            return new LocalFileCacheClient();
        }

        public void Configure(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        private void Initialize()
        {
            if(_isInitialized)
            {
                return;
            }

            // set up SQLite local cache index

            _isInitialized = true;
        }
    }
}
