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
    public class TripDAL
    {
        public static List<Trip> GetTripsByUser(int userId)
        {
            List<Trip> trips = new List<Trip>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Trip WHERE UserID = @UserID", DbHelper.GetConnection());
            cmd.Parameters.AddWithValue("@UserID", userId);

            try
            {
                DbHelper.OpenConnection();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    trips.Add(new Trip
                    {
                        TripID = Convert.ToInt32(reader["TripID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        TripName = reader["TripName"].ToString(),
                        StartDate = Convert.ToDateTime(reader["StartDate"]),
                        EndDate = Convert.ToDateTime(reader["EndDate"]),
                        TravelMethod = reader["TravelMethod"].ToString(),
                        Notes = reader["Notes"].ToString()
                    });
                }
                reader.Close();
            }
            finally
            {
                DbHelper.CloseConnection();
            }

            return trips;
        }
    }
}
