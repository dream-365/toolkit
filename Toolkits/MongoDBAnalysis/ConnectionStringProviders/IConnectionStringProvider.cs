using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBAnalysis.ConnectionStringProviders
{
    public interface IConnectionStringProvider
    {
        /// <summary>
        /// Get the Connection String
        /// </summary>
        /// <param name="dBName">DB Name</param>
        /// <returns></returns>
        string GetConnectionString(string dBName);

        /// <summary>
        /// Execute Dispose
        /// </summary>
        void Dispose();
    }
}