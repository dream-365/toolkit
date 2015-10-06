using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Utility.HttpCache.UnitTest
{
    [TestClass]
    public class LocalFileSystemPathPathResolverTest
    {
        [TestMethod]
        public void TestDefaultResolve()
        {
            var resolver = new DefaultPathResolver();

            var path = resolver.Resolve(new Uri("https://social.msdn.microsoft.com/Forums/windowsapps/en-US/home?forum=wpdevelop&filter=alltypes&sort=firstpostdesc"));
        }

        [TestMethod]
        public void TestRegexResolve()
        {
            Regex urlRegex = new Regex(@"(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})");

            var resolver = new RegexPathResolver(urlRegex, "thread_id_{1}");

            var path = resolver.Resolve(new Uri("https://social.msdn.microsoft.com/Forums/windowsapps/en-US/2f2dd185-e7f7-4dcd-aaf9-f7fc09c8716d/uwpc-how-to-see-what-a-string-contains-with-certain-length?forum=wpdevelop"));
        }
    }
}
