using Rahhal_System1.DAL;
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Rahhal_System1.DAL
{
    public class TripDAL
    {
        // دالة لجلب كل الرحلات الخاصة بمستخدم معين ولم يتم حذفها (Soft Delete)
        public static List<Trip> GetTripsByUser(int userId)
        {
            List<Trip> trips = new List<Trip>(); // قائمة لتخزين النتائج

            using (SqlConnection con = DbHelper.GetConnection()) // إنشاء اتصال بقاعدة البيانات
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Trip WHERE UserID = @UserID AND IsDeleted = 0", con)) // الاستعلام لجلب الرحلات حسب معرف المستخدم والتي لم تُحذف
                {
                    cmd.Parameters.AddWithValue("@UserID", userId); // تمرير معرف المستخدم كمعامل للاستعلام
                    con.Open(); // فتح الاتصال

                    using (SqlDataReader reader = cmd.ExecuteReader()) // تنفيذ الاستعلام وقراءة النتائج
                    {
                        while (reader.Read()) // التكرار على كل سجل تم إرجاعه
                        {
                            DateTime? updatedAt = null;

                            // التحقق إذا كان الحقل UpdatedAt ليس فارغًا (NULL) ثم تحويله إلى تاريخ
                            if (reader["UpdatedAt"] != DBNull.Value)
                                updatedAt = Convert.ToDateTime(reader["UpdatedAt"]);

                            // إنشاء كائن Trip وتعبئته بالبيانات المأخوذة من السطر الحالي
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

        // دالة لتنفيذ الحذف الناعم (Soft Delete) لرحلة حسب معرفها
        public static bool SoftDeleteTrip(int tripId)
        {
            using (SqlConnection con = DbHelper.GetConnection()) // إنشاء الاتصال بقاعدة البيانات
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Trip SET IsDeleted = 1, UpdatedAt = @UpdatedAt WHERE TripID = @TripID", con)) // تحديث السجل لتعيين IsDeleted إلى 1
                {
                    cmd.Parameters.AddWithValue("@TripID", tripId); // تمرير معرف الرحلة
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now); // تعيين تاريخ التحديث إلى الآن

                    con.Open(); // فتح الاتصال
                    return cmd.ExecuteNonQuery() > 0; // تنفيذ الأمر وإرجاع true إذا تم التحديث بنجاح
                }
            }
        }
    }
}
