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

        internal void ExecuteSiblings(SqlConnection cn, SqlTransaction trans)
        {
            if (SiblingStatement != null)
                SiblingStatement.ExecuteSiblings(cn, trans);
            this.Action(cn, trans, base.EvaluateDependencies(cn, trans));
        }

        public void Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    trans.Commit();
                    if (SiblingStatement != null)
                        SiblingStatement.ExecuteSiblings(cn, trans);
                    this.Action(cn, trans, base.EvaluateDependencies(cn, trans));

                }
            }
        }

        public void ExecuteAsTransaction()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    if (SiblingStatement != null)
                        SiblingStatement.ExecuteSiblings(cn, trans);
                    this.Action(cn, trans, base.EvaluateDependencies(cn, trans));

                    trans.Commit();
                }
            }
        }
    }
}
