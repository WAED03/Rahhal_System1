using Rahhal_System1.DAL;
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rahhal_System1.DAL
{
    public class TripDAL
    {
        // دالة لجلب جميع الرحلات الخاصة بمستخدم معين (التي لم تُحذف)
        public static List<Trip> GetTripsByUser(int userId)
        {
            List<Trip> trips = new List<Trip>(); // قائمة لتخزين الرحلات

            // استخدام جملة using لضمان إغلاق الاتصال تلقائياً بعد الانتهاء
            using (var con = DbHelper.GetConnection())
            {
                // تجهيز أمر SQL لاستعلام الرحلات للمستخدم المحدد والتي ليست محذوفة (IsDeleted = 0)
                using (var cmd = new SqlCommand("SELECT * FROM Trip WHERE UserID = @UserID AND IsDeleted = 0", con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId); // إضافة قيمة المعامل

                    con.Open(); // فتح الاتصال بقاعدة البيانات

                    using (var reader = cmd.ExecuteReader()) // تنفيذ الاستعلام وقراءة النتائج
                    {
                        while (reader.Read()) // التكرار على كل سجل موجود
                        {
                            // التعامل مع حقل UpdatedAt، حيث يمكن أن يكون فارغاً (NULL)
                            DateTime? updatedAt = reader["UpdatedAt"] != DBNull.Value
                                ? (DateTime?)Convert.ToDateTime(reader["UpdatedAt"])
                                : null;

                            // إنشاء كائن Trip جديد من بيانات السجل الحالي وإضافته للقائمة
                            trips.Add(new Trip
                            {
                                TripID = Convert.ToInt32(reader["TripID"]),
                                UserID = Convert.ToInt32(reader["UserID"]),
                                TripName = reader["TripName"].ToString(),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"]),
                                TravelMethod = reader["TravelMethod"].ToString(),
                                Notes = reader["Notes"].ToString(),
                                IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                                UpdatedAt = updatedAt
                            });
                        }
                    }
                }
            }

            return trips; // إرجاع قائمة الرحلات
        }

        // دالة لجلب بيانات رحلة محددة بناءً على معرف الرحلة (TripID)
        public static Trip GetTripById(int tripId)
        {
            Trip trip = null; // المتغير الذي سيخزن بيانات الرحلة بعد الجلب

            using (var con = DbHelper.GetConnection())
            {
                using (var cmd = new SqlCommand("SELECT * FROM Trip WHERE TripID = @TripID", con))
                {
                    cmd.Parameters.AddWithValue("@TripID", tripId); // إضافة معرف الرحلة كمعامل

                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // إذا وجد سجل مطابق
                        {
                            DateTime? updatedAt = reader["UpdatedAt"] != DBNull.Value
                                ? (DateTime?)Convert.ToDateTime(reader["UpdatedAt"])
                                : null;

                            // ملء خصائص كائن Trip من بيانات السجل
                            trip = new Trip
                            {
                                TripID = Convert.ToInt32(reader["TripID"]),
                                UserID = Convert.ToInt32(reader["UserID"]),
                                TripName = reader["TripName"].ToString(),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"]),
                                TravelMethod = reader["TravelMethod"].ToString(),
                                Notes = reader["Notes"].ToString(),
                                IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                                UpdatedAt = updatedAt
                            };
                        }
                    }
                }
            }

            return trip; // إرجاع كائن الرحلة (أو null إذا لم يُوجد)
        }

        public static Trip GetTripByVisitId(int visitId)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
            SELECT t.TripID, t.TripName, t.UserID
            FROM Trip t
            INNER JOIN CityVisit v ON v.TripID = t.TripID
            WHERE v.VisitID = @VisitID", con);

                cmd.Parameters.AddWithValue("@VisitID", visitId);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Trip
                        {
                            TripID = Convert.ToInt32(reader["TripID"]),
                            TripName = reader["TripName"].ToString(),
                            UserID = Convert.ToInt32(reader["UserID"])
                        };
                    }
                }
            }

            return null; // في حال لم يتم العثور على الرحلة
        }


        // دالة لإضافة رحلة جديدة إلى قاعدة البيانات
        public static bool AddTrip(Trip trip)
        {
            using (var con = DbHelper.GetConnection())
            {
                // أمر SQL لإضافة بيانات الرحلة الجديدة
                using (var cmd = new SqlCommand(
                    @"INSERT INTO Trip (UserID, TripName, StartDate, EndDate, TravelMethod, Notes)
                      VALUES (@UserID, @TripName, @StartDate, @EndDate, @TravelMethod, @Notes)", con))
                {
                    // تمرير قيم الرحلة كمعاملات لحماية من هجمات SQL Injection
                    cmd.Parameters.AddWithValue("@UserID", trip.UserID);
                    cmd.Parameters.AddWithValue("@TripName", trip.TripName);
                    cmd.Parameters.AddWithValue("@StartDate", trip.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", trip.EndDate);
                    cmd.Parameters.AddWithValue("@TravelMethod", trip.TravelMethod);
                    cmd.Parameters.AddWithValue("@Notes", trip.Notes);

                    con.Open();
                    // ExecuteNonQuery يرجع عدد الصفوف المتأثرة، نتحقق هل هي أكبر من 0 للنجاح
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // دالة لتحديث بيانات رحلة موجودة بناءً على معرفها
        public static bool UpdateTrip(Trip trip)
        {
            using (var con = DbHelper.GetConnection())
            {
                // أمر SQL لتحديث بيانات الرحلة
                using (var cmd = new SqlCommand(
                    @"UPDATE Trip SET TripName = @TripName, StartDate = @StartDate, EndDate = @EndDate, 
                      TravelMethod = @TravelMethod, Notes = @Notes, UpdatedAt = GETDATE() 
                      WHERE TripID = @TripID", con))
                {
                    // تمرير القيم والمعرف كمعاملات
                    cmd.Parameters.AddWithValue("@TripID", trip.TripID);
                    cmd.Parameters.AddWithValue("@TripName", trip.TripName);
                    cmd.Parameters.AddWithValue("@StartDate", trip.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", trip.EndDate);
                    cmd.Parameters.AddWithValue("@TravelMethod", trip.TravelMethod);
                    cmd.Parameters.AddWithValue("@Notes", trip.Notes);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0; // إرجاع حالة نجاح التحديث
                }
            }
        }

        // دالة لتنفيذ الحذف الناعم (Soft Delete) لرحلة معينة (تعيين IsDeleted = 1)
        public static bool SoftDeleteTrip(int tripId)
        {
            using (var con = DbHelper.GetConnection())
            {
                using (var cmd = new SqlCommand(
                    "UPDATE Trip SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE TripID = @TripID", con))
                {
                    cmd.Parameters.AddWithValue("@TripID", tripId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now); // تسجيل وقت الحذف

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0; // إرجاع نجاح العملية
                }
            }
        }

        // دالة لتنفيذ الحذف الناعم (Soft Delete) لرحلة معيّنة وكل البيانات المرتبطة بها مثل الزيارات والعبارات.
        // تأخذ معرف الرحلة واسمها بالإضافة إلى الاتصال والترانزكشن المفتوحين من الخارج.
        // تُرجع true في حال تنفيذ الحذف بنجاح.
        public static bool SoftDeleteTripWithDependencies(int tripID, string tripName, SqlConnection con, SqlTransaction transaction)
        {
            // 1. حذف الرحلة
            SqlCommand cmdTrip = new SqlCommand("UPDATE Trip SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE TripID = @TripID", con, transaction);
            cmdTrip.Parameters.AddWithValue("@TripID", tripID);
            cmdTrip.ExecuteNonQuery();

            // 2. حذف زيارات المدن
            SqlCommand cmdVisits = new SqlCommand("UPDATE CityVisit SET IsDeleted = 1, UpdatedAt = GETDATE() WHERE TripID = @TripID", con, transaction);
            cmdVisits.Parameters.AddWithValue("@TripID", tripID);
            cmdVisits.ExecuteNonQuery();

            // 3. حذف العبارات المرتبطة بالزيارات
            SqlCommand cmdPhrases = new SqlCommand(@"
        UPDATE Phrase SET IsDeleted = 1, UpdatedAt = GETDATE()
        WHERE VisitID IN (SELECT VisitID FROM CityVisit WHERE TripID = @TripID)", con, transaction);
            cmdPhrases.Parameters.AddWithValue("@TripID", tripID);
            cmdPhrases.ExecuteNonQuery();

            // 4. تسجيل العملية
            ActivityLogger.Log(con, transaction, "SoftDelete Trip", $"Soft-deleted trip '{tripName}' (ID = {tripID})");

            return true;
        }

    }
}


