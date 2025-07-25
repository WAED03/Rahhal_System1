using Rahhal_System1.DAL;      // استيراد الطبقة المسؤولة عن التعامل مع قاعدة البيانات
using Rahhal_System1.Models;   // استيراد الكائنات النموذجية (Models) مثل City, Country, CityVisit
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rahhal_System1.DAL
{
    public class CityVisitDAL
    {
        /// <summary>
        /// جلب جميع زيارات المدن المتعلقة برحلة معينة باستخدام معرف الرحلة
        /// </summary>
        public static List<CityVisit> GetVisitsByTrip(int tripId)
        {
            List<CityVisit> visits = new List<CityVisit>(); // قائمة لتخزين نتائج الزيارات

            using (SqlConnection con = DbHelper.GetConnection()) // فتح اتصال بقاعدة البيانات
            {
                con.Open(); // فتح الاتصال

                // تعريف الاستعلام لجلب بيانات الزيارة وربطها بالمدينة والدولة
                SqlCommand cmd = new SqlCommand(@"
            SELECT v.VisitID, v.TripID, v.CityID, v.VisitDate, v.Rating, v.Notes, v.IsDeleted, v.UpdatedAt,
                   c.CityName, c.CountryID,
                   co.CountryName
            FROM CityVisit v
            INNER JOIN City c ON v.CityID = c.CityID
            INNER JOIN Country co ON c.CountryID = co.CountryID
            WHERE v.TripID = @TripID AND v.IsDeleted = 0", con);

                cmd.Parameters.AddWithValue("@TripID", tripId); // تمرير رقم الرحلة كـ Parameter

                using (SqlDataReader reader = cmd.ExecuteReader()) // تنفيذ الاستعلام وقراءة النتائج
                {
                    while (reader.Read()) // التكرار على كل سجل
                    {
                        // التحقق مما إذا كان تاريخ التحديث يحتوي على قيمة
                        DateTime? updatedAt = null;
                        if (reader["UpdatedAt"] != DBNull.Value)
                            updatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                        // إنشاء كائن الدولة
                        var country = new Country
                        {
                            CountryID = Convert.ToInt32(reader["CountryID"]),
                            CountryName = reader["CountryName"].ToString()
                        };

                        // إنشاء كائن المدينة وربطه بالدولة
                        var city = new City
                        {
                            CityID = Convert.ToInt32(reader["CityID"]),
                            CityName = reader["CityName"].ToString(),
                            CountryID = country.CountryID,
                            Country = country
                        };

                        // إنشاء كائن الزيارة وربطها بالمدينة
                        var visit = new CityVisit
                        {
                            VisitID = Convert.ToInt32(reader["VisitID"]),
                            TripID = Convert.ToInt32(reader["TripID"]),
                            CityID = Convert.ToInt32(reader["CityID"]),
                            VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                            Rating = reader["Rating"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                            UpdatedAt = updatedAt,
                            City = city
                        };

                        visits.Add(visit); // إضافة الزيارة إلى القائمة
                    }
                }
            }

            return visits; // إرجاع قائمة الزيارات
        }

            // إضافة زيارة جديدة
            public static bool AddVisit(CityVisit visit)
            {
                using (SqlConnection con = DbHelper.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO CityVisit (TripID, CityID, VisitDate, Rating, Notes)
                  VALUES (@TripID, @CityID, @VisitDate, @Rating, @Notes)", con);

                    cmd.Parameters.AddWithValue("@TripID", visit.TripID);
                    cmd.Parameters.AddWithValue("@CityID", visit.CityID);
                    cmd.Parameters.AddWithValue("@VisitDate", visit.VisitDate);
                    cmd.Parameters.AddWithValue("@Rating", visit.Rating);
                    cmd.Parameters.AddWithValue("@Notes", visit.Notes);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }

            // تعديل زيارة موجودة
            public static bool UpdateVisit(CityVisit visit)
            {
                using (SqlConnection con = DbHelper.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(
                        @"UPDATE CityVisit 
                  SET TripID = @TripID, CityID = @CityID, VisitDate = @VisitDate, Rating = @Rating, Notes = @Notes, UpdatedAt = GETDATE()
                  WHERE VisitID = @VisitID", con);

                    cmd.Parameters.AddWithValue("@VisitID", visit.VisitID);
                    cmd.Parameters.AddWithValue("@TripID", visit.TripID);
                    cmd.Parameters.AddWithValue("@CityID", visit.CityID);
                    cmd.Parameters.AddWithValue("@VisitDate", visit.VisitDate);
                    cmd.Parameters.AddWithValue("@Rating", visit.Rating);
                    cmd.Parameters.AddWithValue("@Notes", visit.Notes);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }

            // جلب زيارة واحدة بناءً على VisitID
            public static CityVisit GetVisitById(int visitID)
            {
                using (SqlConnection con = DbHelper.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(
                        @"SELECT TripID, CityID, VisitDate, Rating, Notes 
                  FROM CityVisit 
                  WHERE VisitID = @VisitID", con);

                    cmd.Parameters.AddWithValue("@VisitID", visitID);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CityVisit
                            {
                                VisitID = visitID,
                                TripID = Convert.ToInt32(reader["TripID"]),
                                CityID = Convert.ToInt32(reader["CityID"]),
                                VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                                Rating = reader["Rating"].ToString(),
                                Notes = reader["Notes"].ToString()
                            };
                        }
                    }

                    return null;
                }
            }
        

        /// <summary>
        /// تنفيذ الحذف الناعم لزيارة مدينة بناءً على رقم الزيارة
        /// </summary>
        public static bool SoftDeleteVisit(int visitId)
        {
            using (SqlConnection con = DbHelper.GetConnection()) // فتح الاتصال
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE CityVisit SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE VisitID = @VisitID", con)) // استعلام التحديث
                {
                    cmd.Parameters.AddWithValue("@VisitID", visitId); // تمرير رقم الزيارة
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now); // تعيين وقت الحذف الحالي

                    con.Open(); // فتح الاتصال
                    return cmd.ExecuteNonQuery() > 0; // تنفيذ الاستعلام وإرجاع true إذا تم التحديث
                }
            }
        }

        public static bool SoftDeleteCityVisitWithPhrases(int visitID, string cityName, SqlConnection con, SqlTransaction transaction)
        {
            // 1. حذف زيارة المدينة
            SqlCommand cmdVisit = new SqlCommand("UPDATE CityVisit SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE VisitID = @VisitID", con, transaction);
            cmdVisit.Parameters.AddWithValue("@VisitID", visitID);
            cmdVisit.ExecuteNonQuery();

            // 2. حذف العبارات المرتبطة
            SqlCommand cmdPhrases = new SqlCommand("UPDATE Phrase SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE VisitID = @VisitID", con, transaction);
            cmdPhrases.Parameters.AddWithValue("@VisitID", visitID);
            cmdPhrases.ExecuteNonQuery();

            // 3. تسجيل الحدث
            ActivityLogger.Log(con, transaction, "SoftDelete CityVisit", $"Soft-deleted city visit '{cityName}' (VisitID = {visitID})");

            return true;
        }

    }
}
