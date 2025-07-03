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
    public class PhraseDAL
    {
        public static List<Phrase> GetPhrasesByVisit(int visitId)
        {
            List<Phrase> phrases = new List<Phrase>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Phrase WHERE VisitID = @VisitID", DbHelper.GetConnection());
            cmd.Parameters.AddWithValue("@VisitID", visitId);

            try
            {
                DbHelper.OpenConnection();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    phrases.Add(new Phrase
                    {
                        PhraseID = Convert.ToInt32(reader["PhraseID"]),
                        VisitID = Convert.ToInt32(reader["VisitID"]),
                        OriginalText = reader["OriginalText"].ToString(),
                        Translation = reader["Translation"].ToString(),
                        Language = reader["Language"].ToString(),
                        Notes = reader["Notes"].ToString()
                    });
                }
                reader.Close();
            }
            finally
            {
                DbHelper.CloseConnection();
            }

            return phrases;
        }
    }
}
