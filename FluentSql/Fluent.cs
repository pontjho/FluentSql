using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace FluentSql
{
    public static class Fluent
    {
        private static String config = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        public static Statement Default = new Statement(config);

        internal static void ExecuteSqlCode(SqlConnection conn, string sqlCommandText)
        {
            var cmd = new SqlCommand(sqlCommandText, conn);
            cmd.ExecuteNonQuery();
        }

        internal static void ExecuteBulk(this IEnumerable<NonQueryAction> actions, SqlConnection connection)
        {
            actions.ToList().ForEach(action => action(connection));
        }



        public static NonQueryStatement WithNonQueryStatement(this NonQueryStatement statement, String newStatement)
        {
            var newQueries = statement.Actions.Union(new NonQueryAction[] { newStatement.AsNonQueryAction() });
            return new NonQueryStatement(statement.ConnectionString, newQueries);
        }

        private static NonQueryAction AsNonQueryAction(this String newStatement)
        {
            Action<SqlConnection> ya = FunctionalLib.CurryExtension.Curry<SqlConnection, String>(Fluent.ExecuteSqlCode, newStatement);
            NonQueryAction newAction = ya.Invoke;
            return newAction;
        }

        public static ScalarQueryStatement<T> WithScalarQueryStatement<T>(this NonQueryStatement statement, String newQueryStatement)
        {
            return new ScalarQueryStatement<T>(statement.ConnectionString, statement.Actions, newQueryStatement);
        }

        public static ReaderQueryStatement<T> WithReaderQueryStatement<T>(this NonQueryStatement statement, String newQueryStatement, Func<SqlDataReader, T> readerFunction)
        {
            return new ReaderQueryStatement<T>(statement.ConnectionString, statement.Actions, newQueryStatement, readerFunction);
        }

        /*          */
        public static NonQueryStatement WithNonQueryStatement(this Statement statement, String newStatement)
        {
            return new NonQueryStatement(statement.ConnectionString, newStatement.AsNonQueryActionList());
        }

        public static NonQueryStatement WithNonQueryAction(this Statement statement, NonQueryAction action)
        {
            return new NonQueryStatement(statement.ConnectionString, action.AsEnumerable());
        }

        /*public static ScalarQueryStatement<T> WithScalarQueryStatement<T>(this Statement statement, String newQueryStatement)
        {
            return new ScalarQueryStatement<T>(statement.ConnectionString, new NonQueryAction[0], newQueryStatement);
        }

        public static ReaderQueryStatement<T> WithReaderQueryStatement<T>(this Statement statement, String newQueryStatement, Func<SqlDataReader, T> readerFunction)
        {
            return new ReaderQueryStatement<T>(statement.ConnectionString, new NonQueryAction[0], newQueryStatement, readerFunction);
        }*/

        public static IEnumerable<NonQueryAction> AsNonQueryActionList(this String statement)
        {
            return statement.AsNonQueryAction().AsEnumerable();
        }

        private static IEnumerable<T> AsEnumerable<T>(this T newQueryStatement)
        {
            var newQueries = new T[] { newQueryStatement };
            return newQueries;
        }
    }
}
