using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{
    public delegate void NonQueryAction(SqlConnection conn, Object[] paramaters);
}
