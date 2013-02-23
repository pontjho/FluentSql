using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentSql
{
    public static class ExecutionHelper
    {
        public static void Execute(this NonQueryStatement statement)
        {
            using (var cn = new SqlConnection(statement.ConnectionString))
            {
                cn.Open();
                Fluent.ExecuteBulk(statement.NonQueryActions, cn);
            }
        }

        public static T Execute<T>(this ScalarQueryStatement<T> statement)
        {
            using (var cn = new SqlConnection(statement.ConnectionString))
            {
                cn.Open();
                statement.NonQueryActions.ExecuteBulk(cn);

                var cmd = new SqlCommand(statement.ScalarAction, cn);
                try
                {
                    return (T)(Object)Int32.Parse(cmd.ExecuteScalar().ToString());
                }
                catch (FormatException)
                {
                    return (T)cmd.ExecuteScalar();
                }
            }
        }

        public static IEnumerable<T> Execute<T>(this ReaderQueryStatement<T> statement)
        {
            using (var cn = new SqlConnection(statement.ConnectionString))
            {
                cn.Open();
                statement.NonQueryActions.ExecuteBulk(cn);

                var cmd = new SqlCommand(statement.ScalarAction, cn);

                var resultSet = cmd.ExecuteReader();

                var theReturn = new List<T>();
                while (resultSet.Read())
                {
                    theReturn.Add(statement.ParseLine(resultSet));
                }
                return theReturn;
            }
        }

        public static void Execute(this DependencyNonQueryStatement statement)
        {
            using (var cn = new SqlConnection(statement.ConnectionString))
            {
                cn.Open();
                Fluent.ExecuteBulk(statement.Actions, cn);

                var dependencyValues = statement.Dependencies.Select(dependency => dependency.Execute());

                String finalQuery = String.Format(statement.QueryFormat, dependencyValues);

                Fluent.ExecuteSqlCode(cn, finalQuery);
            }
        }
    }
}
