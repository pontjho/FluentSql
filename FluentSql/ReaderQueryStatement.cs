using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{

    public class ReaderQueryStatement<T>
    {
        private List<NonQueryAction> NonQueryActions { get; set; }
        private String ConnectionString { get; set; }
        private String ScalarAction { get; set; }
        private Func<SqlDataReader, T> ParseLine { get; set; }

        internal ReaderQueryStatement(String connectionString, IEnumerable<NonQueryAction> nonQueryActions, String scalarAction, Func<SqlDataReader, T> lineReader)
        {
            this.NonQueryActions = new List<NonQueryAction>(nonQueryActions);
            this.ConnectionString = connectionString;
            this.ScalarAction = scalarAction;
            this.ParseLine = lineReader;
        }

        public IEnumerable<T> Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                NonQueryActions.ExecuteBulk(cn);

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
