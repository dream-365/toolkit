using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Linq;

namespace Utility.MSDN.UnitTest
{
    [TestClass]
    public class ThreadParserTest
    {
        [TestMethod]
        public async Task ReadThreadInfoTest()
        {
            var parser = new ThreadParser(Guid.Parse("18273ef6-92f8-4f11-818d-7e3b8470307a"));

            var info = await parser.ReadThreadInfoAsync();

            // HAPPY_PASS
            Assert.IsNotNull(info);
        }

        [TestMethod]
        public async Task ThreadCollectionTest()
        {
            var collection = new ThreadCollection("wpdevelop");

            var threads = await collection.NavigateToPageAsync(1);

            Assert.AreEqual(20, threads.Count());
        }
    }
}
