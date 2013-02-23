using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{

    public class NonQueryStatement : Statement
    {
        internal IEnumerable<NonQueryAction> Actions { get; private set; }

        internal NonQueryStatement(String connectionString, IEnumerable<NonQueryAction> actions)
            : base(connectionString)
        {
            this.Actions = new List<NonQueryAction>(actions);
        }

        public void Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                Fluent.ExecuteBulk(this.Actions, cn);
            }
        }
    }
}
