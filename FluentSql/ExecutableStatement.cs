using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentSql
{
    public abstract class ExecutableStatement : Statement
    {
        internal NonQueryStatement SiblingStatement { get; private set; }
        internal IEnumerable<ScalarQueryStatement<Object>> Dependencies { get; private set; }

        public ExecutableStatement(String connectionString,
                                   NonQueryStatement siblingStatement,
                                   IEnumerable<ScalarQueryStatement<Object>> dependencies)
            : base(connectionString)
        {
            this.SiblingStatement = siblingStatement;
            this.Dependencies = dependencies;
        }

        internal Object[] EvaluateDependencies(SqlConnection cn)
        {
            return this.Dependencies.Select(dep => dep.Execute(cn)).ToArray();
        }
    }
}
