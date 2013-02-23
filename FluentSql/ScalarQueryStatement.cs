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
                                      IEnumerable<NonQueryAction> nonQueryActions, 
                                      String scalarAction,
                                      IEnumerable<ScalarQueryStatement<Object>> dependencies)
            : base(connectionString, nonQueryActions, dependencies)
        {
            this.ScalarAction = scalarAction;
        }
    }
}
