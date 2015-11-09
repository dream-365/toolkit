using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using System;

namespace EasyAnalysis.Backend
{
    public class DefaultActionFactory : IActionFactory
    {
        public IAction CreateInstance(string name)
        {
            switch(name.ToLower())
            {
                case "build-thread-profiles":
                    return new Actions.BuildThreadProfiles
                        (new SqlServerConnectionStringProvider(),
                         new MongoDBConnectionStringProvider() );
                case "extract-asker-activies":
                    return new Actions.ExtractAskerActivies(new MongoDBConnectionStringProvider());
                case "import-new-users":
                    return new Actions.ImportNewUsers(new MongoDBConnectionStringProvider());

                default: throw new NotImplementedException(string.Format("Action[{0}] is not supported yet"));
            }
        }
    }
}
