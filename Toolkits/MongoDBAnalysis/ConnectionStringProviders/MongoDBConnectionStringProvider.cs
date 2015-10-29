using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAnalysis.ConnectionStringProviders
{
    public class MongoDBConnectionStringProvider : IConnectionStringProvider
    {
        private string connectionString;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the Connection String for MongoDB
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString(string dBName)
        {
            if (dBName == null || dBName.Trim() == string.Empty)
            {
                return connectionString;
            }
            else
            {
                return connectionString + "/" + dBName;
            }

        }

        public MongoDBConnectionStringProvider() : this(null)
        {
        }

        public MongoDBConnectionStringProvider(string connectionString)
        {
            if (connectionString == null || connectionString.Trim() == string.Empty)
            {
                try
                {
                    this.connectionString = ConfigurationManager.ConnectionStrings["MongoDBConnection"].ConnectionString;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                this.connectionString = connectionString;
            }
        }
    }
}
