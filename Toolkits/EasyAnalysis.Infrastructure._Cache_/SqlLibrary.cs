using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Infrastructure.Cache
{
    public class SqlLibrary
    {
        public static SqlLibrary Instance = new SqlLibrary();

        SqlLibrary()
        {

        }

        public string Require(string name)
        {
            return SQL.ResourceManager.GetString(name);
        }
    }
}
