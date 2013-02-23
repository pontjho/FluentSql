using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{

    public class NonQueryStatement : ExecutableStatement
    {
        internal NonQueryStatement(String connectionString, IEnumerable<NonQueryAction> nonQueryActions)
            : base(connectionString, nonQueryActions)
        {
        }
    }
}
