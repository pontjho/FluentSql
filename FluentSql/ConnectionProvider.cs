﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FluentSql
{
    internal interface ConnectionProvider
    {
        SqlConnection GetConnection();
    }
}
