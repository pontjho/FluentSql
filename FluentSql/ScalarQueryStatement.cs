using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{

    public class ScalarQueryStatement<T>
    {
        private List<NonQueryAction> NonQueryActions { get; set; }
        private String ConnectionString { get; set; }
        private String ScalarAction { get; set; }

        internal ScalarQueryStatement(String connectionString, IEnumerable<NonQueryAction> nonQueryActions, String scalarAction)
        {
            this.NonQueryActions = new List<NonQueryAction>(nonQueryActions);
            this.ConnectionString = connectionString;
            this.ScalarAction = scalarAction;
        }

        public T Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                NonQueryActions.ExecuteBulk(cn);

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
