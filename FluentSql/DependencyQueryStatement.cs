using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentSql
{
    public class DependencyNonQueryStatement : Statement
    {
        internal IEnumerable<NonQueryAction> Actions { get; private set; }
        internal IEnumerable<ScalarQueryStatement<Object>> Dependencies { get; private set; }
        internal String QueryFormat { get; private set; }

        internal DependencyNonQueryStatement(String connectionString, IEnumerable<NonQueryAction> actions, IEnumerable<ScalarQueryStatement<Object>> dependencies, String queryFormat)
            : base(connectionString)
        {
            this.Actions = new List<NonQueryAction>(actions);
            this.Dependencies = dependencies;
            this.QueryFormat = queryFormat;
        }
    }
}
