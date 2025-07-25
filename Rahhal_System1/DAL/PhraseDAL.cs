using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rahhal_System1.DAL
{
    public class PhraseDAL
    {
        // جلب كل العبارات المرتبطة بمستخدم معين حسب رحلاته
        public static List<Phrase> GetPhrasesByUser(int userId)
        {
            var phrases = new List<Phrase>();

            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT p.PhraseID, p.VisitID, p.OriginalText, p.Translation, p.Language, p.Notes, p.IsDeleted, p.UpdatedAt,
                           v.TripID, v.CityID, v.VisitDate, v.Rating, v.Notes as VisitNotes, v.IsDeleted as VisitIsDeleted, v.UpdatedAt as VisitUpdatedAt,
                           c.CityName, c.CountryID,
                           co.CountryName,
                           t.TripName, t.UserID
                    FROM Phrase p
                    INNER JOIN CityVisit v ON p.VisitID = v.VisitID
                    INNER JOIN City c ON v.CityID = c.CityID
                    INNER JOIN Country co ON c.CountryID = co.CountryID
                    INNER JOIN Trip t ON v.TripID = t.TripID
                    WHERE p.IsDeleted = 0 AND t.UserID = @UserID", con);

                cmd.Parameters.AddWithValue("@UserID", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var country = new Country
                        {
                            CountryID = Convert.ToInt32(reader["CountryID"]),
                            CountryName = reader["CountryName"].ToString()
                        };

                        var city = new City
                        {
                            CityID = Convert.ToInt32(reader["CityID"]),
                            CityName = reader["CityName"].ToString(),
                            CountryID = country.CountryID,
                            Country = country
                        };

                        var trip = new Trip
                        {
                            TripID = Convert.ToInt32(reader["TripID"]),
                            TripName = reader["TripName"].ToString(),
                            UserID = Convert.ToInt32(reader["UserID"])
                        };

                        var visit = new CityVisit
                        {
                            VisitID = Convert.ToInt32(reader["VisitID"]),
                            TripID = trip.TripID,
                            CityID = city.CityID,
                            VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                            Rating = reader["Rating"].ToString(),
                            Notes = reader["VisitNotes"].ToString(),
                            IsDeleted = Convert.ToBoolean(reader["VisitIsDeleted"]),
                            UpdatedAt = reader["VisitUpdatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VisitUpdatedAt"]),
                            City = city,
                            Trip = trip
                        };

                        phrases.Add(new Phrase
                        {
                            PhraseID = Convert.ToInt32(reader["PhraseID"]),
                            VisitID = visit.VisitID,
                            OriginalText = reader["OriginalText"].ToString(),
                            Translation = reader["Translation"].ToString(),
                            Language = reader["Language"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                            UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"]),
                            Visit = visit
                        });
                    }
                }
            }

            return phrases;
        }

        // جلب عبارة محددة حسب المعرف
        public static Phrase GetPhraseById(int phraseId)
        {
            Phrase phrase = null;

            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Phrase WHERE PhraseID = @PhraseID", con);
                cmd.Parameters.AddWithValue("@PhraseID", phraseId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        phrase = new Phrase
                        {
                            PhraseID = Convert.ToInt32(reader["PhraseID"]),
                            VisitID = Convert.ToInt32(reader["VisitID"]),
                            OriginalText = reader["OriginalText"].ToString(),
                            Translation = reader["Translation"].ToString(),
                            Language = reader["Language"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                            UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"])
                        };
                    }
                }
            }

            return phrase;
        }

        // إضافة عبارة جديدة إلى قاعدة البيانات
        public static bool AddPhrase(Phrase phrase)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Phrase (VisitID, OriginalText, Translation, Language, Notes)
                    VALUES (@VisitID, @OriginalText, @Translation, @Language, @Notes)", con);

                cmd.Parameters.AddWithValue("@VisitID", phrase.VisitID);
                cmd.Parameters.AddWithValue("@OriginalText", phrase.OriginalText);
                cmd.Parameters.AddWithValue("@Translation", phrase.Translation);
                cmd.Parameters.AddWithValue("@Language", phrase.Language);
                cmd.Parameters.AddWithValue("@Notes", phrase.Notes);

                con.Open();

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // تعديل عبارة موجودة
        public static bool UpdatePhrase(Phrase phrase)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Phrase
                    SET VisitID = @VisitID,
                        OriginalText = @OriginalText,
                        Translation = @Translation,
                        Language = @Language,
                        Notes = @Notes,
                        UpdatedAt = GETDATE()
                    WHERE PhraseID = @PhraseID", con);

                cmd.Parameters.AddWithValue("@PhraseID", phrase.PhraseID);
                cmd.Parameters.AddWithValue("@VisitID", phrase.VisitID);
                cmd.Parameters.AddWithValue("@OriginalText", phrase.OriginalText);
                cmd.Parameters.AddWithValue("@Translation", phrase.Translation);
                cmd.Parameters.AddWithValue("@Language", phrase.Language);
                cmd.Parameters.AddWithValue("@Notes", phrase.Notes);

                con.Open();

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // الحذف الناعم لجملة (تعيين IsDeleted بدل الحذف النهائي)
        public static bool SoftDeletePhrase(int phraseID)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE Phrase SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE PhraseID = @PhraseID", con))
                {
                    cmd.Parameters.AddWithValue("@PhraseID", phraseID);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    con.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
