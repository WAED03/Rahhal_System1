using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace Rahhal_System1.DAL
{
    public static class DbHelper
    {
        private static readonly string connectionString = "Data Source=DESKTOP-0S2FHEG\\SQLEXPRESS;Database=RAHHAL;User Id=sa;Password=Test@123;";

        private static SqlConnection connection;

        public static SqlConnection GetConnection()
        {
            if (connection == null)
                connection = new SqlConnection(connectionString);

            return connection;
        }

        public static void OpenConnection()
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
        }

        public static void CloseConnection()
        {
            if (connection.State != System.Data.ConnectionState.Closed)
                connection.Close();
        }
    }
}
