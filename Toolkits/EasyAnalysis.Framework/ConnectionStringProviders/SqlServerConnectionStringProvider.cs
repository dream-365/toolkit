using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Framework.ConnectionStringProviders
{
    public class SqlServerConnectionStringProvider : IConnectionStringProvider
    {
        public void Dispose()
        {
            // nothing to disponse
        }

        public string GetConnectionString(string name = null)
        {
            if(string.IsNullOrEmpty(name))
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }

            var node = ConfigurationManager.ConnectionStrings[name];

            if (node == null)
            {
                throw new Exception(string.Format("the connection name {0} is not found", name));
            }

            return node.ConnectionString;
        }
    }
}
