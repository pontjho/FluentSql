using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{
    public class ScalarQueryStatement<T> : ExecutableStatement
    {
        internal String ScalarAction { get; private set; }

        internal ScalarQueryStatement(String connectionString,
                                      NonQueryStatement siblingStatement,
                                      String scalarAction,
                                      IEnumerable<ScalarQueryStatement<Object>> dependencies)
            : base(connectionString, siblingStatement, dependencies)
        {
            this.ScalarAction = scalarAction;
        }

        public T Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();

                if(this.SiblingStatement != null)
                    this.SiblingStatement.Execute();

                var cmd = new SqlCommand(this.ScalarAction, cn);
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
    }
}
