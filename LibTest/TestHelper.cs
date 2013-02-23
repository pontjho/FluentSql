using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTest
{
    public static class TestHelper
    {

        public static String GetTable()
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString))
            {
                cn.Open();
                var cmd = new SqlCommand("SELECT TABLE_NAME FROM Test.information_schema.tables", cn);
                return cmd.ExecuteScalar().ToString();
            }
        }
    }
}
