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
    public class CityDAL
    {
        public static List<City> GetCitiesByCountry(int countryId)
        {
            List<City> cities = new List<City>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM City WHERE CountryID = @CountryID", DbHelper.GetConnection());
            cmd.Parameters.AddWithValue("@CountryID", countryId);

            try
            {
                DbHelper.OpenConnection();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cities.Add(new City
                    {
                        CityID = Convert.ToInt32(reader["CityID"]),
                        CountryID = Convert.ToInt32(reader["CountryID"]),
                        CityName = reader["CityName"].ToString()
                    });
                }
                reader.Close();
            }
            finally
            {
                DbHelper.CloseConnection();
            }

            return cities;
        }
    }
}
