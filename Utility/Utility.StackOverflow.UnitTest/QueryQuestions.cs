using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web;
using System.Threading.Tasks;

namespace Utility.StackOverflow.UnitTest
{
    [TestClass]
    public class QueryQuestions
    {
        [TestMethod]
        public async Task JsonDeserizeTest()
        {
            var collection = new QuestionCollection("uwp");

            var items = await collection.NavigateToPageAsync(1);
        }
    }
}
