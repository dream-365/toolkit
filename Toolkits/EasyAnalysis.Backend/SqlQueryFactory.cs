using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Backend
{
    class SqlQueryFactory
    {
        public static SqlQueryFactory Instance = new SqlQueryFactory();

        private SqlQueryFactory()
        {

        }

        public string Get(string name)
        {
            try
            {
                var filePath = Path.Combine("Query", name + ".sql");

                var text = File.ReadAllText(filePath);

                return text;

            }catch(FileNotFoundException e)
            {
                throw new Exception("No SQL Statement found");
            }
        }
    }
}
