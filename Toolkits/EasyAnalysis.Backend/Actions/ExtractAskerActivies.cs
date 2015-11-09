using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace EasyAnalysis.Backend.Actions
{
    public class ExtractUserActivies : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        private class EmitScope : IDisposable
        {
            private SqlConnection _sqlConnection;

            public EmitScope(SqlConnection sqlConnection)
            {
                _sqlConnection = sqlConnection;
            }

            public void Emit(string userId, string action, DateTime time, string effectOn)
            {
                var signature = string.Format("{0}-{1}-{2:MM/dd/yy H:mm:ss}-{3}", userId, action.ToLower(), time, effectOn);

                var md5Hash = Utils.ComputeStringMD5Hash(signature);

                var match = _sqlConnection.Query(SqlQueryFactory.Instance.Get("find_user_activity_by_hash"), new { Hash = md5Hash });

                if(match.Count() == 0)
                {
                    _sqlConnection.Execute(
                        SqlQueryFactory.Instance.Get("insert_user_activity"),
                        new
                        {
                            Hash = md5Hash,
                            UserId = userId,
                            Action = action,
                            Time = time,
                            EffectOn = effectOn
                        });
                }

                PrintProgress();
            }

            public void Dispose()
            {
                _sqlConnection.Dispose();
            }

            private void PrintProgress()
            {
                Console.Write(".");
            }
        }

        public ExtractUserActivies(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public string Description
        {
            get
            {
                return "Run asker analysis to extract the actions of askers";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">[repository] [user collection name] [thread collection name] [target collection name]</param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            var repository = args[0];

            var threadCollectionName = args[1];

            var client = new MongoClient(_connectionStringProvider.GetConnectionString(string.Format("mongo:{0}", repository)));

            var database = client.GetDatabase(repository);

            var threadCollection = database.GetCollection<BsonDocument>(threadCollectionName);

            var list = await threadCollection.Find("{}").ToListAsync();

            using (var scope = new EmitScope(new SqlConnection(_connectionStringProvider.GetConnectionString("EasIndexConnection"))))
            {
                list.ForEach(item =>
                {
                    ExtractInThread(item, scope);
                });
            }
        }

        private static void ExtractInThread(BsonDocument item, EmitScope scope)
        {
            var threadId = item.GetValue("id").AsString;

            var authorId = item.GetValue("authorId").AsString;

            var createdOn = item.GetValue("createdOn").AsString;

            scope.Emit(authorId, "Ask", DateTime.Parse(createdOn), threadId);

            var messages = item.GetElement("messages").Value.AsBsonArray;

            foreach (BsonDocument message in messages)
            {
                ExtractInMessage(scope, threadId, message);
            }
        }

        private static void ExtractInMessage(EmitScope scope, string threadId, BsonDocument message)
        {
            var replyAuthorId = message.GetElement("authorId").Value.AsString;

            var replyOn = message.GetElement("createdOn").Value.AsString;

            scope.Emit(replyAuthorId, "Reply", DateTime.Parse(replyOn), threadId);

            BsonArray histories = message.GetElement("histories").Value.AsBsonArray;

            foreach (BsonDocument hisotry in histories)
            {
                ExtractInHistory(scope, threadId, hisotry);
            }
        }

        private static void ExtractInHistory(EmitScope insert, string threadId, BsonDocument hisotry)
        {
            var user = hisotry.GetValue("user").AsString;

            var date = hisotry.GetValue("date").AsString;

            var type = hisotry.GetValue("type").AsString;

            insert.Emit(user, type, DateTime.Parse(date), threadId);
        }
    }
}
