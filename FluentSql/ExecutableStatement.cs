using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentSql
{
    public class ExecutableStatement : Statement
    {
        internal IEnumerable<NonQueryAction> NonQueryActions { get; private set; }
        internal IEnumerable<ScalarQueryStatement<Object>> Dependencies { get; private set; }

        public ExecutableStatement(String connectionString, 
                                   IEnumerable<NonQueryAction> actions,
                                   IEnumerable<ScalarQueryStatement<Object>> dependencies)
            : base(connectionString)
        {
            this.NonQueryActions = actions;
            this.Dependencies = dependencies;
        }
    }
}
