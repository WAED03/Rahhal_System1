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
    public class CityVisitDAL
    {
        public static List<CityVisit> GetVisitsByTrip(int tripId)
        {
            List<CityVisit> visits = new List<CityVisit>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM CityVisit WHERE TripID = @TripID", DbHelper.GetConnection());
            cmd.Parameters.AddWithValue("@TripID", tripId);

            try
            {
                DbHelper.OpenConnection();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    visits.Add(new CityVisit
                    {
                        VisitID = Convert.ToInt32(reader["VisitID"]),
                        TripID = Convert.ToInt32(reader["TripID"]),
                        CityID = Convert.ToInt32(reader["CityID"]),
                        VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                        Rating = reader["Rating"].ToString(),
                        Notes = reader["Notes"].ToString()
                    });
                }
                reader.Close();
            }
            finally
            {
                DbHelper.CloseConnection();
            }

            return visits;
        }
    }
}
