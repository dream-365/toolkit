using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public class CacheManagement
    {
        private readonly string _root;

        public CacheManagement(string root)
        {
            _root = root;
        }

        public void CreateCacheManagementDBIfNotExists()
        {
            var dbFilePath = Path.Combine(_root, "cache_mgr.db");

            if(!File.Exists(dbFilePath))
            {
                using (var connection = new System.Data.SQLite.SQLiteConnection("Data Source=" + dbFilePath + ";Version=3;"))
                {
                    var createTableCmd = connection.CreateCommand();

                    createTableCmd.CommandText = "";
                }
            }

            


        }

    }
}
