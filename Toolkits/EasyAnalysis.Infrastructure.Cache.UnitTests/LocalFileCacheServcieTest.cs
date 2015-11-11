using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace EasyAnalysis.Infrastructure.Cache.UnitTests
{
    [TestClass]
    public class LocalFileCacheServcieTest
    {
        [TestMethod]
        public void CreateClientTest()
        {
            var dbFilePath = @"D:\forum_cache\index.sqlite3";

            if (File.Exists(dbFilePath))
            {
                File.Delete(dbFilePath);
            }

            var service = new LocalFileCacheServcie();

            service.Configure(@"D:\forum_cache");

            var client = service.CreateClient();
        }

        [TestMethod]
        public void CacheTest()
        {
            var service = new LocalFileCacheServcie();

            service.Configure(@"D:\forum_cache");

            var client = service.CreateClient();

            client.SetCache(new Uri("http://www.localhost.com"), null);

            var cache = client.GetCache(new Uri("http://www.localhost.com"));
        }
    }
}
