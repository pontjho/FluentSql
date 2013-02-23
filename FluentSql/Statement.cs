using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentSql
{

    public class Statement
    {
        internal String ConnectionString { get; private set; }

        public Statement(String connectionString)
        {
            this.ConnectionString = connectionString;
        }
    }
}
