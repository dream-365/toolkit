using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text.RegularExpressions;

namespace Utility.HttpCache.UnitTest
{
    [TestClass]
    public class LocalFileSystemHttpCacheProviderTest
    {
        [TestMethod]
        public void TestIsCached()
        {
            var provider = DefaultLocalFileSystemHttpCacheProvider.Current;

            Regex urlRegex = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");

            var resolver = new RegexPathResolver(urlRegex, "thread_id_{1}");

            provider.Configure(@"D:\temp\cache", resolver);

            var isCached = provider.IsCached(new Uri("https://social.msdn.microsoft.com/Forums/windowsapps/en-US/2f2dd185-e7f7-4dcd-aaf9-f7fc09c8716d/uwpc-how-to-see-what-a-string-contains-with-certain-length?forum=wpdevelop"));

            Assert.IsTrue(isCached);
        }

        [TestMethod]
        public void TestGetCache()
        {
            var provider = DefaultLocalFileSystemHttpCacheProvider.Current;

            Regex urlRegex = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");

            var resolver = new RegexPathResolver(urlRegex, "thread_id_{1}");

            provider.Configure(@"D:\temp\cache", resolver);

            var cache = provider.GetCache(new Uri("https://social.msdn.microsoft.com/Forums/windowsapps/en-US/2f2dd185-e7f7-4dcd-aaf9-f7fc09c8716d/uwpc-how-to-see-what-a-string-contains-with-certain-length?forum=wpdevelop"));

            var sr = new StreamReader(cache);

            var text = sr.ReadLine();

            sr.Close();
        }

        [TestMethod]
        public void TestCache()
        {
            var provider = DefaultLocalFileSystemHttpCacheProvider.Current;
            Regex urlRegex = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");

            var resolver = new RegexPathResolver(urlRegex, "thread_id_{1}");

            provider.Configure(@"D:\temp\cache", resolver);

            using (var mem = new MemoryStream())
            using (var sw = new StreamWriter(mem))
            {
                sw.WriteLine("HELLO Cache");

                sw.Flush();

                mem.Position = 0;

                provider.Cache(mem, new Uri("https://social.msdn.microsoft.com/Forums/windowsapps/en-US/2f2dd185-e7f7-4dcd-aaf9-f7fc09c8716d/uwpc-how-to-see-what-a-string-contains-with-certain-length?forum=wpdevelop"));
            }
        }
    }
}
