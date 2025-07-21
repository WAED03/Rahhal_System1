using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;


namespace Rahhal_System1.DAL
{
    public static class DbHelper
    {
        private static readonly string connectionString =
            "Data Source=DESKTOP-0S2FHEG\\SQLEXPRESS;Database=RAHHAL;User Id=sa;Password=Test@123;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}


