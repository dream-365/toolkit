using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyAnalysis.Models;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;

namespace EasyAnalysis.Repository
{
    public class TypeProvider : ITypeProvider
    {
        private readonly string _sql = @"
SELECT [Types].[Id] AS [Id]
	  ,[Types].[Name] AS [TypeName]
	  ,[Categories].[Name] AS [CategoryName]
	  ,[Categories].[Repository]
FROM [uwpdb].[dbo].[Types]
INNER JOIN [uwpdb].[dbo].[Categories]
ON [Types].[CategoryId] = [Categories].[Id]
WHERE [Categories].[Repository] = 'UWP'
";

        public IEnumerable<TypeModel> GetTypesByRepository(string repository)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var connection = new SqlConnection(cs))
            {
                return connection.Query<TypeModel>(_sql);
            }
        }
    }
}