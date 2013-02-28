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

        public T Execute(SqlConnection cn, SqlTransaction trans)
        {
            if (this.SiblingStatement != null)
                this.SiblingStatement.ExecuteSiblings(cn, trans);

            var actionText = String.Format(this.ScalarAction, base.EvaluateDependencies(cn, trans));

            var cmd = new SqlCommand(actionText, cn, trans);
            try
            {
                return (T)(Object)Int32.Parse(cmd.ExecuteScalar().ToString());
            }
            catch (FormatException)
            {
                return (T)cmd.ExecuteScalar();
            }
        }

        public T Execute()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    trans.Commit();
                    var theReturn = Execute(cn, trans);
                    return theReturn;
                }
            }
        }

        public T ExecuteAsTransaction()
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    var theReturn = Execute(cn, trans);
                    trans.Commit();
                    return theReturn;
                }
            }
        }
    }
}
