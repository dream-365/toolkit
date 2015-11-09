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

        public string GetConnectionString(string database = null)
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
    }
}
