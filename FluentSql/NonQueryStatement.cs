using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{

    public class NonQueryStatement : ExecutableStatement
    {
        internal NonQueryAction Action { get; private set; }

        internal NonQueryStatement(String connectionString,
                                   NonQueryStatement siblingStatement,
                                   IEnumerable<ScalarQueryStatement<Object>> dependencies,
                                   NonQueryAction action)
            : base(connectionString, siblingStatement, dependencies)
        {
            this.Action = action;
        }

        internal void ExecuteSiblings(SqlConnection cn)
        {
            if (SiblingStatement != null)
                SiblingStatement.ExecuteSiblings(cn);
            this.Action(cn, base.EvaluateDependencies(cn));
        }

        public void Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                if (SiblingStatement != null)
                    SiblingStatement.ExecuteSiblings(cn);
                this.Action(cn, base.EvaluateDependencies(cn));
            }
        }
    }
}
