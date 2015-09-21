using EasyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace EasyAnalysis.Repository
{
    public class FeedFactory
    {
        private const string _todo_item_query = @"SELECT TOP 10 RTRIM([Id]) AS [Id]
      ,[Title]
      ,[CreateOn]
  FROM [uwpdb].[dbo].[VwTodoList] ORDER BY [CreateOn] DESC";

        public IEnumerable<TodoItem> GenerateTodoItems(string repository)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var connection = new SqlConnection(cs))
            {
                return connection.Query<TodoItem>(_todo_item_query);
            }
        }
    }
}