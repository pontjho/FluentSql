using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{

    public class NonQueryStatement : Statement
    {
        internal IEnumerable<String> Actions { get; private set; }

        internal NonQueryStatement(String connectionString, IEnumerable<String> actions)
            : base(connectionString)
        {
            this.Actions = new List<String>(actions);
        }

        public void Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                Actions.ToList().ForEach(actionText => Fluent.ExecuteSqlCode(actionText, cn));
            }
        }
    }
}
