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

        internal static void ExecuteSqlCode(string sqlCommandText, SqlConnection conn)
        {
            var cmd = new SqlCommand(sqlCommandText, conn);
            cmd.ExecuteNonQuery();
        }
        public static NonQueryStatement WithNonQueryStatement(this NonQueryStatement statement, String newStatement)
        {
            var newQueries = statement.Actions.Union(new String[] { newStatement });
            return new NonQueryStatement(statement.ConnectionString, newQueries);
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
            var newQueries = new String[] { newStatement };
            return new NonQueryStatement(statement.ConnectionString, newQueries);
        }

        public static ScalarQueryStatement<T> WithScalarQueryStatement<T>(this Statement statement, String newQueryStatement)
        {
            var newQueries = new String[] { newQueryStatement };
            return new ScalarQueryStatement<T>(statement.ConnectionString, newQueries, newQueryStatement);
        }

        public static ReaderQueryStatement<T> WithReaderQueryStatement<T>(this Statement statement, String newQueryStatement, Func<SqlDataReader, T> readerFunction)
        {
            var newQueries = new String[] { newQueryStatement };
            return new ReaderQueryStatement<T>(statement.ConnectionString, newQueries, newQueryStatement, readerFunction);
        }
    }

    public class ReaderQueryStatement<T>
    {
        private List<String> NonQueryActions { get; set; }
        private String ConnectionString { get; set; }
        private String ScalarAction { get; set; }
        private Func<SqlDataReader, T> ParseLine { get; set; }

        internal ReaderQueryStatement(String connectionString, IEnumerable<String> nonQueryActions, String scalarAction, Func<SqlDataReader, T> lineReader)
        {
            this.NonQueryActions = new List<String>(nonQueryActions);
            this.ConnectionString = connectionString;
            this.ScalarAction = scalarAction;
            this.ParseLine = lineReader;
        }

        public IEnumerable<T> Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                NonQueryActions.ToList().ForEach(actionText => Fluent.ExecuteSqlCode(actionText, cn));

                var cmd = new SqlCommand(this.ScalarAction, cn);

                var resultSet = cmd.ExecuteReader();

                var theReturn = new List<T>();
                while (resultSet.Read())
                {
                    theReturn.Add(ParseLine(resultSet));
                }
                return theReturn;
            }
        }
    }
}
