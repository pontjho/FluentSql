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

        public ExecutableStatement(String connectionString, IEnumerable<NonQueryAction> actions) : base(connectionString)
        {
            this.NonQueryActions = actions;
        }
    }
}
