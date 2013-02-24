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

        internal static void ExecuteSqlCode(string sqlCommandText, SqlConnection conn, params Object[] parameters)
        {
            var text = String.Format(sqlCommandText, parameters);
            var cmd = new SqlCommand(text, conn);
            cmd.ExecuteNonQuery();
        }

        /*internal static void ExecuteBulk(this ExecutableStatement head, SqlConnection connection)
        {
            //actions.ToList().ForEach(action => action(connection));
            if (head.SiblingStatement != null)
                ExecuteBulk(head.SiblingStatement, connection);
            head.Action();
        }*/

        public static NonQueryStatement WithNonQueryStatement(this NonQueryStatement statement, String newStatement)
        {
            //var newQueries = statement.NonQueryActions.Union(new NonQueryAction[] { newStatement.AsNonQueryAction() });
            return new NonQueryStatement(statement.ConnectionString, statement, statement.Dependencies, newStatement.AsNonQueryAction());
        }

        private static NonQueryAction AsParameterisedDelegate(String stuff)
        {
            return (conn, paramis) => Fluent.ExecuteSqlCode(stuff, conn, paramis);
        }

        private static NonQueryAction AsNonQueryAction(this String newStatement)
        {
                //FunctionalLib.CurryExtension.Curry<SqlConnection, String>(Fluent.ExecuteSqlCode, newStatement);
            NonQueryAction newAction = AsParameterisedDelegate(newStatement);
            return newAction;
        }

        public static ScalarQueryStatement<T> WithScalarQueryStatement<T>(this NonQueryStatement statement, String newQueryStatement)
        {
            return new ScalarQueryStatement<T>(statement.ConnectionString, statement, newQueryStatement, statement.Dependencies);
        }

        public static ReaderQueryStatement<T> WithReaderQueryStatement<T>(this NonQueryStatement statement, String newQueryStatement, Func<SqlDataReader, T> readerFunction)
        {
            return new ReaderQueryStatement<T>(statement.ConnectionString, statement, newQueryStatement, readerFunction, statement.Dependencies);
        }

        /*          */
        public static NonQueryStatement WithNonQueryStatement(this Statement statement, String newStatement)
        {
            return new NonQueryStatement(statement.ConnectionString, null, new ScalarQueryStatement<Object>[0], newStatement.AsNonQueryAction());
        }

        public static NonQueryStatement WithNonQueryAction(this NonQueryStatement statement, NonQueryAction action)
        {
            return new NonQueryStatement(statement.ConnectionString, statement, new ScalarQueryStatement<Object>[0], action);
        }

        public static NonQueryStatement WithNonQueryAction(this Statement statement, NonQueryAction action)
        {
            return new NonQueryStatement(statement.ConnectionString, null, new ScalarQueryStatement<Object>[0], action);
        }

        /*public static ScalarQueryStatement<T> WithScalarQueryStatement<T>(this Statement statement, String newQueryStatement)
        {
            return new ScalarQueryStatement<T>(statement.ConnectionString, new NonQueryAction[0], newQueryStatement);
        }*/

        public static ReaderQueryStatement<T> WithReaderQueryStatement<T>(this Statement statement, String newQueryStatement, Func<SqlDataReader, T> readerFunction)
        {
            return new ReaderQueryStatement<T>(statement.ConnectionString, null, newQueryStatement, readerFunction, new ScalarQueryStatement<Object>[0]);
        }

        public static IEnumerable<NonQueryAction> AsNonQueryActionList(this String statement)
        {
            return statement.AsNonQueryAction().AsEnumerable();
        }

        public static NonQueryStatement WithDependency(this NonQueryStatement statement, String dependenceText)
        {
            ScalarQueryStatement<Object> dependency = new ScalarQueryStatement<object>(statement.ConnectionString, null, dependenceText, new ScalarQueryStatement<Object>[0]);
            var dependencies = statement.Dependencies.Union(dependency.AsEnumerable());
            return new NonQueryStatement(statement.ConnectionString, statement.SiblingStatement, dependencies, statement.Action);
        }

        public static ScalarQueryStatement<T> WithDependency<T>(this ScalarQueryStatement<T> statement, ScalarQueryStatement<Object> dependency)
        {
            var dependencies = statement.Dependencies.Union(dependency.AsEnumerable());
            return new ScalarQueryStatement<T>(statement.ConnectionString, statement.SiblingStatement, statement.ScalarAction, dependencies);
        }

        public static ReaderQueryStatement<T> WithDependency<T>(this ReaderQueryStatement<T> statement, ScalarQueryStatement<Object> dependency)
        {
            var dependencies = statement.Dependencies.Union(dependency.AsEnumerable());
            return new ReaderQueryStatement<T>(statement.ConnectionString, statement.SiblingStatement, statement.ScalarAction, statement.ParseLine, dependencies);
        }

        private static IEnumerable<T> AsEnumerable<T>(this T newQueryStatement)
        {
            var newQueries = new T[] { newQueryStatement };
            return newQueries;
        }
    }
}
