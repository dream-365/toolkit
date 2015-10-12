using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAnalysis
{
    public class UserAnalysisExample
    {
        public async Task<IDictionary<string, dynamic>> RunAskerAnalysis()
        {
            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");

            var users = forums.GetCollection<BsonDocument>("users");

            var threads = forums.GetCollection<BsonDocument>("uwp_sep_threads");

            var result = new Dictionary<string, dynamic>();

            await threads
                .Aggregate()
                .Group("{ _id : '$authorId', total: { $sum: 1 } }")
                .Sort("{total: -1}")
                .Limit(20)
                .ForEachAsync(async (item) =>
                {
                    var userId = item.GetElement("_id").Value.AsString;

                    var total = item.GetElement("total").Value.AsInt32;

                    var user = await users.Find("{id: \"" + userId + "\"}").FirstOrDefaultAsync();

                    dynamic container;

                    if (result.ContainsKey(userId))
                    {
                        container = result[userId];
                    }
                    else
                    {
                        container = new ExpandoObject();

                        container.display_name = user.GetElement("display_name").Value.AsString;

                        container.id = userId;

                        result[userId] = container;
                    }

                    container.total = total;

                    var answered = await threads.Find(string.Format("{{ authorId: \"{0}\", answered: \"true\" }}", userId)).CountAsync();

                    container.answered = answered;
                });

            return result;
        }

        public async Task<IDictionary<string, dynamic>> RunAnswerAnalysis(string userType)
        {
            var client = new MongoClient("mongodb://rpt:$DB$1passAz@app-svr.cloudapp.net:27017/forums");

            var forums = client.GetDatabase("forums");

            var users = forums.GetCollection<BsonDocument>("users");

            var threads = forums.GetCollection<BsonDocument>("uwp_sep_threads");

            var result = new Dictionary<string, dynamic>();

            var filter = Builders<BsonDocument>.Filter.Eq(userType, "true");

            await users.Find(filter).ForEachAsync(async (user) =>
            {
                await CalculateParticaptionCount(user, threads, result);
            });

            return result;
        }

        private static async Task CalculateParticaptionCount(BsonDocument user, IMongoCollection<BsonDocument> threads, Dictionary<string, dynamic> result)
        {
            var userId = user.GetElement("id").Value.AsString;

            var viewsProjection = Builders<BsonDocument>.Projection.Include("views").Exclude("_id");

            dynamic container;

            if (result.ContainsKey(userId))
            {
                container = result[userId];
            }
            else
            {
                container = new ExpandoObject();

                container.DisplayName = user.GetElement("display_name").Value.AsString;

                result[userId] = container;
            }

            container.Total = await threads.Find(string.Format("{{authorId: {{$ne: \"{0}\"}}, messages: {{ $elemMatch: {{ authorId: \"{0}\" }} }} }}", userId)).CountAsync();

            container.Answered = await threads.Find(string.Format("{{authorId: {{$ne: \"{0}\"}}, answered: \"true\", messages: {{ $elemMatch: {{ authorId: \"{0}\" }} }} }}", userId)).CountAsync();

            container.AnsweredByMe = await threads.Find(string.Format("{{authorId: {{$ne: \"{0}\"}}, answered: \"true\", messages: {{ $elemMatch: {{ authorId: \"{0}\", is_answer: \"true\" }} }} }}", userId)).CountAsync();

            var views = await threads
                .Find(string.Format("{{ messages: {{ $elemMatch: {{ authorId: \"{0}\" }} }} }}", userId))
                .Project(viewsProjection).ToListAsync();

            int totalViews = 0;

            foreach(var view in views)
            {
                totalViews = totalViews + view.GetElement("views").Value.AsInt32;
            }

            container.TotalViews = totalViews;
        }
    }
}
