using Rahhal_System1.DAL;
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.DAL
{
    public class CountryDAL
    {
        public static List<Country> GetAllCountries()
        {
            List<Country> countries = new List<Country>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Country", DbHelper.GetConnection());

            try
            {
                DbHelper.OpenConnection();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    countries.Add(new Country
                    {
                        CountryID = Convert.ToInt32(reader["CountryID"]),
                        CountryName = reader["CountryName"].ToString(),
                        Continent = reader["Continent"].ToString()
                    });
                }
                reader.Close();
            }
            finally
            {
                DbHelper.CloseConnection();
            }

            return countries;
        }
    }
}
