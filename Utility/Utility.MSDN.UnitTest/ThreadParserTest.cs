using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Utility.MSDN.UnitTest
{
    [TestClass]
    public class ThreadParserTest
    {
        [TestMethod]
        public async Task ReadThreadInfoTest()
        {
            var parser = new ThreadParser(Guid.Parse("d0da6e88-6f7c-4450-9749-26efb44b098f"));

            var info = await parser.ReadThreadInfoAsync();

            // HAPPY_PASS
            Assert.IsNotNull(info);
        }
    }
}
