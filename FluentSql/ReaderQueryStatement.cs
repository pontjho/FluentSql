using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{

    public class ReaderQueryStatement<T> : ExecutableStatement
    {
        internal String ScalarAction { get; private set; }
        internal Func<SqlDataReader, T> ParseLine { get; private set; }

        internal ReaderQueryStatement(String connectionString, 
                                      NonQueryStatement siblingStatement, 
                                      String scalarAction,
                                      Func<SqlDataReader, T> lineReader,
                                      IEnumerable<ScalarQueryStatement<Object>> dependencies)
            : base(connectionString, siblingStatement, dependencies)
        {
            this.ScalarAction = scalarAction;
            this.ParseLine = lineReader;
        }

        public IEnumerable<T> Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();

                if(this.SiblingStatement != null)
                    SiblingStatement.ExecuteSiblings(cn);

                var cmd = new SqlCommand(this.ScalarAction, cn);

                var resultSet = cmd.ExecuteReader();

                var theReturn = new List<T>();
                while (resultSet.Read())
                {
                    theReturn.Add(this.ParseLine(resultSet));
                }
                return theReturn;
            }
        }
    }
}
